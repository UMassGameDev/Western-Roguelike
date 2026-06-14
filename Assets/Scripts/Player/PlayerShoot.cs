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
    [SerializeField, Tooltip("Bullet speed.")] 
    float bulletSpeed = 60f;
    [SerializeField, Tooltip("Bullet damage.")] 
    int bulletDamage = 1;
    [SerializeField, Tooltip("Maximum number of enemies the bullet can hit before getting destroyed.")]
    int bulletPiercing = 1;

    [Header("Audio")]
    [SerializeField, Tooltip("The sound played on Shoot().")]
    AudioClip shootSound;
    [SerializeField, Tooltip("Where <shootSound> is played.")]
    AudioSource audioSource;
    [SerializeField, Tooltip("The animator that controls gun animations.")]
    Animator gunAnimator;

    // Internal variables:
    float lastShotTime;

    // On each update, Shoot() if mouse is clicked and gun is not on cooldown.
    void Update()
    {
        // Calculate conditions neccesary for a Shoot() to occur.
        bool mouseClicked = Input.GetMouseButton(0);
        bool onCooldown = Time.time <= lastShotTime + cooldown;

        // If mouse was clicked, Shoot() is not on cooldown, and the game is not paused, shoot.
        if (mouseClicked && !onCooldown && Time.timeScale != 0.0f)
        {
            gunAnimator.ResetTrigger("firedShot");
            gunAnimator.SetTrigger("firedShot");
            Shoot();
            lastShotTime = Time.time;  // Reset cooldown.
        }
    }

    void Shoot()
    {
        GameObject flare = Instantiate(flareObj, muzzle.position, muzzle.rotation, muzzle);
        Destroy(flare, 0.1f);
        GameObject bullet = Instantiate(bulletObj, transform.position, muzzle.rotation);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.SetVelocity(-muzzle.up * bulletSpeed);
        bulletScript.SetDamage(bulletDamage);
        bulletScript.SetPiercing(bulletPiercing);
        bulletScript.SetPlayerBullet(true);
        // The player's bullet appears in front of the player for a frame, so this fixes that.
        // The bullet script will automatically reenables its renderer after 1 frame.
        bulletScript.GetComponent<Renderer>().enabled = false;

        bool soundInitialized = audioSource != null && shootSound != null;
        if (soundInitialized) { PlaySound(); }
    }

    void PlaySound() => audioSource.PlayOneShot(shootSound);
}