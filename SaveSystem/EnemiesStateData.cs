using System.Collections.Generic;

[System.Serializable]
public class EnemiesStateData
{
    public List<int> enemiesIds;
    public List<int> enemiesHps;
    public List<List<float>> enemiesLocations;
    public int startEnemiesAmount;
    public int aliveEnemiesAmount;
    public int deadEnemiesAmount;
    public bool isBossDead;

    public EnemiesStateData(List<int> enemiesIds, List<int> enemiesHps, List<List<float>> enemiesLocations,
        int startEnemiesAmount, int aliveEnemiesAmount, int deadEnemiesAmount, bool isBossDead)
    {
        this.enemiesLocations = enemiesLocations;
        this.enemiesHps = enemiesHps;
        this.enemiesIds = enemiesIds;
        this.startEnemiesAmount = startEnemiesAmount;
        this.aliveEnemiesAmount = aliveEnemiesAmount;
        this.deadEnemiesAmount = deadEnemiesAmount;
        this.isBossDead = isBossDead;
    }
}