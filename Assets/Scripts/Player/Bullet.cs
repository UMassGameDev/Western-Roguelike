/*******************************************************
* Script:      Bullet.cs
* Author(s):   Nicholas Johnson, Alexander Art
* 
* Description:
*    This script handles bullet
*    behaviour on collision.
*******************************************************/

using JetBrains.Annotations;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField, Tooltip("Hit effect on collision.")] 
    private GameObject hitEffect;

    // If the bullet belongs to the player, playerBullet is true, and the player is immune to the bullet.
    private bool playerBullet = false;

    private Vector3 velocity;
    private int damage;
    private int piercing; // Number of hits before the bullet is destroyed

    // Keeps track of how many physics frames this bullet has been alive
    private int framesAlive = 0;

    void FixedUpdate()
    {
        // Make the player's bullet visible after 1 frame.
        // For context: the player's shoot script spawns in the bullet directly on top of the player, so
        // the bullet is invisible initially and this script makes it visible after the first frame.
        if (framesAlive >= 1)
        {
            this.GetComponent<Renderer>().enabled = true;
        }

        // Raycast between current position and next position
        RaycastHit2D[] hits = Physics2D.RaycastAll(this.transform.position, velocity.normalized, (velocity * Time.fixedDeltaTime).magnitude);

        // In order of what is hit first
        foreach (RaycastHit2D hit in hits)
        {
            // If bullet hit non-trigger collider (excluding the enemy if playerBullet is false)
            if (!hit.collider.isTrigger && !playerBullet && hit.transform.GetComponent<EnemyHealth>() == null)
            {
                // Show debug info
                Debug.Log($"Bullet hit {hit.collider.gameObject.name} at {hit.point}");
                Debug.DrawLine(this.transform.position, hit.point, Color.red, 100f);
                
                // Check if the collided object has PlayerHealth
                PlayerHealth playerHealth = hit.collider.gameObject.GetComponent<PlayerHealth>(); 
                if (playerHealth != null)
                {
                    // Damage the collided object
                    playerHealth.Damage(damage);
                }
                else
                {
                    piercing = 0;
                }

                --piercing;
                if (piercing <= 0)
                {
                    // Temporarily instantiate the hit effect
                    GameObject effect = Instantiate(hitEffect, hit.point, Quaternion.identity);
                    Destroy(effect, 0.1f);

                    // Delete the bullet
                    Destroy(this.gameObject);
                }

                // End loop
                break;
            }
            // If bullet hit non-trigger collider (excluding the player if playerBullet is true)
            else if (!hit.collider.isTrigger && playerBullet && hit.transform.GetComponent<PlayerHealth>() == null)
            {
                // Show debug info
                Debug.Log($"Bullet hit {hit.collider.gameObject.name} at {hit.point}");
                Debug.DrawLine(this.transform.position, hit.point, Color.red, 100f);

                // Check if the collided object has EnemyHealth
                EnemyHealth enemyHealth = hit.collider.gameObject.GetComponent<EnemyHealth>(); 
                if (enemyHealth != null)
                {
                    // Damage the collided object
                    enemyHealth.Damage(damage);
                }
                else
                {
                    piercing = 0;
                }

                --piercing;
                if (piercing <= 0)
                {
                    // Temporarily instantiate the hit effect
                    GameObject effect = Instantiate(hitEffect, hit.point, Quaternion.identity);
                    Destroy(effect, 0.1f);

                    // Delete the bullet
                    Destroy(this.gameObject);
                }
                
                // End loop
                break;
            }
            // If bullet hit trigger collider
            else if (hit.collider.isTrigger)
            {
                // Check if the collided object has ObjectHealth
                ObjectHealth objectHealth = hit.collider.gameObject.GetComponent<ObjectHealth>(); 
                if (objectHealth != null)
                {
                    // Damage the collided object
                    objectHealth.Damage(damage);
                }
            }
        }

        // Update position
        this.transform.position += velocity * Time.fixedDeltaTime;

        framesAlive++;
    }
    
    //~(SetVelocity)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Bullet velocity is set from an external script
    public void SetVelocity(Vector3 newVelocity)
    {
        velocity = newVelocity;
    }

    //~(SetDamage)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Bullet damage is set from an external script
    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }

    //~(SetPiercing)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Bullet piercing is set from an external script
    public void SetPiercing(int newPiercing)
    {
        piercing = newPiercing;
    }

    //~(SetPlayerBullet)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // The player is immune to their own bullets
    public void SetPlayerBullet(bool isPlayerBullet)
    {
        playerBullet = isPlayerBullet;
    }
}
