using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControls : MonoBehaviour
{
    #region variables

    #region public-variables

    #endregion

    #region private-variables
    [SerializeField] private GameObject playerSprite;
    [SerializeField] private Animator animator;

    private string idle = "PixelCharAnim_Plain_idle";
    private string run = "PixelCharAnim_Plain_run";
    private string jump = "PixelCharAnim_Plain_jump";
    private string dash = "PixelCharAnim_Plain_wallRide";
    private string backDash = "PixelCharAnim_Plain_slide";

    private bool isFilpped = false;

    [SerializeField] private string currentState;

    private PlayerMovement playerMovement;

    #endregion

    #endregion

    #region funcitons

    #region public-functions

    #endregion

    #region private-functions

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        currentState = idle;
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        string state = GetAnimationState();
        if (state == currentState)
        {
            return;
        }
        animator.Play(state);
        currentState = state;
    }

    private string GetAnimationState()
    {
        if (!playerMovement.isWallSliding && isFilpped)
        {
            playerSprite.transform.localScale = new Vector3(-playerSprite.transform.localScale.x, playerSprite.transform.localScale.y, playerSprite.transform.localScale.z);
            isFilpped= false;
        }
        if (playerMovement.isDashing)
        {
            if (playerMovement.isBackDashing)
            {
                return backDash;
            }
            else
            {
                return dash;
            }
        }
        if (playerMovement.isWallSliding)
        {
            if (!isFilpped)
            {
                playerSprite.transform.localScale = new Vector3(-playerSprite.transform.localScale.x, playerSprite.transform.localScale.y, playerSprite.transform.localScale.z);
                isFilpped = true;
            }
            return dash;
        }
        if (playerMovement.isJumping)
        {
            return jump;
        }
        if (playerMovement.isRunning)
        {
            return run;
        }
        return idle;
        
    }

    #endregion

    #endregion
}
