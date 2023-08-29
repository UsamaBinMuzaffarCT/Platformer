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

    protected AnimatorsScriptable animatorsScriptable;

    #endregion

    #endregion

    #region funcitons

    #region public-functions

    #endregion

    #region protected-functions

    protected virtual void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerSprite = GameObject.FindGameObjectWithTag("PlayerSprite");
        animator = playerSprite.GetComponent<Animator>();
        animatorsScriptable = Resources.Load<AnimatorsScriptable>("ScriptableObjects/AnimatorsScriptableObject");

        if (playerMovement.faction == Enumirators.Faction.Gunman)
        {
            animator.runtimeAnimatorController = animatorsScriptable.factionAnimators.Find(x => x.faction == Enumirators.Faction.Gunman).controller;
        }
        else if(playerMovement.faction == Enumirators.Faction.Warrior)
        {
            animator.runtimeAnimatorController = animatorsScriptable.factionAnimators.Find(x => x.faction == Enumirators.Faction.Warrior).controller;
        }
        else if(playerMovement.faction == Enumirators.Faction.Mage)
        {
            animator.runtimeAnimatorController = animatorsScriptable.factionAnimators.Find(x => x.faction == Enumirators.Faction.Mage).controller;
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
