using System.Collections.Generic;
[System.Serializable]
public class AllQuestsData
{
    public List<QuestData> allQuestsData;

    public AllQuestsData(List<QuestData> allQuestsData)
    {
        this.allQuestsData = allQuestsData;
    }
    public AllQuestsData() { }
}