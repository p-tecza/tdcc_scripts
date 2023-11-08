using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class GameController : MonoBehaviour
{

    [SerializeField]
    private GameObject playerObject;
    [SerializeField]
    private FullDungeonGenerator dungeonGenerator;

    public GameOverScreen gameOverScreen;

    private AllItemsData allItemsData;

    [SerializeField]
    private PlayerController characterController;

    public TMP_Text tmpCoinsAmount;
    public TMP_Text playerToughness;
    public TMP_Text playerAttackDamage;
    public TMP_Text playerAttackRange;
    public TMP_Text playerAttackSpeed;
    public TMP_Text playerMovementSpeed;
    public TMP_Text ownedHpPots;
    public TMP_Text ownedStars;

    public Canvas priceCanvas;
    public Canvas hintCanvas;
    public GameObject hintObject;
    public float additionalHintOffset = 0.5f;

    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    [Range(1f, 10f)]
    private float cameraZoom = 1f;
    [SerializeField]
    private bool hintsVisible = false;

    private static List<int> availableSpecificItemLoot = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        this.allItemsData = DataReader.ReadAllItemsData();
        dungeonGenerator.GenerateDungeon();
        Vector3 startPosition = dungeonGenerator.GetStartingPosition();
        playerObject.transform.position = startPosition;
        mainCamera.transform.position = startPosition;
        characterController.SetUpCharacter();
        characterController.enabled = true;
        PlayerStats ps = playerObject.GetComponent<PlayerController>().GetStats();
        UpdateUIPlayerStats(ps);
    }

    // Update is called once per frame
    void Update()
    {
        mainCamera.transform.position = 
            new Vector3(playerObject.transform.position.x, playerObject.transform.position.y, -cameraZoom);
    }

    public void UpdateUICoinsAmount(int newCoinsAmount)
    {
        this.tmpCoinsAmount.text = newCoinsAmount.ToString();
    }

    public void GameOver()
    {
        this.gameOverScreen.GameOverScreenPopUp();
    }

    public void UpdateUIPlayerStats(PlayerStats ps)
    {
        this.playerToughness.text = ps.toughness.ToString();
        this.playerAttackDamage.text = ps.attackDamage.ToString();
        this.playerAttackRange.text = ps.attackRange.ToString();
        this.playerAttackSpeed.text = ps.attackSpeed.ToString();
        this.playerMovementSpeed.text = ps.movementSpeed.ToString();
    }

    public void UpdateUICollectables(int hpPotsAmount, int starsAmount)
    {
        this.ownedHpPots.text = hpPotsAmount.ToString();
        this.ownedStars.text = starsAmount.ToString();
    }

    public static void RemoveItemFromAvailableSpecificItemLoot(int itemId)
    {
        Debug.Log(itemId);
        availableSpecificItemLoot.Remove(itemId);
    }
    public static void SetAvailableSpecificItemLoot(List<int> availableLoot)
    {
        availableSpecificItemLoot = availableLoot;
    }

    public static List<int> GetAvailableSpecificItemLoot()
    {
        return availableSpecificItemLoot;
    }

    public void RemoveItemRelatedPriceText(GameObject shopObject)
    {

        // name of shop item pricetext must be unique, add some sort of iterator for specific dung lvl

        string nameOfPriceText = "PriceText" + shopObject.name;
        GameObject canvasObject = this.priceCanvas.gameObject;
        int childrenCount = canvasObject.transform.childCount;
        for (int i = 0; i < childrenCount; i++)
        {
            GameObject childObject = canvasObject.transform.GetChild(i).gameObject;
            Debug.Log(childObject.name + "(Clone)");
            Debug.Log(nameOfPriceText);
            if (childObject.name + "(Clone)" == nameOfPriceText)
            {
                Destroy(childObject);
            }
        }
    }

    public void AddNewHint(Vector3 itemPosition, GameObject gameObject)
    {
        Vector3 finalHintPosition = new Vector3(itemPosition.x, itemPosition.y, itemPosition.z);
        finalHintPosition.y = itemPosition.y + gameObject.GetComponent<BoxCollider2D>().size.y / 2 + this.additionalHintOffset;
        GameObject newCanvasObject = Instantiate(this.hintCanvas.gameObject, finalHintPosition, Quaternion.identity);
        TMP_Text hintText = newCanvasObject.transform.Find("HintObject").Find("HintText").GetComponent<TMP_Text>();
        hintText.text = ItemHelper.GetNameOfItem(gameObject.name);
        

        newCanvasObject.transform.SetParent(gameObject.transform, true);
    }


    public void ToggleHints()
    {
        this.hintsVisible = !this.hintsVisible;
        GameObject[] hintObjects = GameObject.FindGameObjectsWithTag("Hint");
        for (int i = 0; i < hintObjects.Length; i++) 
        {
            GameObject currentHintCanvas = hintObjects[i];
            int hintCanvChildCount = currentHintCanvas.transform.childCount;
            for (int j = 0; j < hintCanvChildCount; j++)
            {
                currentHintCanvas.transform.GetChild(j).gameObject.SetActive(this.hintsVisible);
            }
        }
    }

    public bool AreHintsVisible()
    {
        return this.hintsVisible;
    }

    public AllItemsData GetAllItemsData()
    {
        return this.allItemsData;
    }

    public void AddNewDescription(GameObject gameObject)
    {

    }

}
