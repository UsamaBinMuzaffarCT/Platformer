using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleportation : MonoBehaviour
{
    private GameManager gameManager;

    public GameObject next = null;

    private void Awake()
    {
        gameManager = GameObject.FindWithTag("GameManager").transform.GetComponent<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.CompareTag("Player"))
        {
            gameManager.TeleportToNextRoom(next);
        }
    }
}
