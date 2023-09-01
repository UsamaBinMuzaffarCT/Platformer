using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;

public class movement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float horizontal;
    [SerializeField] private float vertical;
    private Rigidbody2D rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
        vertical = context.ReadValue<Vector2>().y;
        //if(horizontal > 0.1)
        //{
        //    horizontal = 1;
        //}
        //else if(horizontal < -0.1)
        //{
        //    horizontal = -1;
        //}
        //else
        //{
        //    horizontal = 0;
        //}
        //vertical = context.ReadValue<Vector2>().y;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Horixontal : " + horizontal);
        Debug.Log("Vertical : " + vertical);
        rb.velocity = new Vector2(horizontal*speed, vertical*speed);
    }
}
