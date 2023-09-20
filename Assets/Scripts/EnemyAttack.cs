using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (transform.position.x >= collision.transform.position.x)
            {
                collision.GetComponent<PlayerMovement>().knockBackFromRight = true;
                collision.GetComponent<PlayerHealth>().TakeDamage(5);
            }
            else
            {
                collision.GetComponent<PlayerMovement>().knockBackFromRight = false;
                collision.GetComponent<PlayerHealth>().TakeDamage(5);
            }
        }
    }
}
