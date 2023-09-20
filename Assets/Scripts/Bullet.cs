using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour {

	public float speed = 20f;
	public int damage = 40;
	public Rigidbody2D rb;
	public GameObject impactEffect;

	private GameObject player;

    private void Awake()
    {
		List<GameObject> players = GameObject.FindGameObjectsWithTag("Player").ToList();

        player = players.Find(x=>x.GetComponent<PlayerMovement>().IsOwner);
    }

    void Start () {
		rb.velocity = (player.transform.localScale.x/math.abs(player.transform.localScale.x)) * transform.right * speed;
	}

	void OnTriggerEnter2D (Collider2D hitInfo)
	{
		Instantiate(impactEffect, transform.position, transform.rotation);
		if (hitInfo.CompareTag("Boss"))
		{
			hitInfo.transform.GetComponentInParent<Animator>().GetComponent<BossHealth>().TakeDamage(10);
		}
        if (hitInfo.CompareTag("Enemy"))
        {
            hitInfo.transform.GetComponentInParent<Animator>().GetComponent<EnemyHealth>().TakeDamage(10);
        }

        Destroy(gameObject);
	}
	
}
