/*******************************************************
* Script:      EnemyHealth.cs
* Author(s):   Alexander Art
* 
* Description:
*    Basic health management for enemies.
*******************************************************/

using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField, Tooltip("Default health for this enemy.")] 
    private int maxHealth = 2;

    private int currentHealth;

    [SerializeField, Tooltip("These functions are called when the enemy runs out of health.")] 
    private UnityEvent OnDeath; // Attach functions to this in the Unity Inspector

    void Awake()
    {
        currentHealth = maxHealth;
    }

    //~(Damage)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Other scripts can call this function to damage the object.
    public void Damage(int amount)
    {
        // Subtract health
        currentHealth -= amount;

        // Check if the damage killed the object
        if (currentHealth <= 0)
        {
            Kill();
        }
    }

    //~(Kill)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Kills enemy (respectfully)
    public void Kill()
    {
        // Invokes all functions attached to the OnDeath event.
        // If you want to attach functions to the OnDeath event, do it in the Unity Inspector.
        OnDeath.Invoke();
    }

    //~(Destroy)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // This function should attached to the enemy prefab's OnDeath event
    // so that the enemy GameObject is destroyed when its health reaches 0.
    public void Destroy()
    {
        GameObject.Destroy(this.gameObject);
    }
}
