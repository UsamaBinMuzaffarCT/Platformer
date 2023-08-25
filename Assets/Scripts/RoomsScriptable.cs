using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomsScriptable", menuName = "ScriptableObjects/RoomsScriptable", order = 1)]

public class RoomsScriptable : ScriptableObject
{
    public List<Classes.RoomObjects> roomObjects;

    public GameObject GetRandomRoom(Enumirators.RoomType roomType)
    {
        List<Classes.RoomObjects> matchingRooms = roomObjects.FindAll(room => room.roomType == roomType);

        if (matchingRooms.Count == 0)
        {
            Debug.LogWarning($"No rooms of type {roomType} found.");
            return null;
        }

        int randomIndex = Random.Range(0, matchingRooms.Count);
        return matchingRooms[randomIndex].room;
    }
}
