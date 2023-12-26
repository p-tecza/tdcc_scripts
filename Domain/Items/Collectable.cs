using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class Collectable : MonoBehaviour
{
    public bool isFromTreasure = false;
    public GameObject treasureReferance = null;
    public int pickUpEntityID;
    private void Awake()
    {
        this.pickUpEntityID = GenerationEntityIDController.pickUpEntityID;
        GenerationEntityIDController.pickUpEntityID += 1;
    }
    private void OnDestroy()
    {
        if (this.isFromTreasure && this.treasureReferance != null)
        {
            Treasure treasure = treasureReferance.GetComponent<Treasure>();
            treasure.droppedItems.Remove(this.gameObject);
        }
    }
    public void ControlTheCollectableDrop()
    {
        Debug.Log("LECI KOLEKTABLE");
        Invoke("FreezeItemTransition", 2f);
    }

    private void FreezeItemTransition()
    {
        Debug.Log("STOP TRANSLACJA");
        Rigidbody2D rigidbody2D = this.GetComponent<Rigidbody2D>();
        rigidbody2D.constraints = RigidbodyConstraints2D.FreezePosition;
    }

}