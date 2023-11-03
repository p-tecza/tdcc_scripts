using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardInteraction : MonoBehaviour
{

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("UZYTO POTKI");
            this.GetComponent<PlayerController>().UseHealthPotion();
        }

        if(Input.GetKeyDown(KeyCode.LeftAlt)) 
        {
            this.GetComponent<PlayerController>().ToggleHints();
        }

    }

}
