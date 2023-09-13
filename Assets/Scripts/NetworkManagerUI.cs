using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : NetworkBehaviour
{
    [SerializeField] private Button serverBtn;
    [SerializeField] private Button clientBtn;
    [SerializeField] private Button hostBtn;
    private void Awake()
    {
        serverBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer();
            NetworkManager.SceneManager.LoadScene("Menu", UnityEngine.SceneManagement.LoadSceneMode.Single);
        });
        clientBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
            NetworkManager.SceneManager.LoadScene("Menu", UnityEngine.SceneManagement.LoadSceneMode.Single);
        });
        hostBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
            NetworkManager.SceneManager.LoadScene("Menu", UnityEngine.SceneManagement.LoadSceneMode.Single);
        });
    }

    private void OnDestroy()
    {
        GetComponent<NetworkObject>().Despawn();
    }
}
