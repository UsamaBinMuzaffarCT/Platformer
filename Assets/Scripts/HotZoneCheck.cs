using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotZoneCheck : MonoBehaviour
{
    private EnemyBehaviourPatrolling enemyBehaviour;
    private bool inRange;
    private Animator animator;
    [SerializeField] private GameObject currentPlayer;

    private void Awake()
    {
        currentPlayer = null;
        enemyBehaviour = GetComponentInParent<EnemyBehaviourPatrolling>();
        animator = GetComponentInParent<Animator>(); 
    }

    private void Update()
    {
        if(inRange && !animator.GetCurrentAnimatorStateInfo(0).IsName("Enemy_attack"))
        {
            enemyBehaviour.Flip();
        }
        if(currentPlayer.GetComponent<PlayerMovement>().isDead == true)
        {
            currentPlayer = null;
            inRange = false;
            gameObject.SetActive(false);
            enemyBehaviour.triggerArea.SetActive(true);
            enemyBehaviour.inRange = false;
            enemyBehaviour.SelectTarget();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && collision.GetComponent<PlayerMovement>().isDead == false)
        {
            currentPlayer = collision.gameObject;
            inRange = true;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            currentPlayer = null;
            inRange = false;
            gameObject.SetActive(false);
            enemyBehaviour.triggerArea.SetActive(true);
            enemyBehaviour.inRange = false;
            enemyBehaviour.SelectTarget();
        }
    }
}
