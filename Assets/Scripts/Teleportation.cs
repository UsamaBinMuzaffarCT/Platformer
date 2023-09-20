using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Teleportation : MonoBehaviour
{
    private GameManager gameManager;
    private int playerCount = 0;
    public GameObject next = null;
    public int id;

    private void Awake()
    {
        gameManager = GameObject.FindWithTag("GameManager").transform.GetComponent<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.CompareTag("Player"))
        {
            if (collision.GetComponent<PlayerMovement>().IsOwner)
            {
                GameManager.Instance.IncrementTeleoprtationTouchCountServerRpc(next.transform.parent.GetComponent<RoomConnections>().id, next.GetComponent<Teleportation>().id);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            if (collision.GetComponent<PlayerMovement>().IsOwner)
            {
                GameManager.Instance.DeccrementTeleoprtationTouchCountServerRpc();
            }
        }
    }
}
