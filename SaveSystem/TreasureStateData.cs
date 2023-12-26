using System.Collections.Generic;

[System.Serializable]
public class TreasureStateData
{
    public List<int> treasuresIds = new List<int>();
    public List<bool> treasureOpenStates = new List<bool>();
    public List<List<int>> droppedItemsIds = new List<List<int>>();
    public List<List<List<float>>> droppedItemsLocations = new List<List<List<float>>>();
    public TreasureStateData(List<int> treasuresIds, List<bool> treasureOpenStates, List<List<int>> droppedItemsIds
        ,List<List<List<float>>> droppedItemsLocations)
    {
        this.treasuresIds = treasuresIds;
        this.treasureOpenStates = treasureOpenStates;
        this.droppedItemsIds = droppedItemsIds;
        this.droppedItemsLocations = droppedItemsLocations;
    }
}