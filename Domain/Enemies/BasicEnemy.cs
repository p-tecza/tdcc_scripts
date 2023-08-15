using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BasicEnemy : Enemy
{
    public GameObject player;
    public Animator animator;
    public Transform basicEnemyAttackPoint;
    public float basicEnemyAttackRange;
    public int enemyAttackDamage;
    [SerializeField]
    private LayerMask playerLayer;

    private bool isRunning;
    private bool isAttacking;

    private BoxCollider2D playerBoxCollider;

    private FacingTowards facingTowards;

    private Dictionary<FacingTowards, float> playerHitBoxBound;

    void Start()
    {
        this.currentHealth = this.healthPoints;
        InitializeHitRangeExtension();
    }

    void FixedUpdate()
    {
        float step = this.movementSpeed * Time.deltaTime;
        if (this.isActive)
        {

            this.animator.SetBool("isRunning", isRunning);
            this.animator.SetBool("isAttacking", isAttacking);
/*            this.animator.SetBool("isDead", isDead);
            this.animator.SetBool("isHurt", isHurt);*/




            if (Vector3.Distance(this.basicEnemyAttackPoint.position, player.transform.position) 
                < this.basicEnemyAttackRange + this.playerHitBoxBound[this.facingTowards])
            {
                Debug.Log(Vector3.Distance(this.basicEnemyAttackPoint.position, player.transform.position));
                this.StartAttacking();
            }
            else if (this.transform.position.x > player.transform.position.x && CheckIfAnimationsEnded())
            {
                this.StartRunning();
                this.transform.rotation = new Quaternion(this.transform.rotation.x,
                    0f, this.transform.rotation.z, this.transform.rotation.w);
                this.transform.position = Vector3.MoveTowards(this.transform.position, player.transform.position, step);
                this.healthSlider.direction = Slider.Direction.LeftToRight;
                this.facingTowards = FacingTowards.West;

            }
            else if (CheckIfAnimationsEnded())
            {
                this.StartRunning();
                this.transform.rotation = new Quaternion(this.transform.rotation.x,
                    180f, this.transform.rotation.z, this.transform.rotation.w);
                this.transform.position = Vector3.MoveTowards(this.transform.position, player.transform.position, step);
                //by slider sie nie obracal
                this.healthSlider.direction = Slider.Direction.RightToLeft;
                this.facingTowards = FacingTowards.East;
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
        this.animator.SetTrigger("gotHurt");

        Debug.Log("Enemy dostal obrazenia, current hp: " + this.currentHealth);
        if (this.currentHealth <= 0)
        {
            this.currentHealth = 0;
            Debug.Log("Enemy UMIERA.");
            ResetAllStatesToFalse();
            this.animator.SetBool("isAttacking", isAttacking);
            this.animator.SetBool("isRunning", isRunning);
            this.isActive = false;
            this.GetComponent<Collider2D>().enabled = false;
          /*  this.GetComponent<Canvas>().enabled = false;*/
            this.animator.SetTrigger("gotKilled");
            Destroy(gameObject, this.destroyBodyAfterSeconds);
        }
        else if ((float)this.currentHealth / this.healthPoints < 0.3) this.healthSliderFill.color = Color.red;
        else if ((float)this.currentHealth / this.healthPoints < 0.6) this.healthSliderFill.color = Color.yellow;

        this.healthSlider.value = (float)this.currentHealth / this.healthPoints;

    }

    public override void TryDealDamage()
    {

        Collider2D playerHit = Physics2D.OverlapCircle(basicEnemyAttackPoint.position, basicEnemyAttackRange, playerLayer);

        if(playerHit != null)
        {
            playerHit.GetComponent<PlayerController>().TakeDamage(this.enemyAttackDamage);
            Debug.Log("PLAYER DOSTAL");
        }

        /*throw new System.NotImplementedException();*/
    }

    /*    IEnumerator CleanUpBody()
        {
            Debug.Log("USUWA SIE?");
            yield return new WaitForSeconds(this.destroyBodyAfterSeconds);
            Debug.Log("USUWA SIE PO WAIT FOR SECONDS");
            Destroy(this);
        }*/

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

    private void ResetAllStatesToFalse()
    {
        this.isAttacking = false;
        this.isRunning = false;
    }


    private bool CheckIfAnimationsEnded()
    {
        return !(this.animator.GetCurrentAnimatorStateInfo(0).IsName("LightBandit_Attack")
           || this.animator.GetCurrentAnimatorStateInfo(0).IsName("LightBandit_Hurt"));
    }

    private void InitializeHitRangeExtension()
    {
        this.playerBoxCollider = this.player.GetComponent<BoxCollider2D>();
        this.playerHitBoxBound = new Dictionary<FacingTowards, float>
        {
            { FacingTowards.East, playerBoxCollider.size.x / 2 },
            { FacingTowards.West, playerBoxCollider.size.x / 2 },
            { FacingTowards.South, playerBoxCollider.size.y / 2 },
            { FacingTowards.North, playerBoxCollider.size.y / 2 }
        };
    }

}
