using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public GameObject playerObject;

    [SerializeField]
    [Range (0.1f, 2f)]
    private float moveSpeed = 0.1f;


    // Start is called before the first frame update
    void Start()
    {
        this.playerObject.SetActive(true);
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
}
