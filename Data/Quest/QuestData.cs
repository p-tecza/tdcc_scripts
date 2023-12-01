[System.Serializable]
public class QuestData
{
    public int questId;
    public string questName;
    public string questDialog;
    public string questDescription;
    public string questNotCompletedYetDialog;
    public string questCompletedDialog;
    public string questSolvedDialog;
    public string questGoal;
    public string questReward;
    public int questRewardAmount;
    public string givenBy;

    public QuestData(int questId, string questName, string questDescription, string questNotCompletedYetDialog, string questCompletedDialog, string questSolvedDialog,
        string questGoal, string questReward, string givenBy, int questRewardAmount, string questDialog)
    {
        this.questId = questId;
        this.questName = questName;
        this.questDescription = questDescription;
        this.questNotCompletedYetDialog = questNotCompletedYetDialog;
        this.questCompletedDialog = questCompletedDialog;
        this.questSolvedDialog = questSolvedDialog;
        this.questGoal = questGoal;
        this.questReward = questReward;
        this.givenBy = givenBy;
        this.questRewardAmount = questRewardAmount;
        this.questDialog = questDialog;
    }
    public QuestData() { }
}