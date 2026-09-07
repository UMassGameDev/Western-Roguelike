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
    private int maxHealth;
    [SerializeField, Tooltip("Number of points awarded when this enemy is defeated.")] 
    private int points;

    [SerializeField, Tooltip("These functions are called when the object runs out of health.")] 
    private UnityEvent OnDeath; // Attach functions to this in the Unity Inspector

    private int currentHealth;

    private Score score; // Reference to the score object

    void Awake()
    {
        currentHealth = maxHealth;

        score = FindAnyObjectByType<Score>();
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
        // Update the player's score
        score.AddScore(points);
        
        // Invokes all functions attached to the OnDeath event.
        // If you want to attach functions to the OnDeath event, do it in the Unity Inspector.
        OnDeath.Invoke();
    }

    //~(Destroy)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // This function should attached to the object prefab's OnDeath event
    // so that the object is destroyed when its health reaches 0.
    public void Destroy()
    {
        GameObject.Destroy(this.gameObject);
    }
}
