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

    private Dictionary<int, Room> roomInfo;
    private int startingRoomID;
    private Room currentRoom;


    // Start is called before the first frame update
    void Start()
    {
        this.playerObject.SetActive(true);
        this.startingRoomID = FullDungeonGenerator.GetStartingRoomID();
        this.roomInfo = FullDungeonGenerator.GetFinalizedRooms();
        currentRoom = this.roomInfo[this.startingRoomID];
    }

    // Update is called once per frame
    void FixedUpdate()
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

    }

    //Detect collisions between the GameObjects with Colliders attached
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("KOLIZJEEE");
        //Check for a match with the specified name on any GameObject that collides with your GameObject
        if (collision.gameObject.name == "Walls")
        {
            //If the GameObject's name matches the one you suggest, output this message in the console

            var pos = this.playerObject.transform.position;
            Vector2Int v2iPosition = new Vector2Int((int)pos.x, (int)pos.y);

            if (Vector2Int.Distance(v2iPosition,this.currentRoom.entrance) < 1.5)
            {
                Debug.Log("WYCHODZE");
            }

            Debug.Log(this.currentRoom.exit);
            Debug.Log(v2iPosition.ToString());

            if (Vector2Int.Distance(v2iPosition, this.currentRoom.exit) < 1.5)
            {
                Debug.Log("WCHODZE");
                int nextRoomId = currentRoom.connections[0];
                this.currentRoom = this.roomInfo[nextRoomId];
                Vector2Int nextRoomCords = this.roomInfo[nextRoomId].FloorTiles.First();
                this.playerObject.transform.position = new Vector3(nextRoomCords.x,nextRoomCords.y,0);
            }

        }

        //Check for a match with the specific tag on any GameObject that collides with your GameObject
        if (collision.gameObject.tag == "MyGameObjectTag")
        {
            //If the GameObject has the same tag as specified, output this message in the console
            Debug.Log("Do something else here");
        }
    }



}
