/*******************************************************
* Script:      BountyHunterController.cs
* Author(s):   Alexander Art, Nicholas Johnson
* 
* Description:
*    Implements behavior for a bounty hunter ranged enemy.
*******************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BountyHunterController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField, Tooltip("Approximate speed of the enemy.")]
    private float speed = 12f;
    [SerializeField, Tooltip("Rate at which the speed can change.")]
    private float turnSpeed = 2f;
    [SerializeField, Tooltip("Distance the target can be detected when there is a clear line of sight.")]
    private float detectionRadius = 12f;
    [SerializeField, Tooltip("Distance the target will remain detected.")]
    private float persistentRadius = 30f;
    [SerializeField, Tooltip("Min distance to the player the enemy will try to go.")]
    private float closeRange = 1f;
    [SerializeField, Tooltip("Max distance to the player the enemy will try to go.")]
    private float farRange = 6f;
    [SerializeField, Tooltip("Distance to the enemy's pathfinding destination before it considers the pathfinding complete.")]
    private float destinationRadius = 2f;


    [Header("Shooting")]
    [SerializeField, Tooltip("The bullet prefab.")]
    GameObject bulletObj;
    [SerializeField, Tooltip("Where the bullet is spawned.")]
    Transform muzzle;
    [SerializeField, Tooltip("Delay between target detection and bullet firing."), Min(0f)]
    float reactionTime = 0.3f;
    [SerializeField, Tooltip("Time until next bullet can be shot."), Min(0f)]
    float cooldown = 1f;
    [SerializeField, Tooltip("Bullet damage.")] 
    int bulletDamage = 1;
    [SerializeField, Tooltip("Maximum bullets fired before a reload is needed.")] 
    float chambers = 2f;
    [SerializeField, Tooltip("Time needed to reload.")] 
    float reloadTime = 1f;

    [Header("Audio")]
    [SerializeField, Tooltip("The sound played on Shoot().")]
    AudioClip shootSound;
    [SerializeField, Tooltip("Where <shootSound> is played.")]
    AudioSource audioSource;

    private Rigidbody2D enemyRB;
    private Transform target; // The player is the enemy's target and this gets assigned automatically
    private Camera cameraInstance;

    private Pathfinder pathfinder; // Alternative implementation: each enemy could use its own pathfinder object
    private Tilemap wallTilemap;
    private List<Vector2Int> path;
    private Vector2 destination = new Vector2(0f, 0f); // It's not letting me set it to null. Very annoying.
    private bool destSet = false; // This variable is used because it wasn't letting me set destination to null
    private Vector2Int prevTargetPosition;
    private Vector2 unroundedPrevTargetPosition = new Vector2(0f, 0f);

    private int cycle = 0;
    private float offset;
    private float initDistance; // Initial distance to destination

    private bool lineOfSight = false;
    private bool targetDetected = false;
    private bool onScreen = false;

    private float lastDetectionTime;
    private float lastShotTime;
    private float lastReloadTime;
    private float ammo;

    void Start()
    {
        // Assign self rigidbody
        enemyRB = GetComponent<Rigidbody2D>();

        // Assigns playerObj to target automatically
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            target = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("playerObj not found.");
        }

        // Assign camera instance
        cameraInstance = GameObject.FindAnyObjectByType<Camera>().GetComponent<Camera>();

        // Assign pathfinder
        pathfinder = GameObject.Find("Pathfinder").GetComponent<Pathfinder>();

        // Assign tilemap
        wallTilemap = GameObject.Find("Walls (Tilemap)").GetComponent<Tilemap>();

        // Load chambers
        ammo = chambers;
    }

    void Update()
    {
        // Direction and distance to target
        Vector2 targetDirection = ((Vector2)target.position - enemyRB.position).normalized;
        float targetDistance = Vector2.Distance(enemyRB.position, target.position);

        // Calculate if the enemy has an unobstructed line of sight to the target
        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = false;                                     // Don't detect triggers in the raycast
        filter.SetLayerMask(~(1 << LayerMask.NameToLayer("Enemy")));    // Don't detect self or other enemies in the raycast
        RaycastHit2D[] result = new RaycastHit2D[1];                    // This is where the first result gets stored
        Physics2D.Raycast(enemyRB.position, targetDirection, filter, result, targetDistance); // Cast the ray
        // For calculating if the player is on the enemy's "screen"
        float cameraTileWidth = cameraInstance.orthographicSize * cameraInstance.aspect * 2f;
        float cameraTileHeight = cameraInstance.orthographicSize * 2f;
        // If either lineOfSight or onScreen is false but is about to turn true, reset the reaction time delay
        if (!lineOfSight && result[0].transform == target || !onScreen && Mathf.Abs(target.position.x - enemyRB.position.x) < cameraTileWidth / 2f && Mathf.Abs(target.position.y - enemyRB.position.y) < cameraTileHeight / 2f)
        {
            lastDetectionTime = Time.time;
        }
        // If the raycast result is the player, then the line of sight is valid
        lineOfSight = result[0].transform == target;
        // If the player is on the enemy's "screen"
        onScreen = Mathf.Abs(target.position.x - enemyRB.position.x) < cameraTileWidth / 2f && Mathf.Abs(target.position.y - enemyRB.position.y) < cameraTileHeight / 2f;
        
        // If the target is already detected
        if (targetDetected)
        {
            // The target stays detected until it moves outside the persistent radius
            targetDetected = targetDistance < persistentRadius;
        }
        // If the target is not already detected
        else
        {
            // The target is detected when it can be seen and is within the detection radius 
            targetDetected = lineOfSight && targetDistance < detectionRadius;
            
            // Reset reaction time delay when the target is detected
            if (targetDetected)
            {
                lastDetectionTime = Time.time;
            }
        }

        // If the target is detected, the target is on sreen, the target is close enough, the enemy had time to react,
        // Shoot() is not on cooldown, reloading is not on cooldown, and the enemy has ammo, shoot.
        if (targetDetected && onScreen && targetDistance < 1.5f && Time.time > lastDetectionTime + reactionTime && Time.time > lastShotTime + cooldown && Time.time > lastReloadTime + reloadTime && ammo > 0)
        {
            Shoot();
            lastShotTime = Time.time;  // Reset cooldown
            ammo--;
        }

        // Reload when ammo runs out
        if (ammo == 0)
        {
            ammo = chambers;
            lastReloadTime = Time.time;  // Reset time needed to reload
        }
    }

    void FixedUpdate()
    {
        // If no target, don't do anything
        if (target == null) return;

        Vector2 targetDirection = ((Vector2)target.position - enemyRB.position).normalized;
        float targetDistance = Vector2.Distance(enemyRB.position, target.position);
        Vector2Int targetPosition = new Vector2Int(Convert.ToInt32(target.position.x - 0.5f), Convert.ToInt32(target.position.y - 0.5f));
        Vector2 targetVelocity = (Vector2)target.position - unroundedPrevTargetPosition;

        // If the target moves to a different tile,
        // and the destination is far from the player or the target is moving parallel to the bounty hunter,
        // then move the destination
        if (prevTargetPosition != null && destSet == true && targetPosition != prevTargetPosition
            && (cycle == 0 || Mathf.Abs(Vector2.Dot(targetVelocity.normalized, ((Vector2)target.position - enemyRB.position).normalized)) > 0.707f))
        {
            int dx = targetPosition.x - prevTargetPosition.x;
            int dy = targetPosition.y - prevTargetPosition.y;
            Vector2 newDestination = destination + new Vector2(dx, dy);
            // Move the destination if it can be moved to match the target's movement
            // If the destination being moved would cause it to collide with a wall, then calculate a new destination
            if (wallTilemap.GetTile(new Vector3Int(Convert.ToInt32(newDestination.x), Convert.ToInt32(newDestination.y), 0)) == null)
            {
                destination = newDestination;
            }
            else
            {
                destination = CalculateNewDestination();
                destSet = true;
            }
        }

        // Calculate a new destination if there is no path yet, the tilemap has had time to load in, and the target is detected
        if (path == null && Time.fixedTime > 0 && targetDetected)
        {
            destination = CalculateNewDestination();
            destSet = true;
        }

        // Calculate the path based on the destination
        if (destSet)
        {
            path = pathfinder.FindPath(new Vector2Int(Convert.ToInt32(enemyRB.position.x - 0.5f), Convert.ToInt32(enemyRB.position.y - 0.5f)), new Vector2Int(Convert.ToInt32(destination.x - 0.5f), Convert.ToInt32(destination.y - 0.5f)), wallTilemap);
        }

        // If the player has been detected, the default direction is toward the player
        // If the player has not been detected, then the default direction is to not move
        Vector2 localDestination;
        if (targetDetected)
        {
            localDestination = target.position;
        }
        else
        {
            localDestination = enemyRB.position;
        }

        // If a path is found, move along the path
        // If the path is not found, but there is supposed to be one, assume it has been completed, so calculate a new path
        // or if the distance between the enemy and destination is small enough, the destination has been reached, so calculate a new path
        if (path != null && path.Count > 0 && destSet && Vector2.Distance(enemyRB.position, destination) > destinationRadius)
        {
            // Calculate where to move to based on the path
            localDestination = (Vector2)path[0] + new Vector2(0.5f, 0.5f);

            // Sometimes the current position of the enemy is part of the path. Use the second point of the path if that is the case.
            if (localDestination == enemyRB.position && path.Count >= 2)
            {
                localDestination = (Vector2)path[1] + new Vector2(0.5f, 0.5f);
            }
        }
        else if (destSet == true)
        {
            destination = CalculateNewDestination();
        }

        // Calculate how fast the enemy should be moving, but lerp toward it to avoid too sharp of turns (bounty hunter is on a horse)
        Vector2 desiredVelocity = (localDestination - enemyRB.position).normalized * speed;
        enemyRB.linearVelocity = Vector2.Lerp(enemyRB.linearVelocity, desiredVelocity, turnSpeed * Time.fixedDeltaTime);

        // Face the target if it is detected
        if (targetDetected)
        {           
            float angle = CalculateAngle(targetDirection);
            transform.rotation = RotateBy(angle);
        }

        prevTargetPosition = new Vector2Int(Convert.ToInt32(target.position.x - 0.5f), Convert.ToInt32(target.position.y - 0.5f));
    }

    void OnDrawGizmos()
    {
        if (path == null || path.Count < 2)
            return;

        Gizmos.color = Color.red;

        for (int i = 0; i < path.Count - 1; i++)
        {
            Vector3 current = new Vector3(path[i].x + 0.5f, path[i].y + 0.5f, 0f);
            Vector3 next = new Vector3(path[i + 1].x + 0.5f, path[i + 1].y + 0.5f, 0f);

            Gizmos.DrawLine(current, next);
        }
    }

    Vector2 CalculateNewDestination()
    {
        // If cycle is 0, move close to the target
        // If cycle is 1 (or any other number), move past the target
        if (cycle == 0)
        {
            // Get close to target
            float dist = closeRange;
            // Angle between bounty hunter and its target
            float angle = Mathf.Atan2(target.position.y - enemyRB.position.y, target.position.x - enemyRB.position.x);
            // +90 or -90 degree offset
            offset = (UnityEngine.Random.Range(0, 2) - 0.5f) * Mathf.PI;
            angle += offset;
            // Failsafe for if the selected position is a wall tile
            // Repeat while the randomly selected position is not empty
            while (wallTilemap.GetTile(new Vector3Int(Convert.ToInt32(target.position.x + dist * Mathf.Cos(angle)), Convert.ToInt32(target.position.y + dist * Mathf.Sin(angle)), 0)) != null)
            {
                // Calculate a new random position
                dist = UnityEngine.Random.Range(closeRange, farRange);
                angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
            }
            // Update cycle before returning
            cycle = (cycle + 1) % 2;
            // Return the selected position as the destination
            Vector2 selectedPosition = new Vector2(target.position.x + dist * Mathf.Cos(angle), target.position.y + dist * Mathf.Sin(angle));
            initDistance = Vector2.Distance(enemyRB.position, selectedPosition);
            return selectedPosition;
        }
        else
        {
            // Get far from target
            float dist = farRange;
            // Keep moving past the player
            float angle = Mathf.Atan2(enemyRB.linearVelocity.y, enemyRB.linearVelocity.x);
            // Failsafe for if the selected position is a wall tile
            // Repeat while the randomly selected position is not empty
            while (wallTilemap.GetTile(new Vector3Int(Convert.ToInt32(target.position.x + dist * Mathf.Cos(angle)), Convert.ToInt32(target.position.y + dist * Mathf.Sin(angle)), 0)) != null)
            {
                // Calculate a new random position
                dist = UnityEngine.Random.Range(closeRange, farRange);
                angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
            }
            // Update cycle before returning
            cycle = (cycle + 1) % 2;
            // Return the selected position as the destination
            Vector2 selectedPosition = new Vector2(target.position.x + dist * Mathf.Cos(angle), target.position.y + dist * Mathf.Sin(angle));
            initDistance = Vector2.Distance(enemyRB.position, selectedPosition);
            return selectedPosition;
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletObj, transform.position, muzzle.rotation, muzzle);
        ShotgunBullet bulletScript = bullet.GetComponent<ShotgunBullet>();
        bulletScript.SetDamage(bulletDamage);
        bulletScript.SetPlayerBullet(false);

        bool soundInitialized = audioSource != null && shootSound != null;
        if (soundInitialized) { PlaySound(); }
    }

    //~(Helper Methods)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    void PlaySound() => audioSource.PlayOneShot(shootSound);

    // Returns the angle made between <dir> and the positive x-axis, in degrees:
    float CalculateAngle(Vector3 dir) => Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    // Returns a quaternion representing a rotation of <angle> degrees:
    Quaternion RotateBy(float angle) => Quaternion.Euler(0f, 0f, angle + 90f);
}
