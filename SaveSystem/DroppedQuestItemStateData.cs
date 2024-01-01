using System.Collections.Generic;

[System.Serializable]
public class DroppedQuestItemStateData
{
    public string itemName;
    public int roomId;
    public List<float> location;
    public DroppedQuestItemStateData(string itemName, int roomId, List<float> location)
    {
        this.itemName = itemName;
        this.roomId = roomId;
        this.location = location;
    }
}