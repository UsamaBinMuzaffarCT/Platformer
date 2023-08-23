using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 20f;
    public int damage = 40;
    public Rigidbody2D rb;
    public bool fizzleOut = false;

    [SerializeField] private float shotDelay = 0.1f;
    private float timer;

    private GameObject player;

    private void Awake()
    {
        timer = 0;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Use this for initialization
    void Start()
    {
        rb.velocity = (player.transform.localScale.x / math.abs(player.transform.localScale.x)) * transform.right * speed;
        Vector3 localScale = transform.localScale;
        localScale.x *= player.transform.localScale.x / math.abs(player.transform.localScale.x);
        transform.localScale = localScale;
    }

    private void Update()
    {
        if (fizzleOut)
        {
            Destroy(gameObject);
        }
        timer += Time.deltaTime;
    }

    private void DestroyFireball()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if(timer > shotDelay)
        {
            rb.velocity = Vector2.zero;
            gameObject.GetComponent<Animator>().SetBool("Impact", true);
            if (hitInfo.CompareTag("Boss"))
            {
                hitInfo.transform.GetComponentInParent<Animator>().GetComponent<BossHealth>().TakeDamage(20);
            }
            if (hitInfo.CompareTag("Enemy"))
            {
                hitInfo.transform.GetComponentInParent<Animator>().GetComponent<EnemyHealth>().TakeDamage(20);
            }
            Invoke(nameof(DestroyFireball), 0.5f);
        }

    }
}
