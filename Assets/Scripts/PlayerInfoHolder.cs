using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerInfoHolder : NetworkBehaviour
{
    public static PlayerInfoHolder Instance { get; private set; }
    public Enumirators.Faction playerFaction = Enumirators.Faction.Mage;
    public int factionID = 1;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }
}
