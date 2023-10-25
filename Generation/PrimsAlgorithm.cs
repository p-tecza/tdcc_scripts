

using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.VisualScripting;
using UnityEngine;

public class PrimsAlgorithm
{
    public static List<GraphConnection> MinimumSpanningTree(List<GraphConnection> connections, int startingNode, int nodesAmount)
    {
        
        List<int> achievableNodes = new List<int>();
        List<GraphConnection> currentlyAvailableConnections = new List<GraphConnection>();
        List<GraphConnection> finalTree = new List<GraphConnection>();
        int currentlyProcessedNode = startingNode;
        achievableNodes.Add(currentlyProcessedNode);
        while(achievableNodes.Count < nodesAmount)
        {
            currentlyAvailableConnections.AddRange(GetConnectionsFromNode(currentlyProcessedNode, connections));
            GraphConnection nearestNode = FindLowestWeightConnection(currentlyAvailableConnections, achievableNodes);
            if (nearestNode == null) break;
            currentlyProcessedNode = nearestNode.childNode;
            finalTree.Add(nearestNode);
            achievableNodes.Add(currentlyProcessedNode);
            currentlyAvailableConnections.Remove(nearestNode);
        }
        return finalTree;
    }

    private static GraphConnection FindLowestWeightConnection(List<GraphConnection> currentlyAvailableConnections, List<int> alreadyProcessed)
    {
        if(currentlyAvailableConnections == null) return null;
        int minWeight = 1337;
        GraphConnection best = null;
        foreach(GraphConnection connection in currentlyAvailableConnections)
        {
            if (connection.weight <= minWeight && !alreadyProcessed.Contains(connection.childNode))
            {
                best = connection;
                minWeight = connection.weight;
            }
        }
        return best;
    }

    public static List<GraphConnection> GetConnectionsFromNode(int nodeId, List<GraphConnection> connections)
    {
        return connections.FindAll(c => c.parentNode == nodeId);
    }
}