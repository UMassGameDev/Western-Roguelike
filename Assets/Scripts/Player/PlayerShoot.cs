using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public GameObject bulletObj;
    public Transform muzzle;

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

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(0, 0, angle + 90);

        bullet.GetComponent<Bullet>().Initialize(direction);
    }
}
