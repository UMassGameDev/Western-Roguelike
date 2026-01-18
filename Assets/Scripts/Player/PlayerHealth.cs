/*******************************************************
* Script:      PlayerHealth.cs
* Author(s):   Alexander Art
* 
* Description:
*    Health management for the player, including functions for
*    getting/setting the player's current and maximum health.
*
* Note:
*    The display for the player health is in a
*    different script (PlayerHealthBar.cs).
*******************************************************/

using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField, Tooltip("Default player health.")] 
    private int maxHealth = 3;
    [SerializeField, Tooltip("Invincibility duration after player gets hit (seconds).")]
    private float iFrameDuration = 0.6f;

    private int currentHealth;
    private float hitTime; // Time the player was hit for calculating I-frame duration

    // Attach functions to these in the Unity Inspector or with .AddListener
    [Tooltip("These functions are called when the player's maximum or current health changes.")] 
    public UnityEvent<int, int> OnHealthChanged;
    [Tooltip("These functions are called when the player runs out of health.")] 
    public UnityEvent OnDeath;

    void Awake()
    {
        // Set health to full by default
        currentHealth = maxHealth;
    }

    //~(Damage)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Other scripts can call this function to damage the player.
    public void Damage(int amount)
    {
        // If no I-frames active
        if (Time.time > hitTime + iFrameDuration)
        {
            // Subtract health
            currentHealth -= amount;

            // Start new I-frames
            hitTime = Time.time;
        }

        // Invoke all functions listening to the OnHealthChanged event
        OnHealthChanged?.Invoke(maxHealth, currentHealth);

        // Check if the damage killed the object
        if (currentHealth <= 0)
        {
            Kill();
        }
    }

    //~(Heal)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Other scripts can call this function to heal the player.
    public void Heal(int amount)
    {
        // Add health
        currentHealth += amount;

        // Invoke all functions listening to the OnHealthChanged event
        OnHealthChanged?.Invoke(maxHealth, currentHealth);
    }

    //~(Kill)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Kills player (respectfully)
    public void Kill()
    {
        // Sets player health to 0 for if it was not already set to 0
        currentHealth = 0;

        // Invoke all functions listening to the OnHealthChanged event
        OnHealthChanged?.Invoke(maxHealth, currentHealth);

        // Invokes all functions listening to the OnDeath event.
        // If you want to attach functions to the OnDeath event, do it in the Unity Inspector or with OnDeath.AddListener(function).
        OnDeath?.Invoke();
    }

    //~(SetMaximumHealth)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Sets the player's maximum health
    public void SetMaximumHealth(int newMaxHealth)
    {
        // newMaxHealth must be at least 1
        if (newMaxHealth < 1) {
            Debug.LogWarning("PlayerHealth.cs: Player maximum health must be at least 1!");
            newMaxHealth = 1;
        }

        // If the new maximum is greater than the current maximum, the change has a healing effect
        if (newMaxHealth > maxHealth)
        {
            Heal(newMaxHealth - maxHealth);
        }

        // If the new maximum is less than the current health, the change reduces the current health
        if (newMaxHealth < currentHealth)
        {
            currentHealth = newMaxHealth;
        }

        // Set new maximum
        maxHealth = newMaxHealth;

        // Invoke all functions listening to the OnHealthChanged event
        OnHealthChanged?.Invoke(maxHealth, currentHealth);
    }

    //~(GetMaximumHealth)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Returns the player's maximum health
    public int GetMaximumHealth()
    {
        return maxHealth;
    }

    //~(GetCurrentHealth)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Returns the player's current health
    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}
