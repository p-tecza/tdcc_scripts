using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    protected GameObject enemyObject { get; set; }
    protected int healthPoints { get; set; }
    protected float movementSpeed = 1f;
    protected float attackSpeed { get; set; }
    public bool isActive = false;
    public abstract void ActivateEnemy();

    public abstract void DeactivateEnemy();
}