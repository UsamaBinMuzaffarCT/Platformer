using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class Boss : NetworkBehaviour
{
    List<GameObject> players;

    public bool isFlipped = false;

    public override void OnNetworkSpawn()
    {
        players = GameObject.FindGameObjectsWithTag("Player").ToList();
    }

    private GameObject ClosestPlayer()
    {
        float minDistance = float.PositiveInfinity;
        GameObject minDistancePlayer = null;
        foreach (GameObject player in players)
        {
            float thisDistance = Vector2.Distance(player.transform.position, transform.position);
            if (thisDistance < minDistance)
            {
                minDistance = thisDistance;
                minDistancePlayer = player;
            }
        }
        return minDistancePlayer;
    }

    public void LookAtPlayer(GameObject player)
    {
        if(players.Count == 0)
        {
            return;
        }
        Vector3 flipped = transform.localScale;
        flipped.z *= -1f;

        if (transform.position.x > player.transform.position.x && isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = false;
        }
        else if (transform.position.x < player.transform.position.x && !isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = true;
        }
    }

}