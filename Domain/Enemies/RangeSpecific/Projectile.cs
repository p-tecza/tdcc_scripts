using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Projectile : MonoBehaviour
{
    private int projectileDamage;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(this.projectileDamage);
            Debug.Log("PLAYER DOSTAJE");
        }
        else if(collision.gameObject.layer == 0 || collision.gameObject.layer == 9) // 0 -> Default ; 9 -> Environment
        {
            Debug.Log("SCIANA DOSTAJE");
        }
        Destroy(this.gameObject);
    }

    public void SetProjectileDamage(int projectileDamage)
    {
        this.projectileDamage = projectileDamage;
    }

}