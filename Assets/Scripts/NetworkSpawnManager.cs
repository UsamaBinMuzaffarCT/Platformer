using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class NetworkSpawnManager : NetworkBehaviour
{
    public static NetworkSpawnManager instance { get; private set; }

    private void Awake()
    {
        instance = this;
    }

    public UnityEvent onProjectileDestroy;

    public void DestroyProjectile()
    {
        onProjectileDestroy?.Invoke();
    }
}
