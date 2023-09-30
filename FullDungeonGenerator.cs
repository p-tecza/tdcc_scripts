using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class FullDungeonGenerator : RoomFirstDungeonGenerator
{

    // TODO ogarnij przechowywanie informacji o pokojach w hashsecie klasy Room, pola podlogi, sciany, id oraz polaczenia
    // TODO dodatkowo generacja randomowych prefabow i polaczen najlepiej

    protected SimpleRandomWalkDungeonGenerator simpleRandomWalkDungeon;
    protected RoomFirstDungeonGenerator roomFirstDungeon;

    [SerializeField]
    protected int treasureRoomsAmount = 2;

    public GameObject availableTreasures;
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
    [Range(0, 3)]
    public int maxHpPotsInChest = 2;
    [Range(0, 8)]
    public int maxCoinsInChest = 4;
    [Range(0, 1)]
    public int maxStarsInChest = 1;

    private Vector3 startingPosition;

    private static Dictionary<int, Room> finalizedRooms = new Dictionary<int, Room>();

    private static int startingRoomID;
    List<int> bossRoomIDs = new List<int>();
    List<int> treasureRoomIDs = new List<int>();

    protected override void RunProceduralGeneration()
    {
        if (this.isSeeded)
        {
            UnityEngine.Random.InitState(this.seed);
            ProceduralGenerationAlgorithms.InitSeed(this.seed);
            Debug.Log("SEEDED");
        }
        else
        {
            Debug.Log("NOT SEEDED");
            UnityEngine.Random.InitState(UnityEngine.Random.Range(0,int.MaxValue));
            ProceduralGenerationAlgorithms.InitSeed(UnityEngine.Random.Range(0,int.MaxValue));
        }

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
            Room newRoom;
            if (roomIt >= bspRoomsAmount)
            {
                newRoom = new Room(true);
            }
            else
            {
                newRoom = new Room(false);
            }

            HashSet<Vector2Int> wallTiles = ScanForWallsOfRoom(roomTiles);
            newRoom.FloorTiles = roomTiles;
            newRoom.WallTiles = wallTiles;
            newRoom.Id = roomIt;
            finalizedRooms.Add(roomIt, newRoom);
            roomIt++;
        }

        Debug.Log("COUNT FINALIZED ROOMS: " + finalizedRooms.Count);

        startingRoomID = DetermineStartingRoom(bspRoomsAmount);
        this.startingPosition = CalculateStartingPosition(finalizedRooms[startingRoomID].FloorTiles);
        this.bossRoomIDs.Add(DetermineBossRoom(startingRoomID, finalizedRooms.Count));


        int amountOfPossibleTreasures = this.availableTreasures.transform.childCount;
        Dictionary<string, int> treasureContent = GenerateTreasureContent(amountOfPossibleTreasures);

        for (int i = 0; i < treasureRoomsAmount; i++)
        {
            int newTreasureRoomID = DetermineTreasureRoom(startingRoomID, this.bossRoomIDs[0], this.treasureRoomIDs, finalizedRooms.Count);
            this.treasureRoomIDs.Add(newTreasureRoomID);

            Dictionary<string, int> thisTreasureContent = GetSpecificTreasureContent(treasureContent, i);
            MarkRoom(finalizedRooms[newTreasureRoomID].FloorTiles, Color.yellow);
            GenerateTreasureRoomsPrefabs(finalizedRooms[newTreasureRoomID].FloorTiles, thisTreasureContent);
        }

        MarkRoom(finalizedRooms[startingRoomID].FloorTiles, Color.green);
        MarkRoom(finalizedRooms[this.bossRoomIDs[0]].FloorTiles, Color.red);

        finalizedRooms = GeneratePrimitiveConnections(finalizedRooms);
        CreatePassagesBetweenRooms(finalizedRooms);
        CreateTeleportToLocations(finalizedRooms);
        GenerateEnemies();
        GenerateCoins();

    }

    private Dictionary<string, int> GetSpecificTreasureContent(Dictionary<string, int> all, int whichRoom)
    {
        return new Dictionary<string, int>()
        {
            { "item", all["item"+whichRoom] },
            { "hpPots", all["hpPots"+whichRoom] },
            { "coins", all["coins"+whichRoom] },
            { "stars", all["stars"+whichRoom] },
        };
    }

    private Dictionary<string, int> GenerateTreasureContent(int amountOfTreasures)
    {
        Dictionary<string, int> treasureContent= new Dictionary<string, int>();
        List<int> rolledValues = new List<int>();
        for(int i = 0; i < this.treasureRoomsAmount; i++)
        {
            int itemIt = UnityEngine.Random.Range(0, amountOfTreasures);
            if (rolledValues.Contains(itemIt))
            {
                i--;
                continue;
            }
            rolledValues.Add(itemIt);
            treasureContent.Add("item" + i, itemIt);
        }

        for(int j = 0; j < this.treasureRoomsAmount; j++)
        {
            int hpPots = UnityEngine.Random.Range(0, this.maxHpPotsInChest);
            int coins = UnityEngine.Random.Range(0, this.maxCoinsInChest);
            int stars = this.maxStarsInChest;

            for (int i = 0; i < maxStarsInChest * 3; i++)
            {
                if (UnityEngine.Random.Range(0, 2) == 0)
                {
                    stars--;
                    if (stars == 0) break;
                }
            }
            treasureContent.Add("hpPots"+j, hpPots);
            treasureContent.Add("coins"+j, coins);
            treasureContent.Add("stars"+j, stars);
        }
        return treasureContent;
    }

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
                    break;
                }

                if (loopIt == randExitPosition)
                {
                    var exitTileToMark = wallTile;
                    finalizedRooms[i].exit = new Teleport();
                    finalizedRooms[i].exit.teleportFrom = exitTileToMark;


                    DeleteSingleTile(exitTileToMark);

                    int rotationAngles = DetermineRotationOfTeleport(exitTileToMark, finalizedRooms[i].FloorTiles);

                    GenerateRoomExitPrefab(exitTileToMark, rotationAngles);
                    CreateWallTilesAroundTeleport(exitTileToMark, finalizedRooms[i].FloorTiles, finalizedRooms[i].WallTiles);
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

                int rotationAngles = DetermineRotationOfTeleport(entranceTileToMark, finalizedRooms[i + 1].FloorTiles);
                GenerateRoomEntrancePrefab(entranceTileToMark, rotationAngles);
                CreateWallTilesAroundTeleport(entranceTileToMark, finalizedRooms[i + 1].FloorTiles, finalizedRooms[i + 1].WallTiles);
                break;

            }
        }
    }

    private void CreateWallTilesAroundTeleport(Vector2Int tileToMark,
        HashSet<Vector2Int> floorTiles, HashSet<Vector2Int> wallTiles)
    {
        int tileX = tileToMark.x;
        int tileY = tileToMark.y;
        HashSet<Vector2Int> additionalWallTiles = new HashSet<Vector2Int>();

        if (!floorTiles.Contains(new Vector2Int(tileX, tileY + 1))
            && !wallTiles.Contains(new Vector2Int(tileX, tileY + 1)))
        {
            additionalWallTiles.Add(new Vector2Int(tileX, tileY + 1));
        }
        if (!floorTiles.Contains(new Vector2Int(tileX, tileY - 1))
            && !wallTiles.Contains(new Vector2Int(tileX, tileY - 1)))
        {
            additionalWallTiles.Add(new Vector2Int(tileX, tileY - 1));
        }
        if (!floorTiles.Contains(new Vector2Int(tileX + 1, tileY))
            && !wallTiles.Contains(new Vector2Int(tileX + 1, tileY)))
        {
            additionalWallTiles.Add(new Vector2Int(tileX + 1, tileY));
        }
        if (!floorTiles.Contains(new Vector2Int(tileX - 1, tileY))
            && !wallTiles.Contains(new Vector2Int(tileX - 1, tileY)))
        {
            additionalWallTiles.Add(new Vector2Int(tileX - 1, tileY));
        }

        WallGenerator.GenerateWallsInGivenLocations(additionalWallTiles, tilemapVisualizer);

    }

    private void CreateTeleportToLocations(Dictionary<int, Room> finalizedRooms)
    {

        foreach (var room in finalizedRooms)
        {
            foreach (int connectedRoomID in room.Value.connections)
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
        return 0;
    }

    private int DetermineBossRoom(int startingRoomID, int roomsAmount)
    {
        return startingRoomID > roomsAmount - startingRoomID - 1 ? 0 : roomsAmount - 1;
    }

    private int DetermineTreasureRoom(int startingRoomID, int bossRoomID, List<int> currentTreasureRooms, int roomsAmount)
    {
        int treasureRoomId;
        UnityEngine.Random.InitState(this.seed);

        if (currentTreasureRooms.Count >= roomsAmount - 2) return currentTreasureRooms[0];
        do
        {
            treasureRoomId = UnityEngine.Random.Range(1, roomsAmount - 1);
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

    private void GenerateTreasureRoomsPrefabs(HashSet<Vector2Int> roomTiles, Dictionary<string, int> treasureContent)
    {
        Vector2Int position = DetermineRoomCenter(roomTiles);
        GameObject cosTam = Instantiate(this.treasurePrefab, (Vector3Int)position, Quaternion.identity);
        this.treasures.Add(cosTam);
        cosTam.GetComponent<Treasure>().SetContent(treasureContent);
    }

    private void GenerateRoomExitPrefab(Vector2Int position, int rotationAngles)
    {
        Vector3 fixedPosition = new Vector3(position.x + 0.5f, position.y + 0.5f, 0);
        Quaternion rotation = Quaternion.Euler(0, 0, rotationAngles);
        GameObject exitObject = Instantiate(this.teleportOutPrefab, fixedPosition, rotation);
        this.exits.Add(exitObject);
    }

    private void GenerateRoomEntrancePrefab(Vector2Int position, int rotationAngles)
    {
        Vector3 fixedPosition = new Vector3(position.x + 0.5f, position.y + 0.5f, 0);
        Quaternion rotation = Quaternion.Euler(0, 0, rotationAngles + 180);
        GameObject entranceObject = Instantiate(this.teleportInPrefab, fixedPosition, rotation);
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

    private int DetermineRotationOfTeleport(Vector2Int tile, HashSet<Vector2Int> roomTiles)
    {

        if (roomTiles.Contains(new Vector2Int(tile.x, tile.y - 1)))
        {
            return 0;
        }
        else if (roomTiles.Contains(new Vector2Int(tile.x, tile.y + 1)))
        {
            return 180;
        }
        else if (roomTiles.Contains(new Vector2Int(tile.x - 1, tile.y)))
        {
            return 90;
        }
        else if (roomTiles.Contains(new Vector2Int(tile.x + 1, tile.y)))
        {
            return 270;
        }

        return 0;
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
        return finalizedRooms;
    }

    public static int GetStartingRoomID()
    {
        return startingRoomID;
    }


}
