/*******************************************************
* Script:      DamageZone.cs
* Author(s):   Alexander Art
* 
* Description:
*    Put this on a trigger to have it damage the player when overlapping.
*    The current implementation gives each object an Update() function. This is probably inefficient and should change.
*******************************************************/

using UnityEngine;

public class DamageZone : MonoBehaviour
{
    [SerializeField, Tooltip("Amount of damage to be dealt on each hit.")]
    public int damage = 1;

    private PlayerHealth playerHealth;
    private bool playerOverlap = false; // For keeping track of when to damage the player

    // Detect overlap with player
    private void OnTriggerEnter2D(Collider2D collider)
    {
        playerHealth = collider.gameObject.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerOverlap = true;
        }
    }

    // Detect overlap with player ending
    private void OnTriggerExit2D(Collider2D collider)
    {
        playerHealth = collider.gameObject.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerOverlap = false;
        }
    }

    void Update()
    {
        // Damage player when zombie overlaps with player
        if (playerOverlap)
        {
            playerHealth.Damage(damage);
        }
    }
}
