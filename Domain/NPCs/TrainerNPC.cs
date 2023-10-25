

using UnityEngine;

public class TrainerNPC : NPC
{
    public int trainingCost;
    public int costRaise;

    public override void Interact()
    {
        /*throw new System.NotImplementedException();*/
        Debug.Log("Interacted with TrainerNPC");
    }

}