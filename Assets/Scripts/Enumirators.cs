using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enumirators : MonoBehaviour
{
    public enum EnemyType
    {
        Patrolling,
        Stationary
    }
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
        OpenRoom,
        MiscRoom
    }

    public enum QuestType
    {
        Fetch,
        Kill,
        Rescue
    }
}
