using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FetchQuest : BaseQuest
{
    public override void CreateQuest()
    {
        bool questAccepted = DialogueLua.GetVariable("FetchQuestAccepted").asBool;
        if (questAccepted && gameManager.nPCQuests.Find(x => x.questType == Enumirators.QuestType.Fetch).count < 1)
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
                        mapVisualization.SetQuestColor(childRoomConnections.id, Color.cyan);
                        questRoomID = childRoomConnections.id;
                        questType = Enumirators.QuestType.Fetch;
                        questRoom = child.gameObject;
                        foreach (Classes.NPCQuest nPCQuest in gameManager.nPCQuests)
                        {
                            if (nPCQuest.questType == Enumirators.QuestType.Fetch)
                            {
                                nPCQuest.count = 1;
                            }
                        }
                    }
                }
                count++;
            }
        }
        
    }
}
