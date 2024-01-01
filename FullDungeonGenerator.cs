using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class FullDungeonGenerator : RoomFirstDungeonGenerator
{

    protected SimpleRandomWalkDungeonGenerator simpleRandomWalkDungeon;
    protected RoomFirstDungeonGenerator roomFirstDungeon;

    [SerializeField]
    protected int treasureRoomsAmount = 2;

    public GameObject availableTreasures;
    public GameObject treasurePrefab, enemyPrefab, rangeEnemyPrefab, coinsPrefab;
    public GameObject teleportInPrefab, teleportPrefab;

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
    [SerializeField]
    public ShopRoomGenerator shopRoomGenerator;
    [SerializeField]
    private BossRoomGenerator bossRoomGenerator;
    private SpecialRoomDeterminer specialRoomDeterminer;
    [SerializeField]
    private GameObject instantiatedDungeonObjects;

    /*UnityEngine.Random.State testState;*/
    /*public bool isThisSavedGame = UndestroyableSceneController.isThisGameFromSave;*/
    private bool savedGameNeedsDeletionOfEntities = false;
    private SaveData loadedData = null;

    protected override void RunProceduralGeneration(int level)
    {


        Debug.Log("IS THIS W FULL DUNG:" + UndestroyableSceneController.isThisGameFromSave);

        if(UndestroyableSceneController.isThisGameFromSave)
        {
            Debug.Log("PROBA GENERACJI Z ZAPISANEGO STATE");
            this.loadedData = SaveSystem.LoadData();
            GameController.gameFromSave = true;

            UnityEngine.Random.state = this.loadedData.gameState;
            Debug.Log("PO GENERACJI DANE ENEMY:" + this.loadedData.slainEnemyIDs.Count);
            Debug.Log("PO GENERACJI DANE COIN:" + this.loadedData.collectedCoinIDs.Count);
            UndestroyableSceneController.isThisGameFromSave = false;
            SaveSystem.gameState = UnityEngine.Random.state;
            this.savedGameNeedsDeletionOfEntities = true;
        }
        else if (this.isSeeded && level == 1)
        {
            UnityEngine.Random.InitState(this.seed);
            /*testState = UnityEngine.Random.state;*/
            /*ProceduralGenerationAlgorithms.InitSeed(this.seed);*/
            SaveSystem.gameState = UnityEngine.Random.state;
            Debug.Log("SEEDED");
        }
        else if(level == 1)
        {
            Debug.Log("NOT SEEDED");
            UnityEngine.Random.InitState(UnityEngine.Random.Range(0,int.MaxValue));
            /*ProceduralGenerationAlgorithms.InitSeed(UnityEngine.Random.Range(0,int.MaxValue));*/
            SaveSystem.gameState = UnityEngine.Random.state;
        }
        else
        {

            /*UnityEngine.Random.state = testState;*/
            SaveSystem.gameState = UnityEngine.Random.state;
            finalizedRooms = new Dictionary<int, Room>();
            treasures = new List<GameObject>();
            enemies = new List<GameObject>();
            coins = new List<GameObject>();
            entrances = new List<GameObject>();
            exits = new List<GameObject>();
            int oldInstantiatedObjectsAmount = this.instantiatedDungeonObjects.transform.childCount;
            for(int i = 0; i < oldInstantiatedObjectsAmount; i++)
            {
                Destroy(this.instantiatedDungeonObjects.transform.GetChild(i).gameObject);
            }

        }
        ProceduralGenerationAlgorithms.LogState();


        specialRoomDeterminer = new SpecialRoomDeterminer();
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

        // GENERACJA STRUKTURY -----------------------------

        List<GraphConnection> dungeonStructure = GridAlgorithm.GenerateDungeonStructure(finalizedRooms);
        specialRoomDeterminer.bossRoomID = specialRoomDeterminer.DetermineBossRoom();
        this.bossRoomGenerator.SetUpBossRoom(finalizedRooms[specialRoomDeterminer.bossRoomID], this.instantiatedDungeonObjects);


        specialRoomDeterminer.startingRoomID = specialRoomDeterminer.DetermineStartingRoom();
        // order matters for specialRoomDeterminer
        // shopRoomID has to be determined at the end

        // GENERACJA STRUKTURY -----------------------------


        this.startingPosition = CalculateStartingPosition(finalizedRooms[specialRoomDeterminer.startingRoomID].FloorTiles);


        int amountOfPossibleTreasures = this.availableTreasures.transform.childCount;
        Dictionary<string, int> treasureContent = GenerateTreasureContent(amountOfPossibleTreasures);
        SetStateOfGlobalLoot(amountOfPossibleTreasures);

        for (int i = 0; i < treasureRoomsAmount; i++)
        {
            int newTreasureRoomID = specialRoomDeterminer.DetermineTreasureRoom(specialRoomDeterminer.startingRoomID, specialRoomDeterminer.bossRoomID,
                specialRoomDeterminer.treasureRoomIDs, finalizedRooms.Count);
           
            specialRoomDeterminer.treasureRoomIDs.Add(newTreasureRoomID);
            Dictionary<string, int> thisTreasureContent = GetSpecificTreasureContent(treasureContent, i);
            MarkRoom(finalizedRooms[newTreasureRoomID].FloorTiles, Color.yellow);
            GenerateTreasureRoomsPrefabs(finalizedRooms[newTreasureRoomID].FloorTiles, thisTreasureContent);
        }

        specialRoomDeterminer.shopRoomID = specialRoomDeterminer.DetermineShopRoom();


        finalizedRooms = CreateNonLinearPassagesBetweenRooms(finalizedRooms, GridAlgorithm.gg);

        finalizedRooms[specialRoomDeterminer.shopRoomID] =
            shopRoomGenerator.GenerateShop(finalizedRooms[specialRoomDeterminer.shopRoomID], tilemapVisualizer,
            this.instantiatedDungeonObjects);

        MarkRoom(finalizedRooms[specialRoomDeterminer.startingRoomID].FloorTiles, Color.green);
        MarkRoom(finalizedRooms[specialRoomDeterminer.bossRoomID].FloorTiles, Color.red);
        MarkRoom(finalizedRooms[specialRoomDeterminer.shopRoomID].FloorTiles, Color.blue);

        GenerateEnemies();
        GenerateCoins();

        if (this.savedGameNeedsDeletionOfEntities)
        {
            ProgressHolder.SetProgressFromPreviousSave(this.loadedData);
            DeleteAllAlreadyDestroyedEntitiesFromSave();
            RepairRooms();
            this.savedGameNeedsDeletionOfEntities = false;
        }

    }

    private void RepairRooms()
    {
        foreach(Room room in finalizedRooms.Values)
        {
            List<GameObject> temp = new List<GameObject>();
            foreach(GameObject go in room.enemies)
            {
                if (!this.loadedData.slainEnemyIDs.Contains(go.GetComponent<Enemy>().GetEnemyID()))
                {
                    temp.Add(go);
                }
            }
            room.enemies = new List<GameObject>(temp);
        }
    }

    private void DeleteAllAlreadyDestroyedEntitiesFromSave()
    {
        List<int> enemyIDsToDelete = this.loadedData.slainEnemyIDs;
        List<int> coinIDsToDelete = this.loadedData.collectedCoinIDs;
        List<GameObject> finalEnemies = new List<GameObject>();
        List<GameObject> finalCoins = new List<GameObject>();

        foreach(GameObject enemyObject in this.enemies)
        {

            if (enemyIDsToDelete.Contains(enemyObject.GetComponent<Enemy>().GetEnemyID()))
            {
                Destroy(enemyObject);
            }
            else
            {
                finalEnemies.Add(enemyObject);
            }
        }
        this.enemies = finalEnemies;

        foreach(GameObject coinObject in this.coins)
        {

            if (coinIDsToDelete.Contains(coinObject.GetComponent<Coin>().GetCoinID()))
            {
                Destroy(coinObject);
            }
            else
            {
                finalCoins.Add(coinObject);
            }
        }
        this.coins = finalCoins;
    }

    private void SetStateOfGlobalLoot(int amountOfPossibleTreasures)
    {
        Debug.Log("SETTING STATE OF GLOBAL LOOT");
        List<int> list = new List<int>();
        for (int i = 0; i < amountOfPossibleTreasures; i++) { list.Add(i); }
        GameController.SetAvailableSpecificItemLoot(list);
        foreach(var x in GameController.GetAvailableSpecificItemLoot())
        {
            Debug.Log(x + "<-itemId");
        }
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

            if (specialRoomDeterminer.treasureRoomIDs.Contains(room.Id) || specialRoomDeterminer.bossRoomID == room.Id
                || specialRoomDeterminer.startingRoomID == room.Id || specialRoomDeterminer.shopRoomID == room.Id)
            {
                continue;
            }

            randNumOfEnemies = UnityEngine.Random.Range(1, maxEnemiesInRoom);
            /*HashSet<GameObject> enemiesInRoom = new HashSet<GameObject>(); //do zmiany, trzeba dopisaæ do listy pokoju*/

            for (int i = 0; i < randNumOfEnemies; i++)
            {
                Vector2Int randomField = room.FloorTiles.ElementAt(UnityEngine.Random.Range(0, room.FloorTiles.Count));

                int value = UnityEngine.Random.Range(0, 3);

                GameObject typeOfEnemy = DetermineTypeOfEnemy(value);

                GameObject enemy = Instantiate(typeOfEnemy, ((Vector3Int)randomField), Quaternion.identity);
                enemy.transform.SetParent(this.instantiatedDungeonObjects.transform, true);
                this.enemies.Add(enemy);
                room.enemies.Add(enemy);
            }

        }
    }

    private GameObject DetermineTypeOfEnemy(int value)
    {
        if(value < 2)
        {
            return this.enemyPrefab;
        }
        else
        {
            return this.rangeEnemyPrefab;
        }
    }

    private void GenerateCoins()
    {
        int randNumOfCoins;
        foreach (var room in finalizedRooms.Values)
        {

            randNumOfCoins = UnityEngine.Random.Range(1, maxEnemiesInRoom);
            for (int i = 0; i < randNumOfCoins; i++)
            {
                Vector2Int randomField = room.FloorTiles.ElementAt(UnityEngine.Random.Range(0, room.FloorTiles.Count));
                GameObject coin = Instantiate(this.coinsPrefab, ((Vector3Int)randomField), Quaternion.identity);
                coin.transform.SetParent(this.instantiatedDungeonObjects.transform, true);
                this.coins.Add(coin);
            }
        }
    }

    private Dictionary<int, Room> CreateNonLinearPassagesBetweenRooms(Dictionary<int, Room> finalizedRooms, GridAlgorithm.GridGraph gg)
    {

        foreach(GraphConnection gc in gg.connections)
        {
            Room parentRoom = finalizedRooms[gc.parentNode];
            Room childRoom = finalizedRooms[gc.childNode];
            Teleport parentRoomTeleport, childRoomTeleport;
            (parentRoomTeleport, childRoomTeleport) = TeleportOrientationHelper.DefineLocationOfTeleports(gc, gg, parentRoom, childRoom);
            finalizedRooms[gc.parentNode].teleports.Add(parentRoomTeleport);
            finalizedRooms[gc.childNode].teleports.Add(childRoomTeleport);
        }
        RearrangeMapWithTeleports(finalizedRooms);
        return finalizedRooms;
    }

    private void RearrangeMapWithTeleports(Dictionary<int, Room> finalizedRooms)
    {
        foreach(Room room in finalizedRooms.Values) 
        {
            foreach(Teleport t in room.teleports){
                Vector2Int tileToMark = t.teleportFrom;
                DeleteSingleTile(tileToMark);
                int rotationAngles = DetermineRotationOfTeleport(tileToMark, room.FloorTiles);
                GenerateTeleportPrefab(tileToMark, rotationAngles, t, room);
                CreateWallTilesAroundTeleport(tileToMark, room.FloorTiles, room.WallTiles);
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

    private void GenerateTreasureRoomsPrefabs(HashSet<Vector2Int> roomTiles, Dictionary<string, int> treasureContent)
    {
        Vector2Int position = RoomHelper.DetermineRoomCenter(roomTiles);
        GameObject treasure = Instantiate(this.treasurePrefab, (Vector3Int)position, Quaternion.identity);
        treasure.transform.SetParent(this.instantiatedDungeonObjects.transform, true);
        this.treasures.Add(treasure);
        treasure.GetComponent<Treasure>().SetContent(treasureContent);
    }

    private void GenerateTeleportPrefab(Vector2Int position, int rotationAngles, Teleport t, Room room)
    {
        Vector3 fixedPosition = new Vector3(position.x + 0.5f, position.y + 0.5f, 0);
        Quaternion rotation = Quaternion.Euler(0, 0, rotationAngles);
        GameObject teleportObject = Instantiate(this.teleportPrefab, fixedPosition, rotation);
        teleportObject.transform.SetParent(this.instantiatedDungeonObjects.transform, true);
        teleportObject.GetComponent<TeleportMonoBehaviour>().teleportInfo = t;
        room.teleportObjects.Add(teleportObject);
        /*this.exits.Add(exitObject);*/
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
        Vector2Int vector2Center = RoomHelper.DetermineRoomCenter(roomTiles);
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
    public void ResetGenerationAfterMainMenuReturn()
    {
        finalizedRooms = new Dictionary<int, Room>();
        if(this.instantiatedDungeonObjects == null)
        {
            return;
        }
        int oldInstantiatedObjectsAmount = this.instantiatedDungeonObjects.transform.childCount;
        for (int i = 0; i < oldInstantiatedObjectsAmount; i++)
        {
            Destroy(this.instantiatedDungeonObjects.transform.GetChild(i).gameObject);
        }
    }

    public SaveData GetLoadedDataFromSave()
    {
        return this.loadedData;
    }

    public List<GameObject> GetTreasures()
    {
        return this.treasures;
    }

}
