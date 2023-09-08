using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health;
    [SerializeField] private float hitTimer;
    private PlayerMovement playerMovement;
    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        hitTimer -= Time.deltaTime;
        if(health <= 0)
        {
            playerMovement.isDead = true;
            playerMovement.n_isDead.Value = true;
        }
    }
    public void TakeDamage(int damage)
    {
        if (health > 0 && hitTimer < 0)
        {
            health -= damage;
            GetComponent<PlayerMovement>().knockBackTimer = 0.2f;
            hitTimer = 0.5f;
        }
    }
}
