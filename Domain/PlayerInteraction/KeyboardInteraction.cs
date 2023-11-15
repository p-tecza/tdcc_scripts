using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardInteraction : MonoBehaviour
{

    [SerializeField]
    private GameController gameController;

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

        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("PROBA INTERAKCJI");
            this.gameController.TryToInteractWithNearestEntity();
        }

    }

}
