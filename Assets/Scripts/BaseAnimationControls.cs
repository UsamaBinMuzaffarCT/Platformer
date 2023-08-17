using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAnimationControls : MonoBehaviour
{
    #region variables

    #region public-variables

    #endregion

    #region protected-variables

    [SerializeField] protected GameObject playerSprite;
    [SerializeField] protected Animator animator;

    protected bool isFilpped = false;

    [SerializeField] protected string currentState;

    protected PlayerMovement playerMovement;
    protected float lockedTill;

    #endregion

    #endregion

    #region funcitons

    #region public-functions

    #endregion

    #region private-functions

    protected virtual void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        animator = GetComponentInChildren<Animator>();
        playerSprite = GetComponentInChildren<SpriteRenderer>().gameObject;

        if (playerMovement.faction == Enumirators.Faction.Gunman)
        {
            animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("PlayerAnimatorControllers/GunMan");
        }
        else if(playerMovement.faction == Enumirators.Faction.Warrior)
        {
            animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("PlayerAnimatorControllers/PixelCharAnim_Sword_idle_0");
        }
        else if(playerMovement.faction == Enumirators.Faction.Mage)
        {
            animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("PlayerAnimatorControllers/PixelCharAnim_Plain_idle_0");
        }
    }

    protected void PlayAnimation(string state)
    {
        if (state == currentState)
        {
            return;
        }
        animator.Play(state);
        currentState = state;
    }

    protected string LockState(string state, float t) 
    {
        lockedTill = Time.time + t;
        return state;
    }

    protected abstract string GetAnimationState();



    #endregion

    #endregion
}
