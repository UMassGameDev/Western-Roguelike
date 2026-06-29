/*******************************************************
* Script:      ShotgunBullet.cs
* Author(s):   Nicholas Johnson, Alexander Art
* 
* Description:
*    This script handles bullet
*    behaviour on collision.
*******************************************************/

using JetBrains.Annotations;
using UnityEngine;

public class ShotgunBullet : MonoBehaviour
{
    [SerializeField, Tooltip("Hit effect on collision.")] 
    private GameObject hitEffect;

    // If the bullet belongs to the player, playerBullet is true, and the player is immune to the bullet.
    private bool playerBullet = false;

    private int damage;

    [SerializeField, Tooltip("Number of physics frames the bullet should stay alive.")] 
    private int lifespan;
    // Keeps track of how many physics frames this bullet has been alive
    private int framesAlive = 0;

    void FixedUpdate()
    {
        if (framesAlive >= lifespan)
        {
            // Delete the bullet
            Destroy(this.gameObject);
        }

        // Anything within 0.4f units of the center of the bullet AoE counts as a hit
        Collider2D[] hits = Physics2D.OverlapCircleAll(this.transform.GetChild(0).position, 0.4f);

        // In order of what is hit first
        foreach (Collider2D hit in hits)
        {
            // If bullet hit non-trigger collider (excluding the enemy if playerBullet is false)
            if (!hit.isTrigger && !playerBullet && hit.transform.GetComponent<EnemyHealth>() == null)
            {
                // Show debug info
                Debug.Log($"Bullet hit {hit.gameObject.name} at {hit.transform.position}");
                Debug.DrawLine(this.transform.position, hit.transform.position, Color.red, 100f);
                
                // Check if the collided object has PlayerHealth
                PlayerHealth playerHealth = hit.gameObject.GetComponent<PlayerHealth>(); 
                if (playerHealth != null)
                {
                    // Damage the collided object
                    playerHealth.Damage(damage);
                }

                // Temporarily instantiate the hit effect
                //GameObject effect = Instantiate(hitEffect, hit.transform.position, Quaternion.identity);
                //Destroy(effect, 0.1f);
            }
            // If bullet hit non-trigger collider (excluding the player if playerBullet is true)
            else if (!hit.isTrigger && playerBullet && hit.transform.GetComponent<PlayerHealth>() == null)
            {
                // Show debug info
                Debug.Log($"Bullet hit {hit.gameObject.name} at {hit.transform.position}");
                Debug.DrawLine(this.transform.position, hit.transform.position, Color.red, 100f);

                // Check if the collided object has EnemyHealth
                EnemyHealth enemyHealth = hit.gameObject.GetComponent<EnemyHealth>(); 
                if (enemyHealth != null)
                {
                    // Damage the collided object
                    enemyHealth.Damage(damage);
                }
                
                // Temporarily instantiate the hit effect
                //GameObject effect = Instantiate(hitEffect, hit.transform.position, Quaternion.identity);
                //Destroy(effect, 0.1f);
            }
            // If bullet hit trigger collider
            else if (hit.isTrigger)
            {
                // Check if the collided object has ObjectHealth
                ObjectHealth objectHealth = hit.gameObject.GetComponent<ObjectHealth>(); 
                if (objectHealth != null)
                {
                    // Damage the collided object
                    objectHealth.Damage(damage);
                }
            }
        }

        framesAlive++;
    }
    
    //~(SetDamage)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Bullet damage is set from an external script
    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }

    //~(SetPlayerBullet)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // The player is immune to their own bullets
    public void SetPlayerBullet(bool isPlayerBullet)
    {
        playerBullet = isPlayerBullet;
    }
}
