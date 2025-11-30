/*******************************************************
* Script:      PlayerHealthBar.cs
* Author(s):   Alexander Art
* 
* Description:
*    Health display for the player.
*
* Note:
*    This is just the display for the health. The actual
*    player health is in a different script (PlayerHealth.cs).
*******************************************************/

using UnityEngine;
using System.Collections.Generic;

public class PlayerHealthBar : MonoBehaviour
{
    // Reference to player health script
    private PlayerHealth playerHealth;

    [SerializeField, Tooltip("Reference to initial health unit. Gets cloned.")] 
    private Transform initialHealthUnit;

    [SerializeField, Tooltip("Spacing between the health icons.")] 
    private int healthIconSpacing;

    // Keep track of all health icons
    private List<Transform> healthUnits = new List<Transform>();

    void Start()
    {
        // Search the scene for the player health script on the player
        playerHealth = FindObjectOfType<PlayerHealth>();

        // Add initial health unit to list of health units
        healthUnits.Add(initialHealthUnit);

        // Initialize health display
        SetHealth(playerHealth.GetMaximumHealth(), playerHealth.GetCurrentHealth());

        // Listen to the player health OnHealthChanged UnityEvent
        playerHealth.OnHealthChanged.AddListener(SetHealth);
    }

    //~(SetHealth)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Sets the player health display.
    // This function is only called on playerHealth.OnHealthChanged.
    private void SetHealth(int maxHealth, int currentHealth)
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
            Transform newHealthUnit = Instantiate(initialHealthUnit, this.transform);

            // Space out the new icon
            newHealthUnit.position = new Vector3(newHealthUnit.position.x + healthIconSpacing * i, newHealthUnit.position.y, newHealthUnit.position.z);

            // Add icon to list
            healthUnits.Add(newHealthUnit);
        }

        // Set the state of each health icon
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
    }
}
