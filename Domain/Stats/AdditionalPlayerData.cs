using System.Collections.Generic;

[System.Serializable]
public class AdditionalPlayerData
{
    public int currentHp;
    public int maxHp;
    public int coinsAmount;
    public int collectedHpPotions;
    public int collectedStars;
    public List<float> playerLocation;
    public int currentActiveRoom;
    public AdditionalPlayerData(int currentHp, int maxHp, int coinsAmount, int collectedHpPotions, int collectedStars,
        List<float> playerLocation, int currentActiveRoom)
    {
        this.currentHp = currentHp;
        this.maxHp = maxHp;
        this.coinsAmount = coinsAmount;
        this.collectedHpPotions = collectedHpPotions;
        this.collectedStars = collectedStars;
        this.playerLocation = playerLocation;
        this.currentActiveRoom = currentActiveRoom;
    }
}