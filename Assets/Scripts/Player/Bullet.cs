/*******************************************************
* Script:      Bullet.cs
* Author(s):   Nicholas Johnson, Alexander Art
* 
* Description:
*    This script handles the player's bullets'
*    behaviour on collision.
*******************************************************/

using JetBrains.Annotations;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField, Tooltip("How much damage the bullet does.")] 
    public int damage = 1;

    [SerializeField, Tooltip("Hit effect on collision.")] 
    private GameObject hitEffect;

    int counter = 0; // What does this do?
    
    //~(OnCollisionEnter2D)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Collision with a solid object
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Show debug info
        Debug.Log($"Bullet hit {collision.gameObject.name} at {collision.contacts[0].point}");
        Debug.DrawLine(transform.position, collision.contacts[0].point, Color.red, 100f);

        // Temporarily instantiate the hit effect
        GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
        Destroy(effect, 0.1f);

        // Delete the bullet
        Destroy(gameObject);
    }

    //~(OnTriggerEnter2D)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Collision with a trigger
    private void OnTriggerEnter2D(Collider2D collider)
    {
        // Check if the collided object has ObjectHealth
        ObjectHealth objectHealth = collider.gameObject.GetComponent<ObjectHealth>(); 
        if (objectHealth != null)
        {
            // Damage the collided object
            objectHealth.Damage(damage);
        }
    }
}
