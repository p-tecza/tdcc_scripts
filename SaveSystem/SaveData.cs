using UnityEngine;

[System.Serializable]
public class SaveData {
    public UnityEngine.Random.State gameState;

    public SaveData(UnityEngine.Random.State gameState)
    {
        this.gameState = gameState;
    }

}
