using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class FollowPlayer : NetworkBehaviour
{

    #region variables

    [SerializeField] private Transform player;
    [SerializeField] private float glideSpeed = 2f;

    #endregion
    
    public void SetPlayer()
    {
        List<GameObject> playerList = GameObject.FindGameObjectsWithTag("Player").ToList();
        foreach (GameObject player in playerList)
        {
            if(player.GetComponent<PlayerMovement>().OwnerClientId == OwnerClientId)
            {
                this.player = player.transform;
                break;
            }
        }
    }

    void Update()
    {
       if(player != null)
        {
            float interpolation = glideSpeed * Time.deltaTime;

            Vector3 position = this.transform.position;
            position.y = Mathf.Lerp(this.transform.position.y, player.position.y, interpolation);
            position.x = Mathf.Lerp(this.transform.position.x, player.position.x, interpolation);

            this.transform.position = position;
        }
    }
}
