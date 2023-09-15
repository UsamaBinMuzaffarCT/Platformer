using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class SpawnPlayers : MonoBehaviour
{
    [SerializeField] private Button spawnPlayersBtn;
    [SerializeField] private Transform startPosition;


    private void Awake()
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            spawnPlayersBtn.gameObject.SetActive(false);
        }
        spawnPlayersBtn.onClick.AddListener(() =>
        {
            NetworkManagement.Instance.EnableAllPlayers();
        });
    }

    
}
