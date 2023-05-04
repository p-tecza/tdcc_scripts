using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets._Scripts
{
    public class TKDevDungeonGenerator : AbstractDungeonGenerator
    {

        //do lepszej optymalizacji

        [SerializeField]
        protected int roomWidth=50;

        [SerializeField]
        protected int roomHeight=40;

        [SerializeField]
        protected int dimensionDeviation=10;

        [SerializeField]
        protected int numberOfRooms=10;


        protected override void RunProceduralGeneration()
        {
            HashSet<Vector2Int> floorPositions = GenerateRooms(100);
            tilemapVisualizer.Clear();
            foreach (var position in floorPositions)
            {
                tilemapVisualizer.PaintFloorTiles(floorPositions);
            }
            WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);
        }


        private HashSet<Vector2Int> GenerateRooms(int radius)
        {
            HashSet<Vector2Int> rooms = new HashSet<Vector2Int>();
            HashSet<Vector2Int> starts = new HashSet<Vector2Int>();

            for(int i = 0; i<this.numberOfRooms; i++)
            {
                Vector2Int start = GetRandomPointInCircle(radius);
                starts.Add(start);
                Vector2Int dims = GetRandomRoomDimensions();

                for (int x = start.x; x < start.x+dims.x; x++)
                {
                    for(int y = start.y; y < start.y+dims.y; y++)
                    {
                        rooms.Add(new Vector2Int(x, y));
                    }
                }

            }

            GenerateConnections(starts);

            return rooms;
        }

        private HashSet<Vector2Int> GenerateConnections(HashSet<Vector2Int> rooms) 
        {

            Dictionary<int,Vector2Int> roomIdentifiers = new Dictionary<int, Vector2Int> ();
            HashSet<Vector2Int> connections = new HashSet<Vector2Int> ();
            int counter = 0;
            float firstShortest;
            float secondShortest;
            int a=0, b=0;
            foreach (var room in rooms)
            {
                roomIdentifiers.Add(counter++, room);
            }

            for (int i = 0; i < roomIdentifiers.Count; i++)
            {
                firstShortest = float.MaxValue;
                secondShortest = float.MaxValue;
                for (int j = 0; j < roomIdentifiers.Count; j++)
                {
                    if (j == i) continue;

                    float distance = Vector2.Distance(roomIdentifiers[i], roomIdentifiers[j]);

                    if(distance < firstShortest)
                    {
                        secondShortest = firstShortest;
                        b = a;
                        firstShortest = distance;
                        a = j;
                    }
                    else if(distance < secondShortest)
                    {
                        secondShortest = distance;
                        b = j;
                    }
                }
                connections.Add(new Vector2Int(i, a));
                connections.Add(new Vector2Int(i, b));

                Debug.Log(new Vector2Int(i, a) + " | "+ new Vector2Int(i, b) + " myPos-> " + roomIdentifiers[i]);
            }

            connections = DeleteRedundantNodes(connections);

            foreach(var conn in connections)
            {
                Debug.Log(conn);
            }

            DrawConnectionLines(connections, roomIdentifiers);
            return connections;


        } 

        private void DrawConnectionLines(HashSet<Vector2Int> connections, Dictionary<int, Vector2Int> roomIdentifiers)
        {
            this.lineController.counterLine = 0;
            this.lineController.ResetLines(connections.Count*2);
            foreach (var line in connections)
            {
                this.lineController.DrawLine(new Vector3((roomIdentifiers[line.x].x), roomIdentifiers[line.x].y, 0),
                    new Vector3((roomIdentifiers[line.y].x), roomIdentifiers[line.y].y, 0));
            }

        }

        private HashSet<Vector2Int> DeleteRedundantNodes(HashSet<Vector2Int> nodes) 
        {
            HashSet<Vector2Int> newNodes = new HashSet<Vector2Int>();
            
            foreach(var node in nodes)
            {
                var reversedNode = new Vector2Int(node.y, node.x);

                if(newNodes.Contains(node) || newNodes.Contains(reversedNode)){
                    continue;
                }
                newNodes.Add(node);
            }

            return newNodes;

        }

        private Vector2Int GetRandomPointInCircle(int radius)
        {

            float t = 2 * Mathf.PI * UnityEngine.Random.value;
            float u = UnityEngine.Random.value + UnityEngine.Random.value;
            float r;

            if (u > 1)
            {
                r = 2 - u;
            }
            else
            {
                r = u;
            }

            return new Vector2Int((int)(radius*r*Mathf.Cos(t)), (int)(radius*r*Mathf.Sin(t)));
        }

        private Vector2Int GetRandomRoomDimensions()
        {
            int widthDeviation = (int)(this.dimensionDeviation * UnityEngine.Random.value);
            int heightDeviation = (int)(this.dimensionDeviation * UnityEngine.Random.value);
            int width, height;

            float rand = UnityEngine.Random.value;

            if (rand < 0.25)
            {
                width = this.roomWidth + widthDeviation;
                height = this.roomHeight + heightDeviation;
            }
            else if(rand < 0.5)
            {
                width = this.roomWidth - widthDeviation;
                height = this.roomHeight + heightDeviation;
            }
            else if (rand < 0.75)
            {
                width = this.roomWidth + widthDeviation;
                height = this.roomHeight - heightDeviation;
            }
            else
            {
                width = this.roomWidth - widthDeviation;
                height = this.roomHeight - heightDeviation;
            }

            return new Vector2Int(width,height);
        }

        protected override void DestroyAllCreatedPrefabs()
        {
            /*throw new NotImplementedException();*/
        }
    }
}