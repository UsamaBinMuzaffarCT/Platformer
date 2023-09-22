using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AddingInteract : MonoBehaviour
{
    private PlayerMovement playerMovement;
    [SerializeField] private Button map;
    [SerializeField] private EventTrigger jump;
    [SerializeField] private EventTrigger dash;
    [SerializeField] private Button attack;
    [SerializeField] private Button interact;

    void Start()
    {
        playerMovement = GameManager.Instance.player.GetComponent<PlayerMovement>();
        
        // Map
        map.onClick.AddListener(playerMovement.OnMapDown);
        
        // Jump
        var onJumpPointerDown = new EventTrigger.Entry();
        onJumpPointerDown.eventID = EventTriggerType.PointerDown;
        onJumpPointerDown.callback.AddListener((e) => playerMovement.OnJumpDown());
        onJumpPointerDown.callback.AddListener((e)=> playerMovement.OnWallJumpDown());
        jump.triggers.Add(onJumpPointerDown);
        //var onJumpPointerUp = new EventTrigger.Entry();
        //onJumpPointerUp.eventID = EventTriggerType.PointerUp;
        //onJumpPointerDown.callback.AddListener((e) => playerMovement.OnJumpUp());
        //jump.triggers.Add(onJumpPointerUp);

        // Dash
        var onDashPointerDown = new EventTrigger.Entry();
        onDashPointerDown.eventID = EventTriggerType.PointerDown;
        onDashPointerDown.callback.AddListener((e) => playerMovement.OnDashDown());
        dash.triggers.Add(onDashPointerDown);

        // Attack
        attack.onClick.AddListener(playerMovement.OnAttackDown);

        // Interact
        interact.onClick.AddListener(playerMovement.Use);
    }
}
