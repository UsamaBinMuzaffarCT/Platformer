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

    [Serializable]
    public class RoomObjects
    {
        public Enumirators.RoomType roomType;
        public GameObject room;
        public int numDoors;
    }

    [Serializable]
    public class Room
    {
        public Enumirators.RoomType roomType;
        [SerializeField] public bool visited = false;
        public int id;
        public List<Room> neighbourRooms = new List<Room>();
        public GameObject room;
        public List<GameObject> teleportationPoints;
    }

    [Serializable]
    public class Map
    {
        public List<Room> rooms = new List<Room>();
    }
}
