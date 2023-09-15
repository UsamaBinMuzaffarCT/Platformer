using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerFactionMenuSelection : MonoBehaviour
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
        SetDefaultFaction();
    }

    private void SetDefaultFaction()
    {
        PlayerInfoHolder.Instance.playerFaction = Enumirators.Faction.Mage;
        NetworkManagement.Instance.AddFactionInfoToListsServerRpc(NetworkManager.Singleton.LocalClientId, 1);
    }

    private void SetAnimator(RuntimeAnimatorController controller)
    {
        animator.runtimeAnimatorController = controller;
        PlayerInfoHolder.Instance.playerFaction = playerFaction;
    }

    private void SetWarriorFaction()
    {
        playerFaction = Enumirators.Faction.Warrior;
        SetAnimator(warriorAnimatorController);
        NetworkManagement.Instance.AddFactionInfoToListsServerRpc(NetworkManager.Singleton.LocalClientId, 0);
    }

    private void SetGunmanFaction()
    {
        playerFaction = Enumirators.Faction.Gunman;
        SetAnimator(gunmanAnimatorController);
        NetworkManagement.Instance.AddFactionInfoToListsServerRpc(NetworkManager.Singleton.LocalClientId, 2);
    }

    private void SetMageFaction()
    {
        playerFaction = Enumirators.Faction.Mage;
        SetAnimator(mageAnimatorController);
        NetworkManagement.Instance.AddFactionInfoToListsServerRpc(NetworkManager.Singleton.LocalClientId, 1);
    }

    private void StartGame()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("SampleScene",LoadSceneMode.Single);
    }

    #endregion
}
