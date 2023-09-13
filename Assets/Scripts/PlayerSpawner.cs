using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    public Enumirators.Faction selectedFaction;

    public void SpawnPlayer()
    {
        NetworkManagement.Instance.SpawnPlayerPrefab(PlayerInfoHolder.Instance.factionID, NetworkManager.Singleton.LocalClientId);
    }
}
