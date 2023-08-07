using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    #region variables

    #region public-variables

    public Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float repulsionForce = 0.05f;

    #endregion

    #region private-variables

    [SerializeField] private GameObject playerCamera;

    private float horizontal;
    [SerializeField] private float speed = 8f;
    [SerializeField] private float fallSpeed = 11f;
    [SerializeField] private float jumpingPower = 16f;

    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float coyoteTimeCounter;

    [SerializeField] private float jumpBufferTime = 0.2f;
    [SerializeField] private float jumpBufferCounter;

    private float gravity = 4.5f;
    private bool isWallSliding;
    private float wallSlidingSpeed = 2f;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;

    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 24f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 0.5f;

    [SerializeField] private float tempHorizontal;

    [SerializeField] private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.4f;
    private Vector2 wallJumpingPower = new Vector2(8f, 16f);

    private bool isFacingRight = true;
    [SerializeField] private bool isMoving = false;

    private Coroutine edgeRepulsionCoroutine = null;

    #endregion

    #endregion

    #region coroutines

    private IEnumerator RepulseFromEdge()
    {
        for(int i = 0; i < 70; i++)
        { 
            if(rb.velocity.y > 0)
            {
                rb.AddForce(new Vector2((transform.localScale.x * -1) * repulsionForce, 0.185f));
            }
        }
        yield return null;
    }

    private IEnumerator PerformDash()
    {
        if (!isWallSliding)
        {
            canDash = false;
            isDashing = true;
            float gravity = rb.gravityScale;
            rb.gravityScale = 0f;
            rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
            yield return new WaitForSeconds(dashingTime);
            rb.gravityScale = gravity;
            isDashing = false;
            yield return new WaitForSeconds(dashingCooldown);
            canDash = true;
        }
        else
        {
            rb.velocity = new Vector2(0f, 0f);
            yield return new WaitForSeconds(dashingTime);
            yield return new WaitForSeconds(dashingCooldown);
            canDash = true;
        }
    }

    #endregion

    #region functions

    #region public-functions

    public void Dash(InputAction.CallbackContext context)
    {
        if(context.performed && canDash)
        {
            StartCoroutine(PerformDash());
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (isDashing)
        {
            return;
        }
        if (context.performed)
        {
            jumpBufferCounter = jumpBufferTime;
        }

        if (context.performed && coyoteTimeCounter > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        }

        if (context.canceled && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            coyoteTimeCounter = 0f;
        }
    }

    public void PerformJump()
    {
        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            jumpBufferCounter = 0;
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        tempHorizontal = context.ReadValue<Vector2>().x;
        if (context.canceled)
        {
            isMoving = false;
        }
        else
        {
            isMoving = true;
        }
        if (isDashing)
        {
            return;
        }
        
        horizontal = context.ReadValue<Vector2>().x;
    }

    public void WallJump(InputAction.CallbackContext context)
    {
        if (IsWalled())
        {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }
        if (context.performed && wallJumpingCounter > 0f && !IsGrounded())
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;

            if (transform.localScale.x != wallJumpingDirection)
            {
                isFacingRight = !isFacingRight;
                transform.localScale = new Vector3(-1 * transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    #endregion

    #region private-functions

    void Update()
    {
        if (isDashing)
        {
            WallSilde();
            return;
        }
        jumpBufferCounter -= Time.deltaTime;
        if (IsGrounded())
        {
            if(jumpBufferCounter > 0f)
            {
                PerformJump();
            }
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (!isWallJumping)
        {
            if (!isFacingRight && horizontal > 0f)
            {
                Flip();
            }
            else if (isFacingRight && horizontal < 0f)
            {
                Flip();
            }
        }
        WallSilde();
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }
        if (isWallSliding )
        {
            horizontal = 0;
            return;
        }
        if (IsSideGrounded())
        {
            rb.velocity = new Vector2(0, Mathf.Clamp(rb.velocity.y, -1 * fallSpeed , float.MaxValue));
            horizontal = 0;
            return;
        }
        if (!isWallJumping)
        {
            rb.velocity = new Vector2(horizontal * speed, Mathf.Clamp(rb.velocity.y, -1 * fallSpeed, float.MaxValue));
        }
        horizontal = tempHorizontal;
    }
   
    
    private void WallSilde()
    {
        if(IsWalled() && !IsGrounded())
        {
            isWallSliding = true;
            if (isMoving)
            {
                rb.velocity = new Vector2(0, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
            }
        }
        else
        {
            rb.gravityScale = gravity;
            isWallSliding = false;
        }
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsGrounded() && collision.tag == "Edge")
        {
            if (edgeRepulsionCoroutine != null)
            {
                StopCoroutine(edgeRepulsionCoroutine);
                edgeRepulsionCoroutine = null;
            }
            edgeRepulsionCoroutine = StartCoroutine(RepulseFromEdge());
        }
        else if (!IsGrounded() && collision.tag == "Slide")
        {
            rb.AddForce(new Vector2((transform.localScale.x * -1) * repulsionForce, -0.5f));
        }
    }

    private bool IsSideGrounded()
    {
        return wallCheck.GetComponent<WallChecking>().isWalled;
    }

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.1f, wallLayer);
    }

    private bool IsGrounded()
    {
        wallJumpingCounter = 0;
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    #endregion

    #endregion
}