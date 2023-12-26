using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Coin : MonoBehaviour
{
    public int coinID;

    public bool isFromTreasure = false;
    public GameObject treasureReferance = null;
    public int pickUpEntityID;
    private void OnDestroy()
    {
        if (this.isFromTreasure && this.treasureReferance != null)
        {
            Treasure treasure = treasureReferance.GetComponent<Treasure>();
            treasure.droppedItems.Remove(this.gameObject);
        }
    }
    private void Awake()
    {
        SetCoinID(GenerationEntityIDController.currentCoinID);
        this.pickUpEntityID = GenerationEntityIDController.pickUpEntityID;
        GenerationEntityIDController.pickUpEntityID += 1;
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