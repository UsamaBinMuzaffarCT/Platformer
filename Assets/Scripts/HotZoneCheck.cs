using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotZoneCheck : MonoBehaviour
{
    private EnemyBehaviourPatrolling enemyBehaviour;
    private bool inRange;
    private Animator animator;

    private void Awake()
    {
        enemyBehaviour = GetComponentInParent<EnemyBehaviourPatrolling>();
        animator = GetComponentInParent<Animator>(); 
    }

    private void Update()
    {
        if(inRange && !animator.GetCurrentAnimatorStateInfo(0).IsName("Enemy_attack"))
        {
            enemyBehaviour.Flip();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            inRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            inRange = false;
            gameObject.SetActive(false);
            enemyBehaviour.triggerArea.SetActive(true);
            enemyBehaviour.inRange = false;
            enemyBehaviour.SelectTarget();
        }
    }
}
