/*******************************************************
* Script:      ZombieController.cs
* Author(s):   Alexander Art, Nicholas Johnson
* 
* Description:
*    Implements behavior for a zombie.
*    Note that hit logic is controlled by DamageZone.cs, not this!
*******************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ZombieController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField, Tooltip("Distance moved per frame in meters per second.")]
    private float moveSpeed = 2f;
    [SerializeField, Tooltip("Distance the target can be detected when there is a clear line of sight.")]
    private float detectionRadius = 12f;
    [SerializeField, Tooltip("Distance the target will remain detected.")]
    private float persistentRadius = 20f;

    private Rigidbody2D zombieRB;
    private Transform target; // The player is the zombie's target and this gets assigned automatically

    private Pathfinder pathfinder; // Alternative implementation: each ZombieController could use its own pathfinder object
    private Tilemap wallTilemap;

    [Header("Audio")]
    [SerializeField, Tooltip("Where sound is played.")]
    private AudioSource audioSource;

    private bool lineOfSight = false;
    private bool targetDetected = false;

    private List<Vector2Int> path;

    void Start()
    {
        // Assign self rigidbody
        zombieRB = GetComponent<Rigidbody2D>();

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

        // Assign pathfinder
        pathfinder = GameObject.Find("Pathfinder").GetComponent<Pathfinder>();

        // Assign tilemap
        wallTilemap = GameObject.Find("Walls (Tilemap)").GetComponent<Tilemap>();
    }

    void Update()
    {
        // Direction and distance to target
        Vector2 targetDirection = ((Vector2)target.position - zombieRB.position).normalized;
        float targetDistance = Vector2.Distance(zombieRB.position, target.position);

        // Calculate if the enemy has an unobstructed line of sight to the target
        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = false;                                     // Don't detect triggers in the raycast
        filter.SetLayerMask(~(1 << LayerMask.NameToLayer("Enemy")));    // Don't detect self or other enemies in the raycast
        RaycastHit2D[] result = new RaycastHit2D[1];                    // This is where the first result gets stored
        Physics2D.Raycast(zombieRB.position, targetDirection, filter, result, targetDistance); // Cast the ray
        // If the raycast result is the player, then the line of sight is valid
        lineOfSight = result[0].transform == target;

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
        }
    }

    void FixedUpdate()
    {
        // If no target, don't do anything
        if (target == null) return;

        // Direction of target
        Vector2 targetDirection = ((Vector2)target.position - zombieRB.position).normalized;

        // A new path should be calculated if the following 3 conditions are true:
        // - The tilemap has had time to load in
        // - The path is null, the path has a length of 0, or the zombie has reached the first point along the path
        // - The target is detected
        if (Time.fixedTime > 0 && (path == null || path.Count == 0 || zombieRB.position == (Vector2)path[0] + new Vector2(0.5f, 0.5f)) && targetDetected)
        {
            // Calculate a path from the zombie to the target
            path = pathfinder.FindPath(new Vector2Int(Convert.ToInt32(zombieRB.position.x - 0.5f), Convert.ToInt32(zombieRB.position.y - 0.5f)), new Vector2Int(Convert.ToInt32(target.position.x - 0.5f), Convert.ToInt32(target.position.y - 0.5f)), wallTilemap);
        }

        // The default direction to move in is directly toward the player
        Vector2 destination = target.position;
        
        // If a path is found and the zombie has not reached its target, move along the path
        if (path != null && path.Count > 0)
        {
            // Calculate where to move to based on the path
            destination = (Vector2)path[0] + new Vector2(0.5f, 0.5f);

            // Sometimes the current position of the zombie is part of the path. Use the second point of the path if that is the case.
            if (destination == zombieRB.position && path.Count >= 2)
            {
                destination = (Vector2)path[1] + new Vector2(0.5f, 0.5f);
            }

            // If very close to the target, go to the target's position
            if (Vector2.Distance(zombieRB.position, target.position) < 1.0f)
            {
                destination = target.position;
            }
        }
        
        // Move and face the target if the target is detected
        if (targetDetected)
        {
            // Move
            zombieRB.MovePosition(Vector2.MoveTowards(zombieRB.position, destination, moveSpeed * Time.fixedDeltaTime));

            // Face the target
            float angle = CalculateAngle(targetDirection);
            transform.rotation = RotateBy(angle);
        }
    }
    
    //~(Helper Methods)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    //void PlaySound(AudioClip sound) => audioSource.PlayOneShot(sound); // Change this to whatever sound the zombie needs to make

    // Returns the angle made between <dir> and the positive x-axis, in degrees:
    float CalculateAngle(Vector3 dir) => Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    // Returns a quaternion representing a rotation of <angle> degrees:
    Quaternion RotateBy(float angle) => Quaternion.Euler(0f, 0f, angle + 90f);
}
