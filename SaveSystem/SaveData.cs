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
    public SaveData(UnityEngine.Random.State gameState, List<int> collectedCoinIDs, List<int> slainEnemyIDs,
        PlayerStats playerStats, AdditionalPlayerData additionalPlayerData, EnemiesStateData enemiesStateData,
        List<int> openedTreasuresSequence, TreasureStateData treasureStateData)
    {
        this.gameState = gameState;
        this.collectedCoinIDs = collectedCoinIDs;
        this.slainEnemyIDs = slainEnemyIDs;
        this.playerStats = playerStats;
        this.additionalPlayerData = additionalPlayerData;
        this.enemiesStateData = enemiesStateData;
        this.openedTreasuresSequence = openedTreasuresSequence;
        this.treasureStateData = treasureStateData;
        Debug.Log("SIZE OF COLLECTED COINS: " + this.collectedCoinIDs.Count);
        Debug.Log("SIZE OF SLAIN ENEMIES: " + this.slainEnemyIDs.Count);
    }

}
