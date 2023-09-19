using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    NetworkObjectPool m_ObjectPool;

    [SerializeField]
    private float patrollingDistance = 4.5f;

    [SerializeField]
    private GameObject emptyPrefab;
    [SerializeField] private EnemiesScriptable spawnableEnemies;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
        spawnableEnemies = Resources.Load<EnemiesScriptable>("ScriptableObjects/EnemiesScriptable");
        if (!NetworkManager.Singleton.IsServer)
        {
            Debug.Log("Not Server");
            return;
        }
        // Invoke("SpawnEnemy", 2f);
    }

    public GameObject SpawnEnemy(GameObject prefab, Enumirators.EnemyType enemyType, Transform location, int roomID)
    {
        GameObject go = m_ObjectPool.GetNetworkObject(prefab).gameObject;
        go.transform.position = location.position;
        go.GetComponent<Enemy>().roomID.Value = roomID;
        go.GetComponent<NetworkObject>().Spawn(true);
        return go;
        if (enemyType == Enumirators.EnemyType.Stationary)
        {
            
        }
        //else
        //{
        //    Transform spawnLocation = location;
        //    GameObject left = m_ObjectPool.GetNetworkObject(emptyPrefab).gameObject;
        //    left.transform.position = new Vector3(spawnLocation.transform.position.x - patrollingDistance, spawnLocation.transform.position.y, spawnLocation.transform.position.z);
        //    GameObject right = m_ObjectPool.GetNetworkObject(emptyPrefab).gameObject;
        //    right.transform.position = new Vector3(spawnLocation.transform.position.x + patrollingDistance, spawnLocation.transform.position.y, spawnLocation.transform.position.z);
        //    GameObject go = m_ObjectPool.GetNetworkObject(prefab).gameObject;
        //    go.transform.position = location.position;
        //    go.GetComponent<Enemy>().roomID.Value = roomID;
        //    EnemyBehaviourPatrolling enemyBehaviourPatrolling = go.GetComponent<EnemyBehaviourPatrolling>();
        //    enemyBehaviourPatrolling.leftLimit.Value = left.transform.position;
        //    enemyBehaviourPatrolling.rightLimit.Value = right.transform.position;
        //    return go;
        //}
    }
}
