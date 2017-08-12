﻿using System.Collections;
using UnityEngine;

[RequireComponent(typeof(EnemyInputManager))]
public class ShooterController : RaycastController
{

    // Public variables
    [HeaderAttribute("Projectile Firing Controller")]
    public Enemy_Projectile_Controller enemyProj;   // Reference to the enemy projectile to fire
    public Transform projectileSpawnLocation;

    [TooltipAttribute("How many projectiles to fire in a burst")]
    public int burstFireCount;                  // How many projectiles to fire in a burst
    [TooltipAttribute("How long to wait between each projectile shot")]
    public float fireTimeBetweenShots = 1f;     // How long before each shot is fired
    [TooltipAttribute("How long to wait before the next set of burst fired shots")]
    public float bursFireWaitTime;              // How long to wait before the next burst fire
    [TooltipAttribute("How fast the projectile will fly")]
    public float projectileFireSpeed;           // COntrols how fast the projectile will fire
    [TooltipAttribute("How long before the projectile despawns")] 
    public float projectileDespawnTime;


    [HeaderAttribute("If not pacing, Determines how big the aggro box will be")]
    public float aggroRaySize;      // How big the aggro box size will be
    public Vector2 aggroBoxSize;

    // Private variables
    // Bools to check if the player has been found
    private bool _LumiFound;

    // Bool checks the direction the enemy is facing
    private bool _facingRight;


    // Reference to the enemy Input Manager for controling the enemy input
    //private EnemyInputManager _enemyInput;  // Reference to the enemy input manager

    // Defines enemy movement
    private Vector2 moveDirection;          // Defines the direction the goblin will move

    // Stores transform values
    // Creates hitbox for the enemy's aggro range
    private Vector2 rayOrigin;     // Origin point of the enemy

    // Stores how bix the aggroBox will be to render with a gizmo
    Vector3 giz_AggroBox;



    // Protected class methods 

    // Overidded start method
    protected override void Start()
    {
        base.Start();

        // checks if the enemy is facing right
        if (this.transform.localScale.x > 0)
        {
            _facingRight = true;
        }
        else
        {
            _facingRight = false;
        }

        // Creates pool for the enemy projectiles
        enemyPoolManager.instance.CreatePool(enemyProj, 5);
        //globalPoolManager.instance.CreateEnemyPool(enemyProj, 5);

        // gets component references
        //_enemyInput = GetComponent<EnemyInputManager>();

        // Starts coroutine for pacing the goblin
        StartCoroutine("ShooterControl");

        // Defaults lumi to be found being false
        _LumiFound = false;

    }

    // Private class methods

    /// <summary>
    /// Coroutine for the shooter control
    /// 
    /// Controls shooting
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShooterControl()
    {
        // Coroutine lasts forever
        while (true)
        {
            
            // If the player Lumi has been found, start shooting
            if (_LumiFound)
            {
                // Loops through burst fire count to shoot that many projectiles
                for (int i = 0; i < burstFireCount; i++)
                {
                    // fires projectile
                    fireProjectile();

                    // Waits an ammount of time before the next shot can be fired
                    yield return new WaitForSeconds(fireTimeBetweenShots);
                }

                // Waits a specific amount of time before the burst fire can be started again
                yield return new WaitForSeconds(bursFireWaitTime);
                
            }
        }
    }
    

    private void Update()
    {
        // Gets the current origin of the raycast
        rayOrigin = transform.position;

        // calculates raycasting
        aggroBoxCollisions();

        // Sets the directionalInput to the current move direction
        //_enemyInput._directionalInput = moveDirection;

        // Sets the direction the enemy is facing
        // checks if the enemy is facing right
        if (this.transform.localScale.x > 0)
        {
            _facingRight = true;
        }
        else
        {
            _facingRight = false;
        }

    }


    /// <summary>
    /// Calculates the hitbox collisions for the enemy
    /// 
    /// 
    /// </summary>
    private void aggroBoxCollisions()
    {
        // Defines the box size
        giz_AggroBox = _boxCol.bounds.size + new Vector3(0.1f, 0.1f, 0.0f);

        // Creates 2 raycasts for checking if the enemy is colliding with the player to chase them
        RaycastHit2D hitAggroLeft = Physics2D.Raycast(rayOrigin, Vector2.left, aggroRaySize, collisionMask);
        RaycastHit2D hitAggroRight = Physics2D.Raycast(rayOrigin, Vector2.right, aggroRaySize, collisionMask);

        // Draws debug rays
        Debug.DrawRay(rayOrigin, Vector2.left * aggroRaySize, Color.blue);
        Debug.DrawRay(rayOrigin, Vector2.right * aggroRaySize, Color.blue);

        // If the box hit anything
        //  Move the enemy
        if (hitAggroLeft)
        {
            // If hit on left side, move the enemy left
            if (hitAggroLeft.collider.tag == "Lumi")
            {
                flipCharacterLeft();

                // lumi has been found on the left
                _LumiFound = true;
            }

        }
        else if (hitAggroRight)
        {
            if (hitAggroRight.collider.tag == "Lumi")
            {
                flipCharacterRight();

                // Lumi has been found on the right
                _LumiFound = true;
            }
        }
        else
        {
            _LumiFound = false;
        }
    }

    /// <summary>
    /// Flips the character left
    /// </summary>
    private void flipCharacterLeft()
    {
        // Temp variable to change scale
        Vector2 enemyScale = transform.transform.localScale;

        if (enemyScale.x > 0)
        {
            // Changes scale variable
            enemyScale.x *= -1;

            // Applies change
            this.transform.localScale = enemyScale;
        }
        
        
    }

    /// <summary>
    /// Flips the character right
    /// </summary>
    private void flipCharacterRight()
    {
        // Temp variable to change scale
        Vector2 enemyScale = transform.transform.localScale;

        if (enemyScale.x < 0)
        {
            // Changes scale variable
            enemyScale.x *= -1;

            // applies change
            this.transform.localScale = enemyScale;
        }
        
    }

    /// <summary>
    /// Fires the enemy projetile from the pool
    /// 
    /// 
    /// </summary>
    private void fireProjectile()
    {
        Vector2 spawnLoc;
        spawnLoc.x = projectileSpawnLocation.transform.position.x;
        spawnLoc.y = projectileSpawnLocation.transform.position.y;
        enemyPoolManager.instance.ReuseObject(enemyProj, spawnLoc, Quaternion.identity, _facingRight, projectileDespawnTime, projectileFireSpeed);
        //globalPoolManager.instance.ReuseEnemyObject(enemyProj, spawnLoc, Quaternion.identity, _facingRight, projectileDespawnTime, projectileFireSpeed);
    }

    /// <summary>
    /// Draws the aggro box size
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);

        Gizmos.DrawCube(rayOrigin, giz_AggroBox);
    }

}
