using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Coin : MonoBehaviour
{
    public int coinID;

    private void Awake()
    {
        SetCoinID(GenerationEntityIDController.currentCoinID);
    }
    public void ControlTheCoinDrop()
    {
        this.GetComponent<BoxCollider2D>().isTrigger = false;
        Invoke("FreezeCoinTransition", 2f);
    }

    private void FreezeCoinTransition()
    {
        Rigidbody2D rigidbody2D = this.GetComponent<Rigidbody2D>();
        rigidbody2D.constraints = RigidbodyConstraints2D.FreezePosition;
        this.GetComponent<BoxCollider2D>().isTrigger = true;
    }

    public void SetCoinID(int coinID)
    {
        this.coinID = coinID;
        GenerationEntityIDController.currentCoinID += 1;
    }
    public int GetCoinID()
    {
        return this.coinID;
    }
}