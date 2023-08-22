using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Bullet : MonoBehaviour {

	public float speed = 20f;
	public int damage = 40;
	public Rigidbody2D rb;
	public GameObject impactEffect;

	private GameObject player;

    private void Awake()
    {
		player = GameObject.FindGameObjectWithTag("Player");
    }

    // Use this for initialization
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
		Destroy(gameObject);
	}
	
}
