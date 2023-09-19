using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
    #region variables

    [SerializeField] private EnemiesScriptable spawnableEnemies;
    [SerializeField] private GameObject emptyGameObject;
    private float patrollingDistance = 4.5f;
    private List<Transform> spawnLocations = new List<Transform>();

    #endregion

    #region functions

    private void Start()
    {
        spawnableEnemies = Resources.Load<EnemiesScriptable>("ScriptableObjects/EnemiesScriptable");
        List<Classes.Enemy> enemies = spawnableEnemies.enemies;
        foreach (Transform child in transform)
        {
            if(child.tag == "SpawnLocation")
            {
                spawnLocations.Add(child);
            }
        }
        foreach(Transform location in spawnLocations)
        {
            int randomEnemy = Random.Range(0, enemies.Count);
            if (enemies[randomEnemy].enemyType == Enumirators.EnemyType.Stationary)
            {
                GameObject enemy = Instantiate(enemies[randomEnemy].prefab);
                enemy.GetComponent<NetworkObject>().Spawn();
                enemy.transform.position = location.position; //
            }
            else
            {
                Transform spawnLocation = location;
                GameObject leftLimit = Instantiate(emptyGameObject);
                leftLimit.transform.position = new Vector3(spawnLocation.transform.position.x - patrollingDistance, spawnLocation.transform.position.y, spawnLocation.transform.position.z);
                leftLimit.GetComponent<NetworkObject>().Spawn();
                GameObject rightLimit = Instantiate(emptyGameObject);
                rightLimit.transform.position = new Vector3(spawnLocation.transform.position.x + patrollingDistance, spawnLocation.transform.position.y, spawnLocation.transform.position.z);
                leftLimit.GetComponent<NetworkObject>().Spawn();
                GameObject enemy = Instantiate(enemies[randomEnemy].prefab);
                EnemyBehaviourPatrolling enemyBehaviourPatrolling = enemy.GetComponent<EnemyBehaviourPatrolling>();
                enemyBehaviourPatrolling.leftLimit.Value = leftLimit.transform.position;
                enemyBehaviourPatrolling.rightLimit.Value = rightLimit.transform.position;
                enemyBehaviourPatrolling.enabled = true;
                enemy.transform.position = location.position; //
                enemy.GetComponent<NetworkObject>().Spawn();
            }
        }
    }
    
    #endregion
}
