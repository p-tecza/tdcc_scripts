using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BasicEnemy : MonoBehaviour
{

    /*public GameController gameController;*/
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private float movementSpeed = 5f;

    [SerializeField]
    private bool IsThisActive;
    public BasicEnemy(GameObject enemyObject)
    {
       /*this.enemyObject = enemyObject;*/
    }

    void Start()
    {
    }

    void FixedUpdate()
    {
       /* Debug.Log("fixed UPDATE");*/

        if (IsThisActive)
        {
            Debug.Log("IS THIS ACTIVE:");

        }

        

        float step = this.movementSpeed * Time.deltaTime;
        if(IsThisActive)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, player.transform.position, step);
        }
    }

    public void ActivateEnemy()
    {
        //CZEMU TO NIE DZIALAAA
        Debug.Log("ACTIVATE ENEMY DZIALA:");
        this.IsThisActive = true;
    }

    public void DeactivateEnemy()
    {
        this.IsThisActive = false;
    }
}
