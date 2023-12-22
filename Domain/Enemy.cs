using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Enemy : MonoBehaviour
{
    public int enemyID;
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

    [SerializeField]
    protected GameObject heldItem;
    protected int currentHealth;
    public float movementSpeed;
    public float attackSpeed;
    public bool isActive = false;

    public abstract void ActivateEnemy();

    public abstract void DeactivateEnemy();

    public abstract void TakeDamage(int damageTaken);

    public abstract void TryDealDamage();
    public void SetHeldItem(GameObject item)
    {
        this.heldItem = item;
    }

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

    protected void DropHeldItem()
    {
        if(this.heldItem != null)
        {
            Debug.Log("DROPPING ITEM: " + this.heldItem.name);
            GameObject obj = Instantiate(this.heldItem, gameObject.transform.parent);
            Debug.Log("INSTANTIATED OBJECT NAME: " + obj.name);
            obj.transform.position = this.gameObject.transform.position;
        }
    }

    protected void AcknowledgeDeath(GameObject enemyObject)
    {
        this.gameController.TryToOpenTeleports(enemyObject);
    }

    public void SetEnemyID(int enemyID)
    {
        Debug.Log("SETTING ENEMY ID");
        this.enemyID = enemyID;
        GenerationEntityIDController.currentEnemyID += 1;
    }
    public int GetEnemyID()
    {
        return this.enemyID;
    }

}