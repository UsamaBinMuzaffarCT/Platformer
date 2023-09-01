using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphNode
{
    public int id;
    public List<GraphNode> neighbors = new List<GraphNode>();
    public bool visited = false;
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
        VisualizeGraph(graph);
    }

    private void Traverse(GraphNode node)
    {
        if (node.visited)
        {
            return;
        }
        node.visited = true;
        foreach(GraphNode neighbor in node.neighbors)
        {
            Traverse(neighbor);
        }
    }

    private bool TraversalPossible(Graph graph)
    {
        Traverse(graph.nodes[0]);
        foreach(GraphNode node in graph.nodes)
        {
            if (!node.visited)
            {
                return false;
            }
        }
        return true;
    }

    private Graph ClearTraversal(Graph graph)
    {
        foreach(GraphNode node in graph.nodes)
        {
            node.visited = false;
        }
        return graph;
    }

    private Graph ConnectUnraachablePaths(Graph graph)
    {
        while (!TraversalPossible(graph))
        {
            foreach(GraphNode node in graph.nodes)
            {
                if (!node.visited)
                {
                    foreach(GraphNode node2 in graph.nodes)
                    {
                        if(node2.visited && node2.neighbors.Count < 3)
                        {
                            node.neighbors.Add(node2);
                            node2.neighbors.Add(node);
                        }
                    }
                }
            }
            graph = ClearTraversal(graph);
        }
        return graph;
    }

    private Graph RemoveUnreachableNodes(Graph graph)
    {
        List<int> removeIds = new List<int>();
        for (int i = 0; i < graph.nodes.Count; i++)
        {
            if (graph.nodes[i].neighbors.Count == 0)
            {
                removeIds.Add(graph.nodes[i].id);
            }
        }

        foreach (int id in removeIds)
        {
            foreach (GraphNode node in graph.nodes)
            {
                if (id == node.id)
                {
                    graph.nodes.Remove(node);
                    break;
                }
            }
        }
        return graph;
    }

    private GraphNode RemoveUnnecessaryConnections(GraphNode node)
    {
        while(node.neighbors.Count > 3)
        {
            foreach (GraphNode neighbor in node.neighbors)
            {
                if (neighbor.neighbors.Count > 2)
                {
                    neighbor.neighbors.Remove(node);
                    node.neighbors.Remove(neighbor);
                    break;
                }
            }
        }
        return node;
    }
    private Graph RemoveOverConnections(Graph graph)
    {
        for(int i = 0; i < graph.nodes.Count; i++)
        {
            if (graph.nodes[i].neighbors.Count > 3)
            {
                graph.nodes[i] = RemoveUnnecessaryConnections(graph.nodes[i]);
            }
        }
        return graph;
    }

    

    private Graph ConnectSingleConnections(Graph graph)
    {
        for (int i = 0; i < graph.nodes.Count; i++)
        {
            if(graph.nodes[i].neighbors.Count == 1)
            {
                int minNode = -1;
                int minCount = 3;
                for (int j = 0; j < graph.nodes.Count; j++)
                {
                    if(j == i)
                    {
                        continue;
                    }
                    if (graph.nodes[j].neighbors.Count < minCount)
                    {
                        minNode = j;
                        minCount = graph.nodes[j].neighbors.Count;
                    }
                }
                if(minNode != -1)
                {
                    graph.nodes[i].neighbors.Add(graph.nodes[minNode]);
                    graph.nodes[minNode].neighbors.Add(graph.nodes[i]);
                }
            }
        }
        return graph;
    }

    private Graph AddStartAndEndNodes(Graph graph)
    {
        GraphNode startNode = new GraphNode { id = -1 };
        int minNode = -1;
        int minCount = 3;
        for (int j = 0; j < graph.nodes.Count; j++)
        {
            if (graph.nodes[j].neighbors.Count < minCount)
            {
                minNode = j;
                minCount = graph.nodes[j].neighbors.Count;
            }
        }
        if (minNode != -1)
        {
            startNode.neighbors.Add(graph.nodes[minNode]);
            graph.nodes[minNode].neighbors.Add(startNode);
        }
        GraphNode endNode = new GraphNode { id = -2 };
        minNode = -1;
        minCount = 3;
        for (int j = 0; j < graph.nodes.Count; j++)
        {
            if (graph.nodes[j].neighbors.Count < minCount)
            {
                minNode = j;
                minCount = graph.nodes[j].neighbors.Count;
            }
        }
        if (minNode != -1)
        {
            endNode.neighbors.Add(graph.nodes[minNode]);
            graph.nodes[minNode].neighbors.Add(endNode);
        }
        graph.nodes.Add(startNode);
        graph.nodes.Add(endNode);
        return graph;
    }

    private Graph GenerateRandomWalkGraph(int numNodes)
    {
        Graph generatedGraph = new Graph();
        GraphNode startNode = new GraphNode { id = 0 };
        generatedGraph.nodes.Add(startNode);

        for (int i = 1; i < numNodes; i++)
        {
            GraphNode newNode = new GraphNode { id = i };

            int randomNeighborCount = Random.Range(0,3);
            for (int j = 0; j < randomNeighborCount; j++)
            {
                GraphNode randomNeighbor = generatedGraph.nodes[Random.Range(0, generatedGraph.nodes.Count)];
                if(randomNeighbor.neighbors.Count > 2)
                {
                    foreach(GraphNode node in generatedGraph.nodes)
                    {
                        if(node.neighbors.Count > randomNeighbor.neighbors.Count)
                        {
                            randomNeighbor = node;
                            break;
                        }
                    }
                }
                if (randomNeighbor.neighbors.Count < 2 && !randomNeighbor.neighbors.Contains(newNode))
                {
                    newNode.neighbors.Add(randomNeighbor);
                    randomNeighbor.neighbors.Add(newNode);
                }
            }
            generatedGraph.nodes.Add(newNode);
        }
        generatedGraph = RemoveUnreachableNodes(generatedGraph);
        generatedGraph = ConnectUnraachablePaths(generatedGraph);
        generatedGraph = RemoveOverConnections(generatedGraph);
        generatedGraph = ConnectSingleConnections(generatedGraph);
        generatedGraph = AddStartAndEndNodes(generatedGraph);
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
                if (randomReachableNode.neighbors.Count < 2)
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
            if (node.neighbors.Count == 1)
            {
                spriteRenderer.color = Color.yellow;
            }
            else if (node.neighbors.Count > 3)
            {
                spriteRenderer.color = Color.red;
            }
            else
            {
                spriteRenderer.color = Color.green;
            }
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
