

using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.VisualScripting;
using UnityEngine;

public class PrimsAlgorithm
{
/*    private static readonly int MIN_WEIGHT_BASIC_PATH = 6;
    private static readonly int MAX_WEIGHT_BASIC_PATH = 10;
    private static readonly int MIN_WEIGHT_ADDED_PATH = 1;
    private static readonly int MAX_WEIGHT_ADDED_PATH = 6;*/

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

/*            Debug.Log("CURRENT NODE: " + currentlyProcessedNode);
            foreach (GraphConnection graphConnection in currentlyAvailableConnections)
            {
                Debug.Log(graphConnection.ToString());
            }*/


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

   /* public static Dictionary<int, Room> GenerateDungeonStructure(Dictionary<int, Room> rooms)
    {
        List<Dictionary<int, Room>> seperatedRoomsList = SeperateComplexAndNonComplexRooms(rooms);
        List<GraphConnection> finalConnections = new List<GraphConnection>();
        foreach (Dictionary<int,Room> specificRooms in seperatedRoomsList)
        {
            List<GraphConnection> specificConnections = GetNewGraphConnection(specificRooms);
            break; // test tylko dla nie complex
        }

        


        return null;
    }

    private static List<GraphConnection> GetNewGraphConnection(Dictionary<int, Room> specificRooms)
    {
        List<GraphConnection> connections = new List<GraphConnection>();

        foreach(Room room in specificRooms.Values)
        {
            foreach(int conn in room.connections)
            {

                if(CheckIfConnectionToComplexRoom(specificRooms, conn))
                {
                    continue;
                }

                connections.Add(new GraphConnection(room.Id, conn, UnityEngine.Random.Range(
                    MIN_WEIGHT_BASIC_PATH, MAX_WEIGHT_BASIC_PATH
                    )));
            }
        }
        connections = GenerateNewConnections(connections, specificRooms);

        foreach (GraphConnection connection in connections)
        {
            Debug.Log(connection.ToString());
        }

        return connections;
    }

    private static bool CheckIfConnectionToComplexRoom(Dictionary<int, Room> specificRooms, int conn)
    {
        return !specificRooms.ContainsKey(conn);
    }

    private static List<GraphConnection> GenerateNewConnections(List<GraphConnection> connections, Dictionary<int, Room> specificRooms)
    {
        List<int> nodes = new List<int>(specificRooms.Keys);

        Debug.Log("NODES AMOUNT: " + nodes.Count);

        int sizeOfGraph = nodes.Count;
        int desiredAmountOfPaths = DetermineAmountOfPaths(sizeOfGraph, connections.Count);

        List<GraphConnection> newConnections = AppendRandomlyGeneratedConnections(nodes, connections, desiredAmountOfPaths);

        return newConnections;
    }

    private static List<GraphConnection> AppendRandomlyGeneratedConnections(List<int> nodes,
        List<GraphConnection> connections, int desiredAmountOfPaths)
    {
        int connCnt = 0;
        nodes = Shuffle(nodes);

        foreach(int node in nodes)
        {
            foreach(int n in nodes)
            {
                GraphConnection possibleConnection = new GraphConnection(node, n, UnityEngine.Random.Range(
                    MIN_WEIGHT_ADDED_PATH, MAX_WEIGHT_ADDED_PATH
                    ));

                if (CheckIfNonRedundantConnection(connections, possibleConnection) && n!=node)
                {
                    connections.Add(possibleConnection);
                }
                connCnt++;
                if(connCnt >= desiredAmountOfPaths)
                {
                    break;
                }
            }
            if (connCnt >= desiredAmountOfPaths)
            {
                break;
            }
        }
        return connections;
    }

    private static bool CheckIfNonRedundantConnection(List<GraphConnection> connections, GraphConnection possibleConnection)
    {
        foreach(GraphConnection conn in connections)
        {
            if(conn.childNode == possibleConnection.childNode && conn.parentNode == possibleConnection.parentNode
                || conn.parentNode == possibleConnection.childNode && conn.childNode == possibleConnection.parentNode)
            {
                return false;
            }
        }
        return true;
    }

    private static int DetermineAmountOfPaths(int sizeOfGraph, int existingConnectionsAmount)
    {
        int maxAdditionalConnectionsInGraph = sizeOfGraph * (sizeOfGraph - 1) / 2 - existingConnectionsAmount;
        int wantedAmount = (int)Math.Floor(Math.Sqrt(sizeOfGraph)) * sizeOfGraph;
        Debug.Log("MAX CONNS: " + maxAdditionalConnectionsInGraph);
        Debug.Log("WANTED BEFORE: " + wantedAmount);
        if(wantedAmount > maxAdditionalConnectionsInGraph)
        {
            wantedAmount = maxAdditionalConnectionsInGraph;
        }
        Debug.Log("WANTED AFTER: " + wantedAmount);
        return wantedAmount;
    }

    private static List<Dictionary<int, Room>> SeperateComplexAndNonComplexRooms(Dictionary<int, Room> rooms)
    {

        Dictionary<int, Room> complexRooms = new Dictionary<int, Room>();
        Dictionary<int, Room> nonComplexRooms = new Dictionary<int, Room>();

        foreach(Room room in rooms.Values)
        {
            if (room.isComplex)
            {
                complexRooms.Add(room.Id, room);
            }
            else
            {
                nonComplexRooms.Add(room.Id, room);
            }
        }

        Debug.Log("NON COMPLEX ROOMS: "+nonComplexRooms.Count);
        Debug.Log("COMPLEX ROOMS: " + complexRooms.Count);

        return new List<Dictionary<int, Room>> { nonComplexRooms, complexRooms };
    }

    private static List<int> Shuffle(List<int> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            int value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        return list;
    }
*/
}