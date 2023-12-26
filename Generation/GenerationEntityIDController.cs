

public static class GenerationEntityIDController
{
    public static int currentEnemyID;
    public static int currentCoinID;
    public static int treasureID;
    public static int pickUpEntityID;
    public static void ResetAllIDs()
    {
        currentEnemyID = 0;
        currentCoinID = 0;
        treasureID = 0;
        pickUpEntityID = 0;
    }
}