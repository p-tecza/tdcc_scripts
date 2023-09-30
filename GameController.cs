using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{

    [SerializeField]
    private GameObject playerObject;
    [SerializeField]
    private FullDungeonGenerator dungeonGenerator;

    public GameOverScreen gameOverScreen;

    [SerializeField]
    private PlayerController characterController;

    public TMP_Text tmpCoinsAmount;
    public TMP_Text playerToughness;
    public TMP_Text playerAttackDamage;
    public TMP_Text playerAttackRange;
    public TMP_Text playerAttackSpeed;
    public TMP_Text playerMovementSpeed;

    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    [Range(1f, 10f)]
    private float cameraZoom = 1f;

    // Start is called before the first frame update
    void Start()
    {
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

    private void UpdateUIPlayerStats(PlayerStats ps)
    {
        this.playerToughness.text = ps.toughness.ToString();
        this.playerAttackDamage.text = ps.attackDamage.ToString();
        this.playerAttackRange.text = ps.attackRange.ToString();
        this.playerAttackSpeed.text = ps.attackSpeed.ToString();
        this.playerMovementSpeed.text = ps.movementSpeed.ToString();
}

}
