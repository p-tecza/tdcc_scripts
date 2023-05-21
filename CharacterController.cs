using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public GameObject playerObject;

    [SerializeField]
    [Range (0.1f, 2f)]
    private float moveSpeed = 0.1f;

    [SerializeField]
    [Range(0.1f, 3f)]
    private float enterRoomIdleDelay = 0.4f;

    private Dictionary<int, Room> roomInfo;
    private int startingRoomID;
    private Room currentRoom;

    private bool enableTeleports;
    private bool enableMovement;


    // Start is called before the first frame update
    public void SetUpCharacter()
    {
        this.playerObject.SetActive(true);
        this.startingRoomID = FullDungeonGenerator.GetStartingRoomID();
        this.roomInfo = FullDungeonGenerator.GetFinalizedRooms();
        Debug.Log("ROOM INGO:"+ this.roomInfo);
        this.enableTeleports = true;
        this.enableMovement = true;
        this.currentRoom = this.roomInfo[this.startingRoomID];
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (this.enableMovement)
        {

        
        if(Input.GetKey(KeyCode.W))
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
        }

        if (Input.GetKey(KeyCode.D))
        {
            this.playerObject.transform.position = this.playerObject.transform.position += new Vector3(moveSpeed, 0, 0);
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

    //Detect collisions between the GameObjects with Colliders attached
    void OnCollisionEnter2D(Collision2D collision)
    {

        //Check for a match with the specified name on any GameObject that collides with your GameObject
        /*if (collision.gameObject.name == "Walls")
        {
        }*/

        //Check for a match with the specific tag on any GameObject that collides with your GameObject
        if (collision.gameObject.tag == "Teleport" && enableTeleports)
        {

            Debug.Log(this.currentRoom);
            Debug.Log(collision);

            if(this.currentRoom.exit != null && Mathf.Abs(collision.transform.position.x - this.currentRoom.exit.teleportFrom.x) < 1f
                && collision.transform.position.y - this.currentRoom.exit.teleportFrom.y < 1f)
            {

                //WEZ DODAJ ZEBY BYL FREEZE PO TELEPORCIE NA CHWILE

                this.enableTeleports = false;
                this.enableMovement = false;
                this.playerObject.transform.position = new Vector3(this.currentRoom.exit.teleportTo.x,
                    this.currentRoom.exit.teleportTo.y,0);

                Debug.Log("POZYCJA GRACZA PO TPKU: "+this.playerObject.transform.position);

                this.currentRoom = this.roomInfo[this.currentRoom.exit.teleportToRoomId];

                Invoke("EnableMovement", enterRoomIdleDelay);
                Invoke("EnableEnemiesInRoom", enterRoomIdleDelay);

                /*if (0 != this.currentRoom.connections.Count)
                {
                    int nextRoomId = this.currentRoom.connections[0];
                    this.currentRoom = this.roomInfo[nextRoomId];
                }
*/
            }

            if (this.currentRoom.entrance != null && Mathf.Abs(collision.transform.position.x - this.currentRoom.entrance.teleportFrom.x) < 1f
                && collision.transform.position.y - this.currentRoom.entrance.teleportFrom.y < 1f)
            {

                this.enableTeleports = false;
                this.enableMovement = false;
                this.playerObject.transform.position = new Vector3(this.currentRoom.entrance.teleportTo.x,
                    this.currentRoom.entrance.teleportTo.y, 0);

                Debug.Log("POZYCJA GRACZA PO TPKU: " + this.playerObject.transform.position);

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
        foreach(GameObject e in this.currentRoom.enemies)
        {
            e.GetComponent<BasicEnemy>().player = this.playerObject;
            e.GetComponent<BasicEnemy>().ActivateEnemy();
            Debug.Log(e.GetComponent<BasicEnemy>().isActive);
        }
    }

    private void DisableEnemiesInRoom()
    {
        foreach (GameObject e in this.currentRoom.enemies)
        {
            e.GetComponent<BasicEnemy>().DeactivateEnemy();
        }
    }

}
