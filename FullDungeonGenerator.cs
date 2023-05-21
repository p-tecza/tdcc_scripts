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
    public GameObject teleportInPrefab, teleportOutPrefab;

    public List<GameObject> treasures = new List<GameObject>();
    public List<GameObject> enemies = new List<GameObject>();
    public List<GameObject> coins = new List<GameObject>();
    public List<GameObject> entrances = new List<GameObject>();
    public List<GameObject> exits = new List<GameObject>();
    [Range(1, 20)]
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
        Dictionary<Vector2Int, HashSet<Vector2Int>> rooms = base.RunBSPGeneration();

        int minX = int.MaxValue;
        int minY = int.MaxValue;
        foreach (var cords in rooms.Keys)
        {
            if (minY > cords.y) minY = cords.y;
            if (minX > cords.x) minX = cords.x;
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

        int roomIt = 0;
        foreach (var roomTiles in rooms.Values)
        {
            Room newRoom = new Room();
            HashSet<Vector2Int> wallTiles = ScanForWallsOfRoom(roomTiles);
            newRoom.FloorTiles = roomTiles;
            newRoom.WallTiles = wallTiles;
            newRoom.Id = roomIt;
            finalizedRooms.Add(roomIt, newRoom);
            roomIt++;
        }

        Debug.Log("COUNT FINALIZED ROOMS: " + finalizedRooms.Count);

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
        MarkRoom(finalizedRooms[startingRoomID].FloorTiles, Color.green);
        MarkRoom(finalizedRooms[this.bossRoomIDs[0]].FloorTiles, Color.red);

        finalizedRooms = GeneratePrimitiveConnections(finalizedRooms);
        CreatePassagesBetweenRooms(finalizedRooms);
        CreateTeleportToLocations(finalizedRooms);
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

            if (treasureRoomIDs.Contains(room.Id) || bossRoomIDs.Contains(room.Id) || startingRoomID == room.Id)
            {
                continue;
            }

            randNumOfEnemies = UnityEngine.Random.Range(1, maxEnemiesInRoom);
            HashSet<GameObject> enemiesInRoom = new HashSet<GameObject>(); //do zmiany, trzeba dopisaæ do listy pokoju

            for (int i = 0; i < randNumOfEnemies; i++)
            {
                Vector2Int randomField = room.FloorTiles.ElementAt(UnityEngine.Random.Range(0, room.FloorTiles.Count));
                GameObject enemy = Instantiate(this.enemyPrefab, ((Vector3Int)randomField), Quaternion.identity);
                /*enemy.AddComponent<BasicEnemy>();*/
                this.enemies.Add(enemy);
                room.enemies.Add(enemy);
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

    private void CreatePassagesBetweenRooms(Dictionary<int, Room> finalizedRooms)
    {
        for (int i = 0; i < finalizedRooms.Count - 1; i++)
        {
            HashSet<Vector2Int> wallTiles = finalizedRooms[i].WallTiles;
            HashSet<Vector2Int> nextRoomWallTiles = finalizedRooms[i + 1].WallTiles;
            int randExitPosition, randEntrancePosition;
            randExitPosition = UnityEngine.Random.Range(0, (int)(wallTiles.Count / 2));
            randEntrancePosition = UnityEngine.Random.Range((int)(nextRoomWallTiles.Count / 2), nextRoomWallTiles.Count);

            int loopIt = 0;
            foreach (Vector2Int wallTile in wallTiles)
            {
                if (this.bossRoomIDs.Contains(i))
                {
                    Debug.Log("JESTEM W BOSS ROOMIE WIEC BREJKUJE WYJSCIE!");
                    break;
                }

                if (loopIt == randExitPosition)
                {
                    var exitTileToMark = wallTile;
                    finalizedRooms[i].exit = new Teleport();
                    finalizedRooms[i].exit.teleportFrom = exitTileToMark;


                    DeleteSingleTile(exitTileToMark);
                    GenerateRoomExitPrefab(exitTileToMark);
                    break;
                }
                loopIt++;
            }

            int pseudoIt = 0;
            foreach (Vector2Int nextRoomWallTile in nextRoomWallTiles)
            {
                if (startingRoomID == i + 1)
                {
                    break;
                }

                if (pseudoIt < randEntrancePosition)
                {
                    pseudoIt++;
                    continue;
                }

                var entranceTileToMark = nextRoomWallTile;
                finalizedRooms[i + 1].entrance = new Teleport();
                finalizedRooms[i + 1].entrance.teleportFrom = entranceTileToMark;
                finalizedRooms[i + 1].entrance.teleportToRoomId = i;
                DeleteSingleTile(entranceTileToMark);
                GenerateRoomEntrancePrefab(entranceTileToMark);
                break;

            }
        }
    }

    private void CreateTeleportToLocations(Dictionary<int, Room> finalizedRooms)
    {

        foreach(var room in finalizedRooms)
        {
            foreach(int connectedRoomID in room.Value.connections)
            {
                room.Value.exit.teleportTo = FindProperRoomTileAroundTeleportLocation(finalizedRooms[connectedRoomID].FloorTiles,
                    finalizedRooms[connectedRoomID].entrance.teleportFrom);
                room.Value.exit.teleportToRoomId = connectedRoomID;
                finalizedRooms[connectedRoomID].entrance.teleportTo = FindProperRoomTileAroundTeleportLocation(
                    room.Value.FloorTiles, room.Value.exit.teleportFrom);
            }
        }

        

    }


    private int DetermineStartingRoom(int rangeOfRooms)
    {
        return 0;/*UnityEngine.Random.Range(0, rangeOfRooms);*/
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
        } while (treasureRoomId == startingRoomID || treasureRoomId == bossRoomID || currentTreasureRooms.Contains(treasureRoomId));

        return treasureRoomId;
    }

    private void MarkRoom(HashSet<Vector2Int> roomTiles, Color color)
    {
        foreach (Vector2Int v in roomTiles)
        {
            tilemapVisualizer.PaintSingleTileWithColor(v, color);
        }
    }

    private void DeleteSingleTile(Vector2Int singleWallTile)
    {
        tilemapVisualizer.DeleteSingleTile(singleWallTile);
    }

    private Dictionary<int, Room> GeneratePrimitiveConnections(Dictionary<int, Room> finalizedRooms)
    {
        Dictionary<int, Room> returnRooms = new Dictionary<int, Room>();
        /*Dictionary<int,int> newDict = new Dictionary<int,int>();*/
        for (int i = 0; i < finalizedRooms.Count; i++)
        {

            Room room = finalizedRooms[i];
            
            List<int> conns = new List<int>
            {
                i + 1
            };
            if (this.bossRoomIDs.Contains(room.Id))
            {
                conns = new List<int>();
            }
            room.connections = conns;

            returnRooms.Add(i, room);
        }

        return returnRooms;
    }

    private void GenerateTreasureRoomsPrefabs(HashSet<Vector2Int> roomTiles)
    {
        Vector2Int position = DetermineRoomCenter(roomTiles);
        GameObject cosTam = Instantiate(this.treasurePrefab, (Vector3Int)position, Quaternion.identity);
        this.treasures.Add(cosTam);

    }

    private void GenerateRoomExitPrefab(Vector2Int position)
    {
        Vector3 fixedPosition = new Vector3(position.x + 0.5f, position.y + 0.5f, 0);
        GameObject exitObject = Instantiate(this.teleportOutPrefab, fixedPosition, Quaternion.identity);
        this.exits.Add(exitObject);
    }

    private void GenerateRoomEntrancePrefab(Vector2Int position)
    {
        Vector3 fixedPosition = new Vector3(position.x + 0.5f, position.y + 0.5f, 0);
        GameObject entranceObject = Instantiate(this.teleportInPrefab, fixedPosition, Quaternion.identity);
        this.entrances.Add(entranceObject);
    }

    private Vector2Int DetermineRoomCenter(HashSet<Vector2Int> roomTiles)
    {

        int minX = Int32.MaxValue, minY = Int32.MaxValue, maxX = Int32.MinValue, maxY = Int32.MinValue;
        Vector2Int substitatePlacement = new Vector2Int();

        foreach (Vector2Int tile in roomTiles)
        {

            if (tile.x < minX) minX = tile.x;
            else if (tile.x > maxX) maxX = tile.x;

            if (tile.y < minY) minY = tile.y;
            else if (tile.y > maxY) maxY = tile.y;

            substitatePlacement = tile;
        }

        Vector2Int pseudoCenter = new Vector2Int((minX + maxX) / 2, (minY + maxY) / 2);

        if (!roomTiles.Contains(pseudoCenter))
        {
            return substitatePlacement;
        }

        return pseudoCenter;
    }

    protected Vector2Int FindProperRoomTileAroundTeleportLocation(HashSet<Vector2Int> roomFloorTiles, Vector2Int teleportLocation)
    {
        Vector2Int newPosition = CalculateProperRoomTileAroundTeleportLocation(roomFloorTiles, teleportLocation, 2);
        if (newPosition == teleportLocation)
        {
            newPosition = CalculateProperRoomTileAroundTeleportLocation(roomFloorTiles, teleportLocation, 1);
        }

       /* foreach(Vector2Int roomTile in roomFloorTiles) {
            Debug.Log(roomTile);
        }
*/
        Debug.Log("TELEPORT LOCATION: "+teleportLocation);

        return newPosition;
    }

    protected Vector2Int CalculateProperRoomTileAroundTeleportLocation(HashSet<Vector2Int> roomFloorTiles,
        Vector2Int teleportLocation, int distance)
    {

        Vector2Int leftPosition = new Vector2Int(teleportLocation.x - distance, teleportLocation.y);
        Vector2Int rightPosition = new Vector2Int(teleportLocation.x + distance, teleportLocation.y);
        Vector2Int topPosition = new Vector2Int(teleportLocation.x, teleportLocation.y + distance);
        Vector2Int bottomPosition = new Vector2Int(teleportLocation.x + 1, teleportLocation.y - distance);


        if (roomFloorTiles.Contains(leftPosition))
        {
            return leftPosition;
        }
        else if (roomFloorTiles.Contains(rightPosition))
        {
            return rightPosition;
        }
        else if (roomFloorTiles.Contains(bottomPosition))
        {
            return bottomPosition;
        }
        else if (roomFloorTiles.Contains(topPosition))
        {
            return topPosition;
        }
        return teleportLocation;

    }

    protected override void DestroyAllCreatedPrefabs()
    {
        foreach (GameObject go in this.treasures)
        {
            DestroyImmediate(go);
            Debug.Log("DESTROYING " + go.ToString());
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
        foreach (GameObject go in this.exits)
        {
            DestroyImmediate(go);
            Debug.Log("DESTROYING " + go.ToString());
        }
        foreach (GameObject go in this.entrances)
        {
            DestroyImmediate(go);
            Debug.Log("DESTROYING " + go.ToString());
        }
        this.treasures = new List<GameObject>();
        this.enemies = new List<GameObject>();
        this.coins = new List<GameObject>();
        this.exits = new List<GameObject>();
        this.entrances = new List<GameObject>();
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

        foreach (Vector2Int tile in roomTiles)
        {
            if (wallTiles.Contains(tile + Vector2Int.left))
            {
                roomWallTiles.Add(tile + Vector2Int.left);
            }
            else if (wallTiles.Contains(tile + Vector2Int.right))
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
    public static Dictionary<int, Room> GetFinalizedRooms()
    {
        Debug.Log("IDs");
        foreach(Room r in finalizedRooms.Values)
        {
            Debug.Log(r.Id);
        }
        Debug.Log("Koniec IDs");
        return finalizedRooms;
    }

    public static int GetStartingRoomID()
    {
        return startingRoomID;
    }


}
