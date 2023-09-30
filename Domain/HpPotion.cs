using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HpPotion : MonoBehaviour
{
    public void ControlTheHealthPotionDrop()
    {
        Invoke("FreezeHealthPotionTransition", 2f);
    }

    private void FreezeHealthPotionTransition()
    {
        Rigidbody2D rigidbody2D = this.GetComponent<Rigidbody2D>();
        rigidbody2D.constraints = RigidbodyConstraints2D.FreezePosition;
    }
}