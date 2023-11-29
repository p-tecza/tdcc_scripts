using System.Collections.Generic;

public class ItemRepository
{
    private List<ItemData> items = new List<ItemData>();
    private List<CollectableData> collectables = new List<CollectableData>();
    public ItemRepository(List<ItemData> items, List<CollectableData> collectables)
    {
        this.collectables = collectables;
        this.items = items;
    }
    public ItemRepository() { }

    public List<ItemData> GetAllItems() { return items; }
    public List<CollectableData> GetAllCollectables() {  return collectables; }
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

}