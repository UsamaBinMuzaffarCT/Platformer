using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class Fireball : NetworkBehaviour
{
    public float speed = 8f;
    public int damage = 40;
    public Rigidbody2D rb;
    public bool fizzleOut = false;

    [SerializeField] private float shotDelay = 0.1f;
    private float timer;
    private NetworkObject networkObject;

    private GameObject player;

    public override void OnNetworkSpawn()
    {
        NetworkSpawnManager.instance.onProjectileDestroy.AddListener(DestroyFireball);
        networkObject = GetComponent<NetworkObject>();
        timer = 0;
    }

    private void Update()
    {
        if (fizzleOut)
        {
            DestroyFireball();
        }
        timer += Time.deltaTime;
    }

    public void DestroyFireball()
    {
        Destroy(gameObject);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyFireballServerRpc()
    {
        if (!IsServer)
        {
            DestroyFireballClientRpc();
        }
        else
        {
            networkObject.DontDestroyWithOwner = true;
            networkObject.Despawn();
        }
    }

    [ClientRpc]
    private void DestroyFireballClientRpc()
    {
        if (!IsOwner)
        {
            networkObject.DontDestroyWithOwner = true;
            networkObject.Despawn();
        }
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
