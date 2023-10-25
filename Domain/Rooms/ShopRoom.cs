using System.Collections.Generic;
using UnityEngine;

public class ShopRoom
{
    private static int amountOfSpecificItems = 1;
    private static int amountOfCollectables = 2;
    public static Dictionary<string, int> costs = new Dictionary<string, int>()
    {
        { "hpPotion", 5 },
        { "star", 10 },
        { "item", 20 }
    };
    public Room room;
    public NPC npc;
    public List<int> itemIds;
    public List<Collectable> collectables;

    public ShopRoom(Room room)
    {
        this.room = room;
        switch (NPC.npcTypes[NPC.DetermineRandomNPC()])
        {
            case "Trainer":
                npc = new TrainerNPC();
                break;
            case "Trader":
                //todo
                break;
            case "Wanderer":
                //todo
                break;
            case "Astray":
                //todo
                break;
            default: break;
        }
        RandomlyPickLootItems();
        this.collectables = new List<Collectable>()
        {
            new HpPotion(),
            new Star()
        };
    }

    private void RandomlyPickLootItems()
    {
        List<int> itemIds = GameController.GetAvailableSpecificItemLoot();
        int itemsToRoll = amountOfSpecificItems;
        if (itemsToRoll > itemIds.Count) itemsToRoll = itemIds.Count;

        for(int i = 0; i < itemsToRoll; i++)
        {
            int chosenItem = itemIds[UnityEngine.Random.Range(0, itemIds.Count)];
            this.itemIds.Add(chosenItem);
            GameController.RemoveItemFromAvailableSpecificItemLoot(chosenItem);
            itemIds.Remove(chosenItem);
        }
    }
}