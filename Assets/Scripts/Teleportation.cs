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

    private void Update()
    {
        //if(GameManager.Instance.teleportationTimer > 0f)
        //{
        //    return;
        //}
        //foreach(GameObject player in NetworkManagement.Instance.players)
        //{
        //    playerCount += player.GetComponent<PlayerMovement>().n_teleportSet.Value;
        //}
        //if (playerCount == GameManager.Instance.n_connectedClientCount.Value)
        //{
        //    playerCount = 0;
        //    gameManager.TeleportToNextRoom(next);
        //    foreach (GameObject player in NetworkManagement.Instance.players)
        //    {
        //        player.GetComponent<PlayerMovement>().n_teleportSet.Value = 0;
        //    }
        //    return;
        //}
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
