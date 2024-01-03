using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField]
    private PlayerController playerController;
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

    protected bool isReadFromSave = false;
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
            GameObject obj = Instantiate(this.heldItem, gameObject.transform.parent);
            obj.transform.position = this.gameObject.transform.position;
            QuestItem questItem = obj.GetComponent<QuestItem>();
            if(questItem != null)
            {
                questItem.idOfRoom = playerController.GetCurrentlyActivatedRoom().Id;
                playerController.AddDroppedItemToRoomInfo(obj);
            }

            
        }
    }

    protected void AcknowledgeDeath(GameObject enemyObject)
    {
        this.gameController.TryToOpenTeleports(enemyObject);
    }

    public void SetEnemyID(int enemyID)
    {
        this.enemyID = enemyID;
        GenerationEntityIDController.currentEnemyID += 1;
    }
    public int GetEnemyID()
    {
        return this.enemyID;
    }

    public void SetEnemyCurrentHP(int newHpVal)
    {
        Debug.Log("SETTING MY HP - ID: " + this.enemyID +"TO: "+newHpVal);
        this.currentHealth = newHpVal;
        SetSlider((float)this.currentHealth / this.healthPoints);
    }

    public void MarkEnemyAsReadFromSave()
    {
        this.isReadFromSave = true;
    }

    public int GetEnemyCurrentHP()
    {
        return this.currentHealth;
    }

    protected void RemoveRoomReference()
    {
        this.playerController.RemoveReferenceFromHeldEnemiesInRoom(this.gameObject);
    }

}