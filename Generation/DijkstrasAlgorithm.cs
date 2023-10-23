using System.Collections.Generic;
using UnityEngine;

public class DijkstrasAlgorithm
{

    public static List<int> GetDistancesToAllNode(List<GraphConnection> connections, int nodesAmount)
    {
        // Returns distances to all nodes from node 0 ( list it <-> node id )
        List<int> processedNodes = new List<int>();
        List<int> distances = new List<int>();

        for(int i = 0; i < nodesAmount; i++)
        {
            if (i == 0) distances.Add(0);
            else distances.Add(int.MaxValue);
        }
        int currentlyProcessed = 0;
        processedNodes.Add(currentlyProcessed);
        while(processedNodes.Count < nodesAmount)
        {
            List<GraphConnection> graphConnectionsOfNode = GetConnectionsFromParent(connections, currentlyProcessed);
            distances = ApplyDistances(distances, graphConnectionsOfNode);
            currentlyProcessed = FindIdOfShortestPath(distances, processedNodes);
            processedNodes.Add(currentlyProcessed);
        }

        return distances;
    }

    private static int FindIdOfShortestPath(List<int> distances, List<int> processedNodes)
    {
        int shortest = int.MaxValue;
        int idToReturn = 0;
        for(int i = 0; i < distances.Count; i++)
        {
            if (distances[i] < shortest && !processedNodes.Contains(i))
            {
                shortest = distances[i];
                idToReturn = i;
            }
        }
        return idToReturn;
    }

    private static List<int> ApplyDistances(List<int> distances, List<GraphConnection> graphConnectionsOfNode)
    {
        foreach(GraphConnection connection in graphConnectionsOfNode)
        {

            distances[connection.childNode] = distances[connection.parentNode] + connection.weight;
        }
        return distances;
    }

    private static List<GraphConnection> GetConnectionsFromParent(List<GraphConnection> connections, int nodeId) 
    {
        return connections.FindAll(c => c.parentNode == nodeId);
    }


}