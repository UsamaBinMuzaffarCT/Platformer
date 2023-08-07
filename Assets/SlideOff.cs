using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideOff : MonoBehaviour
{
    [SerializeField] private Rigidbody2D parentRb;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Slide")
        {
            transform.GetComponentInParent<Rigidbody2D>().AddForce(new Vector2((transform.localScale.x * -1) * transform.GetComponentInParent<PlayerMovement>().repulsionForce, -0.5f));
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
