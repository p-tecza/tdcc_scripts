using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FullDungeonGenerator : RoomFirstDungeonGenerator
{

    // TODO ogarnij przechowywanie informacji o pokojach w hashsecie klasy Room, pola podlogi, sciany, id oraz polaczenia
    // TODO dodatkowo generacja randomowych prefabow i polaczen najlepiej

    protected SimpleRandomWalkDungeonGenerator simpleRandomWalkDungeon;
    protected RoomFirstDungeonGenerator roomFirstDungeon;

    [SerializeField]
    protected int treasureRoomsAmount = 2;

    public GameObject treasurePrefab;

    public List<GameObject> treasures = new List<GameObject>();

    private Vector3 startingPosition;

    private HashSet<Room> finalizedRooms = new HashSet<Room>(); 

    protected override void RunProceduralGeneration()
    {
        Dictionary < Vector2Int, HashSet < Vector2Int >> rooms = base.RunBSPGeneration();

        int minX = int.MaxValue;
        int minY =int.MaxValue;
        foreach(var cords in rooms.Keys)
        {
            if(minY>cords.y) minY = cords.y;
            if(minX>cords.x) minX = cords.x;
        }
        minX -= 25;
        minY -= 25;

        int randomAspect = UnityEngine.Random.Range(0, 2);
        int bspRoomsAmount = rooms.Count;



        int minSwX = minX;
        int minSwY = minY;
        HashSet<Vector2Int> swRoom;
        for (int i = 0; i < bspRoomsAmount + randomAspect; i++)
        {
            swRoom = base.RunRandomWalkGeneration(new Vector2Int(minX, minY));

            foreach (Vector2Int field in swRoom)
            {
                if (minSwX > field.x) minSwX = field.x;
                if (minSwY > field.y) minSwY = field.y;
            }

            rooms.Add(new Vector2Int(minSwX, minSwY), swRoom);
            minX -= 50;
        }

        Debug.Log("Rooms amount: "+rooms.Count);

        Debug.Log("typ keys: " + rooms.Keys.ToHashSet().GetType());

        Dictionary<int, Vector2Int> identifiedRooms = GenerateRoomsIDs(rooms.Keys.ToHashSet());

        
        int startingRoomID = DetermineStartingRoom(bspRoomsAmount);
        this.startingPosition = CalculateStartingPosition(rooms[identifiedRooms[startingRoomID]]);


        int bossRoomID = DetermineBossRoom(startingRoomID, identifiedRooms.Count);
        List<int> treasureRoomIDs = new List<int>();
        for (int i = 0; i < treasureRoomsAmount; i++)
        {
            int newTreasureRoomID = DetermineTreasureRoom(startingRoomID, bossRoomID, treasureRoomIDs, identifiedRooms.Count);
            treasureRoomIDs.Add(newTreasureRoomID);
            MarkRoom(rooms[identifiedRooms[newTreasureRoomID]], Color.yellow);
            GenerateTreasureRoomsPrefabs(rooms[identifiedRooms[newTreasureRoomID]]);
        }
        MarkRoom(rooms[identifiedRooms[startingRoomID]],Color.green);
        MarkRoom(rooms[identifiedRooms[bossRoomID]], Color.red);

        Dictionary<int, int> connections = GeneratePrimitiveConnections(identifiedRooms);

    }

    private Dictionary<int,Vector2Int> GenerateRoomsIDs(HashSet<Vector2Int> roomCoords) 
    {
        Dictionary<int, Vector2Int> newDict = new Dictionary<int, Vector2Int>();
        int iterator = 0;
        foreach (Vector2Int room in roomCoords)
        {
            newDict.Add(iterator,room);
            iterator++;
        }

        return newDict;
    }

    private int DetermineStartingRoom(int rangeOfRooms)
    {
        return UnityEngine.Random.Range(0, rangeOfRooms);
    }

    private int DetermineBossRoom(int startingRoomID, int roomsAmount)
    {
        return startingRoomID > roomsAmount - startingRoomID - 1 ? 0 : roomsAmount - 1;
    }

    private int DetermineTreasureRoom(int startingRoomID, int bossRoomID, List<int> currentTreasureRooms, int roomsAmount)
    {
        int treasureRoomId = 0;
        if (currentTreasureRooms.Count >= roomsAmount - 2) return currentTreasureRooms[0];
        do
        {
            treasureRoomId = UnityEngine.Random.Range(0, roomsAmount);
        }while (treasureRoomId == startingRoomID || treasureRoomId == bossRoomID || currentTreasureRooms.Contains(treasureRoomId));
        
        return treasureRoomId;
    }

    private void MarkRoom(HashSet<Vector2Int> roomTiles, Color color)
    {
        foreach(Vector2Int v in roomTiles)
        {
            tilemapVisualizer.PaintSingleTileWithColor(v, color);
        }
    }

    private Dictionary<int,int> GeneratePrimitiveConnections(Dictionary<int, Vector2Int> identifiedRooms)
    {
        Dictionary<int,int> newDict = new Dictionary<int,int>();
        for(int i = 0; i< identifiedRooms.Count - 1; i++)
        {
            newDict.Add(i, i + 1);
            /*Debug.Log("Connection: " + i + " -> " + (i + 1) + " | KORDY: " + identifiedRooms[i]+ " -> " + identifiedRooms[i + 1]);*/
        }
        return newDict;
    }

    private void GenerateTreasureRoomsPrefabs(HashSet<Vector2Int> roomTiles)
    {
        Vector2Int position = DetermineRoomCenter(roomTiles);
        GameObject cosTam = Instantiate(this.treasurePrefab, (Vector3Int)position, Quaternion.identity);
        this.treasures.Add(cosTam);

        Debug.Log("costam: " + cosTam.ToString());

    }

    private Vector2Int DetermineRoomCenter(HashSet<Vector2Int> roomTiles)
    {

        int minX = Int32.MaxValue, minY = Int32.MaxValue, maxX = Int32.MinValue, maxY = Int32.MinValue;
        Vector2Int substitatePlacement = new Vector2Int();

        foreach(Vector2Int tile in roomTiles)
        {

            if(tile.x < minX) minX = tile.x;
            else if(tile.x > maxX) maxX = tile.x;

            if(tile.y < minY) minY = tile.y;
            else if (tile.y > maxY) maxY = tile.y;

            substitatePlacement = tile;
        }

        Vector2Int pseudoCenter = new Vector2Int((minX + maxX) / 2, (minY + maxY) / 2);

        if (!roomTiles.Contains(pseudoCenter))
        {
            Debug.Log("NIE POSIADAM");
            return substitatePlacement;
        }

        return pseudoCenter;
    }

    protected override void DestroyAllCreatedPrefabs()
    {
        foreach(GameObject go in this.treasures)
        {
            DestroyImmediate(go);
            Debug.Log("DESTROYING "+go.ToString());
        }
        this.treasures = new List<GameObject>();
    }

    private Vector3 CalculateStartingPosition(HashSet<Vector2Int> roomTiles)
    {
        Vector2Int vector2Center = DetermineRoomCenter(roomTiles);
        return new Vector3(vector2Center.x, vector2Center.y, 0);
    }

    public Vector3 GetStartingPosition()
    {
        return this.startingPosition;
    }

    private HashSet<Vector2Int> ScanForWallsOfRoom(HashSet<Vector2Int> roomTiles)
    {
        HashSet<Vector2Int> roomWallTiles = new HashSet<Vector2Int>();        
        HashSet<Vector2Int> wallTiles = WallGenerator.getAllWallTiles();

        foreach(Vector2Int tile in roomTiles)
        {
            if(wallTiles.Contains(tile + Vector2Int.left))
            {
                roomWallTiles.Add(tile + Vector2Int.left);

            }else if (wallTiles.Contains(tile + Vector2Int.right))
            {
                roomWallTiles.Add(tile + Vector2Int.right);
            }
            else if (wallTiles.Contains(tile + Vector2Int.down))
            {
                roomWallTiles.Add(tile + Vector2Int.down);
            }
            else if (wallTiles.Contains(tile + Vector2Int.up))
            {
                roomWallTiles.Add(tile + Vector2Int.up);
            }
        }

        return roomWallTiles;

    }

}
