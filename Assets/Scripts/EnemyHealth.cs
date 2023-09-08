using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemyHealth : NetworkBehaviour
{

    public int health;

    private NetworkVariable<int> n_health = new NetworkVariable<int>(0, writePerm:NetworkVariableWritePermission.Owner);
    private Animator animator;
    [SerializeField] private float hitTimer;

    public override void OnNetworkSpawn()
    {
        n_health.Value = health;
        animator = GetComponent<Animator>();
    }

    private void DeathDestroy()
    {
        GetComponent<NetworkObject>().Despawn();
    }

    private void Update()
    {
        hitTimer -= Time.deltaTime;
        if (n_health.Value <= 0)
        {
            if(animator != null)
            {
                animator.SetTrigger("isDead");
            }
        }
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
        
    }
}
