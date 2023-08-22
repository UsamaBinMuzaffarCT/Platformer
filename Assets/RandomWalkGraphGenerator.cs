using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphNode
{
    public int id;
    public List<GraphNode> neighbors = new List<GraphNode>();
}

public class Graph
{
    public List<GraphNode> nodes = new List<GraphNode>();
}

public class RandomWalkGraphGenerator : MonoBehaviour
{
    public GameObject nodePrefab;
    public int numNodesToGenerate = 10;

    private Graph graph;
    private Dictionary<GraphNode, GameObject> visualNodes = new Dictionary<GraphNode, GameObject>();

    private void Start()
    {
        graph = GenerateRandomWalkGraph(numNodesToGenerate);
        ConnectUnreachableNodes(graph);
        VisualizeGraph(graph);
    }

    private Graph GenerateRandomWalkGraph(int numNodes)
    {
        Graph generatedGraph = new Graph();
        GraphNode startNode = new GraphNode { id = 0 };
        generatedGraph.nodes.Add(startNode);

        for (int i = 1; i < numNodes; i++)
        {
            GraphNode newNode = new GraphNode { id = i };

            int randomNeighborCount = Random.Range(1, 3);
            for (int j = 0; j < randomNeighborCount; j++)
            {
                GraphNode randomNeighbor = generatedGraph.nodes[Random.Range(0, generatedGraph.nodes.Count)];
                if (randomNeighbor.neighbors.Count < 3 && !randomNeighbor.neighbors.Contains(newNode))
                {
                    newNode.neighbors.Add(randomNeighbor);
                    randomNeighbor.neighbors.Add(newNode);
                }
            }

            generatedGraph.nodes.Add(newNode);
        }

        return generatedGraph;
    }

    private void ConnectUnreachableNodes(Graph graph)
    {
        List<GraphNode> reachableNodes = new List<GraphNode>();
        List<GraphNode> unreachableNodes = new List<GraphNode>();

        reachableNodes.Add(graph.nodes[0]); 

        foreach (GraphNode node in graph.nodes)
        {
            if (reachableNodes.Contains(node))
            {
                foreach (GraphNode neighbor in node.neighbors)
                {
                    if (!reachableNodes.Contains(neighbor))
                    {
                        reachableNodes.Add(neighbor);
                    }
                }
            }
            else
            {
                unreachableNodes.Add(node);
            }
        }

        foreach (GraphNode unreachableNode in unreachableNodes)
        {
            bool adding = true;
            while (adding)
            {
                GraphNode randomReachableNode = reachableNodes[Random.Range(0, reachableNodes.Count)];
                if (randomReachableNode.neighbors.Count < 3)
                {
                    randomReachableNode.neighbors.Add(unreachableNode);
                    unreachableNode.neighbors.Add(randomReachableNode);
                    adding = false;
                }
            }
        }
    }

    private void VisualizeGraph(Graph graph)
    {
        foreach (GraphNode node in graph.nodes)
        {
            GameObject visualNode = Instantiate(nodePrefab);
            SpriteRenderer spriteRenderer = visualNode.GetComponent<SpriteRenderer>();
            float halfWidth = spriteRenderer.bounds.size.x * 0.5f;

            Vector3 nodePosition = Vector3.zero;
            bool positionFound = false;

            // Find a non-overlapping position for the node
            while (!positionFound)
            {
                nodePosition = new Vector3(Random.Range(-8f, 8f), Random.Range(-4.5f, 4.5f), 0);
                positionFound = !CheckOverlap(nodePosition, halfWidth);
            }

            visualNode.transform.position = nodePosition;
            visualNodes[node] = visualNode;

            foreach (GraphNode neighbor in node.neighbors)
            {
                if (visualNodes.TryGetValue(neighbor, out GameObject neighborVisualNode))
                {
                    Debug.DrawLine(visualNode.transform.position, neighborVisualNode.transform.position, Color.cyan, Mathf.Infinity);
                }
            }
        }
    }

    private bool CheckOverlap(Vector3 position, float radius)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, radius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Node"))
            {
                return true;
            }
        }
        return false;
    }
}
