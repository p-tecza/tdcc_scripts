using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyController : MonoBehaviour
{

    [SerializeField]
    private GameObject player;
    [SerializeField]
    private float movementSpeed = 5f;
    private bool isActive;
    


    // Start is called before the first frame update
    void Start()
    {
        this.isActive = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float step = this.movementSpeed * Time.deltaTime;
        if(isActive)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, player.transform.position, step);
        }

    }
}
