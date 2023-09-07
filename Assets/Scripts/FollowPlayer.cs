using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{

    #region variables

    [SerializeField] private Transform player;
    [SerializeField] private float glideSpeed = 2f;

    #endregion
    
    public void SetPlayer(Transform followTransform)
    {
        player = followTransform;
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
