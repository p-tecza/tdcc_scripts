using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class Item : MonoBehaviour
{
    public string itemName;
    public int healthPoints;
    public int toughness;
    public int attackDamage;
    public float attackSpeed;
    public float attackRange;
    public float movementSpeed;

    public int pickUpEntityID;

    public bool isFromTreasure = false;
    public GameObject treasureReferance = null;

    private void Awake()
    {
        this.pickUpEntityID = GenerationEntityIDController.pickUpEntityID;
        GenerationEntityIDController.pickUpEntityID += 1;
    }

    private void OnDestroy()
    {
        if(this.isFromTreasure && this.treasureReferance != null)
        {
            Treasure treasure = treasureReferance.GetComponent<Treasure>();
            treasure.droppedItems.Remove(this.gameObject);
        }
    }
    public void ControlTheItemDrop()
    {
        Invoke("FreezeItemTransition", 2f);
    }

    private void FreezeItemTransition()
    {
        Rigidbody2D rigidbody2D = this.GetComponent<Rigidbody2D>();
        rigidbody2D.constraints = RigidbodyConstraints2D.FreezePosition;
    }

    protected abstract void SpecificItemAction();

    public ItemStats returnItemStats()
    {
        ItemStats itemStats = new ItemStats();
        itemStats.healthPoints = healthPoints;
        itemStats.toughness = toughness;
        itemStats.attackDamage = attackDamage;
        itemStats.attackSpeed = attackSpeed;
        itemStats.attackRange = attackRange;
        itemStats.movementSpeed = movementSpeed;

        return itemStats;
    }

}