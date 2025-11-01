/*******************************************************
* Script:      PlayerShoot.cs
* Author(s):   Nicholas Johnson
* 
* Description:
*    This script handles player shooting.
*******************************************************/

using UnityEngine;

/// <summary>
///     PlayerShoot is a component that allows shooting as if the attached object
///     was the player. It should only be attached to the player.
/// </summary>
/// <remarks> 
///     As of 11/1/2025, this script assumes a fixed weapon; the revolver. This script
///     should be modified to handle other weapons smoothly when the time comes to implement them.
/// </remarks>
/// <seealso cref="PlayerAim">How the player aims.</seealso>
/// <seealso cref="Bullet">Bullet currently fired.</seealso>
public class PlayerShoot : MonoBehaviour
{
    // Editor variables:
    [Header("Shooting")]
    [SerializeField, Tooltip("The bullet prefab.")]
    GameObject bulletObj;
    [SerializeField, Tooltip("The muzzle flare prefab.")]
    GameObject flareObj;
    [SerializeField, Tooltip("Where the bullet is spawned.")]
    Transform muzzle;
    [SerializeField, Tooltip("Time until next bullet can be shot."), Min(0f)]
    float cooldown = 0.5f;

    [Header("Audio")]
    [SerializeField, Tooltip("The sound played on Shoot().")]
    AudioClip shootSound;
    [SerializeField, Tooltip("Where <shootSound> is played.")]
    AudioSource audioSource;

    // Internal variables:
    float lastShotTime;
    float bulletForce = 60f;

    // On each update, Shoot() if mouse is clicked and gun is not on cooldown.
    void Update()
    {
        // Calculate conditions neccesary for a Shoot() to occur.
        bool mouseClicked = Input.GetMouseButton(0);
        bool onCooldown = Time.time <= lastShotTime + cooldown;

        // If mouse was clicked and Shoot() is not on cooldown, shoot.
        if (mouseClicked && !onCooldown)
        {
            Shoot();
            lastShotTime = Time.time;  // Reset cooldown.
        }
    }

    void Shoot()
    {
        GameObject flare = Instantiate(flareObj, muzzle.position, muzzle.rotation);
        Destroy(flare, 0.1f);
        GameObject bullet = Instantiate(bulletObj, muzzle.position, muzzle.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(-muzzle.up * bulletForce, ForceMode2D.Impulse);

        bool soundInitialized = audioSource != null && shootSound != null;
        if (soundInitialized) { PlaySound(); }
    }

    void PlaySound() => audioSource.PlayOneShot(shootSound);
}