using System.Collections.Generic;
[System.Serializable]
public class InteractiveDialogData
{
    public string npcType;
    public List<string> consecutiveDialogs;
    public List<string> consecutiveOptions;

    public InteractiveDialogData(string npcType, List<string> consecutiveDialogs, List<string> consecutiveOptions)
    {
        this.npcType = npcType;
        this.consecutiveDialogs = consecutiveDialogs;
        this.consecutiveOptions = consecutiveOptions;
    }
    public InteractiveDialogData() { }

}