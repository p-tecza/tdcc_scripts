
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPCInteractionController : MonoBehaviour
{
    [SerializeField]
    private GameObject interactionWindowObject;
    [SerializeField]
    private GameObject playerObject;
    private int interactionProgressCnt = 0;
    private GameObject procBy;
    private InteractiveDialogData interactiveDialogDataTemp;
    [SerializeField]
    private QuestController questController;

    private readonly int maxOptions = 6;


    public void Update()
    {
        if (interactionWindowObject.activeSelf)
        {
            if(procBy!=null && !CheckIfPlayerStillInRange(playerObject, procBy))
            {
                DisableInteractionWindow();
            }
        }
    }

    public void DisableInteractionWindow()
    {
        this.interactionWindowObject.SetActive(false);
        this.interactionProgressCnt = 0;
        this.interactiveDialogDataTemp = null;
        UnhiglightAllChoiceOptions();
    }

    public void EnableInteractionWindow(InteractiveDialogData dialogData, GameObject procBy)
    {
        this.procBy = procBy;
        this.interactiveDialogDataTemp = dialogData;
        if (this.interactionWindowObject.activeSelf)
        {
            this.interactionProgressCnt++;
        }
        else
        {
            this.interactionWindowObject.SetActive(true);
            this.interactionProgressCnt = 0;
        }
        this.SetProperDialogOfNPCInteractionWindow(dialogData.consecutiveDialogs);
        this.SetProperOptionsOfNPCInteractionWindow(dialogData.consecutiveOptions);
        procBy.GetComponent<NPC>().SetOnClicksButtons();
    }

    public void SwitchInteractionWindow()
    {
        if (interactionWindowObject.activeSelf)
        {
            this.interactionProgressCnt++;
        }
        this.SetProperDialogOfNPCInteractionWindow(this.interactiveDialogDataTemp.consecutiveDialogs);
        this.SetProperOptionsOfNPCInteractionWindow(this.interactiveDialogDataTemp.consecutiveOptions);
    }

    public void SetNameOfNPCInteractionWindow(string nameOfNpc)
    {
        TMP_Text npcName = interactionWindowObject.transform.Find("InteractionContent").Find("InteractionNPCName").GetComponent<TMP_Text>();
        npcName.text = nameOfNpc;
    }

    public void SetIconOfNPCInteractionWindow(Sprite sprite)
    {
        Image npcIcon = interactionWindowObject.transform.Find("InteractionContent").Find("InteractionNPCIcon").GetComponent<Image>();
        npcIcon.sprite = sprite;
    }

    public void SetProperDialogOfNPCInteractionWindow(List<string> dialogs)
    {
        TMP_Text dialog = interactionWindowObject.transform.Find("InteractionContent").Find("InteractionText").GetComponent<TMP_Text>();

        if(dialogs.Count <= this.interactionProgressCnt)
        {
            this.interactionProgressCnt--;
            return;
        }
        dialog.text = dialogs[this.interactionProgressCnt];
    }

    public void SetProperOptionsOfNPCInteractionWindow(List<string> options)
    {
        List<string> currentWindowOptions = PickOptionsForCurrentProgressCnt(options); // cnt doesn't need decrement cuz of sequence SetProperDialog -> SetProperOptions
        for(int i = 0; i < this.maxOptions; i++)
        {
            TMP_Text optionText = interactionWindowObject.transform.Find("InteractionContent").Find("InteractionOptions")
                .Find("Option"+(i+1)).GetComponent<TMP_Text>();
            if(currentWindowOptions.Count > i)
            {
                optionText.text = currentWindowOptions[i];
            }
            else
            {
                optionText.text = "";
            }
        }
        ChangeInteractionWindowDisplayBasedOnNPC(); // changes interaction window for specific quests / behaviours
    }

    private void ChangeInteractionWindowDisplayBasedOnNPC()
    {
        string currentNpcType = "";
        if (this.interactiveDialogDataTemp != null && this.interactionProgressCnt >= 1)
        {
            currentNpcType = this.interactiveDialogDataTemp.npcType;
        }

        switch(currentNpcType)
        {
            case "Trainer":
                TrainerNPC trainerNPC = procBy.GetComponent<TrainerNPC>();
                AppendSomeInfoToInteractionWindowDescription(" [ cost:" + trainerNPC.trainingCost + " | multiplier:" + trainerNPC.costRaiseMultiplier + " ]");
                break;
            case "Astray":
                AstrayNPC astrayNPC = procBy.GetComponent<AstrayNPC>();
                QuestData astrayQuestData = astrayNPC.questData;
                SetQuestContentForInteractionWindowDescription(astrayQuestData);
                break;
            case "Wanderer":
                break;
            case "Trader":
                break;
            default: break;
        }

    }

    private void AppendSomeInfoToInteractionWindowDescription(string additionalInfo)
    {
        TMP_Text dialog = interactionWindowObject.transform.Find("InteractionContent").Find("InteractionText").GetComponent<TMP_Text>();
        dialog.text += additionalInfo;
    }

    private void SetQuestContentForInteractionWindowDescription(QuestData questData)
    {
        QuestState currentQuestState = this.questController.GetQuestState();
        Debug.Log("STATE: " + currentQuestState);

        if (currentQuestState == QuestState.NOT_STARTED)
        {
            TMP_Text dialog = interactionWindowObject.transform.Find("InteractionContent").Find("InteractionText").GetComponent<TMP_Text>();
            dialog.text = questData.questDialog;
            dialog.text += "\nQuest: " + questData.questDescription;
        }
        else if(currentQuestState == QuestState.STARTED || currentQuestState == QuestState.FINISHED)
        {
            TMP_Text dialog = interactionWindowObject.transform.Find("InteractionContent").Find("InteractionText").GetComponent<TMP_Text>();
            string questPhrase, option;
            (questPhrase, option) = this.questController.GetStateDataOfQuestForInteractionWindow();
            dialog.text = questPhrase;
            SetQuestOption(option);
        }
    }

    private void SetQuestOption(string option)
    {
        Debug.Log("SETTING QUEST OPTION: " + option);

        for (int i = 0; i < this.maxOptions; i++)
        {
            Debug.Log("INSIDE OF FOR");
            TMP_Text optionText = interactionWindowObject.transform.Find("InteractionContent").Find("InteractionOptions")
                .Find("Option" + (i + 1)).GetComponent<TMP_Text>();
            if (i == 0)
            {
                Debug.Log("SETTING OPTION");
                optionText.text = option;
            }
            else
            {
                optionText.text = "";
            }
        }
    }


    private List<string> PickOptionsForCurrentProgressCnt(List<string> options)
    {
        List<string> retOptions = new List<string>();
        foreach (string o in options)
        {
            string[] resOfSplit = o.Split('>');
            if(resOfSplit.Length < 2) 
            {
                return retOptions;
            }
            string num = resOfSplit[0];
            string val = resOfSplit[1];
            if (int.Parse(num) == this.interactionProgressCnt + 1)
            {
                retOptions.Add(val);
            }
        }
        return retOptions;
    }

    private bool CheckIfPlayerStillInRange(GameObject player, GameObject interactionWith)
    {
        Vector3 playerPos = player.transform.position;
        Vector3 interactionWithPos = interactionWith.transform.position;

        if(Vector3.Distance(playerPos, interactionWithPos) > player.GetComponent<PlayerController>().playerInteractionRange)
        {
            return false;
        }
        return true;
    }

    private void UnhiglightAllChoiceOptions()
    {
        Transform interactionOptionsTransform = this.interactionWindowObject.transform.Find("InteractionContent").Find("InteractionOptions");
        int childAmount = interactionOptionsTransform.childCount;
        for(int i = 0; i < childAmount; i++)
        {
            GameObject optionObject = interactionOptionsTransform.GetChild(i).gameObject;
            if (optionObject != null)
            {
                TextHighlight th = optionObject.GetComponent<TextHighlight>();
                th.UnhighlightTextOutOfHover();
            }
        }

    }


    public void DetermineOnClickAction(string npcType, int optionNumber)
    {
        if(this.interactionProgressCnt == 0)
        {
            if(optionNumber == 1 && this.interactiveDialogDataTemp != null)
            {
                this.SwitchInteractionWindow();
            }
            else if(optionNumber == 2) 
            {
                this.DisableInteractionWindow();
            }
        }else if(this.interactionProgressCnt >= 1)
        {
            PlayerController playerController = this.playerObject.GetComponent<PlayerController>();

            if(npcType == "Trainer")
            {
                if (this.procBy == null) return;

                TrainerNPC trainerNPC = this.procBy.GetComponent<TrainerNPC>();
                int trainingCost = trainerNPC.trainingCost;
                switch (optionNumber)
                {
                    case 1: //TGH
                        playerController.UpgradeStat(StatType.Toughness, 1f, trainerNPC);
                        break;
                    case 2: //ATD
                        playerController.UpgradeStat(StatType.AttackDamage, 1f, trainerNPC);
                        break;
                    case 3: //ATR
                        playerController.UpgradeStat(StatType.AttackRange, 0.1f, trainerNPC);
                        break;
                    case 4: //ATS
                        playerController.UpgradeStat(StatType.AttackSpeed, 0.1f, trainerNPC);
                        break;
                    case 5: //MVS
                        playerController.UpgradeStat(StatType.MovementSpeed, 0.05f, trainerNPC);
                        break;
                    case 6:
                        break;
                    default:
                        break;
                }
                this.SetProperDialogOfNPCInteractionWindow(this.interactiveDialogDataTemp.consecutiveDialogs);

            }
            else if(npcType == "Trader")
            {

            }
            else if (npcType == "Wanderer")
            {

            }
            else if (npcType == "Astray")
            {
                QuestState currentQuestState = this.questController.GetQuestState();
                switch (optionNumber) 
                {
                    case 1:
                        if (currentQuestState == QuestState.NOT_STARTED)
                        {
                            AstrayNPC astrayNPC = this.procBy.GetComponent<AstrayNPC>();
                            playerController.StartQuest(astrayNPC.questData);
                            this.DisableInteractionWindow();
                        }
                        else if(currentQuestState == QuestState.STARTED)
                        {
                            this.DisableInteractionWindow();
                        }
                        else if(currentQuestState == QuestState.FINISHED)
                        {
                            this.questController.FinishQuest();
                            this.DisableInteractionWindow();
                        }
                        break;
                    case 2:
                        if(currentQuestState == QuestState.NOT_STARTED)
                        {
                            this.DisableInteractionWindow();
                        }
                        break;
                    default:
                        break;
                }
            }


        }



    }

}