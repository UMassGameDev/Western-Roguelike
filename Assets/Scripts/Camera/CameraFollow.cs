using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform target;
    public float smoothTime = 0.1f;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        // Finds where the player is.
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            target = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("playerObj not found.");
        }
    }

    void FixedUpdate()
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