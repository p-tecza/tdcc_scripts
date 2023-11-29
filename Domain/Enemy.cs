using System.Collections.Generic;
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
    [SerializeField]
    protected GameController gameController;
    protected int currentHealth;
    public float movementSpeed;
    public float attackSpeed;
    public bool isActive = false;

    public abstract void ActivateEnemy();

    public abstract void DeactivateEnemy();

    public abstract void TakeDamage(int damageTaken);

    public abstract void TryDealDamage();

    public void SetSlider(float sliderValue)
    {
        this.healthSlider.value = sliderValue;
        if(sliderValue < 0.3) this.healthSliderFill.color = UnityEngine.Color.red;
        else if (sliderValue < 0.6) this.healthSliderFill.color = UnityEngine.Color.yellow;
        else this.healthSliderFill.color = new Color(0, 0.6f, 0);
    }

    protected void UpdateQuestProgress()
    {
        this.gameController.UpdateQuestProgress();
    }

}