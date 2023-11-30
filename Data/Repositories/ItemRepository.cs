using System.Collections.Generic;

public class ItemRepository
{
    private List<ItemData> items = new List<ItemData>();
    private List<CollectableData> collectables = new List<CollectableData>();
    private List<QuestItemData> questItems = new List<QuestItemData>();
    public ItemRepository(List<ItemData> items, List<CollectableData> collectables, List<QuestItemData> questItems)
    {
        this.collectables = collectables;
        this.items = items;
        this.questItems = questItems;
    }
    public ItemRepository() { }

    public List<ItemData> GetAllItems() { return items; }
    public List<CollectableData> GetAllCollectables() {  return collectables; }
    public List<QuestItemData> GetAllQuestItems() {  return questItems; }
    public ItemData GetItemByName(string itemName)
    {
        foreach(ItemData item in items)
        {
            if(item.name == itemName)
            {
                return item;
            }
        }
        return null;
    }
    public CollectableData GetCollectableByName(string collectableName)
    {
        foreach(CollectableData collectable in collectables)
        {
            if(collectable.name == collectableName)
            {
                return collectable;
            }
        }
        return null;
    }
    public QuestItemData GetQuestItemByName(string questItemName)
    {
        foreach(QuestItemData questItem in questItems)
        {
            if(questItem.name == questItemName)
            {
                return questItem;
            }
        }
        return null;
    }

}