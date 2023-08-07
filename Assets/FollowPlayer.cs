using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{

    #region variables

    [SerializeField] private Transform player;
    [SerializeField] private float glideSpeed = 2f;

    #endregion
    
    void Start()
    {
        
    }

    void Update()
    {
        float interpolation = glideSpeed * Time.deltaTime;

        Vector3 position = this.transform.position;
        position.y = Mathf.Lerp(this.transform.position.y, player.position.y, interpolation);
        position.x = Mathf.Lerp(this.transform.position.x, player.position.x, interpolation);

        this.transform.position = position;
    }
}
