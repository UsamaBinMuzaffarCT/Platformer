using System.Collections.Generic;
using UnityEngine;

public class MapGeneration : MonoBehaviour
{
    public Classes.Map map;
    public int num;

    private RoomsScriptable roomsScriptable;

    public void Initialize(int generationSeed)
    {
        Random.InitState(generationSeed);
        roomsScriptable = Resources.Load<RoomsScriptable>("ScriptableObjects/RoomsScriptable");
        map = GenerateMap(num);
        map = ClearTraversal(map);
        InstantiateRooms();
        MakeRoomConnections();
    }

    private void RemoveRoomConnection(Classes.Room room)
    {
        foreach (Transform child in room.room.transform)
        {
            if(child.tag == "TeleportationPoint")
            {
                room.teleportationPoints.Remove(child.gameObject);
                Destroy(child.gameObject);
                if(room.teleportationPoints.Count == room.neighbourRooms.Count)
                {
                    return;
                }
            }
        }
    }

    private void MakeRoomConnections()
    {
        foreach(Classes.Room room in map.rooms)
        {
            int teleportIndex = 0;
            int neighbourIndex = 0;
            while(teleportIndex < room.teleportationPoints.Count)
            {
                if (room.teleportationPoints[teleportIndex].GetComponent<Teleportation>().next == null)
                {
                    while (neighbourIndex < room.neighbourRooms.Count)
                    {
                        bool added = true;
                        for (int i = 0; i < room.neighbourRooms[neighbourIndex].teleportationPoints.Count; i++)
                        {
                            if (room.neighbourRooms[neighbourIndex].teleportationPoints[i].GetComponent<Teleportation>().next == null)
                            {
                                room.neighbourRooms[neighbourIndex].teleportationPoints[i].GetComponent<Teleportation>().next = room.teleportationPoints[teleportIndex];
                                room.teleportationPoints[teleportIndex].GetComponent<Teleportation>().next = room.neighbourRooms[neighbourIndex].teleportationPoints[i];
                                teleportIndex++;
                                neighbourIndex++;
                                added = false;
                                break;
                            }
                        }
                        if (added)
                        {
                            neighbourIndex++;
                        }
                    }
                    teleportIndex++;
                }
                else
                {
                    teleportIndex++;
                } 
            }
        }
    }

    private Enumirators.RoomType RandomRoomType()
    {
        int randomNum = Random.Range(0, 4);
        if(randomNum == 0)
        {
            return Enumirators.RoomType.OpenRoom;
        }
        else if(randomNum == 1)
        {
            return Enumirators.RoomType.DungeonRoom;
        }
        else
        {
            return Enumirators.RoomType.MiscRoom;
        }
    }

    private void InstantiateRooms()
    {
        foreach(Classes.Room room in map.rooms)
        {
            if(room.id == -1)
            {
                room.roomType = Enumirators.RoomType.StartRoom;
            }
            else if (room.id == -2)
            {
                room.roomType = Enumirators.RoomType.BossRoom;
            }
            else
            {
                room.roomType = RandomRoomType();
            }
            room.room = Instantiate(roomsScriptable.GetRandomRoom(room.roomType));
            room.room.SetActive(false);
            room.room.GetComponent<RoomConnections>().id = room.id;
            room.teleportationPoints = room.room.GetComponent<RoomConnections>().teleportationPoints;
            room.room.transform.SetParent(transform, false);
            if (room.neighbourRooms.Count < room.teleportationPoints.Count)
            {
                RemoveRoomConnection(room);
            }
        }
    }

    private void Traverse(Classes.Room room)
    {
        if (room.visited)
        {
            return;
        }
        room.visited = true;
        foreach(Classes.Room neighbor in room.neighbourRooms)
        {
            Traverse(neighbor);
        }
    }

    private bool TraversalPossible(Classes.Map map)
    {
        Traverse(map.rooms[0]);
        foreach(Classes.Room room in map.rooms)
        {
            if (!room.visited)
            {
                return false;
            }
        }
        return true;
    }

    private Classes.Map ClearTraversal(Classes.Map map)
    {
        foreach(Classes.Room room in map.rooms)
        {
            room.visited = false;
        }
        return map;
    }

    private Classes.Map ConnectUnraachablePaths(Classes.Map map)
    {
        while (!TraversalPossible(map))
        {
            foreach(Classes.Room room in map.rooms)
            {
                if (!room.visited)
                {
                    foreach(Classes.Room room2 in map.rooms)
                    {
                        if(room2.visited && room2.neighbourRooms.Count < 3)
                        {
                            room.neighbourRooms.Add(room2);
                            room2.neighbourRooms.Add(room);
                        }
                    }
                }
            }
            map = ClearTraversal(map);
        }
        return map;
    }

    private Classes.Map RemoveUnreachableNodes(Classes.Map map)
    {
        List<int> removeIds = new List<int>();
        for (int i = 0; i < map.rooms.Count; i++)
        {
            if (map.rooms[i].neighbourRooms.Count == 0)
            {
                removeIds.Add(map.rooms[i].id);
            }
        }

        foreach (int id in removeIds)
        {
            foreach (Classes.Room room in map.rooms)
            {
                if (id == room.id)
                {
                    map.rooms.Remove(room);
                    break;
                }
            }
        }
        return map;
    }

    private Classes.Room RemoveUnnecessaryConnections(Classes.Room room)
    {
        while(room.neighbourRooms.Count > 3)
        {
            foreach (Classes.Room neighbor in room.neighbourRooms)
            {
                if (neighbor.neighbourRooms.Count > 2)
                {
                    neighbor.neighbourRooms.Remove(room);
                    room.neighbourRooms.Remove(neighbor);
                    break;
                }
            }
        }
        return room;
    }
    private Classes.Map RemoveOverConnections(Classes.Map map)
    {
        for(int i = 0; i < map.rooms.Count; i++)
        {
            if (map.rooms[i].neighbourRooms.Count > 3)
            {
                map.rooms[i] = RemoveUnnecessaryConnections(map.rooms[i]);
            }
        }
        return map;
    }

    

    private Classes.Map ConnectSingleConnections(Classes.Map map)
    {
        for (int i = 0; i < map.rooms.Count; i++)
        {
            if(map.rooms[i].neighbourRooms.Count == 1)
            {
                int minNode = -1;
                int minCount = 3;
                for (int j = 0; j < map.rooms.Count; j++)
                {
                    if(j == i)
                    {
                        continue;
                    }
                    if (map.rooms[j].neighbourRooms.Count < minCount)
                    {
                        minNode = j;
                        minCount = map.rooms[j].neighbourRooms.Count;
                    }
                }
                if(minNode != -1)
                {
                    map.rooms[i].neighbourRooms.Add(map.rooms[minNode]);
                    map.rooms[minNode].neighbourRooms.Add(map.rooms[i]);
                }
            }
        }
        return map;
    }

    private Classes.Map AddStartAndEndNodes(Classes.Map map)
    {
        Classes.Room startRoom = new Classes.Room { id = -1 };
        int minNode = -1;
        int minCount = 3;
        for (int j = 0; j < map.rooms.Count; j++)
        {
            if (map.rooms[j].neighbourRooms.Count < minCount)
            {
                minNode = j;
                minCount = map.rooms[j].neighbourRooms.Count;
            }
        }
        if (minNode != -1)
        {
            startRoom.neighbourRooms.Add(map.rooms[minNode]);
            map.rooms[minNode].neighbourRooms.Add(startRoom);
        }
        Classes.Room endNode = new Classes.Room { id = -2 };
        minNode = -1;
        minCount = 3;
        for (int j = 0; j < map.rooms.Count; j++)
        {
            if (map.rooms[j].neighbourRooms.Count < minCount)
            {
                minNode = j;
                minCount = map.rooms[j].neighbourRooms.Count;
            }
        }
        if (minNode != -1)
        {
            endNode.neighbourRooms.Add(map.rooms[minNode]);
            map.rooms[minNode].neighbourRooms.Add(endNode);
        }
        map.rooms.Add(startRoom);
        map.rooms.Add(endNode);
        return map;
    }

    private void AssignLevel(Classes.Room room, int level)
    {
        room.level = level;
        foreach(Classes.Room neighbour in room.neighbourRooms)
        {
            if(neighbour.level == -1)
            {
                AssignLevel(neighbour, level + 1);
            }
        }
    }

    private Classes.Map SetLevelsFromStartingNode(Classes.Map map, Classes.Room startingNode)
    {
        foreach (var room in map.rooms)
        {
            room.level = int.MaxValue;
        }

        startingNode.level = 0;

        Queue<Classes.Room> queue = new Queue<Classes.Room>();
        queue.Enqueue(startingNode);

        while (queue.Count > 0)
        {
            Classes.Room currentRoom = queue.Dequeue();

            foreach (var neighbor in currentRoom.neighbourRooms)
            {
                if (neighbor.level == int.MaxValue)
                {
                    neighbor.level = currentRoom.level + 1;
                    queue.Enqueue(neighbor);
                }
            }
        }
        return map;
    }

    private Classes.Map GenerateMap(int numRoom)
    {
        Classes.Map generatedMap = new Classes.Map();
        Classes.Room startRoom = new Classes.Room { id = 0 };
        generatedMap.rooms.Add(startRoom);

        for (int i = 1; i < numRoom; i++)
        {
            Classes.Room newRoom = new Classes.Room { id = i };

            int randomNeighborCount = Random.Range(0,3);
            for (int j = 0; j < randomNeighborCount; j++)
            {
                Classes.Room randomNeighbor = generatedMap.rooms[Random.Range(0, generatedMap.rooms.Count)];
                if(randomNeighbor.neighbourRooms.Count > 2)
                {
                    foreach(Classes.Room room in generatedMap.rooms)
                    {
                        if(room.neighbourRooms.Count > randomNeighbor.neighbourRooms.Count)
                        {
                            randomNeighbor = room;
                            break;
                        }
                    }
                }
                if (randomNeighbor.neighbourRooms.Count < 2 && !randomNeighbor.neighbourRooms.Contains(newRoom))
                {
                    newRoom.neighbourRooms.Add(randomNeighbor);
                    randomNeighbor.neighbourRooms.Add(newRoom);
                }
            }
            generatedMap.rooms.Add(newRoom);
        }
        generatedMap = RemoveUnreachableNodes(generatedMap);
        generatedMap = ConnectUnraachablePaths(generatedMap);
        generatedMap = RemoveOverConnections(generatedMap);
        generatedMap = ConnectSingleConnections(generatedMap);
        generatedMap = AddStartAndEndNodes(generatedMap);
        foreach(Classes.Room room in generatedMap.rooms)
        {
            if(room.id == -1)
            {
                generatedMap = SetLevelsFromStartingNode(generatedMap, room);
                break;
            }
        }
        return generatedMap;
    }
}
