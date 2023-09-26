using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Net.Sockets;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button serverBtn;
    [SerializeField] private Button clientBtn;
    [SerializeField] private Button hostBtn;
    [SerializeField] private TMP_InputField connectionIpInputField;
    private string Pattern = @"^([1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$";
    private async void Awake()
    {
        await Authenticate();
        serverBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer();
        });
        clientBtn.onClick.AddListener(async () =>
        {
            string input = connectionIpInputField.text;
            //if (string.IsNullOrEmpty(input))
            //{
            //    return;
            //}
            //else
            //{
            //    Regex check = new Regex(Pattern);
            //    if (check.IsMatch(input))
            //    {
            //        NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = input;
            //        NetworkManager.Singleton.StartClient();
            //    }
            //}
            if(input != null)
            {
                JoinAllocation a = await RelayService.Instance.JoinAllocationAsync(input);
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData, a.HostConnectionData);
                NetworkManager.Singleton.StartClient();
            }
        });
        hostBtn.onClick.AddListener(async () =>
        {
            //NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = GetLocalIPAddress();
            Allocation a = await RelayService.Instance.CreateAllocationAsync(3);
            NetworkManagement.Instance.joinCode = await RelayService.Instance.GetJoinCodeAsync(a.AllocationId);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData);

            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.SceneManager.LoadScene("Menu", LoadSceneMode.Single);
        });
    }

    private static async Task Authenticate()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }
}
