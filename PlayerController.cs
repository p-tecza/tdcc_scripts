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

    [SerializeField]
    [Range(0.1f, 2f)]
    private float moveSpeed = 0.1f;

    [SerializeField]
    [Range(0.1f, 3f)]
    private float enterRoomIdleDelay = 0.4f;

    public int moneyAmount;

    private Dictionary<int, Room> roomInfo;
    private int startingRoomID;
    private Room currentRoom;

    private bool enableTeleports;
    private bool enableMovement;

    private bool isRunning;
    private bool isAttacking;


    // STATS

    [SerializeField]
    private int playerMaxHealth;
    private int playerCurrentHealth;
    [SerializeField]
    private int playerAttackDamage;
    [SerializeField]
    private float playerAttackSpeed;
    [SerializeField]
    private float playerAttackRange;
    [SerializeField]
    private Transform playerAttackPoint;
    [SerializeField]
    private LayerMask enemyLayer;

    [SerializeField]
    protected Slider healthSlider;
    [SerializeField]
    protected Image healthSliderFill;


    public void SetUpCharacter()
    {
        this.playerObject.SetActive(true);
        this.startingRoomID = FullDungeonGenerator.GetStartingRoomID();
        this.roomInfo = FullDungeonGenerator.GetFinalizedRooms();
        this.enableTeleports = true;
        this.enableMovement = true;
        this.currentRoom = this.roomInfo[this.startingRoomID];
        this.playerCurrentHealth = this.playerMaxHealth;
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

            if (Input.GetKey(KeyCode.W) && !isAttacking)
            {
                this.playerObject.transform.position = this.playerObject.transform.position += new Vector3(0, moveSpeed, 0);
            }

            if (Input.GetKey(KeyCode.S) && !isAttacking)
            {
                this.playerObject.transform.position = this.playerObject.transform.position += new Vector3(0, -moveSpeed, 0);
            }

            if (Input.GetKey(KeyCode.A) && !isAttacking)
            {
                this.playerObject.transform.position = this.playerObject.transform.position += new Vector3(-moveSpeed, 0, 0);
                this.playerObject.transform.rotation = new Quaternion(this.playerObject.transform.rotation.x,
                            0f, this.playerObject.transform.rotation.z, this.playerObject.transform.rotation.w);
            }

            if (Input.GetKey(KeyCode.D) && !isAttacking)
            {
                this.playerObject.transform.position = this.playerObject.transform.position += new Vector3(moveSpeed, 0, 0);
                this.playerObject.transform.rotation = new Quaternion(this.playerObject.transform.rotation.x,
                            180f, this.playerObject.transform.rotation.z, this.playerObject.transform.rotation.w);

            }     

            //Temporary solution
            if (Input.GetKey(KeyCode.Return) && !this.enableTeleports)
            {
                Debug.Log("ENABLE TELEPORTS");
                this.DisableEnemiesInRoom();
                this.enableTeleports = true;
            }

        }

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.animator.SetTrigger("attack");
            this.isAttacking = true;
        }
       
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        /*if (collision.gameObject.name == "Walls")
        {
        }*/
        if (collision.gameObject.tag == "Teleport" && enableTeleports)
        {

            Debug.Log(this.currentRoom);
            Debug.Log(collision);
            this.isRunning = false;
            this.animator.SetBool("isRunning", isRunning);

            if (this.currentRoom.exit != null && Mathf.Abs(collision.transform.position.x - this.currentRoom.exit.teleportFrom.x) < 1f
                && collision.transform.position.y - this.currentRoom.exit.teleportFrom.y < 1f)
            {
                this.enableTeleports = false;
                this.enableMovement = false;
                this.playerObject.transform.position = new Vector3(this.currentRoom.exit.teleportTo.x,
                    this.currentRoom.exit.teleportTo.y, 0);

                Debug.Log("POZYCJA GRACZA PO TPKU: " + this.playerObject.transform.position);

                this.currentRoom = this.roomInfo[this.currentRoom.exit.teleportToRoomId];

                Invoke("EnableMovement", enterRoomIdleDelay);
                Invoke("EnableEnemiesInRoom", enterRoomIdleDelay);

            }

            if (this.currentRoom.entrance != null && Mathf.Abs(collision.transform.position.x - this.currentRoom.entrance.teleportFrom.x) < 1f
                && collision.transform.position.y - this.currentRoom.entrance.teleportFrom.y < 1f)
            {

                this.enableTeleports = false;
                this.enableMovement = false;
                this.playerObject.transform.position = new Vector3(this.currentRoom.entrance.teleportTo.x,
                    this.currentRoom.entrance.teleportTo.y, 0);

                this.currentRoom = this.roomInfo[this.currentRoom.entrance.teleportToRoomId];

                Invoke("EnableMovement", enterRoomIdleDelay);
                Invoke("EnableEnemiesInRoom", enterRoomIdleDelay);

            }


        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Coin")
        {
            Destroy(collision.gameObject);
            PickUpCoin();
            Debug.Log(this.moneyAmount);
        }
    }

    void AttackAnimationEnd()
    {
        this.isAttacking = false;
    }


    void DealDamage()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(playerAttackPoint.position, playerAttackRange, enemyLayer);

        foreach(Collider2D enemy in hitEnemies)
        {
            Debug.Log("Hit: " + enemy.name);
            enemy.GetComponent<Enemy>().TakeDamage(this.playerAttackDamage);
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
        this.playerCurrentHealth -= dmg;

        this.animator.SetTrigger("hurt");

        Debug.Log("MOJE HP: " + this.playerCurrentHealth);
        Debug.Log("MOJE MAX HP: " + this.playerMaxHealth);

        if (this.playerCurrentHealth <= 0)
        {
            this.healthSlider.value = 0;
            this.playerCurrentHealth = 0;
            Debug.Log("PLAYER UMIERA");
            this.animator.SetTrigger("dead");
        }
        else if ((float)this.playerCurrentHealth / this.playerMaxHealth < 0.3) this.healthSliderFill.color = Color.red;
        else if ((float)this.playerCurrentHealth / this.playerMaxHealth < 0.6) this.healthSliderFill.color = Color.yellow;

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

    private void OnDrawGizmosSelected()
    {
        if (playerAttackPoint == null)
            return;
        Gizmos.DrawWireSphere(playerAttackPoint.position, playerAttackRange);
    }

}
