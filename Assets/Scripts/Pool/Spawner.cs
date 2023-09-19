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
    [SerializeField] private EnemiesScriptable spawnableEnemies;

    // Start is called before the first frame update
    void Start()
    {
        spawnableEnemies = Resources.Load<EnemiesScriptable>("ScriptableObjects/EnemiesScriptable");
        if (!NetworkManager.Singleton.IsServer)
        {
            Debug.Log("Not Server");
            return;
        }

        if (Enemy.numOfEnemies == 0)
        {
            SpawnEnemy();
        }
        // Invoke("SpawnEnemy", 2f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && NetworkManager.Singleton.IsServer)
        {
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        foreach(var item in spawnableEnemies.enemies)
        {
            if(item.enemyType == Enumirators.EnemyType.Stationary)
            {
                GameObject go = m_ObjectPool.GetNetworkObject(item.prefab).gameObject;
                go.GetComponent<NetworkObject>().Spawn(true); // TODO
            }

            //for (int i = 0; i < m_Amount; i++)
            //{
            //    //go.transform.position = new Vector3(Random.Range(-40, 40), Random.Range(-40, 40));

            //    //go.transform.localScale = new Vector3(4, 4, 4);
            //    //go.GetComponent<Enemy>().Size.Value = 4;

            //    //float dx = Random.Range(-40, 40) / 10.0f;
            //    //float dy = Random.Range(-40, 40) / 10.0f;
            //    //float dir = Random.Range(-40, 40);
            //    //go.transform.rotation = Quaternion.Euler(0, 0, dir);
            //    //go.GetComponent<Rigidbody2D>().angularVelocity = dir;
            //    //go.GetComponent<Rigidbody2D>().velocity = new Vector2(dx, dy);
            //    //go.GetComponent<Enemy>().enemyPrefab = item.prefab;
            //}
        }
    }
}
