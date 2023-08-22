using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorAnimatorControls : BaseAnimationControls
{
    #region variables

    #region private-variables

    private string idle = "PixelCharAnim_Sword_idle";
    private string run = "PixelCharAnim_Sword_run";
    private string jump = "PixelCharAnim_Sword_jump";
    private string dash = "PixelCharAnim_Sword_wallRide";
    private string attack = "PixelCharAnim_Sword_quickAtk";
    private string backDash = "PixelCharAnim_Sword_slideAtk";
    private string death = "PixelCharAnim_Sword_death";

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
        if(Time.time < lockedTill)
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
}
