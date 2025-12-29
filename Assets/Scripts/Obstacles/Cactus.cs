/*******************************************************
* Script:      Cactus.cs
* Author(s):   Alexander Art
* 
* Description:
*    Basic function(s) for cacti objects.
*******************************************************/

using UnityEngine;

public class Cactus : MonoBehaviour
{
    [SerializeField, Tooltip("Amount of damage the cactus should do.")]
    public int damage = 1;

    // Detect overlap with player and apply damage on entry
    private void OnTriggerEnter2D(Collider2D collider)
    {
        PlayerHealth playerHealth = collider.gameObject.GetComponent<PlayerHealth>();
        if (playerHealth != null)
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
