using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Classes;

public class MapVisualization : MonoBehaviour
{ 
    private List<GameObject> nodes = new List<GameObject>();
    [SerializeField] private Transform startTransform;
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private LineRendererController controller;
    [SerializeField] private MapGeneration mapGenerator;

    private void Start()
    {
        PopulateMapNodes();
        ConnectNodes();
        transform.parent.gameObject.SetActive(false);
    }

    public void PopulateMapNodes()
    {
        Classes.Map map = mapGenerator.map;
        Dictionary<int, List<Classes.Room>> levelToRooms = new Dictionary<int, List<Classes.Room>>();

        // Group rooms by their levels
        foreach (var room in map.rooms)
        {
            if (!levelToRooms.ContainsKey(room.level))
            {
                levelToRooms[room.level] = new List<Room>();
            }
            levelToRooms[room.level].Add(room);
        }

        // Calculate canvas width and height (adjust these values as needed)
        float canvasWidth = 1000.0f;
        float canvasHeight = 800.0f;

        int maxLevel = map.rooms.Max(room => room.level);

        // Calculate horizontal and vertical spacing
        float horizontalSpacing = canvasWidth / (maxLevel + 2); // Adjust for max level
        float verticalSpacing = canvasHeight / (map.rooms.Count + 1);

        foreach (var level in levelToRooms.Keys)
        {
            float xPosition = level * horizontalSpacing; // Adjust xPosition calculation
            List<Room> roomsAtLevel = levelToRooms[level];
            float yOffset = verticalSpacing * (roomsAtLevel.Count - 1) / 2;

            foreach (var room in roomsAtLevel)
            {
                float yPosition = canvasHeight / 2 - yOffset;
                InstantiateNode(room, new Vector3(xPosition, yPosition, 0));
                yOffset -= verticalSpacing;
            }
        }
    }

    void InstantiateNode(Classes.Room room, Vector3 position)
    {
        GameObject node = Instantiate(nodePrefab, position, Quaternion.identity);
        node.transform.SetParent(transform, false);
        node.GetComponent<RoomConnections>().id = room.id;
        node.transform.localPosition = new Vector3(position.x- Random.Range(310, 400), position.y-Random.Range(300,500),0);
        if (room.id == -1)
        {
            node.GetComponent<Image>().color = Color.blue;
        }
        else if(room.id == -2)
        {
            node.GetComponent<Image>().color = Color.red;
        }
        else
        {
            node.GetComponent<Image>().color = Color.green;
        }
        nodes.Add(node);
    }

    void ConnectNodes()
    {
        RectTransform node1;
        RectTransform node2;
        foreach (Room room in mapGenerator.map.rooms)
        {
           foreach(Room neighbour in room.neighbourRooms)
           {
                node1 = nodes.Find(x => x.GetComponent<RoomConnections>().id == room.id).GetComponent<RectTransform>();
                node2 = nodes.Find(x => x.GetComponent<RoomConnections>().id == neighbour.id).GetComponent<RectTransform>();
                controller.DrawLineBetweenTransforms(node1, node2);
           }
        }
    }

    public void SetQuestColor(int id, Color color)
    {
        foreach (GameObject node in nodes)
        {
            Image image = node.GetComponent<Image>();
            if (node.GetComponent<RoomConnections>().id == id)
            {
                image.color = color;
            }
        }
    }

    public void SetActiveColor(int id)
    {
        foreach(GameObject node in nodes)
        {
            int nodeID = node.GetComponent<RoomConnections>().id;
            Image image = node.GetComponent<Image>();
            if (nodeID == -1)
            {
                image.color = Color.yellow;
            }
            else if (nodeID == -2)
            {
                image.color = Color.red;
            }
            else
            {
                if(image.color != Color.cyan && image.color != Color.grey && image.color != Color.magenta)
                {
                    image.color = Color.green;
                }
            }
        }
        foreach(GameObject node in nodes)
        {
            Image image = node.GetComponent<Image>();
            if (node.GetComponent<RoomConnections>().id == id)
            {
                image.color = Color.blue;
            }
        }
    }
}
