using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Hit Player");
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Hit Player");
            if(transform.position.x >= collision.transform.position.x)
            {
                collision.GetComponent<PlayerMovement>().knockBackFromRight = true;
                collision.GetComponent<PlayerHealth>().TakeDamage(10);
            }
            else
            {
                collision.GetComponent<PlayerMovement>().knockBackFromRight = false;
                collision.GetComponent<PlayerHealth>().TakeDamage(10);
            }
        }
    }
}
