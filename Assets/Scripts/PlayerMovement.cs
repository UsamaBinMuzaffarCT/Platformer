using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    #region variables

    #region public-variables
    public Enumirators.Faction faction;
    public Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float repulsionForce = 0.05f;


    public Canvas playerCanvas;

    [HideInInspector] public bool isAttacking;
    [HideInInspector] public bool isIdle;
    [HideInInspector] public bool isRunning;
    [HideInInspector] public bool isJumping;
    [HideInInspector] public bool isDashing;
    [HideInInspector] public bool isBackDashing;
    [HideInInspector] public bool isWallSliding;
    [HideInInspector] public bool isDead;

    [HideInInspector] public bool map = false;

    [HideInInspector] public bool interact;
    [HideInInspector] public bool knockBackFromRight;
    [HideInInspector] public float knockBackTimer;
    [HideInInspector] public float knockBackForce;

    [SerializeField] private Button jumpButton;
    public event Action jumpActivity;

    #endregion

    #region private-variables

    [SerializeField] private Joystick joystick;
    //[SerializeField] private bl_Joystick joystick;

    private BaseAnimationControls animationControls;
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject fireball;
    [SerializeField] private GameObject gunBarrel;

    [SerializeField] private GameObject playerCamera;

    private bool otherInput = true;

    [SerializeField] private float horizontal;
    private float speed = 8f;
    private float fallSpeed = 15f;
    private float jumpingPower = 18f;

    private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    private float gravity = 4.5f;
    private float wallSlidingSpeed = 2f;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;

    private bool canDash = true;
    private float dashingPower = 24f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 0.5f;
    private Coroutine dashCoroutine = null;

    private float tempHorizontal;

    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.4f;
    private Vector2 wallJumpingPower = new Vector2(8f, 16f);

    private bool isFacingRight = true;
    private bool isMoving = false;

    private Coroutine edgeRepulsionCoroutine = null;
    private bool edge = false;

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
            GetComponentInChildren<SpriteRenderer>().color = Color.black;
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
            transform.GetComponent<Collider2D>().enabled = false;
            foreach (Transform child in transform)
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
            GetComponentInChildren<SpriteRenderer>().color = Color.white;
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

    public void Map(InputAction.CallbackContext context)
    {
        if (!isDead)
        {
            if (context.performed)
            {
                if (map)
                {
                    map = false;
                }
                else
                {
                    map = true;
                }
            }
        }
    }

    public void OnMapDown()
    {
        if (!isDead)
        {
            if (map)
            {
                map = false;
            }
            else
            {
                map = true;
            }
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (!isDead && !map)
        {
            if (context.performed)
            {
                interact = true;
            }
            if (context.canceled)
            {
                interact = false;
            }
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!isDead && !map)
            {
                isAttacking = true;
                Invoke(nameof(StopAttack), 0.1f);
                if (faction == Enumirators.Faction.Gunman)
                {
                    GameObject instantiatedBullet = Instantiate(bullet);
                    instantiatedBullet.transform.position = gunBarrel.transform.position;
                }
                if (faction == Enumirators.Faction.Mage)
                {
                    GameObject instantiatedBullet = Instantiate(fireball);
                    instantiatedBullet.transform.position = gunBarrel.transform.position;
                }
            }
        }
    }

    public void OnAttackDown()
    {
        if (!isDead && !map)
        {
            isAttacking = true;
            Invoke(nameof(StopAttack), 0.1f);
            if (faction == Enumirators.Faction.Gunman)
            {
                GameObject instantiatedBullet = Instantiate(bullet);
                instantiatedBullet.transform.position = gunBarrel.transform.position;
            }
            if (faction == Enumirators.Faction.Mage)
            {
                GameObject instantiatedBullet = Instantiate(fireball);
                instantiatedBullet.transform.position = gunBarrel.transform.position;
            }
        }
    }

    public void ShadowDash(InputAction.CallbackContext context)
    {
        if (faction.Equals(Enumirators.Faction.Mage))
        {
            if (context.performed && canDash)
            {
                if (!isDead && !map)
                {
                    if (dashCoroutine != null)
                    {
                        StopCoroutine(dashCoroutine);
                        dashCoroutine = null;
                    }
                    dashCoroutine = StartCoroutine(PerformShadowDash());
                }
            }
        }
    }

    public void ShadowBackDash(InputAction.CallbackContext context)
    {
        if (faction.Equals(Enumirators.Faction.Mage))
        {
            if (context.performed && canDash)
            {
                if (!isDead && !map)
                {
                    if (dashCoroutine != null)
                    {
                        StopCoroutine(dashCoroutine);
                        dashCoroutine = null;
                    }
                    dashCoroutine = StartCoroutine(PerformShadowDash(false));
                }
            }
        }
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash)
        {
            if (!isDead && !map)
            {
                if (dashCoroutine != null)
                {
                    StopCoroutine(dashCoroutine);
                    dashCoroutine = null;
                }
                dashCoroutine = StartCoroutine(PerformDash());
            }
        }
    }

    public void OnDashDown()
    {
        if (canDash)
        {
            if (!isDead && !map)
            {
                if (dashCoroutine != null)
                {
                    StopCoroutine(dashCoroutine);
                    dashCoroutine = null;
                }
                dashCoroutine = StartCoroutine(PerformDash());
            }
        }
    }

    public void BackDash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash)
        {
            if (!isDead && !map)
            {
                if (dashCoroutine != null)
                {
                    StopCoroutine(dashCoroutine);
                    dashCoroutine = null;
                }
                dashCoroutine = StartCoroutine(PerformDash(false));
            }
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
            if (!isDead && !map)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            }
        }

        if (context.canceled && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            coyoteTimeCounter = 0f;
        }
    }

    public void OnJumpDown()
    {
        if (isDashing)
        {
            return;
        }
        jumpBufferCounter = jumpBufferTime;

        if (coyoteTimeCounter > 0f)
        {
            if (!isDead && !map)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            }
        }
    }

    public void OnJumpUp()
    {
        if (rb.velocity.y > 0f)
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
        if (tempHorizontal > 0.1f)
        {
            tempHorizontal = 1f;
        }
        else if (tempHorizontal < -0.1f)
        {
            tempHorizontal = -1f;
        }
        else
        {
            tempHorizontal = 0;
        }
        if (context.canceled)
        {
            isMoving = false;
            isRunning = false;
            otherInput = true;
        }
        if (isDashing)
        {
            return;
        }
        if (context.performed)
        {
            otherInput = false;
            isMoving = true;
            isRunning = true;
            isIdle = false;
        }

        horizontal = context.ReadValue<Vector2>().x;
        if (horizontal > 0.1f)
        {
            horizontal = 1f;
        }
        else if (horizontal < -0.1f)
        {
            horizontal = -1f;
        }
        else
        {
            horizontal = 0;
        }
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

    public void OnWallJumpDown()
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
        if (wallJumpingCounter > 0f && !IsGrounded())
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

    private void Awake()
    {
        isDead = false;
        if (faction == Enumirators.Faction.Mage)
        {
            animationControls = gameObject.AddComponent<MageAnimationsController>();
        }
        else if (faction == Enumirators.Faction.Warrior)
        {
            animationControls = gameObject.AddComponent<WarriorAnimatorControls>();
        }
        else if (faction == Enumirators.Faction.Gunman)
        {
            animationControls = gameObject.AddComponent<GunManAnimationControls>();
        }
    }

    private void Start()
    {
        isIdle = true;
    }

    private void StopAttack()
    {
        isAttacking = false;
    }

    private void JoystickHorizontal()
    {
        if (otherInput)
        {
            tempHorizontal = joystick.Horizontal;
            if (isDashing)
            {
                return;
            }
            horizontal = joystick.Horizontal;
            if (horizontal > 0.2f)
            {
                isMoving = true;
                isRunning = true;
                isIdle = false;
                horizontal = 1f;
            }
            else if (horizontal < -0.2f)
            {
                isMoving = true;
                isRunning = true;
                isIdle = false;
                horizontal = -1f;
            }
            else
            {
                isMoving = false;
                isRunning = false;
            }
        }
    }

    void Update()
    {
        if (!isDead && !map)
        {
            JoystickHorizontal();
            if (horizontal != 0)
            {
                isRunning = true;
            }
            if (isDashing)
            {
                WallSilde();
                return;
            }
            jumpBufferCounter -= Time.deltaTime;
            if (IsGrounded())
            {
                if (horizontal == 0)
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
        else
        {
            GetComponentInChildren<SpriteRenderer>().color = Color.white;
        }
    }

    private void FixedUpdate()
    {
        if (!isDead && !map)
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
                if (knockBackTimer <= 0f)
                {
                    transform.GetComponentInChildren<SpriteRenderer>().color = Color.white;
                    rb.velocity = new Vector2(horizontal * speed, Mathf.Clamp(rb.velocity.y, -1 * fallSpeed, float.MaxValue));
                }
                else
                {
                    if (knockBackFromRight)
                    {
                        rb.velocity = new Vector2(-knockBackForce, knockBackForce);
                        transform.GetComponentInChildren<SpriteRenderer>().color = Color.red;
                    }
                    else
                    {
                        rb.velocity = new Vector2(knockBackForce, knockBackForce);
                        transform.GetComponentInChildren<SpriteRenderer>().color = Color.red;
                    }
                    knockBackTimer -= Time.deltaTime;
                }
            }
            horizontal = tempHorizontal;
        }
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

    private void TempMovementStop()
    {
        horizontal = tempHorizontal;
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