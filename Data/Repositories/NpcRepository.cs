using System.Collections.Generic;

public class NpcRepository
{
    public List<InteractiveDialogData> allNPCData = new List<InteractiveDialogData>();

    public NpcRepository(List<InteractiveDialogData> allNPCData) 
    {
        this.allNPCData = allNPCData;
    }

    public NpcRepository() { }

    public List<InteractiveDialogData> GetAllNpcDialogData()
    {
        return allNPCData;
    }

    public InteractiveDialogData GetNpcDialogDataByNpcType(string npcType)
    {
        foreach(InteractiveDialogData data in allNPCData)
        {
            if(data.npcType == npcType)
            {
                return data;
            }
        }
        return null;
    }

}