using System.Collections.Generic;
using UnityEngine;

public class ShopRoom
{
    private static int amountOfSpecificItems = 1;
    private static int amountOfCollectables = 2; // 1 star + 1 hpPotion
    public static Dictionary<string, int> costs = new Dictionary<string, int>()
    {
        { "hpPotion", 5 },
        { "star", 10 },
        { "item", 20 }
    };
    public Room room;
    public string npcType;
    public int itemId = 0;
    public List<string> collectables = new List<string>();

    public ShopRoom(Room room)
    {
        this.room = room;
        npcType = NPC.npcTypes[NPC.DetermineRandomNPC()];
        RandomlyPickLootItems();
        this.collectables = new List<string>()
        {
            "hpPotion",
            "star"
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
            this.itemId = chosenItem;
            GameController.RemoveItemFromAvailableSpecificItemLoot(chosenItem);
            itemIds.Remove(chosenItem);
        }
    }
}