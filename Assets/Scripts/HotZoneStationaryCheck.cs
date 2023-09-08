using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotZoneStationaryCheck : MonoBehaviour
{
    private EnemyBehaviour enemyBehaviour;
    private bool inRange;
    private Animator animator;
    [SerializeField] private GameObject currentPlayer;

    private void Awake()
    {
        enemyBehaviour = GetComponentInParent<EnemyBehaviour>();
        animator = GetComponentInParent<Animator>();
    }

    private void Update()
    {
        if (inRange && !animator.GetCurrentAnimatorStateInfo(0).IsName("Enemy_attack"))
        {
            enemyBehaviour.Flip();
        }
        if(currentPlayer != null) {
            if (currentPlayer.GetComponent<PlayerMovement>().n_isDead.Value == true)
            {
                enemyBehaviour.inRange = false;
                currentPlayer = null;
                inRange = false;
                gameObject.SetActive(false);
                enemyBehaviour.triggerArea.SetActive(true);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            currentPlayer = collision.gameObject;
            inRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            currentPlayer = null;
            inRange = false;
            gameObject.SetActive(false);
            enemyBehaviour.triggerArea.SetActive(true);
            enemyBehaviour.inRange = false;
        }
    }
}
