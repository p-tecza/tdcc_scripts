using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Treasure : MonoBehaviour
{
    [SerializeField]
    private Transform instantiatedDungeonObjects;
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

    public int treasureID;
    public bool isOpened = false;
    public List<GameObject> droppedItems = new List<GameObject>();

    internal void SetContent(Dictionary<string, int> treasureContent)
    {
        this.treasureID = GenerationEntityIDController.treasureID;
        GenerationEntityIDController.treasureID += 1;
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

    public void DropItems(Transform playerCurrentPosition, bool useForSave)
    {
        Debug.Log("KOLIZJA ZE SKRZYNKA");
        Debug.Log(StringOfTreasureContent());

        if(!useForSave)
        ProgressHolder.openedTreasuresSequence.Add(this.treasureID);

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
                hpPotion.GetComponent<HpPotion>().isFromTreasure = true;
                hpPotion.GetComponent<HpPotion>().treasureReferance = this.gameObject;
                this.gameController.AddNewHint(fixedObjectRespawn, hpPotion);
                droppedItems.Add(hpPotion);
                hpPotion.transform.SetParent(this.instantiatedDungeonObjects, true);
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
                coinObject.GetComponent<Coin>().isFromTreasure = true;
                coinObject.GetComponent<Coin>().treasureReferance = this.gameObject;
                droppedItems.Add(coinObject);
                coinObject.transform.SetParent(this.instantiatedDungeonObjects, true);
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
                starObject.GetComponent<Star>().isFromTreasure = true;
                starObject.GetComponent<Star>().treasureReferance = this.gameObject;
                this.gameController.AddNewHint(fixedObjectRespawn, starObject);
                droppedItems.Add(starObject);
                starObject.transform.SetParent(this.instantiatedDungeonObjects, true);
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
            containedItemObject.GetComponent<Item>().isFromTreasure = true;
            containedItemObject.GetComponent<Item>().treasureReferance = this.gameObject;
            this.gameController.AddNewHint(fixedObjectRespawn, containedItemObject);
            droppedItems.Add(containedItemObject);
            containedItemObject.transform.SetParent(this.instantiatedDungeonObjects, true);
        }

        this.isOpened = true;
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

    public void SetAlreadyLootedTreasureFromSave()
    {
        this.GetComponent<SpriteRenderer>().sprite = this.openChestSprite;
        DisableTreasureBoxCollider();
    }

}