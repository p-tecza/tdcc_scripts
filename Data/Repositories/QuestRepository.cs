using System.Collections.Generic;

public class QuestRepository
{
    private List<QuestData> allQuestsData = new List<QuestData>();
    public QuestRepository(List<QuestData> allQuestsData)
    {
        this.allQuestsData = allQuestsData;
    }
    public QuestRepository() { }
    
    public QuestData GetQuestById(int id)
    {
        return this.allQuestsData[id];
    }

    public List<QuestData> GetQuestsByNpc(string npcType)
    {
        List<QuestData> npcQuests = new List<QuestData>();
        foreach(QuestData q in this.allQuestsData)
        {
            if(q.givenBy == npcType)
            {
                npcQuests.Add(q);
            }
        }
        return npcQuests;
    }

    public List<QuestData> GetAllQuests()
    {
        return allQuestsData;
    }

}