public class EnemiesTracker
{
    private int startEnemiesAmount=0;
    private int aliveEnemiesAmount=0;
    private int deadEnemiesAmount=0;
    private bool isBossDead = false;


    public void EnemyDies()
    {
        this.aliveEnemiesAmount--;
        this.deadEnemiesAmount++;
    }
    public int GetDeadEnemiesAmount()
    {
        return this.deadEnemiesAmount;
    }
    public int GetAliveEnemiesAmount()
    {
        return this.aliveEnemiesAmount;
    }
    public int GetStartEnemiesAmount()
    {
        return this.startEnemiesAmount;
    }
    public bool CheckIfBossIsDead()
    {
        return this.isBossDead;
    }
    public void SetDeadEnemiesAmount(int deadAmount)
    {
        this.deadEnemiesAmount = deadAmount;
    }
    public void SetAliveEnemiesAmount(int aliveAmount)
    {
        this.aliveEnemiesAmount = aliveAmount;
    }
    public void SetStartEnemiesAmount(int startAmount)
    {
        this.startEnemiesAmount = startAmount;
    }

    public void SetBossState(bool isDead)
    {
        this.isBossDead = isDead;
    }

}