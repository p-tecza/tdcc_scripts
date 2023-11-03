using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    protected Slider healthSlider;
    [SerializeField]
    protected Image healthSliderFill;

    [SerializeField]
    private int ownedHpPotions;
    [SerializeField]
    private int ownedStars;

    public void SetUpCharacter()
    {
        this.playerObject.SetActive(true);
        this.startingRoomID = 0;
        this.roomInfo = FullDungeonGenerator.GetFinalizedRooms();
        this.enableTeleports = true;
        this.enableMovement = true;
        this.currentRoom = this.roomInfo[this.startingRoomID];
        this.playerCurrentHealth = this.playerMaxHealth;
        this.stopAllActions = false;
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

            //Temporary solution
            if (Input.GetKey(KeyCode.Return) && !this.enableTeleports)
            {
                this.DisableEnemiesInRoom();
                this.enableTeleports = true;
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
       
    }

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
            Invoke("EnableMovement", enterRoomIdleDelay);
            Invoke("EnableEnemiesInRoom", enterRoomIdleDelay);
            //-------------

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
            this.gameController.RemoveItemRelatedPriceText(shopObject);
            RemovePickedUpObject(shopObject);
            /*GameController.RemoveItemHint(shopObject);*/
        }
        else if(shopObjectName == "Star(Clone)")
        {
            if (this.moneyAmount < ShopRoom.costs["star"])
            {
                return;
            }
            SetMoneyAmountAndUpdateUI(this.moneyAmount - ShopRoom.costs["star"]);
            PickUpCollectable(shopObject);
            this.gameController.RemoveItemRelatedPriceText(shopObject);
            RemovePickedUpObject(shopObject);
            /*GameController.RemoveItemHint(shopObject);*/
        }
        else
        {
            if (this.moneyAmount < ShopRoom.costs["item"])
            {
                return;
            }
            SetMoneyAmountAndUpdateUI(this.moneyAmount - ShopRoom.costs["item"]);
            PickUpItem(shopObject);
            this.gameController.RemoveItemRelatedPriceText(shopObject);
            RemovePickedUpObject(shopObject);
            /*GameController.RemoveItemHint(shopObject);*/
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
            Destroy(collision.gameObject);
            PickUpCoin();
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
        ApplyItemStats(item);
        this.gameController.UpdateUIPlayerStats(GetStats());
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

    private void OnDrawGizmosSelected()
    {
        if (playerAttackPoint == null)
            return;
        Gizmos.DrawWireSphere(playerAttackPoint.position, this.playerAttackRange);
    }

}
