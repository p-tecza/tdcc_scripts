using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public GameObject playerObject;
    public GameController gameController;
    public Animator animator;

/*    [SerializeField]
    [Range(0.1f, 2f)]
    private float moveSpeed = 0.1f;*/

    [SerializeField]
    [Range(0.1f, 3f)]
    private float enterRoomIdleDelay = 0.4f;

    [SerializeField]
    private int onCollisionDamage = 10;

    public int moneyAmount;

    private Dictionary<int, Room> roomInfo;
    private int startingRoomID;
    private Room currentRoom;

    private bool enableTeleports;
    private bool enableMovement;

    private bool isRunning;
    private bool isAttacking;

    private bool stopAllActions;


    // STATS

    public PlayerStats stats;

    [SerializeField]
    private int playerMaxHealth;
    private int playerCurrentHealth;
    [SerializeField]
    private int playerToughness;
    [SerializeField]
    private int playerAttackDamage;
    [SerializeField]
    private float playerAttackSpeed;
    [SerializeField]
    private float playerAttackRange;
    [SerializeField]
    private float playerMovementSpeed;
    [SerializeField]
    private AttackType playerAttackType;
    [SerializeField]
    private Transform playerAttackPoint;
    [SerializeField]
    private LayerMask enemyLayer;
    [SerializeField]
    private LayerMask interactiveLayer;

    [SerializeField]
    protected Slider healthSlider;
    [SerializeField]
    protected Image healthSliderFill;

    [SerializeField]
    private int ownedHpPotions;
    [SerializeField]
    private int ownedStars;

    private List<Item> ownedItems;
    private List<QuestItem> ownedQuestItems;

    [SerializeField]
    public float playerInteractionRange;

    private bool isQuestActive;
    private int questItemsLayer;

    private bool enabledNextLevelTeleport;
    public void SetUpCharacter()
    {
        this.playerObject.SetActive(true);
        this.ownedItems = new List<Item>();
        this.ownedQuestItems = new List<QuestItem>();
        this.questItemsLayer = LayerMask.NameToLayer("QuestItems");
        this.startingRoomID = 0;
        this.isQuestActive = false;
        this.roomInfo = FullDungeonGenerator.GetFinalizedRooms();
        this.enableTeleports = true;
        this.enableMovement = true;
        this.currentRoom = this.roomInfo[this.startingRoomID];
        this.playerCurrentHealth = this.playerMaxHealth;
        this.stopAllActions = false;
        this.enabledNextLevelTeleport = false;
        this.gameController.UpdateUICollectables(this.ownedHpPotions, this.ownedStars);

        //TEMP
        /*PrimsAlgorithm.GenerateDungeonStructure(FullDungeonGenerator.GetFinalizedRooms());*/
        GridAlgorithm.GenerateDungeonStructure(FullDungeonGenerator.GetFinalizedRooms());

        this.stats = new PlayerStats(
            this.playerMaxHealth,
            this.playerToughness,
            this.playerAttackDamage,
            this.playerAttackSpeed,
            this.playerAttackRange,
            this.playerMovementSpeed,
            this.playerAttackType
            );
            
    }

    public void SetUpCharacterForNextDungeonLevel()
    {
        this.roomInfo = FullDungeonGenerator.GetFinalizedRooms();
        this.enableTeleports = true;
        this.enableMovement = true;
        this.stopAllActions = false;
        this.currentRoom = this.roomInfo[this.startingRoomID];
        this.ownedQuestItems = new List<QuestItem>();
        this.enabledNextLevelTeleport = false;
    }

    void FixedUpdate()
    {
        if (this.enableMovement)
        {

            if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            {
                this.isRunning = true;
            }
            else
            {
                this.isRunning = false;
            }
            
            this.animator.SetBool("isRunning", isRunning);

            if (Input.GetKey(KeyCode.W) && !isAttacking && !this.stopAllActions)
            {
                this.playerObject.transform.position = this.playerObject.transform.position += new Vector3(0, this.stats.movementSpeed, 0);
            }

            if (Input.GetKey(KeyCode.S) && !isAttacking && !this.stopAllActions)
            {
                this.playerObject.transform.position = this.playerObject.transform.position += new Vector3(0, -this.stats.movementSpeed, 0);
            }

            if (Input.GetKey(KeyCode.A) && !isAttacking && !this.stopAllActions)
            {
                this.playerObject.transform.position = this.playerObject.transform.position += new Vector3(-this.stats.movementSpeed, 0, 0);
                this.playerObject.transform.rotation = new Quaternion(this.playerObject.transform.rotation.x,
                            0f, this.playerObject.transform.rotation.z, this.playerObject.transform.rotation.w);
            }

            if (Input.GetKey(KeyCode.D) && !isAttacking && !this.stopAllActions)
            {
                this.playerObject.transform.position = this.playerObject.transform.position += new Vector3(this.stats.movementSpeed, 0, 0);
                this.playerObject.transform.rotation = new Quaternion(this.playerObject.transform.rotation.x,
                            180f, this.playerObject.transform.rotation.z, this.playerObject.transform.rotation.w);

            }     

        }

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !this.stopAllActions)
        {
            this.animator.SetTrigger("attack");
            this.isAttacking = true;
        }

        // Admin 
        if (Input.GetKeyDown(KeyCode.Return))
        {
            this.SmiteAllEnemies_ADMIN();
        }

    }

/*    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "NextLevelTeleport")
        {
            Debug.Log("SCHODZIMY NIZEJ");
            this.enabledNextLevelTeleport = ;
        }
    }*/

    void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.tag == "Teleport" && enableTeleports)
        {

            Debug.Log("KOLIZJA Z TPKIEM!!!");

            this.isRunning = false;
            this.animator.SetBool("isRunning", isRunning);

            //-------------
            Teleport teleportObject = collision.gameObject.GetComponent<TeleportMonoBehaviour>().teleportInfo;
            this.enableTeleports = false;
            this.enableMovement = false;
            this.playerObject.transform.position = new Vector3(teleportObject.teleportTo.x, teleportObject.teleportTo.y, 0);
            this.currentRoom = this.roomInfo[teleportObject.teleportToRoomId];
            SetProperTeleportSprites();
            Invoke("EnableTeleportsOnEmptyRoom", enterRoomIdleDelay);
            Invoke("EnableMovement", enterRoomIdleDelay);
            Invoke("EnableEnemiesInRoom", enterRoomIdleDelay);
            //-------------

        }

        if(collision.gameObject.tag == "NextLevelTeleport" && enableTeleports)
        {
            Debug.Log("SCHODZIMY NIZEJ");
            if (this.enabledNextLevelTeleport)
            {
                this.gameController.CreateNextDungeonLevel();
            }
        }

        if(collision.gameObject.tag == "Enemy")
        {
            TakeDamage(this.onCollisionDamage);
        }

        if(collision.gameObject.tag == "TreasureChest")
        {
            collision.gameObject.GetComponent<Treasure>().DropItems(this.transform);
        }

        if (collision.gameObject.tag == "Coin")
        {
            ProgressHolder.collectedCoinIDs.Add(collision.gameObject.GetComponent<Coin>().GetCoinID());
            Destroy(collision.gameObject);
            PickUpCoin();
            ResetMyVelocity();
        }

        if (collision.gameObject.tag == "Item")
        {
            PickUpItem(collision.gameObject);
            Destroy(collision.gameObject);
            ResetMyVelocity();
        }

        if(collision.gameObject.tag == "Collectable")
        {
            PickUpCollectable(collision.gameObject);
            Destroy(collision.gameObject);
            ResetMyVelocity();
        }

        if(collision.gameObject.tag == "ShopItem")
        {
            TryToPickUpShopItem(collision.gameObject);
        }

    }

    private void SetProperTeleportSprites()
    {
        if (this.enableTeleports)
        {
            foreach(GameObject teleportObj in this.currentRoom.teleportObjects)
            {
                teleportObj.GetComponent<TeleportMonoBehaviour>().SetOpenImage();
            }
        }
        else
        {
            foreach (GameObject teleportObj in this.currentRoom.teleportObjects)
            {
                teleportObj.GetComponent<TeleportMonoBehaviour>().SetClosedImage();
            }
        }
    }


    private void TryToPickUpShopItem(GameObject shopObject)
    {
        string shopObjectName = shopObject.name;
        if(shopObjectName == "HealthPotion(Clone)")
        {
            if(this.moneyAmount < ShopRoom.costs["hpPotion"])
            {
                return;
            }
            SetMoneyAmountAndUpdateUI(this.moneyAmount - ShopRoom.costs["hpPotion"]);
            PickUpCollectable(shopObject);
            RemovePickedUpObject(shopObject);
        }
        else if(shopObjectName == "Star(Clone)")
        {
            if (this.moneyAmount < ShopRoom.costs["star"])
            {
                return;
            }
            SetMoneyAmountAndUpdateUI(this.moneyAmount - ShopRoom.costs["star"]);
            PickUpCollectable(shopObject);
            RemovePickedUpObject(shopObject);
        }
        else
        {
            if (this.moneyAmount < ShopRoom.costs["item"])
            {
                return;
            }
            SetMoneyAmountAndUpdateUI(this.moneyAmount - ShopRoom.costs["item"]);
            PickUpItem(shopObject);
            RemovePickedUpObject(shopObject);
        }
    }

    private void SetMoneyAmountAndUpdateUI(int newMoneyAmount)
    {
        this.moneyAmount = newMoneyAmount;
        this.gameController.UpdateUICoinsAmount(newMoneyAmount);
    }

    private void RemovePickedUpObject(GameObject pickedUpObject)
    {
        Destroy(pickedUpObject);
        ResetMyVelocity();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Coin")
        {
            ProgressHolder.collectedCoinIDs.Add(collision.gameObject.GetComponent<Coin>().GetCoinID());
            Destroy(collision.gameObject);
            PickUpCoin();
        }

        if(collision.gameObject.layer == questItemsLayer)
        {
            QuestItem questItem = collision.gameObject.GetComponent<QuestItem>();

            if (questItem.CanPickUp())
            {
                PickUpQuestItem(collision.gameObject);
                Destroy(collision.gameObject);
            }
        }

    }

    private void PickUpCollectable(GameObject collectableObject)
    {
        string collectableName = collectableObject.name;
        if(collectableName == "HealthPotion(Clone)")
        {
            this.ownedHpPotions++;
        }
        else if(collectableName == "Star(Clone)")
        {
            this.ownedStars++;
        }

        this.gameController.UpdateUICollectables(this.ownedHpPotions, this.ownedStars);
    }
    private void PickUpItem(GameObject itemObject)
    {
        Item item = itemObject.GetComponent<Item>();
        this.ownedItems.Add(item);
        ApplyItemStats(item);
        this.gameController.UpdateUIPlayerStats(GetStats());
    }
    private void PickUpQuestItem(GameObject itemObject)
    {
        Debug.Log("PICK UP QUEST ITEM PROCED");
        QuestItem item = itemObject.GetComponent<QuestItem>();
        this.ownedQuestItems.Add(item);
        this.gameController.UpdateQuestProgress();
    }

    private void ApplyItemStats(Item item)
    {
        //TODO do ogarniêcia wartoœci graniczne (0 i jakis cap gorny)
        this.stats.attackSpeed += item.attackSpeed;
        this.stats.attackDamage += item.attackDamage;
        this.stats.attackRange += item.attackRange;
        this.stats.toughness += item.toughness;
        this.stats.movementSpeed += item.movementSpeed;
    }

    public void CheckIfRoomIsCleared(GameObject enemyObject)
    {


        if (this.currentRoom.enemies.Contains(enemyObject))
        {
            this.currentRoom.enemies.Remove(enemyObject);
        }

        Debug.Log("CHEK IF ROOM CLEARED");
        Debug.Log("CNT: " + this.currentRoom.enemies.Count);
        if(this.currentRoom.enemies.Count <= 0) {
            this.enableTeleports = true;
            SetProperTeleportSprites();
        }
    }

    void AttackAnimationEnd()
    {
        this.isAttacking = false;
    }

    void DealDamage()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(playerAttackPoint.position, this.stats.attackRange, enemyLayer);

        foreach(Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamage(this.stats.attackDamage);
        }

    }

    void GameOver()
    {
        this.gameController.GameOver();
    }
    private void EnableTeleportsOnEmptyRoom()
    {
        if(this.currentRoom.enemies.Count <= 0)
        {
            this.enableTeleports = true;
            SetProperTeleportSprites();
        }
    }

    public void PickUpCoin()
    {
        this.moneyAmount++;
        gameController.UpdateUICoinsAmount(this.moneyAmount);
    }
    public void TakeDamage(int dmg)
    {

        if (this.stopAllActions) return;

        this.playerCurrentHealth -= dmg;
        ResetMyVelocity();

        this.animator.SetBool("isRunning", false);
        this.animator.SetTrigger("hurt");

        isAttacking = false;

        if (this.playerCurrentHealth <= 0)
        {
            this.healthSlider.value = 0;
            this.playerCurrentHealth = 0;

            this.animator.SetTrigger("dead");
            this.stopAllActions = true;
        }
        else if ((float)this.playerCurrentHealth / this.playerMaxHealth < 0.3) this.healthSliderFill.color = UnityEngine.Color.red;
        else if ((float)this.playerCurrentHealth / this.playerMaxHealth < 0.6) this.healthSliderFill.color = UnityEngine.Color.yellow;

        this.healthSlider.value = (float)this.playerCurrentHealth / this.playerMaxHealth;
    }

    private void EnableMovement()
    {
        this.enableMovement = true;
    }

    private void EnableEnemiesInRoom()
    {
        foreach (GameObject e in this.currentRoom.enemies)
        {
            if (e != null)
            e.GetComponent<Enemy>().ActivateEnemy();
        }
    }

    private void DisableEnemiesInRoom()
    {
        foreach (GameObject e in this.currentRoom.enemies)
        {
            if(e!=null)
            e.GetComponent<Enemy>().DeactivateEnemy();
        }
    }

    private void ResetMyVelocity()
    {
        gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        gameObject.GetComponent<Rigidbody2D>().angularVelocity = 0;
    }

    public Room GetCurrentlyActivatedRoom()
    {
        return this.currentRoom;
    }

    public PlayerStats GetStats()
    {
        return this.stats;
    }

    public void UseHealthPotion()
    {
        if(this.ownedHpPotions > 0)
        {
            this.playerCurrentHealth += HpPotion.healthPower;
            SetSlider((float)this.playerCurrentHealth / this.playerMaxHealth);
            this.ownedHpPotions--;
            this.gameController.UpdateUICollectables(this.ownedHpPotions, this.ownedStars);
        }
    }

    public void ToggleHints()
    {
        this.gameController.ToggleHints();
    }

    private void SetSlider(float sliderValue)
    {
        this.healthSlider.value = sliderValue;
        if (sliderValue < 0.3) this.healthSliderFill.color = UnityEngine.Color.red;
        else if (sliderValue < 0.6) this.healthSliderFill.color = UnityEngine.Color.yellow;
        else this.healthSliderFill.color = new Color(0, 0.6f, 0);
    }

    public GameObject FindNearestInteractiveEntity()
    {
        Vector3 interactionRefPoint = transform.Find("InteractionRefPoint").position;
        Collider2D[] allNearbyInteractiveEntities = Physics2D.OverlapCircleAll(interactionRefPoint, this.playerInteractionRange, this.interactiveLayer);
        float minDist = float.MaxValue;
        GameObject nearestInteractiveEntity = null;
        foreach (Collider2D interactive in allNearbyInteractiveEntities)
        {
            float distanceBetween = Vector3.Distance(interactionRefPoint, interactive.transform.position);
            if(distanceBetween < minDist)
            {
                minDist = distanceBetween;
                nearestInteractiveEntity = interactive.gameObject;
            }
        }
        return nearestInteractiveEntity;
    }

    public void UpgradeStat(StatType statType, float statUpgradeValue, TrainerNPC trainerNPC)
    {
        int cost = trainerNPC.trainingCost;
        if(cost > this.moneyAmount)
        {
            return;
        }

        switch (statType)
        {
            case StatType.Toughness:
                this.stats.toughness += (int)statUpgradeValue;
                break;
            case StatType.AttackDamage:
                this.stats.attackDamage += (int)statUpgradeValue;
                break;
            case StatType.AttackRange:
                this.stats.attackRange += statUpgradeValue;
                break;
            case StatType.AttackSpeed:
                this.stats.attackSpeed += statUpgradeValue;
                break;
            case StatType.MovementSpeed:
                this.stats.movementSpeed += statUpgradeValue;
                break;
        }
        trainerNPC.trainingCost *= trainerNPC.costRaiseMultiplier;
        this.moneyAmount -= cost;
        this.gameController.UpdateUIPlayerStats(GetStats());
        this.gameController.UpdateUICoinsAmount(this.moneyAmount);
    }

    public void StartQuest(QuestData questData)
    {
        this.isQuestActive = true;
        this.gameController.PickUpQuest();
        this.gameController.SetQuestData(questData);
    }

    public void AddQuestReward(QuestReward reward)
    {
        string rewardType = reward.GetRewardType();
        if (rewardType == "Money")
        {
            this.moneyAmount += reward.GetMoneyAmount();
            this.gameController.UpdateUICoinsAmount(this.moneyAmount);
        }
        else if(rewardType == "Item")
        {
            // TODO Handling Items in equipment
        }

    }

    public List<QuestItem> GetOwnedQuestItems()
    {
        return this.ownedQuestItems;
    }

    private void OnDrawGizmosSelected()
    {
        if (playerAttackPoint == null)
            return;
        Gizmos.DrawWireSphere(playerAttackPoint.position, this.playerAttackRange);

        Transform playerInteractionPoint = transform.Find("InteractionRefPoint");

        if (playerInteractionPoint == null)
            return;

        Gizmos.DrawWireSphere(playerInteractionPoint.position, this.playerInteractionRange);
    }

    private void SmiteAllEnemies_ADMIN()
    {
        foreach(GameObject go in this.currentRoom.enemies)
        {
            Enemy x = go.GetComponent<Enemy>();
            x.TakeDamage(999999);
        }
    }

    public void EnableNextLevelTeleport()
    {
        this.enabledNextLevelTeleport = true;
    }

}
