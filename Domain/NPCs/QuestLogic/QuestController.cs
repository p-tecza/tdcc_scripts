using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestController : MonoBehaviour
{
    [SerializeField]
    private GameObject questWindowObject;
    [SerializeField]
    private GameController gameController;
    [SerializeField]
    private GameObject questCompleted;
    [SerializeField]
    private GameObject questPickedUp;

    private QuestData currentQuestData;
    private int currentQuestProgress = 0;
    private int questProgressThreshold = 1;
    private bool isQuestActive = false;
    private bool isQuestCompleted = false;


    private static readonly string QUEST_ASTRAY_ITEM_NAME = "Necklace";

    public void PickUpQuest()
    {
        this.questWindowObject.SetActive(true);
        this.isQuestActive = true;
        this.questPickedUp.SetActive(true);
        this.questCompleted.SetActive(false);
    }

    public void SetQuestData(QuestData questData)
    {
        this.currentQuestData = questData;
        SetQuestInfoWindow(questData);
    }

    private void SetQuestInfoWindow(QuestData questData)
    {
        Transform questContentTransform = this.questWindowObject.transform.Find("QuestContent");
        TMP_Text questDisplayType = questContentTransform.Find("QuestText").gameObject.GetComponent<TMP_Text>();
        TMP_Text questDisplayProgress = questContentTransform.Find("QuestProgress").gameObject.GetComponent<TMP_Text>();
        int questProgressThreshold = DetermineQuestProgressThreshold(questData);
        int questCurrentProgress = DetermineQuestCurrentProgress();
        questDisplayType.text = questData.questGoal;
        if (CheckIfQuestIsCompleted())
        {
            this.isQuestCompleted = true;
            questCurrentProgress = questProgressThreshold;
            this.currentQuestProgress = this.questProgressThreshold;
            HighlightQuestAfterCompletion(questDisplayProgress);
        }
        questDisplayProgress.text = questCurrentProgress + "/" + questProgressThreshold;
    }

    private void UpdateQuestInfoProgressWindow()
    {
        Transform questContentTransform = this.questWindowObject.transform.Find("QuestContent");
        TMP_Text questDisplayProgress = questContentTransform.Find("QuestProgress").gameObject.GetComponent<TMP_Text>();
        questDisplayProgress.text = this.currentQuestProgress + "/" + this.questProgressThreshold;
        if (this.isQuestCompleted)
        {
            HighlightQuestAfterCompletion(questDisplayProgress);
        }
    }

    private int DetermineQuestProgressThreshold(QuestData questData)
    {
        string questType = questData.questGoal;
        switch(questType)
        {
            case "Retrieve item":
                this.questProgressThreshold = 1;
                return 1;
            case "Purge":
                int allEnemiesInDung = this.gameController.GetAmountOfAllEnemiesInDungeonFloor();
                int enemiesThreshold = Mathf.FloorToInt(allEnemiesInDung / 4);
                this.questProgressThreshold = enemiesThreshold;
                return enemiesThreshold;
            default: break;
        }
        return 0;
    }

    public int DetermineQuestCurrentProgress()
    {
        if(this.isQuestActive && this.currentQuestData != null && !isQuestCompleted)
        {
            string questType = this.currentQuestData.questGoal;
            switch (questType)
            {
                case "Retrieve item":
                    List<Item> itemList = this.gameController.GetListOfPlayerOwnedItems();
                    if(CheckIfPlayerPossessesQuestItem(itemList, QUEST_ASTRAY_ITEM_NAME))
                    {
                        this.currentQuestProgress = 1;
                        return 1;
                    }
                    this.currentQuestProgress = 0;
                    return 0;
                case "Purge":
                    int killedEnemies = this.gameController.GetAmountOfDeadEnemiesInDungeonFloor();
                    this.currentQuestProgress = killedEnemies;
                    return killedEnemies;
                default: break;
            }
        }
        return 0;
    }

    public void UpdateQuestProgress()
    {
        DetermineQuestCurrentProgress();
        if (CheckIfQuestIsCompleted())
        {
            this.isQuestCompleted = true;
            this.currentQuestProgress = this.questProgressThreshold;
        }
        UpdateQuestInfoProgressWindow();
    }

    private bool CheckIfQuestIsCompleted()
    {
        return this.currentQuestProgress >= this.questProgressThreshold;
    }

    private bool CheckIfPlayerPossessesQuestItem(List<Item> possessedItems, string questItemName)
    {
        foreach(Item item in possessedItems)
        {
            if(item.itemName == questItemName)
            {
                return true;
            }
        }
        return false;
    }

    private void HighlightQuestAfterCompletion(TMP_Text questProgress)
    {
        this.questPickedUp.SetActive(false);
        this.questCompleted.SetActive(true);
        float r = (float)(125f / 255f);
        float g = (float)(125f / 255f);
        float b = (float)(125f / 255f);
        questProgress.color = new UnityEngine.Color(r, g, b, 1);
    }

    public (string, string) GetStateDataOfQuestForInteractionWindow()
    {
        if(this.isQuestActive && !this.isQuestCompleted)
        {
            return (this.currentQuestData.questNotCompletedYetDialog, "Quit");
        }
        else if (this.isQuestActive && this.isQuestCompleted)
        {
            return (this.currentQuestData.questCompletedDialog, "Finish");
        }
        return ("", "");
    }

    public QuestState GetQuestState()
    {
        if (this.isQuestActive && !this.isQuestCompleted)
        {
            return QuestState.STARTED;
        }
        else if (this.isQuestActive && this.isQuestCompleted)
        {
            return QuestState.FINISHED;
        }
        else if (!this.isQuestActive && this.isQuestCompleted)
        {
            return QuestState.SOLVED;
        }
        else
        {
            return QuestState.NOT_STARTED;
        }
    }

    public void FinishQuest()
    {
        Debug.Log("FINISHED THE QUEST!!!!!");
        // TODO logic for reward...
        // TODO Dialog Window after quest solved
        this.isQuestActive = false;
        this.questPickedUp.SetActive(false);
        this.questCompleted.SetActive(false);
        this.questWindowObject.SetActive(false);
        
    }
}