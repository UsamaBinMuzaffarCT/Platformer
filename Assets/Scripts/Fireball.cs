using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 20f;
    public int damage = 40;
    public Rigidbody2D rb;

    private GameObject player;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Use this for initialization
    void Start()
    {
        rb.velocity = (player.transform.localScale.x / math.abs(player.transform.localScale.x)) * transform.right * speed;
    }

    private void DestroyFireball()
    {
        Destroy(gameObject);

    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        Invoke(nameof(DestroyFireball), 0.5f);
    }
}
