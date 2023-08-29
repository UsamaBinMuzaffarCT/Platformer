using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
        if (players[0].GetComponent<PlayerMovement>().map)
        {
            players[0].GetComponent<PlayerMovement>().playerCanvas.gameObject.SetActive(true);
        }
        else
        {
            players[0].GetComponent<PlayerMovement>().playerCanvas.gameObject.SetActive(false);
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
