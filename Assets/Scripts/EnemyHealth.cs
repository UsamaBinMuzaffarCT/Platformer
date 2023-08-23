using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int health;
    private Animator animator;
    [SerializeField] private float hitTimer;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void DeathDestroy()
    {
        Destroy(GetComponentInParent<Rigidbody2D>().gameObject);
    }

    private void Update()
    {
        hitTimer -= Time.deltaTime;
        if (health <= 0)
        {
            animator.SetTrigger("isDead");
        }
    }

    public void TakeDamage(int damage)
    {
        if (health > 0 && hitTimer <= 0)
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                health -= damage;
                animator.SetTrigger("Hurt");
                hitTimer = 0.5f;
            }
        }
        
    }
}
