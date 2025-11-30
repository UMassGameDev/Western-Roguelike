/*******************************************************
* Script:      Cactus.cs
* Author(s):   Alexander Art
* 
* Description:
*    Basic functions for cacti objects.
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

    //~(Destroy)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // This function is attached to the cactus prefab's objectHealth OnDeath event
    // so that the cactus is destroyed when its health reaches 0.
    public void Destroy()
    {
        GameObject.Destroy(this.gameObject);
    }
}
