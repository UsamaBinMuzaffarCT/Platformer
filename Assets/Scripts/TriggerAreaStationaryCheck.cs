using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAreaStationaryCheck : MonoBehaviour
{
    private EnemyBehaviour enemyBehaviour;

    private void Awake()
    {
        enemyBehaviour = GetComponentInParent<EnemyBehaviour>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            gameObject.SetActive(false);
            enemyBehaviour.target = collision.gameObject;
            enemyBehaviour.inRange = true;
            enemyBehaviour.hotZone.SetActive(true);
        }
    }
}
