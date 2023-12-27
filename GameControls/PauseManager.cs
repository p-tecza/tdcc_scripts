using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseWindow;
    public static PauseManager instance;
    [SerializeField]
    private PlayerController playerController;
    [SerializeField]
    private GameController gameController;
    private void Awake()
    {
        instance = this;
    }

    public void Pause()
    {
        pauseWindow.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Unpause()
    {
        pauseWindow.SetActive(false);
        Time.timeScale = 1f;
    }

    public void SaveAndReturnToMainMenu()
    {
        SaveData data = new SaveData(
            SaveSystem.gameState,
            ProgressHolder.collectedCoinIDs,
            ProgressHolder.slainEnemyIDs,
            this.playerController.stats,
            this.playerController.GetAdditionalPlayerData(),
            this.playerController.GetEnemiesStateData(),
            ProgressHolder.openedTreasuresSequence,
            this.gameController.GetTreasureStateData(),
            this.gameController.GetRemainingShopItemIds(),
            this.gameController.GetFullActiveQuestStateData()
            );
        Debug.Log("CURRENT SEQUENCE OF OPEN: ");

        foreach(int i in ProgressHolder.openedTreasuresSequence)
        {
            Debug.Log(i);
        }

        SaveSystem.SaveData(data);
        GameSceneManager.instance.LoadMainMenuScene();
    }

}
