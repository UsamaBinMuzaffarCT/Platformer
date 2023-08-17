using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAreaCheck : MonoBehaviour
{
    private EnemyBehaviourPatrolling enemyBehaviour;

    private void Awake()
    {
        enemyBehaviour = GetComponentInParent<EnemyBehaviourPatrolling>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            gameObject.SetActive(false);
            enemyBehaviour.target = collision.transform;
            enemyBehaviour.inRange = true;
            enemyBehaviour.hotZone.SetActive(true);
        }
    }
}
