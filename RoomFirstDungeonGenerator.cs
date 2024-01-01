using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomFirstDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    [SerializeField]
    private int minRoomWidth = 4;
    [SerializeField]
    private int minRoomHeight = 4;
    [SerializeField]
    private int dungeonWidth = 20, dungeonHeight = 20;
    [SerializeField]
    [Range(0, 10)]
    private int offset = 1;
    [SerializeField]
    private bool randomWalkRooms = false;


    protected override void RunProceduralGeneration(int currentLvl)
    {
        CreateRooms();
    }

    private List<BoundsInt> CreateRooms()
    {
        var roomsList = ProceduralGenerationAlgorithms.BinarySpacePartitioning(new BoundsInt((Vector3Int)startPosition,
            new Vector3Int(dungeonWidth, dungeonHeight, 0)), minRoomWidth, minRoomHeight);

        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        floor = CreateSimpleRooms(roomsList);
        tilemapVisualizer.PaintFloorTiles(floor, false);
        WallGenerator.CreateWalls(floor, tilemapVisualizer);

        return roomsList;

    }

    private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();

        foreach(var room in roomsList)
        {
            for(int col = offset; col<room.size.x - offset; col++)
            {
                for(int row =  offset; row < room.size.y - offset; row++)
                {
                    Vector2Int position = (Vector2Int)room.min + new Vector2Int(col, row);
                    floor.Add(position);
                }
            }
        }
        return floor;
    }

    protected HashSet<Vector2Int> RunRandomWalkGeneration(Vector2Int swStartPosition)
    {
        return base.CreateSeperateRoomTiles(swStartPosition);
    }

    public (HashSet<Vector2Int>, HashSet<Vector2Int>) CreateGoodFloorWithoutWallTiles(HashSet<Vector2Int> wallTiles, HashSet<Vector2Int> floorTiles, bool complex)
    {
        HashSet<Vector2Int> newFloor = new HashSet<Vector2Int>(floorTiles);
        HashSet<Vector2Int> newWalls = new HashSet<Vector2Int>();
        List<Vector2Int> directions = new List<Vector2Int>
        {
            new Vector2Int(1,1),
            new Vector2Int(1,0),
            new Vector2Int(1,-1),
            new Vector2Int(0,1),
            new Vector2Int(0,-1),
            new Vector2Int(-1,-1),
            new Vector2Int(-1,0),
            new Vector2Int(-1,1)
        };

        foreach(Vector2Int wallTile in wallTiles)
        {
            bool toOverride = true;
            foreach(Vector2Int dir in directions)
            {
                Vector2Int searchVec = wallTile + dir;
                if(!wallTiles.Contains(searchVec) && !floorTiles.Contains(searchVec))
                {
                    toOverride = false;
                    break;
                }
            }
            if(toOverride)
            {
                Debug.Log("DO NAPRAWY");
                newFloor.Add(wallTile);
            }
            else
            {
                newWalls.Add(wallTile);
            }
        }

        tilemapVisualizer.RepairRoomWallsAndTiles(newFloor, complex);

        return (newWalls, newFloor);
    }

    protected Dictionary<Vector2Int, HashSet<Vector2Int>> RunBSPGeneration()
    {
        List<BoundsInt> roomList = CreateRooms();

        Dictionary<Vector2Int, HashSet<Vector2Int>> bspRooms = new Dictionary<Vector2Int, HashSet<Vector2Int>>();

        foreach (BoundsInt b in roomList)
        {
            bspRooms.Add((Vector2Int)b.min,CreateSeperateRoomTiles(b));
        }

        return bspRooms;

    }

    private HashSet<Vector2Int> CreateSeperateRoomTiles(BoundsInt bounds)
    {
        HashSet<Vector2Int> roomTiles = new HashSet<Vector2Int>();
        for (int col = offset; col < bounds.size.x - offset; col++)
        {
            for (int row = offset; row < bounds.size.y - offset; row++)
            {
                Vector2Int position = (Vector2Int)bounds.min + new Vector2Int(col, row);
                roomTiles.Add(position);
            }
        }
        return roomTiles;

    }

}
