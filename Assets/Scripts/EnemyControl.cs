using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    [SerializeField] private EnemyBehaviour enemyBehaviour = null;
    [SerializeField] private EnemyBehaviourPatrolling EnemyBehaviourPatrolling = null;
    private Rigidbody2D rb;
    private float tempMovespeed;
    private float tempGravity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        try
        {
            enemyBehaviour = GetComponent<EnemyBehaviour>();
        }
        catch
        {

        }
        try
        {
            EnemyBehaviourPatrolling = GetComponent<EnemyBehaviourPatrolling>();
        }
        catch
        {

        }
    }

    public void StopMoving()
    {
        
        if(enemyBehaviour != null)
        {
            tempMovespeed = enemyBehaviour.moveSpeed;
            enemyBehaviour.moveSpeed = 0;
            tempGravity = rb.gravityScale;
            rb.gravityScale = 0;
        }
        if (EnemyBehaviourPatrolling != null)
        {
            tempMovespeed = EnemyBehaviourPatrolling.moveSpeed;
            EnemyBehaviourPatrolling.moveSpeed = 0;
            tempGravity = rb.gravityScale;
            rb.gravityScale = 0;
        }

    }
    public void StartMoving()
    {
        if (enemyBehaviour != null)
        {
            enemyBehaviour.moveSpeed = tempMovespeed;
            rb.gravityScale = tempGravity;
        }
        if (EnemyBehaviourPatrolling != null)
        {
            EnemyBehaviourPatrolling.moveSpeed = tempMovespeed;
            rb.gravityScale = tempGravity;
        }
    }


}
