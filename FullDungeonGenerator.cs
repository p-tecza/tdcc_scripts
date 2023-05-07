using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class FullDungeonGenerator : RoomFirstDungeonGenerator
{

    // TODO ogarnij przechowywanie informacji o pokojach w hashsecie klasy Room, pola podlogi, sciany, id oraz polaczenia
    // TODO dodatkowo generacja randomowych prefabow i polaczen najlepiej

    protected SimpleRandomWalkDungeonGenerator simpleRandomWalkDungeon;
    protected RoomFirstDungeonGenerator roomFirstDungeon;

    [SerializeField]
    protected int treasureRoomsAmount = 2;

    public GameObject treasurePrefab, enemyPrefab, coinsPrefab;

    public List<GameObject> treasures = new List<GameObject>();
    public List<GameObject> enemies = new List<GameObject>();
    public List<GameObject> coins = new List<GameObject>();
    [Range(1,20)]
    public int maxEnemiesInRoom = 5;
    [Range(1, 20)]
    public int maxCoinsInRoom = 5;

    private Vector3 startingPosition;

    private static Dictionary<int, Room> finalizedRooms = new Dictionary<int, Room>();

    private static int startingRoomID;
    List<int> bossRoomIDs = new List<int>();
    List<int> treasureRoomIDs = new List<int>();

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

        Debug.Log("Dodawanie pokojow do listy...");

        int roomIt = 0;
        foreach(var roomTiles in rooms.Values)
        {
            Room newRoom = new Room();
            HashSet<Vector2Int> wallTiles = ScanForWallsOfRoom(roomTiles);
            newRoom.FloorTiles = roomTiles;
            newRoom.WallTiles = wallTiles;
            newRoom.Id = roomIt;
            finalizedRooms.Add(roomIt,newRoom);
            roomIt++;
        }

/*        Dictionary<int, Vector2Int> identifiedRooms = GenerateRoomsIDs(rooms.Keys.ToHashSet());*/

        
        startingRoomID = DetermineStartingRoom(bspRoomsAmount);
        /*this.startingPosition = CalculateStartingPosition(rooms[identifiedRooms[startingRoomID]]);*/
        this.startingPosition = CalculateStartingPosition(finalizedRooms[startingRoomID].FloorTiles);

        this.bossRoomIDs.Add(DetermineBossRoom(startingRoomID, finalizedRooms.Count));
        /*List<int> treasureRoomIDs = new List<int>();*/
        for (int i = 0; i < treasureRoomsAmount; i++)
        {
            int newTreasureRoomID = DetermineTreasureRoom(startingRoomID, this.bossRoomIDs[0], this.treasureRoomIDs, finalizedRooms.Count);
            this.treasureRoomIDs.Add(newTreasureRoomID);
            MarkRoom(finalizedRooms[newTreasureRoomID].FloorTiles, Color.yellow);
            GenerateTreasureRoomsPrefabs(finalizedRooms[newTreasureRoomID].FloorTiles);
        }
        MarkRoom(finalizedRooms[startingRoomID].FloorTiles,Color.green);
        MarkRoom(finalizedRooms[this.bossRoomIDs[0]].FloorTiles, Color.red);

        finalizedRooms = GeneratePrimitiveConnections(finalizedRooms);
        CreatePassagesBetweenRooms(finalizedRooms);
        GenerateEnemies();
        GenerateCoins();

        /*Dictionary<int, int> connections = GeneratePrimitiveConnections(identifiedRooms);*/

    }

/*    private Dictionary<int,Vector2Int> GenerateRoomsIDs(HashSet<Vector2Int> roomCoords) 
    {
        Dictionary<int, Vector2Int> newDict = new Dictionary<int, Vector2Int>();
        int iterator = 0;
        foreach (Vector2Int room in roomCoords)
        {
            newDict.Add(iterator,room);
            iterator++;
        }

        return newDict;
    }*/

    private void GenerateEnemies()
    {
        int randNumOfEnemies;
        foreach (var room in finalizedRooms.Values)
        {

            if(treasureRoomIDs.Contains(room.Id) || bossRoomIDs.Contains(room.Id) || startingRoomID == room.Id)
            {
                continue;
            }

            randNumOfEnemies = UnityEngine.Random.Range(1, maxEnemiesInRoom);
            HashSet<GameObject> enemiesInRoom = new HashSet<GameObject>(); //do zmiany, trzeba dopisaæ do listy pokoju

            for (int i = 0; i < randNumOfEnemies; i++)
            {
                Vector2Int randomField = room.FloorTiles.ElementAt(UnityEngine.Random.Range(0, room.FloorTiles.Count));
                GameObject enemy = Instantiate(this.enemyPrefab, ((Vector3Int)randomField), Quaternion.identity);
                this.enemies.Add(enemy);
                enemiesInRoom.Add(enemy);
            }
            
            room.RoomObjects = enemiesInRoom;

            
        }

    }

    private void GenerateCoins()
    {
        int randNumOfCoins;
        foreach (var room in finalizedRooms.Values)
        {

            randNumOfCoins = UnityEngine.Random.Range(1, maxEnemiesInRoom);
            HashSet<GameObject> currentObjectsInRoom = new HashSet<GameObject>(); //do zmiany, trzeba dopisaæ do listy pokoju

            for (int i = 0; i < randNumOfCoins; i++)
            {
                Vector2Int randomField = room.FloorTiles.ElementAt(UnityEngine.Random.Range(0, room.FloorTiles.Count));
                GameObject enemy = Instantiate(this.coinsPrefab, ((Vector3Int)randomField), Quaternion.identity);
                this.coins.Add(enemy);
                currentObjectsInRoom.Add(enemy);
            }
             room.RoomObjects = currentObjectsInRoom;

        }

    }

    private void CreatePassagesBetweenRooms(Dictionary<int,Room> finalizedRooms)
    {


        for(int i=0; i <finalizedRooms.Count-1;i++)
        {
            HashSet<Vector2Int> wallTiles = finalizedRooms[i].WallTiles;
            int rand;



            rand = UnityEngine.Random.Range(0, wallTiles.Count);

            foreach(Vector2Int wallTile in wallTiles)
            {
                rand--;
                if (rand <= 0)
                {
                    var entranceTile = finalizedRooms[i].WallTiles.First();
                    if (wallTile == finalizedRooms[i].entrance)
                    {
                        continue;
                    }

                    var exitTileToMark = new HashSet<Vector2Int>
                    {
                        wallTile
                    };

                    var entranceTileToMark = new HashSet<Vector2Int>
                    {
                        entranceTile
                    };

                    finalizedRooms[i].exit = wallTile;
                    finalizedRooms[i + 1].entrance = entranceTile;
                    MarkWallTile(exitTileToMark,Color.blue);
                    MarkWallTile(entranceTileToMark, Color.magenta);

                    Debug.Log("MALOWANIE SCIANY:");
                    Debug.Log(wallTile.ToString());
                    break;
                }
            }

        }

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

    private void MarkWallTile(HashSet<Vector2Int> roomTiles, Color color)
    {
        foreach (Vector2Int v in roomTiles)
        {
            tilemapVisualizer.PaintSingleWallTileWithColor(v, color);
        }
    }

    private Dictionary<int,Room> GeneratePrimitiveConnections(Dictionary<int, Room> finalizedRooms)
    {
        Dictionary<int, Room> returnRooms = new Dictionary<int, Room>();
        /*Dictionary<int,int> newDict = new Dictionary<int,int>();*/
        for (int i = 0; i< finalizedRooms.Count - 1; i++)
        {
            Room room = finalizedRooms[i];
            List<int> conns = new List<int>
            {
                i + 1
            };
            room.connections = conns;
            returnRooms.Add(i,room);
            /*Debug.Log("Connection: " + i + " -> " + (i + 1) + " | KORDY: " + identifiedRooms[i]+ " -> " + identifiedRooms[i + 1]);*/
        }
        return returnRooms;
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
        foreach (GameObject go in this.enemies)
        {
            DestroyImmediate(go);
            Debug.Log("DESTROYING " + go.ToString());
        }
        foreach (GameObject go in this.coins)
        {
            DestroyImmediate(go);
            Debug.Log("DESTROYING " + go.ToString());
        }
        this.treasures = new List<GameObject>();
        this.enemies = new List<GameObject>();
        this.coins = new List<GameObject>();
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

    public static Dictionary<int,Room> GetFinalizedRooms()
    {
        return finalizedRooms;
    }

    public static int GetStartingRoomID()
    {
        return startingRoomID;
    }


}
