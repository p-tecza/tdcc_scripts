[System.Serializable]
public class ItemData
{
    public string name;
    public string description;

    public ItemData(string name, string description)
    {
        this.name = name;
        this.description = description;
    }
    public ItemData() { }
}