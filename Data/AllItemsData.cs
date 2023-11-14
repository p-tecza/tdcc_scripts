using System.Collections.Generic;
[System.Serializable]
public class AllItemsData
{
    public List<CollectableData> collectables;
    public List<ItemData> items;

    public AllItemsData(List<CollectableData> collectables, List<ItemData> items)
    {
        this.collectables = collectables;
        this.items = items;
    }

    public AllItemsData() { }

}