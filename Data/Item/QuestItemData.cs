[System.Serializable]
public class QuestItemData
{
    public string name;
    public string description;
    public QuestItemData(string name, string description)
    {
        this.name = name;
        this.description = description;
    }
    public QuestItemData() { }
}