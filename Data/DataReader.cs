using System.Collections.Generic;
using UnityEngine;

public class DataReader
{
    public static AllItemsData ReadAllItemsData()
    {
        string jsonFromFile = System.IO.File.ReadAllText("Assets\\_Scripts\\Data\\all-items-data.json");
        AllItemsData allItemsData = JsonUtility.FromJson<AllItemsData>(jsonFromFile);
        return allItemsData;
    }

    public static AllInteractiveDialogData ReadAllNPCDialogData()
    {
        string jsonFromFile = System.IO.File.ReadAllText("Assets\\_Scripts\\Data\\all-npc-data.json");
        AllInteractiveDialogData allNPCData = JsonUtility.FromJson<AllInteractiveDialogData>(jsonFromFile);
        return allNPCData;
    }

    public static AllQuestsData ReadAllQuestsData()
    {
        string jsonFromFile = System.IO.File.ReadAllText("Assets\\_Scripts\\Data\\all-quests-data.json");
        AllQuestsData allQuestsData = JsonUtility.FromJson<AllQuestsData>(jsonFromFile);
        return allQuestsData;
    }

}