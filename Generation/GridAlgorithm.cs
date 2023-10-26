using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class GridAlgorithm
{
    private static readonly int MAX_WEIGHT = 3;
    private static readonly float CHANCE_OF_ADDITIONAL_CONNECTION = 0.5f; // 0.0 - 1.0
    public static GridGraph gg;

    public static List<GraphConnection> GenerateDungeonStructure(Dictionary<int, Room> rooms)
    {
        int dimension = (int)Math.Ceiling(Math.Sqrt(rooms.Count));
        gg = new GridGraph(dimension);
        gg.CreateNodesGrid(rooms);
        gg.CreatePrimitiveNodesConnections();

        List<GraphConnection> mst = PrimsAlgorithm.MinimumSpanningTree(gg.connections, 0, rooms.Count);
        mst = SetWeightsOfConnectionsToOne(mst);

        List<int> distancesToAllNodes = DijkstrasAlgorithm.GetDistancesToAllNode(mst, rooms.Count);
        gg.SetDistancesToAllNodes(distancesToAllNodes);
        

        gg.CreateProperNodesConnections(mst);


        Debug.Log("GRAF WITH ADDITIONAL CONNECTIONS:");
        Debug.Log(GraphToString(rooms.Count, gg.connections));

        return gg.connections;
    }

    private static List<GraphConnection> SetWeightsOfConnectionsToOne(List<GraphConnection> connections)
    {
        foreach(GraphConnection connection in connections)
        {
            connection.weight = 1;
        }
        return connections;
    } 

    private static string GraphToString(int nodesAmount, List<GraphConnection> connections)
    {
        string msg = "GRAPH VISUALIZATION" + "\n" + nodesAmount.ToString() + " "+ connections.Count;
        foreach(GraphConnection gc in connections)
        {
            msg += "\n" + gc.parentNode + " " + gc.childNode + " " + gc.weight;
        }
        return msg;
    }

    public class GridGraph
    {
        public Dictionary<(int, int), Room> nodes { get; set; }
        public List<GraphConnection> connections { get; set; }
        public int dimension { get; set; }
        public List<int> distancesToNodes { get; set; }

        public GridGraph(int dimension)
        {
            this.dimension = dimension;
            this.nodes = new Dictionary<(int, int), Room>();
            this.connections = new List<GraphConnection>();
            this.distancesToNodes = new List<int>();
        }

        public void CreateNodesGrid(Dictionary<int, Room> rooms)
        {
            foreach(Room r in rooms.Values)
            {
                nodes.Add((r.Id % this.dimension, r.Id / this.dimension), r);
               /* Debug.Log("ID: "+r.Id +" I: "+ r.Id % this.dimension + " J: "+ r.Id / this.dimension);*/
            }
        }

        public void CreatePrimitiveNodesConnections()
        {
            int finalNode = this.nodes.Count - 1;
            List<(int,int)> options = new List<(int, int)>()
            {
                (1,0),
                (-1,0),
                (0,1),
                (0,-1)
            };

            foreach((int, int) n in this.nodes.Keys)
            {

                foreach((int, int) o in options)
                {
                    (int, int) proceedTuple = (n.Item1 + o.Item1, n.Item2 + o.Item2);
                    int randWeight = UnityEngine.Random.Range(1, MAX_WEIGHT + 1);
                    GraphConnection proceedConnection = new GraphConnection(ConvertCordsToId(n),ConvertCordsToId(proceedTuple), randWeight);
                    if (CheckIfConnectionToPossible(proceedTuple, finalNode) 
                        && CheckIfNonRedundantConnection(this.connections, proceedConnection))
                    {
                        this.connections.Add(new GraphConnection(ConvertCordsToId(n),ConvertCordsToId(proceedTuple), randWeight));
                    }
                }
            }
        }

        public void CreateProperNodesConnections(List<GraphConnection> mst)
        {
            List<GraphConnection> newProperConnections = new List<GraphConnection>();
            int chance = (int)(CHANCE_OF_ADDITIONAL_CONNECTION * 100);
            foreach(GraphConnection connection in this.connections) 
            {
                connection.weight = 1;
                if (mst.Contains(connection))
                {
                    newProperConnections.Add(connection);
                } 
                else
                {
                    int lot = UnityEngine.Random.Range(0, 100);
                    if (lot < chance)
                    {
                        newProperConnections.Add(connection);
                    }
                }

            }
            this.connections = newProperConnections;
        }

        public void SetDistancesToAllNodes(List<int> distances)
        {
            this.distancesToNodes = distances;
        }
        public int FindFurthestNode()
        {
            int len = 0;
            int ret = 0;
            for(int i = 0; i < this.distancesToNodes.Count; i++)
            {
                if (distancesToNodes[i] > len)
                {
                    ret = i;
                    len = distancesToNodes[i];
                }
            }
            return ret;
        }

        private bool CheckIfConnectionToPossible((int, int) pos, int finalNode)
        {
            if (pos.Item1 < 0 || pos.Item2 < 0 || pos.Item1 >= this.dimension || pos.Item2 >= this.dimension
                || ConvertCordsToId(pos) > finalNode)
            {
                return false;
            }
            return true;
        }

        private int ConvertCordsToId((int, int) i)
        {
            return i.Item1 + i.Item2 * this.dimension;
        }

        private bool CheckIfNonRedundantConnection(List<GraphConnection> connections, GraphConnection possibleConnection)
        {
            foreach (GraphConnection conn in connections)
            {
                if (conn.childNode == possibleConnection.childNode && conn.parentNode == possibleConnection.parentNode
                    || conn.parentNode == possibleConnection.childNode && conn.childNode == possibleConnection.parentNode)
                {
                    return false;
                }
            }
            return true;
        }

    }

}