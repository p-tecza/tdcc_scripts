[System.Serializable]
public class QuestStateData
{
    public bool isQuestCompleted;
    public bool isQuestPickedUp;
    public QuestData questData;
    public int currentQuestProgress;
    public int questProgressThreshold;

    public QuestStateData(bool isQuestCompleted, bool isQuestPickedUp, QuestData questData, int currentQuestProgress, int questProgressThreshold)
    {
        this.isQuestCompleted = isQuestCompleted;
        this.isQuestPickedUp = isQuestPickedUp;
        this.questData = questData;
        this.currentQuestProgress = currentQuestProgress;
        this.questProgressThreshold = questProgressThreshold;
    }
}