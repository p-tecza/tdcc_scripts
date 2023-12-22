using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData {

    public UnityEngine.Random.State gameState;
    public List<int> slainEnemyIDs;
    public List<int> collectedCoinIDs;

    public SaveData(UnityEngine.Random.State gameState, List<int> collectedCoinIDs, List<int> slainEnemyIDs)
    {
        this.gameState = gameState;
        this.collectedCoinIDs = collectedCoinIDs;
        this.slainEnemyIDs = slainEnemyIDs;

        Debug.Log("SIZE OF COLLECTED COINS: " + this.collectedCoinIDs.Count);
        Debug.Log("SIZE OF SLAIN ENEMIES: " + this.slainEnemyIDs.Count);

    }

}
