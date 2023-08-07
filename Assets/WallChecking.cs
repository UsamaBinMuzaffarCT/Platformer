using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallChecking : MonoBehaviour
{

    #region variables

    [SerializeField] public bool isWalled;

    #endregion
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Wall") || collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isWalled = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Wall") || collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isWalled = false;
        }
    }
}
