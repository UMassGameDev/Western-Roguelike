using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform target;
    public float smoothTime = 0.1f;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        // Find the player dynamically
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            target = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("Player not found! Make sure it is tagged 'Player'");
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = new Vector3(
            target.position.x,
            target.position.y,
            transform.position.z
        );

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}