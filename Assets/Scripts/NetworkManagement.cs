using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class NetworkManagement : NetworkBehaviour
{
    #region variables
    public static NetworkManagement Instance { get; private set; }

    public List<GameObject> players = new List<GameObject>();
    public NetworkList<ulong> clientIDs;
    public NetworkList<int> factionIDs;
    public NetworkVariable<int> n_mapSeed = new NetworkVariable<int>(1, readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Server);

    #endregion

    #region private-functions
    private void Awake()
    {
        DontDestroyOnLoad(this);
        Instance = this;
        clientIDs = new NetworkList<ulong>();
        factionIDs = new NetworkList<int>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            n_mapSeed.Value = UnityEngine.Random.Range(0, 1000);
        }
    }

    private void OnDestroy()
    {
        clientIDs.Dispose();
        factionIDs.Dispose();
    }
    #endregion

    #region public-functions

    public void EnableAllPlayers()
    {
        EnableAllPlayerServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void EnableAllPlayerServerRpc()
    {
        EnableAllPlayerClientRpc();
    }

    [ClientRpc]
    public void EnableAllPlayerClientRpc()
    {
        players = GameObject.FindGameObjectsWithTag("Player").ToList();
        Debug.Log("Spawned");
        foreach (GameObject p in players)
        {
            PlayerMovement playerMovement = p.GetComponent<PlayerMovement>();
            playerMovement.ParentCamera();
            if (playerMovement.OwnerClientId == NetworkManager.Singleton.LocalClientId)
            {
                playerMovement.SetFactionAnimator(PlayerInfoHolder.Instance.playerFaction);
            }
            else
            {
                int index = -1;
                for(int i = 0; i < clientIDs.Count; i++)
                {
                    if (clientIDs[i] == playerMovement.OwnerClientId)
                    {
                        index = i;
                        break;
                    }
                }
                if(index != -1)
                {
                   Enumirators.Faction faction = (Enumirators.Faction)Enum.ToObject(typeof(Enumirators.Faction), factionIDs[index]);
                   playerMovement.SetFactionAnimator(faction);
                }
            }
            p.SetActive(true);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddFactionInfoToListsServerRpc(ulong clientID, int factionID)
    {
        int index = -1;
        for(int i = 0; i < clientIDs.Count; i++)
        {
            if (clientIDs[i] == clientID)
            {
                index = i;
            }
        }
        if(index > -1)
        {
            factionIDs[index] = factionID;
        }
        else
        {
            clientIDs.Add(clientID);
            factionIDs.Add(factionID);
        }
    }

    #endregion
}
