using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneration : MonoBehaviour
{
    public Classes.Map map;
    public int num;


    void Start()
    {
        map = GenerateMap(num);
    }

    //private Enumirators.RoomType PickRandomRoomType()
    //{

    //}

    private Classes.Map GenerateMap(int numRooms)
    {
        Classes.Map generatedMap = new Classes.Map();
        Classes.Room startMap = new Classes.Room { id = 0 };
        generatedMap.rooms.Add(startMap);

        for (int i = 1; i < numRooms; i++)
        {
            Classes.Room newRoom = new Classes.Room { id = i };

            int randomNeighborCount = Random.Range(1, 3);
            for (int j = 0; j < randomNeighborCount; j++)
            {
                Classes.Room randomNeighbor = generatedMap.rooms[Random.Range(0, generatedMap.rooms.Count)];
                if (randomNeighbor.neighbourRooms.Count < 3 && !randomNeighbor.neighbourRooms.Contains(newRoom))
                {
                    newRoom.neighbourRooms.Add(randomNeighbor);
                    randomNeighbor.neighbourRooms.Add(newRoom);
                }
            }

            generatedMap.rooms.Add(newRoom);
        }

        return generatedMap;
    }

    void Update()
    {
        
    }
}
