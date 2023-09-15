using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    #region variables

    public static GameManager Instance { get; private set; }

    #region network-variables

    public NetworkVariable<int> n_teleportationTouchCount = new NetworkVariable<int>(0, readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<int> n_connectedClientCount = new NetworkVariable<int>(0, readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Server);
    public GameObject startRoom;
    public float teleportationTimer;


    #endregion

    #region private-variables

    private List<GameObject> players;
    private GameObject player;
    private MapGeneration mapGenerator;
    private GameObject currentActiveRoom;
    [SerializeField] private Canvas playerCanvas;
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
        Instance = this;
        
        Initialize();
    }

    private void Initialize()
    {
        Enumirators.QuestType[] values = (Enumirators.QuestType[])Enum.GetValues(typeof(Enumirators.QuestType));
        for (int i = 0; i < Enum.GetValues(typeof(Enumirators.QuestType)).Length; i++)
        {
            nPCQuests.Add(new Classes.NPCQuest { questType = values[i], count = 0 });
        }
        players = GameObject.FindGameObjectsWithTag("Player").ToList();
        player = players.Find(x => x.GetComponent<PlayerMovement>().IsOwner == true);
        mapGenerator = GameObject.FindWithTag("Map").GetComponent<MapGeneration>();
        mapGenerator.Initialize(NetworkManagement.Instance.n_mapSeed.Value);
        CreateNPCs();
        NetworkManagement.Instance.EnableAllPlayers();
        TakePlayersToStartRoom();
    }



    private void Update()
    {
        teleportationTimer -= Time.deltaTime;
        if (IsServer)
        {
            n_connectedClientCount.Value = NetworkManager.Singleton.ConnectedClients.Count;
        }
        if (player.GetComponent<PlayerMovement>().map)
        {
            playerCanvas.gameObject.SetActive(true);
        }
        else
        {
            playerCanvas.gameObject.SetActive(false);
        }
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
        startRoom = null;
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

    #region rpcs

    //[ServerRpc(RequireOwnership = false)]
    //public void incrementTeleoprtationTouchCountServerRpc()
    //{
    //    n_teleportationTouchCount.Value += 1;
    //}

    //[ServerRpc(RequireOwnership = false)]
    //public void deccrementTeleoprtationTouchCountServerRpc()
    //{
    //    n_teleportationTouchCount.Value -= 1;
    //}

    //[ServerRpc(RequireOwnership = false)]
    //public void resetTeleoprtationTouchCountServerRpc()
    //{
    //    n_teleportationTouchCount.Value = 0;
    //}

    #endregion

    #endregion
}
