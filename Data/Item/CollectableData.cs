[System.Serializable]
public class CollectableData
{
    public string name;
    public string description;
    public string impactType;
    public int impactValue;

    public CollectableData(string name, string description, string impactType, int impactValue)
    {
        this.name = name;
        this.description = description;
        this.impactType = impactType;
        this.impactValue = impactValue;
    }
    public CollectableData() { }
}