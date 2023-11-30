using System.Collections.Generic;
[System.Serializable]
public class AllItemsData
{
    public List<CollectableData> collectables;
    public List<ItemData> items;
    public List<QuestItemData> questItems;

    public AllItemsData(List<CollectableData> collectables, List<ItemData> items, List<QuestItemData> questItems)
    {
        this.collectables = collectables;
        this.items = items;
        this.questItems = questItems;
    }

    public AllItemsData() { }

}