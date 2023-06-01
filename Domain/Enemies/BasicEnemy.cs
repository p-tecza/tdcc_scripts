using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BasicEnemy : Enemy
{
    public GameObject player;
    public Animator animator;


    private bool isRunning;
    private bool isAttacking;
    private bool isHurt;
    private bool isDead;


    void Start()
    {
        /*this.animator.SetBool("isRunning", true);*/
        this.currentHealth = this.healthPoints;
    }

    void FixedUpdate()
    {
        float step = this.movementSpeed * Time.deltaTime;
        if(this.isActive){

            this.animator.SetBool("isRunning", isRunning);
            this.animator.SetBool("isAttacking", isAttacking);
            this.animator.SetBool("isDead", isDead);
            this.animator.SetBool("isHurt", isHurt);

            if (Vector3.Distance(this.transform.position, player.transform.position) < 1.5f )
            {
                //Debug.Log("JEST W ZASIEGU ATAKU");
                this.StartAttacking();
            }
            else if(this.transform.position.x > player.transform.position.x && CheckIfAnimationsEnded())
            {
            
                this.StartRunning();
                this.transform.rotation = new Quaternion(this.transform.rotation.x,
                    0f, this.transform.rotation.z,this.transform.rotation.w);
                this.transform.position = Vector3.MoveTowards(this.transform.position, player.transform.position, step);
            }
            else if(CheckIfAnimationsEnded())
            {
                this.StartRunning();
                this.transform.rotation = new Quaternion(this.transform.rotation.x,
                    180f, this.transform.rotation.z, this.transform.rotation.w);
                this.transform.position = Vector3.MoveTowards(this.transform.position, player.transform.position, step);
            }

            
        }
    }
    public override void ActivateEnemy()
    {
        this.isActive = true;
    }

    public override void DeactivateEnemy()
    {
        this.isActive = false;
    }

    public override void TakeDamage(int damageTaken)
    {
        this.currentHealth -= damageTaken;
        Debug.Log("Enemy dostal obrazenia, current hp: " + this.currentHealth);
        if(this.currentHealth < 0)
        {
            Debug.Log("Enemy UMIERA.");
        }
    }

    private void StartRunning()
    {
        ResetAllStatesToFalse();
        this.isRunning = true;
    }

    private void StartAttacking()
    {
        ResetAllStatesToFalse();
        this.isAttacking = true;
    }

    private void StartDying()
    {
        ResetAllStatesToFalse();
        this.isDead = true;
    }

    private void StartGettingHurt()
    {
        ResetAllStatesToFalse();
        this.isHurt = true;
    }

    private void ResetAllStatesToFalse()
    {
        this.isAttacking = false;
        this.isHurt = false;
        this.isDead = false;
        this.isRunning = false;
    }


    private bool CheckIfAnimationsEnded()
    {
        return !(this.animator.GetCurrentAnimatorStateInfo(0).IsName("LightBandit_Attack")
           || this.animator.GetCurrentAnimatorStateInfo(0).IsName("LightBandit_Hurt"));
    }

}
