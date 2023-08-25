using System.Collections;
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

    #endregion

    #region public-variable

    #endregion

    #endregion

    #region functions

    #region private-functions

    private void Awake()
    {
        players = GameObject.FindGameObjectsWithTag("Player").ToList<GameObject>();
        mapGenerator = GameObject.FindWithTag("Map").GetComponent<MapGeneration>();
    }

    private void Start()
    {
        TakePlayersToStartRoom();
    }

    private void Update()
    {
        teleportationTimer -= Time.deltaTime;
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
