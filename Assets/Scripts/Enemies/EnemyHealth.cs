/*******************************************************
* Script:      EnemyHealth.cs
* Author(s):   Alexander Art
* 
* Description:
*    Basic health management for enemies.
*******************************************************/

using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField, Tooltip("Default health for this enemy.")] 
    private int maxHealth;
    [SerializeField, Tooltip("Number of points awarded when this enemy is defeated.")] 
    private int points;
    
    [SerializeField, Tooltip("Whether the enemy prefab has a health bar.")] 
    private bool healthBarEnabled;
    [SerializeField, Tooltip("Reference to the enemy's health bar's initial unit. Gets cloned.")] 
    private Transform initialHealthUnit;
    [SerializeField, Tooltip("Spacing between the health icons.")] 
    private float healthIconSpacing;

    [SerializeField, Tooltip("These functions are called when the enemy runs out of health.")] 
    private UnityEvent OnDeath; // Attach functions to this in the Unity Inspector

    private int currentHealth;
    private bool healthBarVisible;
    // Keep track of all the health icons
    private List<Transform> healthUnits = new List<Transform>();

    // Reference to the score object, used to increase the score when the enemy is defeated
    private Score score;

    void Awake()
    {
        currentHealth = maxHealth;

        // If the enemy prefab includes a health bar
        if (healthBarEnabled)
        {
            // Add initial health unit to list of health units
            healthUnits.Add(initialHealthUnit);

            // Initialize health display
            SetHealthDisplay(maxHealth, currentHealth);

            // The health bar will not show up until the enemy is damaged, so it is invisible by default
            SetHealthVisible(false);
        }

        score = FindAnyObjectByType<Score>();
    }

    //~(Damage)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Other scripts can call this function to damage the object.
    public void Damage(int amount)
    {
        // Subtract health
        currentHealth -= amount;

        if (healthBarEnabled)
        {
            // Update health display
            SetHealthDisplay(maxHealth, currentHealth);

            // The enemy's health bar becomes visible after it gets damaged
            SetHealthVisible(true);
        }

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
        // Update the player's score
        score.AddScore(points);

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

    //~(SetHealthDisplay)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Sets the enemy health display.
    private void SetHealthDisplay(int maxHealth, int currentHealth)
    {
        // Delete health icons when the max health decreases
        for (int i = healthUnits.Count - 1; i >= maxHealth; --i)
        {
            Destroy(healthUnits[i].gameObject);
            healthUnits.RemoveAt(i);
        }

        // Create health icons when the max health increases
        for (int i = healthUnits.Count; i < maxHealth; ++i)
        {
            // Create icon clone as child
            Transform newHealthUnit = Instantiate(initialHealthUnit, initialHealthUnit.transform.parent);

            // Space out the new icon
            newHealthUnit.position = new Vector3(newHealthUnit.position.x + healthIconSpacing * i, newHealthUnit.position.y, newHealthUnit.position.z);

            // Add icon to list
            healthUnits.Add(newHealthUnit);
        }

        // Center health icons
        Transform firstHealthUnit = healthUnits[0];
        Transform lastHealthUnit = healthUnits[healthUnits.Count - 1];
        float healthWidth = lastHealthUnit.localPosition.x - firstHealthUnit.localPosition.x;
        for (int i = 0; i < healthUnits.Count; ++i)
        {
            // Space out each icon
            healthUnits[i].localPosition = new Vector3(-healthWidth / 2f + healthIconSpacing * i, healthUnits[i].localPosition.y, healthUnits[i].localPosition.z);
        }

        // Set the state of each health icon
        if (healthBarVisible)
            for (int i = 0; i < healthUnits.Count; ++i)
            {
                // Health unit full
                if (currentHealth >= i + 1)
                {
                    healthUnits[i].GetChild(0).gameObject.SetActive(true);
                    healthUnits[i].GetChild(1).gameObject.SetActive(false);
                }
                // Health unit empty
                else
                {
                    healthUnits[i].GetChild(0).gameObject.SetActive(false);
                    healthUnits[i].GetChild(1).gameObject.SetActive(true);
                }
            }
        else
        {
            // Make each icon invisible if healthBarVisible is false
            for (int i = 0; i < healthUnits.Count; ++i)
            {
                healthUnits[i].GetChild(0).gameObject.SetActive(false);
                healthUnits[i].GetChild(1).gameObject.SetActive(false);
            }
        }
    }

    //~(SetHealthVisible)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Sets the visibility of the enemy's health bar.
    // Note that this is NOT the same as healthBarEnabled, which indicates whether or not the enemy's
    // prefab has a health bar, visible or not.
    void SetHealthVisible(bool visible)
    {
        // Update healthBarVisible variable
        healthBarVisible = visible;

        // Refresh the health bar display
        SetHealthDisplay(maxHealth, currentHealth);
    }
}
