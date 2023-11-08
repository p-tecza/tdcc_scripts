using UnityEngine;

public class DataReader
{
    public static AllItemsData ReadAllItemsData()
    {
        string jsonFromFile = System.IO.File.ReadAllText("Assets\\_Scripts\\Data\\all-items-data.json");
        AllItemsData allItemsData = JsonUtility.FromJson<AllItemsData>(jsonFromFile);
        return allItemsData;
    }
}