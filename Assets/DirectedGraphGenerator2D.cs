using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DirectedGraphNode
{
    public int id;
    public List<DirectedGraphNode> neighbors = new List<DirectedGraphNode>();
}

public class DirectedGraph
{
    public DirectedGraphNode startNode;
    public DirectedGraphNode endNode;
}

public class DirectedGraphGenerator2D : MonoBehaviour
{
    public GameObject nodePrefab;
    public GameObject linePrefab;
    public float yOffset = 2.0f;
    private DirectedGraph graph;

    private void Start()
    {
        GenerateAndVisualizeGraph(20);
    }

    public void GenerateAndVisualizeGraph(int numIntermediateNodes)
    {
        graph = GenerateDirectedGraph(numIntermediateNodes);

        // Visualize the nodes and connections
        VisualizeGraph(graph.startNode, new Vector2(0,0), 0);
    }

    private void VisualizeGraph(DirectedGraphNode node, Vector2 position, int level)
    {
        if (node == null)
            return;

        GameObject visualNode = Instantiate(nodePrefab, position, Quaternion.identity);
        // Set visualNode's properties based on node.id if needed

        float xOffset = 2.0f;
        foreach (DirectedGraphNode neighbor in node.neighbors)
        {
            Vector2 newPosition = position + new Vector2(xOffset, -level * yOffset);
            VisualizeGraph(neighbor, newPosition, level + 1);
            xOffset += 2.0f;

            // Draw a line connecting the nodes
            Color lineColor = Color.yellow; // Default color is yellow

            if (node == graph.startNode)
            {
                lineColor = Color.green; // Lines from the start node are green
            }
            else if (neighbor == graph.endNode)
            {
                lineColor = Color.red; // Lines connecting to the end node are red
            }

            DrawLine(position, newPosition, lineColor);
        }
    }

    private void DrawLine(Vector2 start, Vector2 end, Color color)
    {
        Vector2 midpoint = (start + end) / 2.0f;

        GameObject line = Instantiate(linePrefab, midpoint, Quaternion.identity);

        Vector2 direction = (end - start).normalized;
        float distance = Vector2.Distance(start, end);

        line.transform.localScale = new Vector3(distance, 0.1f, 1.0f);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        line.transform.rotation = Quaternion.Euler(0, 0, angle);

        Renderer renderer = line.GetComponent<Renderer>();
        renderer.material.color = color;
    }

    public DirectedGraph GenerateDirectedGraph(int numIntermediateNodes)
    {
        DirectedGraph graph = new DirectedGraph();

        // Create start and end nodes
        graph.startNode = new DirectedGraphNode { id = 0 };
        graph.endNode = new DirectedGraphNode { id = numIntermediateNodes + 1 };

        // Create intermediate nodes and randomly connect them
        for (int i = 1; i <= numIntermediateNodes; i++)
        {
            DirectedGraphNode newNode = new DirectedGraphNode { id = i };
            graph.startNode.neighbors.Add(newNode);

            if (i < numIntermediateNodes)
            {
                DirectedGraphNode nextNode = new DirectedGraphNode { id = i + 1 };
                newNode.neighbors.Add(nextNode);
            }
            else
            {
                newNode.neighbors.Add(graph.endNode);
            }
        }

        return graph;
    }
}
