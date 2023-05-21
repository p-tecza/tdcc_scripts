using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public GameObject playerObject;
    public Animator animator;

    [SerializeField]
    [Range(0.1f, 2f)]
    private float moveSpeed = 0.1f;

    [SerializeField]
    [Range(0.1f, 3f)]
    private float enterRoomIdleDelay = 0.4f;

    private Dictionary<int, Room> roomInfo;
    private int startingRoomID;
    private Room currentRoom;

    private bool enableTeleports;
    private bool enableMovement;

    private bool isRunning;
    private bool isAttacking;
    private bool isHurt;
    private bool isDead;

    public void SetUpCharacter()
    {
        this.playerObject.SetActive(true);
        this.startingRoomID = FullDungeonGenerator.GetStartingRoomID();
        this.roomInfo = FullDungeonGenerator.GetFinalizedRooms();
        this.enableTeleports = true;
        this.enableMovement = true;
        this.currentRoom = this.roomInfo[this.startingRoomID];
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

            if (Input.GetKey(KeyCode.W))
            {
                this.playerObject.transform.position = this.playerObject.transform.position += new Vector3(0, moveSpeed, 0);
            }

            if (Input.GetKey(KeyCode.S))
            {
                this.playerObject.transform.position = this.playerObject.transform.position += new Vector3(0, -moveSpeed, 0);
            }

            if (Input.GetKey(KeyCode.A))
            {
                this.playerObject.transform.position = this.playerObject.transform.position += new Vector3(-moveSpeed, 0, 0);
                this.playerObject.transform.rotation = new Quaternion(this.playerObject.transform.rotation.x,
                            0f, this.playerObject.transform.rotation.z, this.playerObject.transform.rotation.w);
            }

            if (Input.GetKey(KeyCode.D))
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
            e.GetComponent<Enemy>().DeactivateEnemy();
        }
    }

}
