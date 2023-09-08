using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BossHealth : NetworkBehaviour
{
    public int health;
    public NetworkVariable<int> n_health = new NetworkVariable<int>(0, writePerm:NetworkVariableWritePermission.Owner);
    private Animator animator;
    [SerializeField] private float hitTimer;
    private int totalHealth;

    public override void OnNetworkSpawn()
    {
        totalHealth = health;
        n_health.Value = totalHealth;
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
        if (n_health.Value > 0 && hitTimer <= 0)
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                n_health.Value -= damage;
                animator.SetTrigger("Hurt");
                hitTimer = 0.5f;
            }
        }
        if (n_health.Value < 0.5f * totalHealth && !animator.GetBool("IsEnraged"))
        {
            animator.SetBool("IsEnraged", true);
        }
        if(n_health.Value <= 0)
        {
            animator.SetTrigger("Dead");
        }
    }
}
