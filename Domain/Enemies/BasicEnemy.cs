using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BasicEnemy : Enemy
{
    public GameObject player;

    void Start()
    {
    
    }

    void FixedUpdate()
    {
        float step = this.movementSpeed * Time.deltaTime;
        if(this.isActive){
            this.transform.position = Vector3.MoveTowards(this.transform.position, player.transform.position, step);
        }
    }
    public override void ActivateEnemy()
    {
        this.isActive = true;
    }

    public override void DeactivateEnemy()
    {
        this.isActive = false;
    }
}
