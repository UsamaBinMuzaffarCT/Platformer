using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkManagement : NetworkBehaviour
{
    public GameObject playerPrefab;

    private GameObject player;
    public static NetworkManagement Instance { get; private set; }
    private void Awake()
    {
        DontDestroyOnLoad(this);
        Instance = this;
    }

    public void SpawnPlayerPrefab(int factionID, ulong clientID)
    {
        SpawnPlayerPrefabServerRpc(factionID, clientID);
        player.GetComponent<PlayerMovement>().ParentCamera();
    }

    [ServerRpc (RequireOwnership = false)]
    public void SpawnPlayerPrefabServerRpc(int factionID, ulong clientID)
    {
        Enumirators.Faction faction = (Enumirators.Faction)Enum.ToObject(typeof(Enumirators.Faction), factionID);
        player = Instantiate(playerPrefab);
        Debug.Log("Spawning?");
        player.GetComponent<PlayerMovement>().SetFactionAnimator(faction);
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientID);
    }
    [ClientRpc]
    public void SpawnPlayerPrefabClientRpc(int factionID, ulong clientID)
    {
        if(clientID == player.GetComponent<PlayerMovement>().OwnerClientId)
        {
            player.GetComponent<PlayerMovement>().ParentCamera();
        }
    }
}
