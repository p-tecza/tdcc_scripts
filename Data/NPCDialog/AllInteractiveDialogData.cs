using System.Collections.Generic;
[System.Serializable]
public class AllInteractiveDialogData
{
    public List<InteractiveDialogData> allNPCData;

    public AllInteractiveDialogData(List<InteractiveDialogData> allNPCData) 
    {
        this.allNPCData = allNPCData;
    }
    public AllInteractiveDialogData() { }
}