using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class SpawnControl : NetworkBehaviour
{
    [SerializeField] private Button spawnBtn;

    public override void OnNetworkSpawn()
    {
        spawnBtn.onClick.AddListener(() =>
        {
            List<GameObject> authList = GameObject.FindGameObjectsWithTag("NetworkAuth").ToList();
            foreach (GameObject auth in authList)
            {
                PlayerSpawner authPlayerSpawner = auth.GetComponent<PlayerSpawner>();
                if(authPlayerSpawner.IsOwner)
                {
                    authPlayerSpawner.SpawnPlayer();
                    transform.parent.gameObject.SetActive(false);
                    break;
                }
            }
        });
    }
}
