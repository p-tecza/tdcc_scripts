using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Star : MonoBehaviour
{
    public void ControlTheStarDrop()
    {
        Invoke("FreezeStarTransition", 2f);
    }

    private void FreezeStarTransition()
    {
        Rigidbody2D rigidbody2D = this.GetComponent<Rigidbody2D>();
        rigidbody2D.constraints = RigidbodyConstraints2D.FreezePosition;
    }
}