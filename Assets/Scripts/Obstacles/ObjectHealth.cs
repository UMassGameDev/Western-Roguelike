/*******************************************************
* Script:      ObjectHealth.cs
* Author(s):   Alexander Art
* 
* Description:
*    Basic health management for objects.
*******************************************************/

using UnityEngine;
using UnityEngine.Events;

public class ObjectHealth : MonoBehaviour
{
    [SerializeField, Tooltip("Default health for this object.")] 
    private int maxHealth = 1;

    private int currentHealth;

    [SerializeField, Tooltip("These functions are called when the object runs out of health.")] 
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
    // Kills object (respectfully)
    public void Kill()
    {
        // Invokes all functions attached to the OnDeath event.
        // If you want to attach functions to the OnDeath event, do it in the Unity Inspector.
        OnDeath.Invoke();
    }
}
