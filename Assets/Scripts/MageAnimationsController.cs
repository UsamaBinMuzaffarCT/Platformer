using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageAnimationsController : BaseAnimationControls
{
    #region variables

    #region private-variables
    
    //private string idle = "PixelCharAnim_Plain_idle";
    //private string run = "PixelCharAnim_Plain_run";
    //private string jump = "PixelCharAnim_Plain_jump";
    //private string dash = "PixelCharAnim_Plain_wallRide";
    //private string backDash = "PixelCharAnim_Plain_slide";
    //private string death = "PixelCharAnim_Plain_death";

    private string idle = "Mage_idle";
    private string run = "Mage_run";
    private string jump = "Mage_jump";
    private string dash = "Mage_dash";
    private string attack = "Mage_attack";
    private string wallRide = "Mage_wallRide";
    private string backDash = "PixelCharAnim_Plain_slide";
    private string death = "Mage_death";

    #endregion

    #endregion

    protected override void Awake()
    {
        base.Awake();
    }

    private void Update()
    {
        PlayAnimation(GetAnimationState());
    }

    protected override string GetAnimationState()
    {
        if (Time.time < lockedTill)
        {
            return currentState;
        }
        if (playerMovement.isDead)
        {
            return LockState(death, 0.5f);
        }
        if (playerMovement.isAttacking)
        {
            return LockState(attack, 0.2f);
        }
        if (!playerMovement.isWallSliding && isFilpped)
        {
            playerSprite.transform.localScale = new Vector3(-playerSprite.transform.localScale.x, playerSprite.transform.localScale.y, playerSprite.transform.localScale.z);
            isFilpped = false;
        }
        if (playerMovement.isDashing)
        {
            if (playerMovement.isBackDashing)
            {
                return dash;
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
            return wallRide;
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
}
