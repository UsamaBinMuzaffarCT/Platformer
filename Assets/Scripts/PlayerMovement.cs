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

    public bool isIdle;
    public bool isRunning;
    public bool isJumping;
    public bool isDashing;
    public bool isBackDashing;
    public bool isWallSliding;

    #endregion

    #region private-variables


    [SerializeField] private Animator animator;

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
    private float wallSlidingSpeed = 2f;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;

    private bool canDash = true;
    private float dashingPower = 24f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 0.5f;
    private Coroutine dashCoroutine = null;

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
    [SerializeField] private bool edge = false;

    #endregion

    #endregion

    #region coroutines

    private IEnumerator RepulseFromEdge()
    {
        for (int i = 0; i < 90; i++)
        {
            if (rb.velocity.y > 0)
            {
                if (edge)
                {
                    rb.AddForce(new Vector2(0, 1f));
                }
            }
        }
        yield return null;
    }

    private IEnumerator PerformShadowDash(bool forward = true)
    {
        if (!isWallSliding)
        {

            canDash = false;
            isDashing = true;
            float gravity = rb.gravityScale;
            rb.gravityScale = 0f;
            if (forward)
            {
                rb.velocity = new Vector2((transform.localScale.x/ Mathf.Abs(transform.localScale.x)) * dashingPower, 0f);
            }
            else
            {
                isBackDashing = true;
                rb.velocity = new Vector2(-1 * (transform.localScale.x / Mathf.Abs(transform.localScale.x)) * dashingPower, 0f);
            }
            transform.GetComponent<Collider2D>().enabled = false;
            foreach(Transform child in transform)
            {
                try
                {
                    child.GetComponent<Collider2D>().enabled = false;
                }
                catch 
                {
                    continue;
                }
            }
            yield return new WaitForSeconds(dashingTime);
            transform.GetComponent<Collider2D>().enabled = true;
            foreach (Transform child in transform)
            {
                try
                {
                    child.GetComponent<Collider2D>().enabled = true;
                }
                catch
                {
                    continue;
                }
            }
            rb.gravityScale = gravity;
            isDashing = false;
            isBackDashing = false;
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

    private IEnumerator PerformDash(bool forward = true)
    {
        if (!isWallSliding)
        {
            canDash = false;
            isDashing = true;
            float gravity = rb.gravityScale;
            rb.gravityScale = 0f;
            if (forward)
            {
                rb.velocity = new Vector2((transform.localScale.x / Mathf.Abs(transform.localScale.x)) * dashingPower, 0f);
            }
            else
            {
                isBackDashing = true;
                rb.velocity = new Vector2(-1 * (transform.localScale.x / Mathf.Abs(transform.localScale.x)) * dashingPower, 0f);
            }
            yield return new WaitForSeconds(dashingTime);
            rb.gravityScale = gravity;
            isDashing = false;
            isBackDashing = false;
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

    public void ShadowDash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash)
        {
            if(dashCoroutine != null)
            {
                StopCoroutine(dashCoroutine);
                dashCoroutine = null;
            }
            dashCoroutine = StartCoroutine(PerformShadowDash());
        }
    }

    public void ShadowBackDash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash)
        {
            if (dashCoroutine != null)
            {
                StopCoroutine(dashCoroutine);
                dashCoroutine = null;
            }
            dashCoroutine = StartCoroutine(PerformShadowDash(false));
        }
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash)
        {
            if (dashCoroutine != null)
            {
                StopCoroutine(dashCoroutine);
                dashCoroutine = null;
            }
            dashCoroutine = StartCoroutine(PerformDash());
        }
    }

    public void BackDash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash)
        {
            if (dashCoroutine != null)
            {
                StopCoroutine(dashCoroutine);
                dashCoroutine = null;
            }
            dashCoroutine = StartCoroutine(PerformDash(false));
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

        if (!context.canceled)
        {
            
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
            isRunning = false;
        }
        if (isDashing)
        {
            return;
        }
        if (context.performed)
        {
            isMoving = true;
            isRunning = true;
            isIdle = false;
        }

        horizontal = context.ReadValue<Vector2>().x;
    }

    public void WallJump(InputAction.CallbackContext context)
    {
        if (IsWalled())
        {
            isWallJumping = false;
            wallJumpingDirection = -1 * (transform.localScale.x / Mathf.Abs(transform.localScale.x));
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

            if ((transform.localScale.x / Mathf.Abs(transform.localScale.x)) != wallJumpingDirection)
            {
                isFacingRight = !isFacingRight;
                transform.localScale = new Vector3(-1 * transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    #endregion

    #region private-functions

    private void Start()
    {
        isIdle = true;   
    }

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
            if(horizontal == 0)
            {
                isIdle = true;
            }
            if (jumpBufferCounter > 0f)
            {
                PerformJump();
            }
            coyoteTimeCounter = coyoteTime;
            isJumping = false;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
            isJumping = true;
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
        if (isWallSliding)
        {
            horizontal = 0;
            return;
        }
        if (IsSideGrounded())
        {
            rb.velocity = new Vector2(0, Mathf.Clamp(rb.velocity.y, -1 * fallSpeed, float.MaxValue));
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
        if (IsWalled() && !IsGrounded())
        {
            isRunning = false;
            isWallSliding = true;
            if (isMoving)
            {
                rb.velocity = new Vector2(0, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
            }
        }
        else
        {
            if (!isDashing)
            {
                rb.gravityScale = gravity;
            }
            isWallSliding = false;
            if (isMoving)
            {
                isRunning = true;
            }
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
            edge = true;
            if (edgeRepulsionCoroutine != null)
            {
                StopCoroutine(edgeRepulsionCoroutine);
                edgeRepulsionCoroutine = null;
            }
            edgeRepulsionCoroutine = StartCoroutine(RepulseFromEdge());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Edge")
        {
            edge = false;
            rb.AddForce(new Vector2(0, -20f));
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
        isJumping = false;
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