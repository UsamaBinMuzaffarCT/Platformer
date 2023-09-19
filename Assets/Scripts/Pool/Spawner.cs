using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    NetworkObjectPool m_ObjectPool;

    [SerializeField]
    private int m_Amount = 4;

    [SerializeField]
    private GameObject m_Enemy;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        GameObject go = m_ObjectPool.GetNetworkObject(m_Enemy).gameObject;
        go.transform.position = new Vector3(0, 0, 0);

        go.GetComponent<NetworkObject>().Spawn(true);
    }
}
