
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstBossEnemy : Enemy
{

    public GameObject player;
    public Animator animator;
    
    public int enemyAttackDamage;
    [SerializeField]
    private LayerMask playerLayer;
    [SerializeField]
    private Canvas hpCanvas;
    private BoxCollider2D playerBoxCollider;
    private Dictionary<RelativeDirection, float> playerHitBoxBound;

    [SerializeField]
    private Transform attackPoint1;
    [SerializeField]
    private Transform attackPoint2;
    [SerializeField]
    private Transform attackPoint3;

    public float bossAttackRange;
    public float bossSpinAttackRange;
    public float bossWideAttackRange;

    private List<int> attackStreak = new List<int>();
    private readonly int maxAttackStreak = 3;

    private bool isAttacking = false;
    private bool breakStreak = false;

    void Start()
    {
        this.currentHealth = this.healthPoints;
        BoxCollider2D enemyCollider = gameObject.GetComponent<BoxCollider2D>();
        /*this.colliderWidth = enemyCollider.size.x;
        this.colliderHeight = enemyCollider.size.y;
        this.GetComponent<Animator>().SetFloat("attackSpeedMultiplier", this.attackSpeed);
        collisionSensors = new List<Vector2>()
        {
            new Vector2(0,0),
            new Vector2(this.colliderWidth/2, -this.colliderHeight/2),
            new Vector2(-this.colliderWidth/2, -this.colliderHeight/2),
            new Vector2(this.colliderWidth/2, this.colliderHeight/2),
            new Vector2(-this.colliderWidth/2, this.colliderHeight/2),
            new Vector2(this.colliderWidth/2, 0),
            new Vector2(-this.colliderWidth/2, 0),
            new Vector2(0, -this.colliderHeight/2),
            new Vector2(0, this.colliderHeight/2)
        };*/

        InitializeHitRangeExtension();
        /* Physics2D.IgnoreLayerCollision(6, 7); // 6 - enemies, 7 - player*/
    }

    private void FixedUpdate()
    {
        if (this.isActive)
        {
            float step = this.movementSpeed * Time.deltaTime;
            

            if(this.transform.position.x < player.transform.position.x)
            {
                this.transform.rotation = new Quaternion(this.transform.rotation.x,
                    0f, this.transform.rotation.z, this.transform.rotation.w);
                this.healthSlider.direction = Slider.Direction.LeftToRight;
            }
            else
            {
                this.transform.rotation = new Quaternion(this.transform.rotation.x,
                    180f, this.transform.rotation.z, this.transform.rotation.w);
                this.healthSlider.direction = Slider.Direction.RightToLeft;
            }

            Vector2 enemyPosition = new Vector2(this.transform.position.x, this.transform.position.y);
            Vector2 playerPosition = new Vector2(player.transform.position.x, player.transform.position.y);

            if (Vector2.Distance(enemyPosition, playerPosition) < bossWideAttackRange){
                TryDealDamage();
            }
            else
            {
                this.animator.SetInteger("AnimState", 1);
                this.animator.SetBool("Grounded", true);
                this.gameObject.transform.position = Vector3.MoveTowards(this.gameObject.transform.position, player.transform.position, step);
            }

        }
    }

    private void InitializeHitRangeExtension()
    {
        this.playerBoxCollider = this.player.GetComponent<BoxCollider2D>();
        this.playerHitBoxBound = new Dictionary<RelativeDirection, float>
        {
            { RelativeDirection.East, playerBoxCollider.size.x / 2 },
            { RelativeDirection.West, playerBoxCollider.size.x / 2 },
            { RelativeDirection.South, playerBoxCollider.size.y / 2 },
            { RelativeDirection.North, playerBoxCollider.size.y / 2 }
        };
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
/*        this.animator.SetTrigger("Hurt");
        this.isAttacking = false;
        this.breakStreak = false;*/
        if(this.currentHealth <= 0)
        {
            Die();
        }
        else if ((float)this.currentHealth / this.healthPoints < 0.3) this.healthSliderFill.color = Color.red;
        else if ((float)this.currentHealth / this.healthPoints < 0.6) this.healthSliderFill.color = Color.yellow;
        /*TryDealDamage();*/

        this.healthSlider.value = (float)this.currentHealth / this.healthPoints;
    }

    private void Die()
    {
        this.currentHealth = 0;
        this.animator.SetTrigger("Death");
        this.isActive = false;
        this.GetComponent<Collider2D>().enabled = false;
        this.hpCanvas.enabled = false;
        Invoke("AcknowledgeEnemyDeath", 1f);
        Invoke("OpenNextLevelTeleport", 1f);
        Destroy(gameObject, this.destroyBodyAfterSeconds);
    }

    private void OpenNextLevelTeleport()
    {
        this.gameController.ShowNextLevelTeleport();
    }

    private void AcknowledgeEnemyDeath()
    {
        base.AcknowledgeDeath(this.gameObject);
    }

    public override void TryDealDamage()
    {
        if (isAttacking)
            return;
        int randAttack = UnityEngine.Random.Range(0, 3);
        switch (randAttack){
            case 0:
                this.isAttacking = true;
                this.BasicAttack();
                break;
            case 1:
                this.isAttacking = true;
                this.SpinAttack();
                break;
            case 2:
                this.isAttacking = true;
                this.WideAttack();
                    break;
            default:
                break;
        }
        Debug.Log("TRYING DEAL DMG");
    }

    public void BasicAttack()
    {
        this.animator.SetTrigger("Attack1");
    }
    public void SpinAttack()
    {
        this.animator.SetTrigger("Attack2");
    }
    public void WideAttack()
    {
        this.animator.SetTrigger("Attack3");
    }

    public void BasicAttackHit()
    {
        Collider2D playerHit = Physics2D.OverlapCircle(attackPoint1.position, bossAttackRange, playerLayer);
        this.attackStreak.Add(0);
        if (playerHit != null)
        {
            playerHit.GetComponent<PlayerController>().TakeDamage(this.enemyAttackDamage);

            if(this.attackStreak.Count < this.maxAttackStreak)
            {
                int randValContinueStreak = UnityEngine.Random.Range(0, 2);
                if(randValContinueStreak == 0)
                {
                    if(this.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                        this.SpinAttack();
                }
                else
                {
                    this.attackStreak = new List<int>();
                    this.breakStreak = true;
                }
            }

        }
    }
    public void SpinAttackHit()
    {
        Collider2D playerHit = Physics2D.OverlapCircle(attackPoint2.position, bossSpinAttackRange, playerLayer);
        this.attackStreak.Add(1);
        if (playerHit != null)
        {
            playerHit.GetComponent<PlayerController>().TakeDamage(this.enemyAttackDamage);

            if (this.attackStreak.Count < this.maxAttackStreak)
            {
                int randValContinueStreak = UnityEngine.Random.Range(0, 2);
                if (randValContinueStreak == 0)
                {
                    if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                        this.WideAttack();
                }
                else
                {
                    this.attackStreak = new List<int>();
                    this.breakStreak = true;
                }
            }
        }
    }
    public void WideAttackHit()
    {
        Collider2D playerHit = Physics2D.OverlapCircle(attackPoint3.position, bossWideAttackRange, playerLayer);
        this.attackStreak.Add(2);
        if (playerHit != null)
        {
            playerHit.GetComponent<PlayerController>().TakeDamage(this.enemyAttackDamage);
            if (this.attackStreak.Count < this.maxAttackStreak)
            {
                int randValContinueStreak = UnityEngine.Random.Range(0, 2);
                if (randValContinueStreak == 0)
                {
                    if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                        this.BasicAttack();
                }
                else
                {
                    this.attackStreak = new List<int>();
                    this.breakStreak = true;
                }
            }
        }
    }

    public void ResetAttackingState()
    {
        Debug.Log("ATTACK STREAK COUNT: " + this.attackStreak.Count);
        Debug.Log("SHALL BREAK STREAK?: " + this.breakStreak);
        if(this.breakStreak == true)
        {
            Debug.Log("RESETING STATE");

            this.isAttacking = false;
        }
    }


    private void OnDrawGizmosSelected()
    {
        if (attackPoint1 == null)
            return;
        Gizmos.DrawWireSphere(attackPoint1.position, this.bossAttackRange);
        Debug.Log("aaa");
        if (attackPoint2 == null)
            return;

        Debug.Log("EEE");
        Gizmos.DrawWireSphere(attackPoint2.position, this.bossSpinAttackRange);

        if (attackPoint3 == null)
            return;
        Gizmos.DrawWireSphere(attackPoint3.position, this.bossWideAttackRange);
    }

}