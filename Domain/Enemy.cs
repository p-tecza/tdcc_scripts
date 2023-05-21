using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    protected GameObject enemyObject { get; set; }
    protected int healthPoints { get; set; }
    protected float movementSpeed { get; set; }
    protected float attackSpeed { get; set; }
    protected bool isActive = true;
    public abstract void ActivateEnemy();

    public abstract void DeactivateEnemy();
}