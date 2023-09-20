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
            return;
        }
    }

    public GameObject SpawnEnemy(GameObject prefab, Enumirators.EnemyType enemyType, Transform location, int roomID)
    {
        GameObject go = m_ObjectPool.GetNetworkObject(prefab).gameObject;
        go.transform.position = location.position;
        go.GetComponent<Enemy>().roomID.Value = roomID;
        go.GetComponent<NetworkObject>().Spawn(true);
        if (enemyType == Enumirators.EnemyType.Patrolling)
        {
            EnemyBehaviourPatrolling enemyBehaviourPatrolling = go.GetComponent<EnemyBehaviourPatrolling>();
            enemyBehaviourPatrolling.leftLimit.Value = new Vector3(location.transform.position.x - patrollingDistance, location.transform.position.y, location.transform.position.z);
            enemyBehaviourPatrolling.rightLimit.Value = new Vector3(location.transform.position.x + patrollingDistance, location.transform.position.y, location.transform.position.z);
        }
        return go;
    }
}
