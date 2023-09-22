using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordDamage : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D hitInfo)
    {        
        if (hitInfo.CompareTag("Boss"))
        {
            hitInfo.transform.GetComponentInParent<Animator>().GetComponent<BossHealth>().TakeDamage(20);
        }
        if (hitInfo.CompareTag("Enemy"))
        {
            hitInfo.transform.GetComponentInParent<Animator>().GetComponent<EnemyHealth>().TakeDamage(20);
        }
    }
}
