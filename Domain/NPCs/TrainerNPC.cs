

using UnityEngine;

public class TrainerNPC : NPC
{
    public int trainingCost;
    public int costRaiseMultiplier;

    public override void Interact()
    {
        /*throw new System.NotImplementedException();*/
        Debug.Log("Interacted with TrainerNPC");
    }

    public override void InteractOption1()
    {
        Debug.Log("PROCCED TRAINER 1");
        this.npcInteractionController.DetermineOnClickAction("Trainer", 1);
    }
    public override void InteractOption2()
    {
        Debug.Log("PROCCED TRAINER 2");
        this.npcInteractionController.DetermineOnClickAction("Trainer", 2);
    }
    public override void InteractOption3()
    {
        Debug.Log("PROCCED 3");
        this.npcInteractionController.DetermineOnClickAction("Trainer", 3);
    }
    public override void InteractOption4()
    {
        Debug.Log("PROCCED 4");
        this.npcInteractionController.DetermineOnClickAction("Trainer", 4);
    }
    public override void InteractOption5()
    {
        Debug.Log("PROCCED 5");
        this.npcInteractionController.DetermineOnClickAction("Trainer", 5);
    }
    public override void InteractOption6()
    {
        Debug.Log("PROCCED 6");
        this.npcInteractionController.DetermineOnClickAction("Trainer", 6);
    }

}