using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Boss : MonoBehaviour
{
    List<GameObject> players;

    public bool isFlipped = false;

    private void Awake()
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

    public void LookAtPlayer()
    {
        GameObject player = ClosestPlayer();
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