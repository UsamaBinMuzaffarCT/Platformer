using PixelCrushers.DialogueSystem;
using UnityEngine;

public class FetchQuest : BaseQuest
{
    public override void CreateQuest()
    {
        if (GameManager.Instance.n_questActiveStatus.Value != false)
        {
            return;
        }
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
                        gameManager.SetQuestServerRpc(questType, questRoomID);
                        gameManager.DestroyQuestNPCServerRpc(questType);
                    }
                }
                count++;
            }
        }
    }
}
