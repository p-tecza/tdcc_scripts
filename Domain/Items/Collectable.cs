using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class Collectable : MonoBehaviour
{

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