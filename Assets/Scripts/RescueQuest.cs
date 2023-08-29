using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RescueQuest : BaseQuest
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("CreateQuest", 0.1f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected override void CreateQuest()
    {
        Transform roomsParent = mapGeneration.gameObject.transform;
        int pickRandom = Random.Range(0, roomsParent.childCount);
        int count = 0;
        foreach (Transform child in roomsParent)
        {
            RoomConnections childRoomConnections = child.GetComponent<RoomConnections>();
            if (!child.gameObject.activeSelf && !(childRoomConnections.id == -1 || childRoomConnections.id == -2))
            {
                if (count == pickRandom)
                {
                    mapVisualization.SetQuestColor(childRoomConnections.id, Color.magenta);
                    questRoomID = childRoomConnections.id;
                    questType = Enumirators.QuestType.Rescue;
                    questRoom = child.gameObject;
                }
            }
            count++;
        }
    }
}
