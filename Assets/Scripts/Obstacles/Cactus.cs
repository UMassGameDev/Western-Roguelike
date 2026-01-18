/*******************************************************
* Script:      Cactus.cs
* Author(s):   Alexander Art
* 
* Description:
*    Basic function(s) for cacti objects.
*    The current implementation gives each cactus an Update() function. This is probably inefficient and should change.
*******************************************************/

using UnityEngine;

public class Cactus : MonoBehaviour
{
    [SerializeField, Tooltip("Amount of damage the cactus should do.")]
    public int damage = 1;

    private PlayerHealth playerHealth;
    private bool playerOverlap = false;

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

    // Damage player when cactus overlaps with player
    void Update()
    {
        if (playerOverlap)
        {
            playerHealth.Damage(damage);
        }
    }

    // Function for instantiating particles at the cactus's position
    public void instantiateParticles(ParticleSystem particles)
    {
        Instantiate(particles, transform.position, Quaternion.identity);
    }
}
