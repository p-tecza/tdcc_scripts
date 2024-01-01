using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameController : MonoBehaviour
{
    //repositories
    public ItemRepository itemRepository;
    public QuestRepository questRepository;
    public NpcRepository npcRepository;

    [SerializeField]
    private GameObject playerObject;
    [SerializeField]
    private FullDungeonGenerator dungeonGenerator;

    public GameOverScreen gameOverScreen;

    [SerializeField]
    private PlayerController playerController;

    [SerializeField]
    private QuestController questController;

    [SerializeField]
    private GameObjectGenerator gameObjectGenerator;

    public TMP_Text tmpCoinsAmount;
    public TMP_Text playerToughness;
    public TMP_Text playerAttackDamage;
    public TMP_Text playerAttackRange;
    public TMP_Text playerAttackSpeed;
    public TMP_Text playerMovementSpeed;
    public TMP_Text ownedHpPots;
    public TMP_Text ownedStars;

    public Canvas priceCanvas;
    public Canvas hintCanvas;
    public GameObject hintObject;
    public float additionalHintOffset = 0.5f;

    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    [Range(1f, 10f)]
    private float cameraZoom = 1f;
    [SerializeField]
    private bool hintsVisible = false;

    private static List<int> availableSpecificItemLoot = new List<int>();
    private int currentLvl = 0;

    public EnemiesTracker enemiesTracker;
    [SerializeField]
    private GameObject parentQuestItemsObject;
    [SerializeField]
    private BossRoomGenerator bossRoomGenerator;
    [SerializeField]
    private ShopRoomGenerator shopRoomGenerator;

    public static bool gameFromSave = false;

    /*    private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }*/

    // Start is called before the first frame update
    void Start()
    {
        currentLvl++;
        if (currentLvl == 1)
        {
            this.InitializeRepositories();
        }
        GenerationEntityIDController.ResetAllIDs();
        ProgressHolder.ResetProgressIDs();
        dungeonGenerator.GenerateDungeon();
        Vector3 startPosition = dungeonGenerator.GetStartingPosition();
        playerObject.transform.position = startPosition;
        mainCamera.transform.position = startPosition;
        playerController.SetUpCharacter();
        playerController.enabled = true;
        this.enemiesTracker = new EnemiesTracker();
        InitEnemiesTracker();
        if (gameFromSave)
        {
            OverwriteNeccessaryData();
        }
        PlayerStats ps = playerObject.GetComponent<PlayerController>().GetStats();
        UpdateUIPlayerStats(ps);
    }

    private void OverwriteNeccessaryData()
    {
        SaveData saveData = dungeonGenerator.GetLoadedDataFromSave();
        playerController.stats = saveData.playerStats;
        playerController.SetAdditionalPlayerData(saveData.additionalPlayerData);
        UpdateUICoinsAmount(saveData.additionalPlayerData.coinsAmount);
        UpdateUICollectables(saveData.additionalPlayerData.collectedHpPotions, saveData.additionalPlayerData.collectedStars);
        playerController.SetEnemiesStateData(saveData.enemiesStateData);
        playerController.DetermineIfRoomTeleportsShallBeOpen();
        FixAlreadyLootedTreasures(saveData.treasureStateData);
        FixAlreadyPickedUpItemsFromShopRoom(saveData.remainingShopItemIds);
        SetOwnedQuestItems(saveData.heldQuestItems);
        ApplyQuestStateFromSave(saveData.questStateData);
        DropItemsLayingOnTheGround(saveData.droppedQuestItemData);
        gameFromSave = false;
    }

    private void SetOwnedQuestItems(List<string> heldQuestItems)
    {
        this.playerController.SetOwnedQuestItemsFromSave(heldQuestItems);
    }

    private void DropItemsLayingOnTheGround(List<DroppedQuestItemStateData> droppedQuestItemData)
    {
        foreach(DroppedQuestItemStateData item in droppedQuestItemData)
        {
            GameObject instantiatedObj = this.gameObjectGenerator.GenerateEntity(item.itemName);
            QuestItem questItem = instantiatedObj.GetComponent<QuestItem>();
            if (questItem != null)
            {
                questItem.idOfRoom = item.roomId;
                instantiatedObj.transform.position = new Vector3
                (
                    item.location[0],
                    item.location[1],
                    item.location[2]
                );
                List<Room> rooms = FullDungeonGenerator.GetFinalizedRooms().Values.ToList();
                if (rooms.Count > item.roomId)
                {
                    Room consideredRoom = FullDungeonGenerator.GetFinalizedRooms()[item.roomId];
                    consideredRoom.droppedItemsInRoom.Add(instantiatedObj);
                }
            }
        }
    }

    private void FixAlreadyPickedUpItemsFromShopRoom(List<int> pickedUpShopItemIds)
    {
        List<GameObject> itemFromShop = this.shopRoomGenerator.itemsInShopRoom;
        List<GameObject> toRemove = new List<GameObject>();

        foreach (GameObject go in itemFromShop)
        {
            if (go != null)
            {
                int itemId = DetermineIdOfPickUpAbleObject(go);
                if (!pickedUpShopItemIds.Contains(itemId))
                {
                    toRemove.Add(go);
                }
            }
        }

        foreach (GameObject go in toRemove)
        {
            this.shopRoomGenerator.RemoveShopItemFromState(go);
            Destroy(go);
        }
    }

    public void CreateNextDungeonLevel()
    {
        GenerationEntityIDController.ResetAllIDs();
        ProgressHolder.ResetProgressIDs();
        currentLvl++;
        dungeonGenerator.GenerateDungeonNextLevel(currentLvl);
        Vector3 startPosition = dungeonGenerator.GetStartingPosition();
        playerObject.transform.position = startPosition;
        mainCamera.transform.position = startPosition;
        this.ResetQuestsOnNextDungeonLevel();
        this.enemiesTracker = new EnemiesTracker();
        InitEnemiesTracker();
        playerController.SetUpCharacterForNextDungeonLevel();
    }

    // Update is called once per frame
    void Update()
    {
        mainCamera.transform.position =
            new Vector3(playerObject.transform.position.x, playerObject.transform.position.y, -cameraZoom);
    }

    public void UpdateUICoinsAmount(int newCoinsAmount)
    {
        this.tmpCoinsAmount.text = newCoinsAmount.ToString();
    }

    public void GameOver()
    {
        this.gameOverScreen.GameOverScreenPopUp();
    }

    public void UpdateUIPlayerStats(PlayerStats ps)
    {
        this.playerToughness.text = ps.toughness.ToString();
        this.playerAttackDamage.text = ps.attackDamage.ToString();
        this.playerAttackRange.text = ps.attackRange.ToString();
        this.playerAttackSpeed.text = ps.attackSpeed.ToString();
        this.playerMovementSpeed.text = ps.movementSpeed.ToString();
    }

    public void UpdateUICollectables(int hpPotsAmount, int starsAmount)
    {
        this.ownedHpPots.text = hpPotsAmount.ToString();
        this.ownedStars.text = starsAmount.ToString();
    }

    public static void RemoveItemFromAvailableSpecificItemLoot(int itemId)
    {
        availableSpecificItemLoot.Remove(itemId);
    }
    public static void SetAvailableSpecificItemLoot(List<int> availableLoot)
    {
        availableSpecificItemLoot = availableLoot;
    }

    public static List<int> GetAvailableSpecificItemLoot()
    {
        return availableSpecificItemLoot;
    }

    public void AddNewHint(Vector3 itemPosition, GameObject gameObject)
    {
        Vector3 finalHintPosition = new Vector3(itemPosition.x, itemPosition.y, itemPosition.z);
        finalHintPosition.y = itemPosition.y + gameObject.GetComponent<BoxCollider2D>().size.y / 2 + this.additionalHintOffset;
        GameObject newCanvasObject = Instantiate(this.hintCanvas.gameObject, finalHintPosition, Quaternion.identity);
        TMP_Text hintText = newCanvasObject.transform.Find("HintObject").Find("HintText").GetComponent<TMP_Text>();
        hintText.text = ItemHelper.GetNameOfItem(gameObject.name);


        newCanvasObject.transform.SetParent(gameObject.transform, true);
    }


    public void ToggleHints()
    {
        this.hintsVisible = !this.hintsVisible;
        GameObject[] hintObjects = GameObject.FindGameObjectsWithTag("Hint");
        for (int i = 0; i < hintObjects.Length; i++)
        {
            GameObject currentHintCanvas = hintObjects[i];
            int hintCanvChildCount = currentHintCanvas.transform.childCount;
            for (int j = 0; j < hintCanvChildCount; j++)
            {
                currentHintCanvas.transform.GetChild(j).gameObject.SetActive(this.hintsVisible);
            }
        }
    }

    public void PickUpQuest()
    {
        this.questController.PickUpQuest();
    }

    public void SetQuestData(QuestData questData)
    {
        this.questController.SetQuestData(questData);
    }

    public void TryToInteractWithNearestEntity()
    {
        GameObject objectToInteractWith = this.playerController.FindNearestInteractiveEntity();
        if (objectToInteractWith == null) return;
        InteractiveDialogData data = objectToInteractWith.GetComponent<NPC>().GetNPCData();

        gameObject.GetComponent<NPCInteractionController>().EnableInteractionWindow(data, objectToInteractWith);

        Debug.Log("NEAREST interactive ENTITY: " + objectToInteractWith.name);


    }

    public bool AreHintsVisible()
    {
        return this.hintsVisible;
    }

    public AllItemsData GetAllItemsData()
    {
        return new AllItemsData(this.itemRepository.GetAllCollectables(), this.itemRepository.GetAllItems(), this.itemRepository.GetAllQuestItems());
    }

    private void InitializeRepositories()
    {
        this.questRepository = new QuestRepository(DataReader.ReadAllQuestsData().allQuestsData);
        this.npcRepository = new NpcRepository(DataReader.ReadAllNPCDialogData().allNPCData);
        AllItemsData allItemsData = DataReader.ReadAllItemsData();
        this.itemRepository = new ItemRepository(allItemsData.items, allItemsData.collectables, allItemsData.questItems);
    }

    private void InitEnemiesTracker()
    {
        int enemyCounter = 0;
        foreach (Room r in FullDungeonGenerator.GetFinalizedRooms().Values)
        {
            enemyCounter += r.enemies.Count;
        }
        this.enemiesTracker.SetStartEnemiesAmount(enemyCounter);
        this.enemiesTracker.SetAliveEnemiesAmount(enemyCounter);
        this.enemiesTracker.SetDeadEnemiesAmount(0);
    }

    public int GetAmountOfAllEnemiesInDungeonFloor()
    {
        return this.enemiesTracker.GetStartEnemiesAmount();
    }

    public int GetAmountOfDeadEnemiesInDungeonFloor()
    {
        return this.enemiesTracker.GetDeadEnemiesAmount();
    }

    public List<string> GetListOfPlayerOwnedQuestItems()
    {
        return this.playerController.GetOwnedQuestItems();
    }

    public void UpdateQuestProgress()
    {
        Debug.Log("UPDATING QUEST PROGRESS!");
        this.questController.UpdateQuestProgress();
    }

    public void InsertItemToRandomEnemyInRandomRoom(string itemName)
    {
        Debug.Log("INSERTING TO RANDOM ENEMY");
        if(CheckIfQuestItemIsAlreadyInGame(itemName))
        {
            Debug.Log("IS ALREADY IN GAME!");
            return;
        }
        QuestItemData questItemData = this.itemRepository.GetQuestItemByName(itemName);
        GameObject questItemObject = this.parentQuestItemsObject.transform.Find(questItemData.name).gameObject;
        Debug.Log("QUEST ITEM OBJECT: " + questItemObject.name);
        Dictionary<int, Room> rooms = FullDungeonGenerator.GetFinalizedRooms();
        int pickedRoom = UnityEngine.Random.Range(0, rooms.Count);

        if (rooms[pickedRoom].enemies.Count > 0)
        {
            rooms[pickedRoom].enemies[0].GetComponent<Enemy>().SetHeldItem(questItemObject);
            Debug.Log("PRZYPISANIE ITEMA DO ENEMY: " + rooms[pickedRoom].enemies[0].GetComponent<Enemy>().GetEnemyID());
        }
        else
        {
            foreach (Room room in rooms.Values)
            {
                if (room.enemies.Count > 0)
                {
                    room.enemies[0].GetComponent<Enemy>().SetHeldItem(questItemObject);
                    Debug.Log("PRZYPISANIE ITEMA DO ENEMY: " + room.enemies[0].GetComponent<Enemy>().GetEnemyID());
                    break;
                }
            }
        }

    }

    private bool CheckIfQuestItemIsAlreadyInGame(string itemName)
    {
        List<string> playerQuestItems = this.playerController.GetOwnedQuestItems();
        if (playerQuestItems.Contains(itemName))
        {
            Debug.Log("ITEM IS ALREADY IN GAME!");
            return true;
        }
        
        foreach(Room room in FullDungeonGenerator.GetFinalizedRooms().Values.ToList())
        {
            foreach(GameObject objectInRoom in room.droppedItemsInRoom)
            {
                QuestItem questItem = objectInRoom.GetComponent<QuestItem>();
                if(questItem != null && questItem.questItemName == itemName)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void TryToOpenTeleports(GameObject enemyObject)
    {
        this.playerController.CheckIfRoomIsCleared(enemyObject);
    }

    public void ResolveQuestReward(QuestReward reward)
    {
        this.playerController.AddQuestReward(reward);
    }

    public void ResetQuestsOnNextDungeonLevel()
    {
        this.questController.ResetQuestStateOnNextDungeonLevel();
    }

    public void ShowNextLevelTeleport()
    {
        this.bossRoomGenerator.OpenNextLevelTeleport();
    }

    public void ResetAfterMainMenuReturn()
    {
        this.currentLvl = 1;
        this.dungeonGenerator.ResetGenerationAfterMainMenuReturn();
    }

    private void FixAlreadyLootedTreasures(TreasureStateData treasureStateData)
    {
        List<int> treasuresIdsFromSave = treasureStateData.treasuresIds;
        List<bool> treasuresStatesFromSave = treasureStateData.treasureOpenStates;

        // tutaj powinna byc taka sekwencja jak zdefiniowane w treasureStateData w liscie z ID treasures

        List<GameObject> treasuresObjectsInCorrectOrder = GetProperOrderTreasureObjectReference();

        foreach (GameObject go in treasuresObjectsInCorrectOrder)
        {
            Treasure treasure = go.GetComponent<Treasure>();
            int currentTreasureIndex = treasuresIdsFromSave.IndexOf(treasure.treasureID);

            if (currentTreasureIndex != -1 && treasuresStatesFromSave.Count > currentTreasureIndex)
            {
                treasure.isOpened = treasuresStatesFromSave[currentTreasureIndex];
                if (treasure.isOpened)
                {
                    treasure.SetAlreadyLootedTreasureFromSave();
                    treasure.DropItems(go.transform, true);
                    int pickUpId = -1;
                    int droppedItemIt = 0;
                    foreach (GameObject droppedItem in treasure.droppedItems)
                    {

                        pickUpId = DetermineIdOfPickUpAbleObject(droppedItem);
                        if (pickUpId != -1 && treasureStateData.droppedItemsIds.Count > currentTreasureIndex)
                        {
                            if (!treasureStateData.droppedItemsIds[currentTreasureIndex].Contains(pickUpId))
                            {
                                Destroy(droppedItem);
                            }
                            else
                            {
                                List<float> droppedItemsCords =
                                    treasureStateData.droppedItemsLocations[currentTreasureIndex][droppedItemIt];
                                droppedItem.transform.position = new Vector3(
                                        droppedItemsCords[0],
                                        droppedItemsCords[1],
                                        droppedItemsCords[2]
                                    );
                                droppedItemIt++;
                            }
                        }
                    }
                }
            }
        }
    }

    private List<GameObject> GetProperOrderTreasureObjectReference()
    {
        List<GameObject> finalOrder = new List<GameObject>();
        List<int> properSequence = ProgressHolder.openedTreasuresSequence;
        List<GameObject> treasureObjects = dungeonGenerator.GetTreasures();
        foreach (int i in properSequence)
        {
            foreach (GameObject obj in treasureObjects)
            {
                if (obj.GetComponent<Treasure>().treasureID == i)
                {
                    finalOrder.Add(obj);
                }
            }
        }

        foreach (GameObject obj in treasureObjects)
        {
            Treasure t = obj.GetComponent<Treasure>();
            if (!properSequence.Contains(t.treasureID))
            {
                finalOrder.Add(obj);
            }
        }

        return finalOrder;
    }

    public TreasureStateData GetTreasureStateData()
    {
        List<GameObject> treasureObjects = dungeonGenerator.GetTreasures();
        List<int> treasuresIDs = new List<int>();
        List<bool> whichTreasuresAreOpen = new List<bool>();
        List<List<int>> droppedItemsIds = new List<List<int>>();
        List<List<List<float>>> cordsOfAllDroppedItems = new List<List<List<float>>>();

        foreach (GameObject go in treasureObjects)
        {
            Treasure treasure = go.GetComponent<Treasure>();
            treasuresIDs.Add(treasure.treasureID);

            List<List<float>> cordsOfThisTreasureDroppedItems = new List<List<float>>();

            whichTreasuresAreOpen.Add(treasure.isOpened);

            List<GameObject> droppedItems = treasure.droppedItems;
            List<int> idsOfDroppedItemsOfThisTreasure = new List<int>();
            foreach (GameObject droppedItem in droppedItems)
            {
                int pickUpId = DetermineIdOfPickUpAbleObject(droppedItem);

                if (pickUpId != -1)
                {
                    Debug.Log("ADDING ID: " + pickUpId);

                    idsOfDroppedItemsOfThisTreasure.Add(pickUpId);
                    cordsOfThisTreasureDroppedItems.Add(new List<float>
                    {
                        droppedItem.transform.position.x,
                        droppedItem.transform.position.y,
                        droppedItem.transform.position.z
                    });
                }
            }
            droppedItemsIds.Add(idsOfDroppedItemsOfThisTreasure);
            cordsOfAllDroppedItems.Add(cordsOfThisTreasureDroppedItems);
        }
        return new TreasureStateData(
                treasuresIDs,
                whichTreasuresAreOpen,
                droppedItemsIds,
                cordsOfAllDroppedItems
            );
    }

    public void RemoveShopItemFromStateOnPickUp(GameObject pickedUpShopObject)
    {
        this.shopRoomGenerator.RemoveShopItemFromState(pickedUpShopObject);
    }

    internal List<int> GetRemainingShopItemIds()
    {
        List<int> ids = new List<int>();

        foreach (GameObject go in this.shopRoomGenerator.itemsInShopRoom)
        {
            ids.Add(DetermineIdOfPickUpAbleObject(go));
        }
        return ids;
    }


    private int DetermineIdOfPickUpAbleObject(GameObject go)
    {
        int pickUpId = -1;
        if (go.GetComponent<Coin>() != null)
        {
            Coin coin = go.GetComponent<Coin>();
            pickUpId = coin.pickUpEntityID;
        }
        else if (go.GetComponent<Collectable>() != null)
        {
            Collectable collectable = go.GetComponent<Collectable>();
            pickUpId = collectable.pickUpEntityID;
        }
        else if (go.GetComponent<Item>() != null)
        {
            Item item = go.GetComponent<Item>();
            pickUpId = item.pickUpEntityID;
        }
        return pickUpId;
    }

    public QuestStateData GetFullActiveQuestStateData()
    {
        return this.questController.GetFullQuestStateData();
    }

    public void ApplyQuestStateFromSave(QuestStateData data)
    {

        this.questController.SetQuestStateFromSave(data);
        this.questController.UpdateQuestProgressFromSaveProgress(data.currentQuestProgress);
    }

    public void RemoveReferenceOfDroppedItemFromRoomByID(int roomId, GameObject destroyedObject)
    {
        if (FullDungeonGenerator.GetFinalizedRooms().Count > roomId)
        {
            Room room = FullDungeonGenerator.GetFinalizedRooms()[roomId];
            foreach (GameObject obj in room.droppedItemsInRoom)
            {

                Debug.Log("Trying to destroy obj");

                if (obj == destroyedObject)
                {
                    Debug.Log("DESTROYING OBJ");
                    room.droppedItemsInRoom.Remove(obj);
                    break;
                }
            }
        }
    }

    public List<DroppedQuestItemStateData> GetAllDroppedQuestItemStateData()
    {
        List<DroppedQuestItemStateData> data = new List<DroppedQuestItemStateData>();
        List<Room> rooms = FullDungeonGenerator.GetFinalizedRooms().Values.ToList();

        foreach (Room room in rooms)
        {
            foreach (GameObject obj in room.droppedItemsInRoom)
            {
                QuestItem questItem = obj.GetComponent<QuestItem>();
                if (questItem != null)
                {
                    data.Add(new DroppedQuestItemStateData(
                        questItem.questItemName,
                        room.Id,
                        new List<float>
                        {
                            obj.transform.position.x,
                            obj.transform.position.y,
                            obj.transform.position.z
                        }
                        ));
                }
            }
        }
        return data;
    }

    public int GetBossRoomID()
    {
        return this.dungeonGenerator.GetBossRoomID();
    }

    public int GetShopRoomID()
    {
        return this.dungeonGenerator.GetShopRoomID();
    }
}
