[System.Serializable]
public class QuestStateData
{
    public bool isQuestCompleted;
    public bool isQuestPickedUp;
    public QuestData questData;
    public int currentQuestProgress;

    public QuestStateData(bool isQuestCompleted, bool isQuestPickedUp, QuestData questData, int currentQuestProgress)
    {
        this.isQuestCompleted = isQuestCompleted;
        this.isQuestPickedUp = isQuestPickedUp;
        this.questData = questData;
        this.currentQuestProgress = currentQuestProgress;
    }
}