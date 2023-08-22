using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealth : MonoBehaviour
{
    public int health;
    private Animator animator;
    [SerializeField] private float hitTimer;
    private int totalHealth;

    private void Start()
    {
        totalHealth = health;
        animator = GetComponent<Animator>();
    }

    private void DeathDestroy()
    {
        Destroy(GetComponentInParent<Rigidbody2D>().gameObject);
    }

    private void Update()
    {
        hitTimer -= Time.deltaTime;   
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
        if (health < 0.5f * totalHealth && !animator.GetBool("IsEnraged"))
        {
            animator.SetBool("IsEnraged", true);
        }
        if(health <= 0)
        {
            animator.SetTrigger("Dead");
        }
    }
}
