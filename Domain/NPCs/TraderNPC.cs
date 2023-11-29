using UnityEngine;

public class TraderNPC : NPC
{

    public override void Interact()
    {
        /*throw new System.NotImplementedException();*/
        Debug.Log("Interacted with TraderNPC");
    }
    public override void InteractOption1()
    {
        Debug.Log("PROCCED 1");
        this.npcInteractionController.DetermineOnClickAction("Trader", 1);
    }
    public override void InteractOption2()
    {
        Debug.Log("PROCCED 2");
        this.npcInteractionController.DetermineOnClickAction("Trader", 2);
    }
    public override void InteractOption3()
    {
        Debug.Log("PROCCED 3");
        this.npcInteractionController.DetermineOnClickAction("Trader", 3);
    }
    public override void InteractOption4()
    {
        Debug.Log("PROCCED 4");
        this.npcInteractionController.DetermineOnClickAction("Trader", 4);
    }
    public override void InteractOption5()
    {
        Debug.Log("PROCCED 5");
        this.npcInteractionController.DetermineOnClickAction("Trader", 5);
    }
    public override void InteractOption6()
    {
        Debug.Log("PROCCED 6");
        this.npcInteractionController.DetermineOnClickAction("Trader", 6);
    }

}