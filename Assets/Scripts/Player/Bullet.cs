using JetBrains.Annotations;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject hitEffect;
    int counter = 0;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"Bullet hit {collision.gameObject.name} at {collision.contacts[0].point}");
        Debug.DrawLine(transform.position, collision.contacts[0].point, Color.red, 100f);

        GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
        Destroy(effect, 0.1f);

        Destroy(gameObject);
    }
}
