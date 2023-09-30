using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class Item : MonoBehaviour
{
    public int healthPoints;
    public int toughness;
    public int attackDamage;
    public float attackSpeed;
    public float attackRange;
    public float movementSpeed;

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

}