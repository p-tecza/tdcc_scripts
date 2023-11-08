using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Treasure : MonoBehaviour
{
    [SerializeField]
    public List<GameObject> possibleTreasures;
    [SerializeField]
    private Sprite openChestSprite;
    [SerializeField]
    public GameObject healthPotion;
    [SerializeField]
    public GameObject star;
    [SerializeField]
    private GameObject coin;
    public GameObject sprite { get; set; }
    public int mainItemId { get; set; }
    public Dictionary<string,int > additionalItems { get; set; }
    public GameController gameController;

    [SerializeField]
    private float thrustForce = 1f;

    private GameObject contains;
    private AllItemsData allItemsData;


    void Awake()
    {
        this.allItemsData = gameController.GetAllItemsData();
    }

    internal void SetContent(Dictionary<string, int> treasureContent)
    {
        this.mainItemId = treasureContent["item"];
        GameController.RemoveItemFromAvailableSpecificItemLoot(this.mainItemId);
        this.additionalItems = new Dictionary<string, int>()
        {
            {"hpPots", treasureContent["hpPots"]},
            {"coins", treasureContent["coins"]},
            {"stars", treasureContent["stars"]},
        };
        contains = possibleTreasures[this.mainItemId];
    }

    public void DropItems(Transform playerCurrentPosition)
    {
        Debug.Log("KOLIZJA ZE SKRZYNKA");
        Debug.Log(StringOfTreasureContent());

        if(this.additionalItems["hpPots"] != 0)
        {
            for(int i = 0; i < this.additionalItems["hpPots"]; i++)
            {
                float xPartModifierThrustVector = (float) UnityEngine.Random.Range(50, 100) / 100f;
                float yPartModifierThrustVector = (float) UnityEngine.Random.Range(50, 100) / 100f;
                Vector2 thrustMainVector = this.transform.position - playerCurrentPosition.position;
                Vector2 thrustVector = new Vector2(thrustMainVector.x * xPartModifierThrustVector,
                    thrustMainVector.y * yPartModifierThrustVector);
                Vector3 fixedObjectRespawn = new Vector3(thrustMainVector.x, thrustMainVector.y, 0) + this.transform.position;
                GameObject hpPotion = Instantiate(this.healthPotion, fixedObjectRespawn, Quaternion.identity);
                hpPotion.GetComponent<Rigidbody2D>().AddForce(thrustVector * thrustForce, ForceMode2D.Impulse);
                hpPotion.GetComponent<HpPotion>().ControlTheCollectableDrop();
                this.gameController.AddNewHint(fixedObjectRespawn, hpPotion);
                this.gameController.AddNewDescription(hpPotion);
            }
        }

        if (this.additionalItems["coins"] != 0)
        {
            for (int i = 0; i < this.additionalItems["coins"]; i++)
            {
                float xPartModifierThrustVector = (float)UnityEngine.Random.Range(50, 100) / 100f;
                float yPartModifierThrustVector = (float)UnityEngine.Random.Range(50, 100) / 100f;
                Vector2 thrustMainVector = this.transform.position - playerCurrentPosition.position;
                Vector2 thrustVector = new Vector2(thrustMainVector.x*xPartModifierThrustVector,
                    thrustMainVector.y*yPartModifierThrustVector);
                Vector3 fixedObjectRespawn = new Vector3(thrustMainVector.x, thrustMainVector.y, 0) + this.transform.position;
                GameObject coinObject = Instantiate(this.coin, fixedObjectRespawn, Quaternion.identity);
                coinObject.GetComponent<Rigidbody2D>().AddForce(thrustVector * thrustForce, ForceMode2D.Impulse);
                coinObject.GetComponent<Coin>().ControlTheCoinDrop();
            }
        }

        if (this.additionalItems["stars"] != 0)
        {
            for (int i = 0; i < this.additionalItems["stars"]; i++)
            {
                float xPartModifierThrustVector = (float)UnityEngine.Random.Range(50, 100) / 100f;
                float yPartModifierThrustVector = (float)UnityEngine.Random.Range(50, 100) / 100f;
                Vector2 thrustMainVector = this.transform.position - playerCurrentPosition.position;
                Vector2 thrustVector = new Vector2(thrustMainVector.x * xPartModifierThrustVector,
                    thrustMainVector.y * yPartModifierThrustVector);
                Vector3 fixedObjectRespawn = new Vector3(thrustMainVector.x, thrustMainVector.y, 0) + this.transform.position;
                GameObject starObject = Instantiate(this.star, fixedObjectRespawn, Quaternion.identity);
                starObject.GetComponent<Rigidbody2D>().AddForce(thrustVector * thrustForce, ForceMode2D.Impulse);
                starObject.GetComponent<Star>().ControlTheCollectableDrop();
                this.gameController.AddNewHint(fixedObjectRespawn, starObject);
                this.gameController.AddNewDescription(starObject);
            }
        }

        if(this.contains != null)
        {
            float xPartModifierThrustVector = (float)UnityEngine.Random.Range(50, 100) / 100f;
            float yPartModifierThrustVector = (float)UnityEngine.Random.Range(50, 100) / 100f;
            Vector2 thrustMainVector = this.transform.position - playerCurrentPosition.position;
            Vector2 thrustVector = new Vector2(thrustMainVector.x * xPartModifierThrustVector,
                thrustMainVector.y * yPartModifierThrustVector);
            Vector3 fixedObjectRespawn = new Vector3(thrustMainVector.x, thrustMainVector.y, 0) + this.transform.position;
            GameObject containedItemObject = Instantiate(this.contains, fixedObjectRespawn, Quaternion.identity);
            containedItemObject.GetComponent<Rigidbody2D>().AddForce(thrustVector * thrustForce, ForceMode2D.Impulse);
            containedItemObject.GetComponent<Item>().ControlTheItemDrop();
            this.gameController.AddNewHint(fixedObjectRespawn, containedItemObject);
            this.gameController.AddNewDescription(containedItemObject);
        }


        Invoke("DisableTreasureBoxCollider", 0.25f);
        this.GetComponent<SpriteRenderer>().sprite = this.openChestSprite;
    }

    public string StringOfTreasureContent()
    {
        return "hpPots: " + this.additionalItems["hpPots"] + "\n"
            + "coins: " + this.additionalItems["coins"] + "\n"
            + "stars: " + this.additionalItems["stars"] + "\n"
            + "item: "+ this.contains;
    }

    private void DisableTreasureBoxCollider()
    {
        this.GetComponent<BoxCollider2D>().enabled = false;
    }

}