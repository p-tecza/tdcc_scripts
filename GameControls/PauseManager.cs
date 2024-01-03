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
    [SerializeField]
    private GameObject dialogWindowsToDisableParent;
    private List<GameObject> dialogWindowObjectsBuffer = new List<GameObject>();
    private void Awake()
    {
        instance = this;
    }

    public void Pause()
    {
        int childCount = this.dialogWindowsToDisableParent.transform.childCount;

        for(int i = 0; i < childCount; i++)
        {
            GameObject currentObj = this.dialogWindowsToDisableParent.transform.GetChild(i).gameObject;
            if (currentObj.activeSelf)
            {
                this.dialogWindowObjectsBuffer.Add(currentObj);
                currentObj.SetActive(false);
            }
        }
        pauseWindow.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Unpause()
    {
        foreach(GameObject disabledObject in dialogWindowObjectsBuffer)
        {
            disabledObject.SetActive(true);
        }
        this.dialogWindowObjectsBuffer = new List<GameObject>();
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
            this.gameController.GetFullActiveQuestStateData(),
            this.gameController.GetAllDroppedQuestItemStateData(),
            this.playerController.GetOwnedQuestItems(),
            this.gameController.GetCurrentLevel()
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
