using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class GameManager : NetworkBehaviour
{
    #region variables

    public static GameManager Instance { get; private set; }

    #region network-variables

    public NetworkVariable<int> n_teleportationTouchCount = new NetworkVariable<int>(0, readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<int> n_completedQuests = new NetworkVariable<int>(0, readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<int> n_currentQuestRoomID = new NetworkVariable<int>(-3, readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<Enumirators.QuestType> n_currentQuest = new NetworkVariable<Enumirators.QuestType>(Enumirators.QuestType.Rescue, readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> n_questActiveStatus = new NetworkVariable<bool>(false, readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Server);

    #endregion

    #region private-variables

    private List<GameObject> players;
    
    private MapGeneration mapGenerator;
    private GameObject currentActiveRoom;
    private Spawner spawner;
    [SerializeField] private GameObject emptyGameObject;
    [SerializeField] private Canvas playerCanvas;
    [SerializeField] private MapVisualization mapVisualization;
    [SerializeField] private List<GameObject> prefabsNPCs;
    [SerializeField] private EnemiesScriptable spawnableEnemies;



    #endregion

    #region public-variable

    public GameObject player;
    public List<Classes.NPCQuest> nPCQuests = new List<Classes.NPCQuest>();
    public List<GameObject> enemies = new List<GameObject>();
    public GameObject startRoom;
    public float teleportationTimer;

    #endregion

    #endregion

    #region functions

    #region private-functions

    private void Awake()
    {
        spawner = GameObject.FindGameObjectWithTag("Spawner").GetComponent<Spawner>();
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
        CreateEnemies();
        GetEnemyObject();
        NetworkManagement.Instance.EnableAllPlayers();
        TakePlayersToStartRoom();
        if (NetworkManager.Singleton.IsServer)
        {
            n_currentQuestRoomID.Value = -3;
            n_questActiveStatus.Value = false;
        }
        player.GetComponent<PlayerMovement>().SetControls();
    }

    private void Update()
    {
        teleportationTimer -= Time.deltaTime;
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

    private List<Transform> GetSpawnLocations(Transform roomTransform)
    {
        List<Transform> spawnLocations = new List<Transform>();
        foreach (Transform child in roomTransform)
        {
            if (child.tag == "SpawnLocation")
            {
                spawnLocations.Add(child);
            }
        }
        return spawnLocations;
    }

    private void CreateEnemies()
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            return;
        }

        else
        {
            spawnableEnemies = Resources.Load<EnemiesScriptable>("ScriptableObjects/EnemiesScriptable");
            foreach (Classes.Room room in mapGenerator.map.rooms)
            {
                List<Transform> spawnLocations = GetSpawnLocations(room.room.transform);
                List<Classes.Enemy> enemies = spawnableEnemies.enemies;

                foreach (Transform location in spawnLocations)
                {
                    int randomEnemy = UnityEngine.Random.Range(0, enemies.Count);
                    spawner.SpawnEnemy(enemies[randomEnemy].prefab, enemies[randomEnemy].enemyType, location, room.id);
                }
            }
        }
    }

    public void GetEnemyObject()
    {
        List<GameObject> temp = GameObject.FindGameObjectsWithTag("Enemy").ToList();
        foreach (var item in temp)
        {
            if (item.activeInHierarchy)
            {
                if (item.GetComponent<Enemy>())
                {
                    if (item.GetComponent<Enemy>().roomID.Value != -3)
                    {
                        enemies.Add(item);
                    }
                }
            }
        }


    }

    private void EnableEnemiesWithRoomID(int id)
    {
        foreach(GameObject enemy in enemies)
        {
            if(enemy.GetComponent<Enemy>().roomID.Value != id)
            {
                enemy.SetActive(false);
            }
            else
            {
                enemy.SetActive(true);
            }
        }
    }

    private void TakePlayersToStartRoom()
    {
        startRoom = null;
        int i = 0;
        foreach(Classes.Room room in mapGenerator.map.rooms) {
            if(room.id == -1)
            {
                startRoom = room.room;
                EnableEnemiesWithRoomID(-1);
            }
            else
            {
                room.room.SetActive(false);
            }
            i++;
        }
        startRoom.SetActive(true);
        foreach (var player in players)
        {
            player.transform.position = startRoom.GetComponent<RoomConnections>().startPoint.transform.position;
        }
        currentActiveRoom = startRoom;

        SetEnemiesRoom();
    }

    public void SetEnemiesRoom()
    {
        DisableAllEnemies();

        foreach (var item in enemies)
        {
            if (item.GetComponent<Enemy>().roomID.Value == currentActiveRoom.GetComponent<RoomConnections>().id)
            {
                item.SetActive(true);
            }
        }
    }

    public void DisableAllEnemies()
    {
        foreach (var item in enemies)
        {
            item.SetActive(false);
        }
    }

    private GameObject GetNextRoom(int id)
    {
        foreach(Transform child in mapGenerator.gameObject.transform)
        {
            if(child.GetComponent<RoomConnections>().id == id)
            {
                return child.gameObject;
            }
        }
        return null;
    }

    private GameObject GetNextRoomTeleportationPoint(GameObject nextRoom, int teleportationPointID)
    {
        foreach(GameObject teleportationPoint in nextRoom.GetComponent<RoomConnections>().teleportationPoints)
        {
            if(teleportationPoint.GetComponent<Teleportation>().id == teleportationPointID)
            {
                return teleportationPoint;
            }
        }
        return null;
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

    [ServerRpc]
    public void TeleportToNextRoomServerRpc(int nextRoomID, int teleportationPointID)
    {
        TeleportToNextRoomClientRpc(nextRoomID, teleportationPointID);
    }

    [ClientRpc]
    public void TeleportToNextRoomClientRpc(int nextRoomID, int teleportationPointID)
    {
        if (teleportationTimer < 0)
        {
            GameObject nextRoom = GetNextRoom(nextRoomID);
            if (nextRoom != null)
            {
                GameObject nextRoomTeleportPoint = GetNextRoomTeleportationPoint(nextRoom, teleportationPointID);
                if(nextRoomTeleportPoint != null)
                {
                    ResetTeleoprtationTouchCountServerRpc();
                    currentActiveRoom.SetActive(false);
                    nextRoomTeleportPoint.transform.parent.gameObject.SetActive(true);
                    mapVisualization.SetActiveColor(nextRoomTeleportPoint.GetComponentInParent<RoomConnections>().id);
                    currentActiveRoom = nextRoomTeleportPoint.transform.parent.gameObject;
                    SetEnemiesRoom();
                    foreach (var player in players)
                    {
                        player.transform.position = nextRoomTeleportPoint.transform.position;
                    }
                    teleportationTimer = 3f;
                }
            }
            if (currentActiveRoom.GetComponent<RoomConnections>().id == n_currentQuestRoomID.Value && n_questActiveStatus.Value)
            {
                DisableQuestServerRpc();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void IncrementTeleoprtationTouchCountServerRpc(int nextRoomID, int teleportationPointID)
    {
        n_teleportationTouchCount.Value += 1;
        if (IsServer)
        {
            if(n_teleportationTouchCount.Value >= NetworkManager.ConnectedClients.Count)
            {
                TeleportToNextRoomServerRpc(nextRoomID, teleportationPointID);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void DeccrementTeleoprtationTouchCountServerRpc()
    {
        n_teleportationTouchCount.Value -= 1;
        if(n_teleportationTouchCount.Value < 0)
        {
            n_teleportationTouchCount.Value = 0;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ResetTeleoprtationTouchCountServerRpc()
    {
        n_teleportationTouchCount.Value = 0;
    }

    [ServerRpc (RequireOwnership = false)]

    public void SetQuestServerRpc(Enumirators.QuestType questType, int questRoomID)
    {
        n_questActiveStatus.Value = true;
        n_currentQuest.Value = questType;
        n_currentQuestRoomID.Value = questRoomID;
        SetQuestClientRpc(questType, questRoomID);
    }

    [ClientRpc]
    public void SetQuestClientRpc(Enumirators.QuestType questType, int questRoomID) 
    { 
        if(questType == Enumirators.QuestType.Fetch)
        {
            mapVisualization.SetQuestColor(questRoomID, Color.cyan);
        }
        else if (questType == Enumirators.QuestType.Kill)
        {
            mapVisualization.SetQuestColor(questRoomID, Color.grey);
        }
        else if (questType == Enumirators.QuestType.Rescue)
        {
            mapVisualization.SetQuestColor(questRoomID, Color.magenta);
        }
    }

    [ServerRpc (RequireOwnership = false)]
    public void DestroyQuestNPCServerRpc(Enumirators.QuestType questType)
    {
        DestroyQuestNPCClientRpc(questType);
    }

    [ClientRpc]
    public void DestroyQuestNPCClientRpc(Enumirators.QuestType questType)
    {
        if (n_questActiveStatus.Value == true)
        {
            if (questType == Enumirators.QuestType.Fetch)
            {
                Destroy(FindAnyObjectByType<FetchQuest>().gameObject);
            }
            else if (questType == Enumirators.QuestType.Kill)
            {
                Destroy(FindAnyObjectByType<KillQuest>().gameObject);
            }
            else if (questType == Enumirators.QuestType.Rescue)
            {
                Destroy(FindAnyObjectByType<RescueQuest>().gameObject);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DisableQuestServerRpc()
    {
        n_questActiveStatus.Value = false;
        n_completedQuests.Value += 1;
        Debug.Log("Quests Completed : " + n_completedQuests.Value);
    }

    #endregion

    #endregion
}
