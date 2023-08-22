using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health;
    public void TakeDamage(int damage)
    {
        if (health > 0)
        {
            health -= damage;
            GetComponent<PlayerMovement>().knockBackTimer = 0.2f;
        }
        else
        {
            GetComponent<PlayerMovement>().isDead = true;
        }
    }
}
