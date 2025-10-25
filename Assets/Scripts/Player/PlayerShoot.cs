using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("Shooting")]
    [SerializeField, Tooltip("The bullet prefab.")]
    GameObject bulletObj;
    [SerializeField, Tooltip("Where the bullet is spawned.")]
    Transform muzzle;

    [Header("Audio")]
    [SerializeField, Tooltip("The sound played on Shoot().")]
    AudioClip shootSound;
    [SerializeField, Tooltip("Where <shootSound> is played.")]
    AudioSource audioSource;

    const float cooldown = 0.5f;
    float lastShotTime;

    void Update()
    {
        bool mouseClicked = Input.GetMouseButton(0);
        bool onCooldown = Time.time <= lastShotTime + cooldown;
        bool canShoot = mouseClicked && !onCooldown;
        if (canShoot)
        {
            Shoot();
            lastShotTime = Time.time;
        }
    }

    void Shoot()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePos - muzzle.position;

        GameObject bullet = Instantiate(bulletObj, muzzle.position, Quaternion.identity);

        float angle = CalculateAngle(direction);
        bullet.transform.rotation = RotateBy(angle);

        bullet.GetComponent<Bullet>().Initialize(direction);

        bool soundInitialized = audioSource != null && shootSound != null;
        if (soundInitialized) { PlaySound(); }
    }

    float CalculateAngle(Vector2 dir) => Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    Quaternion RotateBy(float angle) => Quaternion.Euler(0, 0, angle + 90);
    void PlaySound() => audioSource.PlayOneShot(shootSound);
}