using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;
using UnityEngine.Assertions;

public class Enemy : NetworkBehaviour
{

    static string s_ObjectPoolTag = "ObjectPool";

    public static int numOfEnemies = 0;

    NetworkObjectPool m_ObjectPool;

    public NetworkVariable<int> Size = new NetworkVariable<int>(4);

    [SerializeField]
    private int m_NumCreates = 3;

    [HideInInspector]
    public GameObject enemyPrefab;

    void Awake()
    {
        m_ObjectPool = GameObject.FindWithTag(s_ObjectPoolTag).GetComponent<NetworkObjectPool>();
        Assert.IsNotNull(m_ObjectPool, $"{nameof(NetworkObjectPool)} not found in scene. Did you apply the {s_ObjectPoolTag} to the GameObject?");
    }

    // Use this for initialization
    void Start()
    {
        numOfEnemies += 1;
    }

    public override void OnNetworkSpawn()
    {
        var size = Size.Value;
        transform.localScale = new Vector3(size, size, size);
    }

    public void Explode()
    {
        if (NetworkObject.IsSpawned == false)
        {
            return;
        }

        Assert.IsTrue(NetworkManager.IsServer);

        numOfEnemies -= 1;

        var newSize = Size.Value - 1;

        if (newSize > 0)
        {
            int num = Random.Range(1, m_NumCreates + 1);

            for (int i = 0; i < num; i++)
            {
                int dx = Random.Range(0, 4) - 2;
                int dy = Random.Range(0, 4) - 2;
                Vector3 diff = new Vector3(dx * 0.3f, dy * 0.3f, 0);

                var go = m_ObjectPool.GetNetworkObject(enemyPrefab, transform.position + diff, Quaternion.identity);

                var enemy = go.GetComponent<Enemy>();
                enemy.Size.Value = newSize;
                enemy.enemyPrefab = enemyPrefab;
                go.GetComponent<NetworkObject>().Spawn();
                go.GetComponent<Rigidbody2D>().AddForce(diff * 10, ForceMode2D.Impulse);
            }
        }

        NetworkObject.Despawn(true);
    }
}
