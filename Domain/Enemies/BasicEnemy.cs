using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using Unity.VisualScripting.FullSerializer.Internal;
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

    private float colliderWidth, colliderHeight;

    private BoxCollider2D playerBoxCollider;

    private Vector3 colliderCenter;

    private List<Vector2> collisionSensors;

    private RelativeDirection facingTowards;

    private Dictionary<RelativeDirection, float> playerHitBoxBound;


    private float obstacleAvoidToMove = 0;

    private Vector3 obstacleAvoidVector = new Vector3();

    private bool ignoreAvoidance = false;

    void Start()
    {
        this.currentHealth = this.healthPoints;
        BoxCollider2D enemyCollider = gameObject.GetComponent<BoxCollider2D>();
        this.colliderWidth = enemyCollider.size.x;
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
        };

        InitializeHitRangeExtension();
        /* Physics2D.IgnoreLayerCollision(6, 7); // 6 - enemies, 7 - player*/
    }

    void FixedUpdate()
    {

        this.colliderCenter = gameObject.GetComponent<BoxCollider2D>().bounds.center;

        float step = this.movementSpeed * Time.deltaTime;
        if (this.isActive)
        {

            this.animator.SetBool("isRunning", isRunning);
            this.animator.SetBool("isAttacking", isAttacking);
            /*            this.animator.SetBool("isDead", isDead);
                        this.animator.SetBool("isHurt", isHurt);*/

            if (this.obstacleAvoidToMove > 0 && !this.ignoreAvoidance)
            {
                // MASZ OBSTACLE AVOID VECTOR I OBSTACLE AVOID TO MOVE, na tym pracuj

                this.transform.position += this.obstacleAvoidVector;
                this.obstacleAvoidToMove -= this.obstacleAvoidVector.x == 0 ?
                    Math.Abs(this.obstacleAvoidVector.y) : Math.Abs(this.obstacleAvoidVector.x);
            }
            else if (Vector3.Distance(this.basicEnemyAttackPoint.position, player.transform.position)
                < this.basicEnemyAttackRange + this.playerHitBoxBound[this.facingTowards])
            {
                this.StartAttacking();
            }
            else if (this.transform.position.x > player.transform.position.x && CheckIfAnimationsEnded())
            {
                this.StartRunning();
                this.transform.rotation = new Quaternion(this.transform.rotation.x,
                    0f, this.transform.rotation.z, this.transform.rotation.w);



                Vector3 movementVector = DetermineMovementVector(this.transform.position, player.transform.position, step);
                /*Vector3 newPosition = Vector3.MoveTowards(this.transform.position, player.transform.position, step);*/
                Vector3 newPosition = this.transform.position + movementVector;

                (bool, Vector2Int) t1 = DetermineIfObstacleInTheWay(this.transform.position, player.transform.position, step);

                if (t1.Item1 && !this.ignoreAvoidance)
                {
                    this.obstacleAvoidVector = FindNewOrthogonalVector(movementVector);
                    Vector2Int tile = t1.Item2;
                    float toMove = 0;
                    if (this.obstacleAvoidVector.Equals(Vector3.zero))
                    {
                        this.obstacleAvoidToMove = 0;
                        this.ignoreAvoidance = true;
                    }
                    else if (this.obstacleAvoidVector.x > 0)
                    {
                        toMove = tile.x + 1 - this.colliderCenter.x + this.colliderWidth / 2;
                    }
                    else if (this.obstacleAvoidVector.x < 0)
                    {
                        toMove = tile.x - this.colliderCenter.x - this.colliderWidth / 2;
                    }
                    else if (this.obstacleAvoidVector.y > 0)
                    {
                        toMove = tile.y + 1 - this.colliderCenter.y + this.colliderHeight / 2;
                    }
                    else if (this.obstacleAvoidVector.y < 0)
                    {
                        toMove = tile.y - this.colliderCenter.y - this.colliderHeight / 2;
                    }

                    this.obstacleAvoidToMove = Math.Abs(toMove);

                }
                else
                {
                    this.transform.position = newPosition;
                    this.ignoreAvoidance = false;
                }


                this.healthSlider.direction = Slider.Direction.LeftToRight;
                this.facingTowards = RelativeDirection.West;

            }
            else if (CheckIfAnimationsEnded())
            {
                this.StartRunning();
                this.transform.rotation = new Quaternion(this.transform.rotation.x,
                    180f, this.transform.rotation.z, this.transform.rotation.w);
                Vector3 movementVector = DetermineMovementVector(this.transform.position, player.transform.position, step);
                /*Vector3 newPosition = Vector3.MoveTowards(this.transform.position, player.transform.position, step);*/
                Vector3 newPosition = this.transform.position + movementVector;

                (bool, Vector2Int) t1 = DetermineIfObstacleInTheWay(this.transform.position, player.transform.position, step);

                if (t1.Item1 && !this.ignoreAvoidance)
                {

                    this.obstacleAvoidVector = FindNewOrthogonalVector(movementVector);
                    Vector2Int tile = t1.Item2;
                    float toMove = 0;
                    if (this.obstacleAvoidVector.Equals(Vector3.zero))
                    {
                        this.obstacleAvoidToMove = 0;
                        this.ignoreAvoidance = true;
                    }
                    else if (this.obstacleAvoidVector.x > 0)
                    {
                        toMove = tile.x + 1 - this.colliderCenter.x + this.colliderWidth / 2;
                    }
                    else if (this.obstacleAvoidVector.x < 0)
                    {
                        toMove = tile.x - this.colliderCenter.x - this.colliderWidth / 2;
                    }
                    else if (this.obstacleAvoidVector.y > 0)
                    {
                        toMove = tile.y + 1 - this.colliderCenter.y + this.colliderHeight / 2;
                    }
                    else if (this.obstacleAvoidVector.y < 0)
                    {
                        toMove = tile.y - this.colliderCenter.y - this.colliderHeight / 2;
                    }

                    this.obstacleAvoidToMove = Math.Abs(toMove) + 0.2f;
                }
                else
                {
                    this.transform.position = newPosition;
                    this.ignoreAvoidance = false;
                }


                //by slider sie nie obracal
                this.healthSlider.direction = Slider.Direction.RightToLeft;
                this.facingTowards = RelativeDirection.East;
            }
        }
    }

    public override void ActivateEnemy()
    {
        this.isActive = true;

        //TODO cos zepsute jest z tymi tile'ami, ogarnij jak one sie rozkladaja
        List<Vector2Int> list = player.GetComponent<PlayerController>().GetCurrentlyActivatedRoom().WallTiles.ToList();


        /*CalculateVectorWithoutObstacles(DetermineMovementVector(this.transform.position,
            player.transform.position, this.movementSpeed * Time.deltaTime));*/
    }

    public override void DeactivateEnemy()
    {
        this.isActive = false;
    }

    public override void TakeDamage(int damageTaken)
    {
        this.currentHealth -= damageTaken;
        this.animator.SetTrigger("gotHurt");
        if (this.currentHealth <= 0)
        {
            Die();
        }
        else if ((float)this.currentHealth / this.healthPoints < 0.3) this.healthSliderFill.color = Color.red;
        else if ((float)this.currentHealth / this.healthPoints < 0.6) this.healthSliderFill.color = Color.yellow;

        this.healthSlider.value = (float)this.currentHealth / this.healthPoints;

    }

    private void Die()
    {
        this.currentHealth = 0;
        ResetAllStatesToFalse();
        this.animator.SetBool("isAttacking", isAttacking);
        this.animator.SetBool("isRunning", isRunning);
        this.isActive = false;
        this.GetComponent<Collider2D>().enabled = false;
        /*  this.GetComponent<Canvas>().enabled = false;*/
        this.animator.SetTrigger("gotKilled");
        this.gameController.enemiesTracker.EnemyDies();
        this.gameController.UpdateQuestProgress();
        Destroy(gameObject, this.destroyBodyAfterSeconds);
    }

    public override void TryDealDamage()
    {

        Collider2D playerHit = Physics2D.OverlapCircle(basicEnemyAttackPoint.position, basicEnemyAttackRange, playerLayer);

        if (playerHit != null)
        {
            playerHit.GetComponent<PlayerController>().TakeDamage(this.enemyAttackDamage);
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
        this.playerHitBoxBound = new Dictionary<RelativeDirection, float>
        {
            { RelativeDirection.East, playerBoxCollider.size.x / 2 },
            { RelativeDirection.West, playerBoxCollider.size.x / 2 },
            { RelativeDirection.South, playerBoxCollider.size.y / 2 },
            { RelativeDirection.North, playerBoxCollider.size.y / 2 }
        };
    }

    private (bool, Vector2Int) DetermineIfObstacleInTheWay(Vector3 enemyPosition, Vector3 playerPosition, float step)
    {
        Vector3 movementVector = DetermineMovementVector(enemyPosition, playerPosition, step);//

        List<Vector2Int> obstaclesInRoom = player.GetComponent<PlayerController>().GetCurrentlyActivatedRoom().WallTiles.ToList();

        return PathFinder.AreAnyObstaclesInAWay(obstaclesInRoom, this.colliderCenter, movementVector, this.collisionSensors);

    }

    private Vector3 FindNewOrthogonalVector(Vector3 movementVector)
    {
        List<Vector2Int> obstaclesInRoom = player.GetComponent<PlayerController>().GetCurrentlyActivatedRoom().WallTiles.ToList();
        Dictionary<string, Vector2Int> ways = new Dictionary<string, Vector2Int>()
        {
            { "x+", new Vector2Int(1, 0) },
            { "x-", new Vector2Int(-1, 0) },
            { "y+", new Vector2Int(0, 1) },
            { "y-", new Vector2Int(0, -1) }
        };

        float movementMagnitude = movementVector.magnitude;
        List<String> availablePaths = new List<string>(); 
        List<String> unavailablePaths = new List<string>();

        foreach (var way in ways)
        {
            Vector3 newWay = new Vector3(movementMagnitude * way.Value.x, movementMagnitude * way.Value.y, 0);
            if (!PathFinder.AreAnyObstaclesInAWay(obstaclesInRoom, this.colliderCenter, newWay, this.collisionSensors).Item1)
            {
                availablePaths.Add(way.Key);
            }
            else
            {
                unavailablePaths.Add(way.Key);
            }
        }


        foreach (string option in availablePaths)
        {
            Vector2 vector1 = new Vector2(movementMagnitude * ways[option].x, movementMagnitude * ways[option].y);
            if (option == "x-")
            {
                if (!unavailablePaths.Contains("x+"))
                {
                    Vector2 vector2 = new Vector2(movementMagnitude * ways["x+"].x, movementMagnitude * ways["x+"].y);
                    Vector3 optimalVec = PathFinder.DetermineOptimalVector(vector1, vector2, movementVector);
                    return optimalVec;
                }
            }
            else if (option == "x+")
            {
                if (!unavailablePaths.Contains("x-"))
                {
                    Vector2 vector2 = new Vector2(movementMagnitude * ways["x-"].x, movementMagnitude * ways["x-"].y);
                    Vector3 optimalVec = PathFinder.DetermineOptimalVector(vector1, vector2, movementVector);
                    return optimalVec;
                }
            }
            else if (option == "y-")
            {
                if (!unavailablePaths.Contains("y+"))
                {
                    Vector2 vector2 = new Vector2(movementMagnitude * ways["y+"].x, movementMagnitude * ways["y+"].y);
                    Vector3 optimalVec = PathFinder.DetermineOptimalVector(vector1, vector2, movementVector);
                    return optimalVec;
                }
            }
            else if (option == "y+")
            {
                if (!unavailablePaths.Contains("y-"))
                {
                    Vector2 vector2 = new Vector2(movementMagnitude * ways["y-"].x, movementMagnitude * ways["y-"].y);
                    Vector3 optimalVec = PathFinder.DetermineOptimalVector(vector1, vector2, movementVector);
                    return optimalVec;
                }
            }
        }

        if(availablePaths.Count == 0)
        {
            return movementVector;
        }
        else
        {
            double minAngle = 360;
            Vector3 retVec = Vector3.zero;
            foreach (var path in availablePaths)
            {
                Vector2 vector = new Vector2(movementMagnitude * ways[path].x, movementMagnitude * ways[path].y);
                double angle = PathFinder.AngleBetween(vector, new Vector2(movementVector.x, movementVector.y));

                if(minAngle > angle) 
                { 
                    minAngle = angle;
                    retVec = new Vector3(vector.x, vector.y, 0);
                }
            }
            return retVec;
        }
    }
    private Vector3 DetermineMovementVector(Vector3 enemyPosition, Vector3 playerPosition, float step)
    {
        Vector3 movementVector = Vector3.MoveTowards(enemyPosition, playerPosition, step * 2) - enemyPosition;
        return movementVector;
    }

}
