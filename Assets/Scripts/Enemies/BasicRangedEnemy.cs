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
    [Header("Movement")]
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

    [Header("Shooting")]
    [SerializeField, Tooltip("The bullet prefab.")]
    GameObject bulletObj;
    [SerializeField, Tooltip("The muzzle flare prefab.")]
    GameObject flareObj;
    [SerializeField, Tooltip("Where the bullet is spawned.")]
    Transform muzzle;
    [SerializeField, Tooltip("Delay between target detection and bullet firing."), Min(0f)]
    float reactionTime = 0.3f;
    [SerializeField, Tooltip("Time until next bullet can be shot."), Min(0f)]
    float cooldown = 0.5f;
    [SerializeField, Tooltip("Bullet speed.")] 
    float bulletSpeed = 60f;
    [SerializeField, Tooltip("Maximum bullets fired before a reload is needed.")] 
    float chambers = 6f;
    [SerializeField, Tooltip("Time needed to reload.")] 
    float reloadTime = 3f;

    [Header("Audio")]
    [SerializeField, Tooltip("The sound played on Shoot().")]
    AudioClip shootSound;
    [SerializeField, Tooltip("Where <shootSound> is played.")]
    AudioSource audioSource;


    private Rigidbody2D enemyRB;
    Transform target; // The player is the enemy's target and this gets assigned automatically

    private bool lineOfSight = false;
    private bool targetDetected = false;

    float lastDetectionTime;
    float lastShotTime;
    float lastReloadTime;
    float ammo;

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
        // If lineOfSight is about to turn true, then reset the reaction time delay
        if (!lineOfSight && result[0].transform == target)
        {
            lastDetectionTime = Time.time;
        }
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
            
            // Reset reaction time delay when the target is detected
            if (targetDetected)
            {
                lastDetectionTime = Time.time;
            }
        }

        // If the target is seen, the enemy had time to react, Shoot() is not on cooldown,
        // reloading is not on cooldown, and the enemy has ammo, shoot.
        if (targetDetected && lineOfSight && Time.time > lastDetectionTime + reactionTime && Time.time > lastShotTime + cooldown && Time.time > lastReloadTime + reloadTime && ammo > 0)
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

    void Shoot()
    {
        GameObject flare = Instantiate(flareObj, muzzle.position, muzzle.rotation);
        Destroy(flare, 0.1f);
        GameObject bullet = Instantiate(bulletObj, transform.position, muzzle.rotation);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.SetVelocity(-muzzle.up * bulletSpeed);
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
