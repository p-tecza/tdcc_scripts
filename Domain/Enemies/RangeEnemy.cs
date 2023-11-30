using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RangeEnemy : Enemy
{
    public GameObject player;
    public GameObject projectile;

    public AttackType attackType;
    public float enemyAttackRange;
    public float enemyProjectileSpeed;
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

    private float timer = 0f;

    void Start()
    {
        this.currentHealth = this.healthPoints;
        CircleCollider2D enemyCollider = gameObject.GetComponent<CircleCollider2D>();
        this.colliderWidth = enemyCollider.radius*2;
        this.colliderHeight = enemyCollider.radius*2;
        /*this.GetComponent<Animator>().SetFloat("attackSpeedMultiplier", this.attackSpeed);*/


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
    }

    void FixedUpdate()
    {

        this.colliderCenter = gameObject.GetComponent<CircleCollider2D>().bounds.center;

        if(this.timer < attackSpeed)
        {
            this.timer += Time.deltaTime;
        }

        float step = this.movementSpeed * Time.deltaTime;
        if (this.isActive)
        {
            if (this.obstacleAvoidToMove > 0 && !this.ignoreAvoidance)
            {
                this.transform.position += this.obstacleAvoidVector;
                this.obstacleAvoidToMove -= this.obstacleAvoidVector.x == 0 ?
                    Math.Abs(this.obstacleAvoidVector.y) : Math.Abs(this.obstacleAvoidVector.x);
            }
            else if (DetermineIfInShootingRange())
            {
                if(timer >= attackSpeed)
                {
                    this.RangeAttack();
                    this.timer = 0f;
                }
                
            }
            else if (this.transform.position.x > player.transform.position.x)
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
            else
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
    }

    public override void DeactivateEnemy()
    {
        this.isActive = false;
    }

    public override void TakeDamage(int damageTaken)
    {
        this.currentHealth -= damageTaken;

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

        this.isActive = false;
        this.GetComponent<Collider2D>().enabled = false;
        /*  this.GetComponent<Canvas>().enabled = false;*/
        this.gameController.enemiesTracker.EnemyDies();
        this.gameController.UpdateQuestProgress();
        DropItem();
        /*DropItem();*/
        Destroy(gameObject, this.destroyBodyAfterSeconds);
    }
    private void DropItem()
    {
        base.DropHeldItem();
    }
    public override void TryDealDamage()
    {
        throw new System.NotImplementedException();
    }

    private void StartRunning()
    {
        ResetAllStatesToFalse();
        this.isRunning = true;
    }

    private void RangeAttack()
    {
        ResetAllStatesToFalse();
        this.isAttacking = true;
        this.Shoot(); //do dodania jakis interwal czasowy przy strzelaniu
    }

    private void ResetAllStatesToFalse()
    {
        this.isAttacking = false;
        this.isRunning = false;
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

        if (availablePaths.Count == 0)
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

                if (minAngle > angle)
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

    private bool DetermineIfInShootingRange()
    {
        Vector3 vectorBetweenEntities = this.player.transform.position - this.transform.position;
        float distanceBetweenEntities = vectorBetweenEntities.magnitude;
        if (distanceBetweenEntities <= this.enemyAttackRange)
        {
            return true;
        }
        return false;
    }

    private GameObject Shoot()
    {
        Debug.Log("SHOOT");
        Vector3 vectorBetweenEntities = this.player.transform.position - this.transform.position;
        vectorBetweenEntities = Vector3.Normalize(vectorBetweenEntities);
        float angle = Vector3.Angle(Vector3.right, vectorBetweenEntities);
        if(vectorBetweenEntities.y < 0)
        {
            angle = 360 - angle;
        }
        Debug.Log("ANGLE: " + angle);
        Quaternion rotationQuaternion = Quaternion.Euler(0, 0, angle);
        GameObject projectile =
            Instantiate(this.projectile, this.transform.position, rotationQuaternion); // <- quaterion.identity do zmiany bedzie
        projectile.GetComponent<Projectile>().SetProjectileDamage(this.enemyAttackDamage);
        
        projectile.GetComponent<Rigidbody2D>().AddForce(vectorBetweenEntities * this.enemyProjectileSpeed);
        /*projectile.transform.SetParent(this.transform, true);*/
        return projectile;
    } 

}
