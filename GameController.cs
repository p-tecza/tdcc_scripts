using System;
using System.Collections.Generic;
using TMPro;
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

    /*private AllItemsData allItemsData;*/

    [SerializeField]
    private PlayerController playerController;

    [SerializeField]
    private QuestController questController;

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

    // Start is called before the first frame update
    void Start()
    {

        if(currentLvl == 0)
        {
            this.InitializeRepositories();
        }

        /*this.allItemsData = DataReader.ReadAllItemsData();*/
        dungeonGenerator.GenerateDungeon();
        Vector3 startPosition = dungeonGenerator.GetStartingPosition();
        playerObject.transform.position = startPosition;
        mainCamera.transform.position = startPosition;
        playerController.SetUpCharacter();
        playerController.enabled = true;
        this.enemiesTracker = new EnemiesTracker();
        InitEnemiesTracker();
        PlayerStats ps = playerObject.GetComponent<PlayerController>().GetStats();
        UpdateUIPlayerStats(ps);
        currentLvl++;
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
        Debug.Log(itemId);
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

    public void RemoveItemRelatedPriceText(GameObject shopObject)
    {

        // name of shop item pricetext must be unique, add some sort of iterator for specific dung lvl

        string nameOfPriceText = "PriceText" + shopObject.name;
        GameObject canvasObject = this.priceCanvas.gameObject;
        int childrenCount = canvasObject.transform.childCount;
        for (int i = 0; i < childrenCount; i++)
        {
            GameObject childObject = canvasObject.transform.GetChild(i).gameObject;
            Debug.Log(childObject.name + "(Clone)");
            Debug.Log(nameOfPriceText);
            if (childObject.name + "(Clone)" == nameOfPriceText)
            {
                Destroy(childObject);
            }
        }
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
        return new AllItemsData(this.itemRepository.GetAllCollectables(),this.itemRepository.GetAllItems(), this.itemRepository.GetAllQuestItems());
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

    public List<QuestItem> GetListOfPlayerOwnedQuestItems()
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
        QuestItemData questItemData = this.itemRepository.GetQuestItemByName(itemName);
        GameObject questItemObject = this.parentQuestItemsObject.transform.Find(questItemData.name).gameObject;
        Debug.Log("QUEST ITEM OBJECT: " + questItemObject.name);
        Dictionary<int,Room> rooms = FullDungeonGenerator.GetFinalizedRooms();
        int pickedRoom = UnityEngine.Random.Range(0, rooms.Count);

        if (rooms[pickedRoom].enemies.Count > 0)
        {
            rooms[pickedRoom].enemies[0].GetComponent<Enemy>().SetHeldItem(questItemObject);
        }
        else
        {
            foreach(Room room in rooms.Values)
            {
                if(room.enemies.Count > 0)
                {
                    room.enemies[0].GetComponent<Enemy>().SetHeldItem(questItemObject);
                    break;
                }
            }
        }

    }

    public void ResolveQuestReward(QuestReward reward)
    {
        this.playerController.AddQuestReward(reward);
    }

/*    public AllInteractiveDialogData GetAllNPCDialogData()
    {
        Debug.Log("GETTING NPC DATA");

        return this.allNPCDialogData;
    }*/

}
