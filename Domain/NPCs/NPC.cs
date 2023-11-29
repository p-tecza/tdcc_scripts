

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    [SerializeField]
    protected NPCInteractionController npcInteractionController;
    [SerializeField]
    protected GameController gameController;
    public string npcName;
    public string npcType;
    /*public string startDialog;*/
    private InteractiveDialogData npcData;

    [SerializeField]
    protected Button opt1Button;
    [SerializeField]
    protected Button opt2Button;
    [SerializeField]
    protected Button opt3Button;
    [SerializeField]
    protected Button opt4Button;
    [SerializeField]
    protected Button opt5Button;
    [SerializeField]
    protected Button opt6Button;

    public static List<string> npcTypes = new List<string>()
    {
        "Trainer", "Trader", "Wanderer", "Astray"
    };
    // Trainer -> possibility to upgrade some of your skills
    // Trader -> possibility to buy an item with discounted price
    // Wanderer -> basic talk, nothing game-changing
    // Astray -> grants an item after returning something he lost
    public virtual void Interact()
    {

    }


    public virtual void Start()
    {
        if (this.npcInteractionController != null)
        {
            this.npcInteractionController.SetNameOfNPCInteractionWindow(this.npcName);
            this.npcInteractionController.SetIconOfNPCInteractionWindow(gameObject.GetComponent<SpriteRenderer>().sprite);
        }
    }

    public void SetNPCData(AllInteractiveDialogData allNPCDialogData)
    {
        foreach(InteractiveDialogData npcDialogData in allNPCDialogData.allNPCData)
        {
            if(npcDialogData.npcType == this.npcType)
            {
                this.npcData = npcDialogData;
                break;
            }
        }
    }

    public InteractiveDialogData GetNPCData()
    {
        return this.npcData;
    }

    public void SetOnClicksButtons()
    {
        this.ResetListenersFromButtons();
        this.opt1Button.onClick.AddListener(InteractOption1);
        this.opt2Button.onClick.AddListener(InteractOption2);
        this.opt3Button.onClick.AddListener(InteractOption3);
        this.opt4Button.onClick.AddListener(InteractOption4);
        this.opt5Button.onClick.AddListener(InteractOption5);
        this.opt6Button.onClick.AddListener(InteractOption6);
    }

    public virtual void InteractOption1()
    {

    }
    public virtual void InteractOption2()
    {

    }
    public virtual void InteractOption3()
    {

    }
    public virtual void InteractOption4()
    {

    }
    public virtual void InteractOption5()
    {

    }
    public virtual void InteractOption6()
    {

    }

    private void ResetListenersFromButtons()
    {
        this.opt1Button.onClick.RemoveAllListeners();
        this.opt2Button.onClick.RemoveAllListeners();
        this.opt3Button.onClick.RemoveAllListeners();
        this.opt4Button.onClick.RemoveAllListeners();
        this.opt5Button.onClick.RemoveAllListeners();
        this.opt6Button.onClick.RemoveAllListeners();
    }

    public static int DetermineRandomNPC()
    {
        return UnityEngine.Random.Range(0, npcTypes.Count);
    }




}