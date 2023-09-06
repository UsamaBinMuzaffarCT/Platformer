using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region variables

    #region private-variables

    private List<GameObject> players;
    private MapGeneration mapGenerator;
    private GameObject currentActiveRoom;
    [SerializeField] private float teleportationTimer;
    [SerializeField] private MapVisualization mapVisualization;
    [SerializeField] private List<GameObject> prefabsNPCs;

    #endregion

    #region public-variable

    public List<Classes.NPCQuest> nPCQuests = new List<Classes.NPCQuest>();

    #endregion

    #endregion

    #region functions

    #region private-functions

    private void Awake()
    {
        Enumirators.QuestType[] values = (Enumirators.QuestType[])Enum.GetValues(typeof(Enumirators.QuestType));
        for (int i = 0; i < Enum.GetValues(typeof(Enumirators.QuestType)).Length; i++)
        {
            nPCQuests.Add(new Classes.NPCQuest { questType = values[i], count = 0 });
        }
        players = GameObject.FindGameObjectsWithTag("Player").ToList<GameObject>();
        mapGenerator = GameObject.FindWithTag("Map").GetComponent<MapGeneration>();
    }

    private void Start()
    {
        CreateNPCs();
        TakePlayersToStartRoom();
    }



    private void Update()
    {
        teleportationTimer -= Time.deltaTime;
        //if (players[0].GetComponent<PlayerMovement>().map)
        //{
        //    players[0].GetComponent<PlayerMovement>().playerCanvas.gameObject.SetActive(true);
        //}
        //else
        //{
        //    players[0].GetComponent<PlayerMovement>().playerCanvas.gameObject.SetActive(false);
        //}
    }

    private bool ContainsNPC(GameObject room)
    {
        foreach(Transform child in room.transform)
        {
            if (child.CompareTag("NPC"))
            {
                return true;
            }
        }
        return false;
    }

    private void CreateNPCs()
    {
        List<GameObject> roomList = new List<GameObject>();
        foreach (Transform child in mapGenerator.gameObject.transform)
        {
            roomList.Add(child.gameObject);
        }
        foreach (GameObject npc in prefabsNPCs)
        {
            int random = UnityEngine.Random.Range(0, mapGenerator.GetComponent<Transform>().childCount);
            var roomConnections = roomList[random].GetComponent<RoomConnections>();
            while (roomConnections.id == -1 || roomConnections.id == -2 || ContainsNPC(roomList[random]))
            {
                random = UnityEngine.Random.Range(0, mapGenerator.GetComponent<Transform>().childCount);
                roomConnections = roomList[random].GetComponent<RoomConnections>();
            }
            roomList[random].SetActive(true);
            GameObject spawned = Instantiate(npc);
            spawned.transform.SetParent(roomList[random].transform);
            foreach (Transform child in roomConnections.transform)
            {
                if (child.CompareTag("NPCSpawn"))
                {
                    spawned.transform.position = child.position;
                }
            }
        }
    }

    private void TakePlayersToStartRoom()
    {
        GameObject startRoom = null;
        foreach(Classes.Room room in mapGenerator.map.rooms) {
            if(room.id == -1)
            {
                startRoom = room.room; 
                break;
            }
            else
            {
                room.room.SetActive(false);
            }
        }
        startRoom.SetActive(true);
        foreach (var player in players)
        {
            player.transform.position = startRoom.GetComponent<RoomConnections>().startPoint.transform.position;
        }
        currentActiveRoom = startRoom;
    }

    #endregion

    #region public-functions

    public void TeleportToNextRoom(GameObject nextRoomTeleportPoint)
    {
        if(teleportationTimer < 0)
        {
            currentActiveRoom.SetActive(false);
            nextRoomTeleportPoint.transform.parent.gameObject.SetActive(true);
            mapVisualization.SetActiveColor(nextRoomTeleportPoint.GetComponentInParent<RoomConnections>().id);
            currentActiveRoom = nextRoomTeleportPoint.transform.parent.gameObject;
            foreach (var player in players)
            {
                player.transform.position = nextRoomTeleportPoint.transform.position;
            }
            teleportationTimer = 3f;
        }
    }

    #endregion

    #endregion
}
