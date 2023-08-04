using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    #region variables

    #region public-variables

    public Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask groundLayer;

    #endregion

    #region private-variables

    [SerializeField] private GameObject playerCamera;

    private float horizontal;
    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpingPower = 16f;

    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float coyoteTimeCounter;

    [SerializeField] private float jumpBufferTime = 0.2f;
    [SerializeField] private float jumpBufferCounter;

    [SerializeField] private float fallTimer = 0.5f;

    private float gravity = 4.5f;
    private bool isWallSliding;
    private float wallSlidingSpeed = 2f;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;

    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 24f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 1f;

    [SerializeField] private bool jump = false;

    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.4f;
    private Vector2 wallJumpingPower = new Vector2(8f, 16f);

    private bool isFacingRight = true;

    #endregion

    #endregion

    #region coroutines

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
        jump = false;
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
            jump = true;
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jump = false;
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
        if (isDashing)
        {
            return;
        }
        if (isWallSliding)
        {
            horizontal = 0;
        }
        horizontal = context.ReadValue<Vector2>().x;
    }

    public void WallJump(InputAction.CallbackContext context)
    {
        if (isWallSliding)
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
        if (context.performed && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            Debug.Log(rb.velocity);
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
        }
        if (!isWallJumping)
        {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        }
    }
   
    private void WallSilde()
    {
        if(IsWalled() && !IsGrounded())
        {
            isWallSliding = true;
            rb.velocity = new Vector2(0, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
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

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.1f, wallLayer);
    }

    private bool IsGrounded()
    {
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