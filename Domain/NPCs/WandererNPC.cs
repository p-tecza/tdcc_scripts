using System.Collections.Generic;
using UnityEngine;

public class WandererNPC : NPC
{
    public QuestData questData;

    public override void Start()
    {
        if (!gameObject.name.Contains("(Clone)"))
        {
            return;
        }
        base.Start();
        List<QuestData> list = new List<QuestData>();
        list.AddRange(this.gameController.questRepository.GetQuestsByNpc("Wanderer"));
        if(list.Count > 0)
        {
            questData = list[UnityEngine.Random.Range(0, list.Count)];
        }
    }

    public override void Interact()
    {
        /*throw new System.NotImplementedException();*/
        Debug.Log("Interacted with WandererNPC");
    }
    public override void InteractOption1()
    {
        Debug.Log("PROCCED 1");
        this.npcInteractionController.DetermineOnClickAction("Wanderer", 1);
    }
    public override void InteractOption2()
    {
        Debug.Log("PROCCED 2");
        this.npcInteractionController.DetermineOnClickAction("Wanderer", 2);
    }
    public override void InteractOption3()
    {
        Debug.Log("PROCCED 3");
        this.npcInteractionController.DetermineOnClickAction("Wanderer", 3);
    }
    public override void InteractOption4()
    {
        Debug.Log("PROCCED 4");
        this.npcInteractionController.DetermineOnClickAction("Wanderer", 4);
    }
    public override void InteractOption5()
    {
        Debug.Log("PROCCED 5");
        this.npcInteractionController.DetermineOnClickAction("Wanderer", 5);
    }
    public override void InteractOption6()
    {
        Debug.Log("PROCCED 6");
        this.npcInteractionController.DetermineOnClickAction("Wanderer", 6);
    }
}