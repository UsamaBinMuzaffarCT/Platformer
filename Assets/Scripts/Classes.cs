using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Classes : MonoBehaviour
{
    [Serializable]
    public class FactionAnimator
    {
        public Enumirators.Faction faction;
        public RuntimeAnimatorController controller;
    }

    [Serializable]
    public class BossExtras
    {
        public GameObject extraAttackEffect;
        public Enumirators.Boss boss;
    }

    public class Room
    {
        public Enumirators.RoomType roomType;
        public bool visited = false;
        public int id;
        public List<Room> neighbourRooms;
    }

    public class Map
    {
        public List<Room> rooms;
    }
}
