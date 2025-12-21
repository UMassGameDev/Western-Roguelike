/*******************************************************
* Script:      BasicRangedEnemy.cs
* Author(s):   Alexander Art, Nicholas Johnson
* 
* Description:
*    Implements behavior for a ranged enemy.
*******************************************************/

using UnityEngine;

public class BasicRangedEnemy : MonoBehaviour
{
    [SerializeField, Tooltip("Distance moved per frame in meters per second.")]
    private float moveSpeed = 2f;
    [SerializeField, Tooltip("Distance the target can be detected when there is a clear line of sight.")]
    private float detectionRadius = 12f;
    [SerializeField, Tooltip("Distance the target will remain detected.")]
    private float persistentRadius = 20f;
    [SerializeField, Tooltip("Min distance to the target where the enemy will try to get closer.")]
    private float farRange = 5f;
    [SerializeField, Tooltip("Max distance to the target where the enemy will try to get further away.")]
    private float closeRange = 3f;

    private Rigidbody2D enemyRB;
    Transform target; // The player is the enemy's target and this gets assigned automatically

    private bool lineOfSight = false;
    private bool targetDetected = false;

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
        lineOfSight = result[0].transform == target; // If the raycast result is the player, then the line of sight is valid
        
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

        Vector2 targetDirection = ((Vector2)target.position - enemyRB.position).normalized;
        float targetDistance = Vector2.Distance(enemyRB.position, target.position);

        // Move toward the target if:
        // - There is a valid line of sight
        // - The target is detected
        // - The enemy is too far from the target
        if (lineOfSight && targetDetected && targetDistance > farRange)
        {
            enemyRB.MovePosition(Vector2.MoveTowards(enemyRB.position, target.position, moveSpeed * Time.fixedDeltaTime));
        }

        // Move away from the target if:
        // - There is a valid line of sight
        // - The target is detected
        // - The enemy is too close to the target
        if (lineOfSight && targetDetected && targetDistance < closeRange)
        {
            enemyRB.MovePosition(Vector2.MoveTowards(enemyRB.position, target.position, -moveSpeed * Time.fixedDeltaTime));
        }

        // Face the target if it is detected
        if (targetDetected)
        {           
            float angle = CalculateAngle(targetDirection);
            transform.rotation = RotateBy(angle);
        }
    }

    //~(Helper Methods)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    // Returns the angle made between <dir> and the positive x-axis, in degrees:
    float CalculateAngle(Vector3 dir) => Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    // Returns a quaternion representing a rotation of <angle> degrees:
    Quaternion RotateBy(float angle) => Quaternion.Euler(0f, 0f, angle + 90f);
}
