using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData {

    public UnityEngine.Random.State gameState;
    public List<int> slainEnemyIDs;
    public List<int> collectedCoinIDs;
    public PlayerStats playerStats;
    public AdditionalPlayerData additionalPlayerData;
    public EnemiesStateData enemiesStateData;
    public List<int> openedTreasuresSequence;
    public TreasureStateData treasureStateData;
    public List<int> remainingShopItemIds;
    public QuestStateData questStateData;
    public List<DroppedQuestItemStateData> droppedQuestItemData;
    public List<string> heldQuestItems;
    public SaveData(UnityEngine.Random.State gameState, List<int> collectedCoinIDs, List<int> slainEnemyIDs,
        PlayerStats playerStats, AdditionalPlayerData additionalPlayerData, EnemiesStateData enemiesStateData,
        List<int> openedTreasuresSequence, TreasureStateData treasureStateData, List<int> pickedUpShopItemIds,
        QuestStateData questStateData, List<DroppedQuestItemStateData> droppedQuestItemData, List<string> heldQuestItems)
    {
        this.gameState = gameState;
        this.collectedCoinIDs = collectedCoinIDs;
        this.slainEnemyIDs = slainEnemyIDs;
        this.playerStats = playerStats;
        this.additionalPlayerData = additionalPlayerData;
        this.enemiesStateData = enemiesStateData;
        this.openedTreasuresSequence = openedTreasuresSequence;
        this.treasureStateData = treasureStateData;
        this.remainingShopItemIds = pickedUpShopItemIds;
        this.questStateData = questStateData;
        this.droppedQuestItemData = droppedQuestItemData;
        this.heldQuestItems = heldQuestItems;
    }

}
