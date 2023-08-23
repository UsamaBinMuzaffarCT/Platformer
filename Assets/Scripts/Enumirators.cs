using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enumirators : MonoBehaviour
{
    public enum Faction
    {
        Warrior,
        Mage,
        Gunman
    }

    public enum Boss
    {
        BringerOfDeath
    }

    public enum RoomType
    {
        StartRoom,
        BossRoom,
        DungeonRoom,
        OpenRoom
    }
}
