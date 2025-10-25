using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 2f;

    private Vector2 direction;

    public void Initialize(Vector2 shootDirection)
    {
        direction = shootDirection.normalized;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision Dectected!");
        // Example: destroy on hit
        Destroy(gameObject);
    }
}