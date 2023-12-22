using System.Collections.Generic;

public static class ProgressHolder
{
    public static List<int> slainEnemyIDs = new List<int>();
    public static List<int> collectedCoinIDs = new List<int>();

    public static void SetProgressFromPreviousSave(SaveData saveData)
    {
        slainEnemyIDs = saveData.slainEnemyIDs;
        collectedCoinIDs = saveData.collectedCoinIDs;
    }
    public static void ResetProgressIDs()
    {
        slainEnemyIDs = new List<int>();
        collectedCoinIDs = new List<int>();
    }

}