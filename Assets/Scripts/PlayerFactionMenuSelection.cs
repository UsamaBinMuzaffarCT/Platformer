using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerFactionMenuSelection : NetworkBehaviour
{
    #region variables
    
    [SerializeField] private Button warriorButton;
    [SerializeField] private Button mageButton;
    [SerializeField] private Button gunmanButton;
    [SerializeField] private Button startButton;
    [SerializeField] private RuntimeAnimatorController warriorAnimatorController;
    [SerializeField] private RuntimeAnimatorController mageAnimatorController;
    [SerializeField] private RuntimeAnimatorController gunmanAnimatorController;


    private Enumirators.Faction playerFaction;
    private Animator animator;

    #endregion

    #region functions

    private void Awake()
    {
        warriorButton.onClick.AddListener(SetWarriorFaction);
        mageButton.onClick.AddListener(SetMageFaction);
        gunmanButton.onClick.AddListener(SetGunmanFaction);
        startButton.onClick.AddListener(StartGame);
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        PlayerInfoHolder.Instance.playerFaction = Enumirators.Faction.Mage;
    }

    private void SetAnimator(RuntimeAnimatorController controller, int factionID)
    {
        animator.runtimeAnimatorController = controller;
        PlayerInfoHolder.Instance.playerFaction = playerFaction;
        PlayerInfoHolder.Instance.factionID = factionID;
    }

    private void SetWarriorFaction()
    {
        playerFaction = Enumirators.Faction.Warrior;
        SetAnimator(warriorAnimatorController,0);
    }

    private void SetGunmanFaction()
    {
        playerFaction = Enumirators.Faction.Gunman;
        SetAnimator(gunmanAnimatorController,2);
    }

    private void SetMageFaction()
    {
        playerFaction = Enumirators.Faction.Mage;
        SetAnimator(mageAnimatorController,1);
    }

    private void StartGame()
    {
        NetworkManager.SceneManager.LoadScene("SampleScene", UnityEngine.SceneManagement.LoadSceneMode.Single);

    }

    #endregion
}
