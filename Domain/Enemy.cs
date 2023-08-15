using UnityEngine;
using UnityEngine.UI;

public abstract class Enemy : MonoBehaviour
{
    protected GameObject enemyObject { get; set; }
    [SerializeField]
    protected Slider healthSlider;
    [SerializeField]
    protected Image healthSliderFill;
    [SerializeField]
    protected int healthPoints;
    [SerializeField]
    [Range(0f, 10f)]
    protected float destroyBodyAfterSeconds;
    protected int currentHealth;
    protected float movementSpeed = 1f;
    protected float attackSpeed { get; set; }
    public bool isActive = false;
    public abstract void ActivateEnemy();

    public abstract void DeactivateEnemy();

    public abstract void TakeDamage(int damageTaken);

    public abstract void TryDealDamage();
}