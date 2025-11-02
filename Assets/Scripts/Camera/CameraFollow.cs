/*******************************************************
* Script:      CameraFollow.cs
* Author(s):   Nicholas Johnson
* 
* Description:
*    This script, when attached to a camera, follows the player
* 
* Notes:
*    This is a poor way to do this, use Cinemachine for player
*    following instead, its more scalable and has other features
*    we may want, like Camera bounding.
*******************************************************/
using UnityEngine;

/// <summary>
///     This component, when attached to a GameObject, will have the
///     camera follow it. It should only be used on the player.
/// </summary>
/// <seealso cref="PlayerMovement">How the player moves.</seealso>
[System.Obsolete("Switch to Cinemachine instead.")]
public class CameraFollow : MonoBehaviour
{
    // Editor variables:
    [SerializeField, Tooltip("Time for Camera to catch up with player.")]
    float smoothTime = 0.1f;

    // Internal variables:
    Transform target;  // May be better to assign this in editor.
    Vector3 velocity = Vector3.zero;

    void Start()
    {
        // Assigns playerObj to target automatically:
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
        // If no target, don't do anything:
        if (target == null) return;

        // Determine the targets position:
        Vector3 targetPosition = new Vector3(
            target.position.x,
            target.position.y,
            transform.position.z
        );

        // Travel to that position:
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}