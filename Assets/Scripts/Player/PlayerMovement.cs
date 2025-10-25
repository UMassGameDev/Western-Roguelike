using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // This [] is an attribute, attributes mostly provide additional info in the editor.
    [SerializeField, Tooltip("Distance moved per frame in meters per second")]
    private float moveSpeed = 5f;

    private Rigidbody2D playerRB;
    private Vector3 moveDir;

    // Gets the <playerRB>:
    void Awake()
    {
        playerRB = GetComponent<Rigidbody2D>();
    }

    // Updates <moveDir> based on what arrow button is held, and rotates sprite accordingly:
    void Update()
    {
        moveDir = Vector3.zero;

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) { moveDir.y += 1; }
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) { moveDir.x -= 1; }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) { moveDir.y -= 1; }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) { moveDir.x += 1; }

        /*
        bool playerIsMoving = moveDir.sqrMagnitude > 0.01f;
        if (playerIsMoving && Time.timeScale != 0)  // Time.timeScale != 0 is needed because otherwise rotating is not bound by time.
        {
            float angle = CalculateAngle(moveDir);
            transform.rotation = RotateBy(angle);
        }
        */
    }

    // Fixed update is just Update() for physics interactions.
    // Moves the sprite by <moveSpeed> * <dt> every frame:
    void FixedUpdate()
    {
        float moveSpeedDt = moveSpeed * Time.fixedDeltaTime;
        playerRB.MovePosition(playerRB.position + (Vector2)(moveDir * moveSpeedDt));
    }

    //~(Helper Methods)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    [System.Obsolete("Obsolete if using multiple direction sprites for player.")]
    // Returns the angle made between <moveDir> and the positive x-axis, in degrees:
    float CalculateAngle(Vector3 moveDir) => Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;

    [System.Obsolete("Obsolete if using multiple direction sprites for player.")]
    // Returns a quaternion representing a rotation of <angle> degrees:
    Quaternion RotateBy(float angle) => Quaternion.Euler(0f, 0f, angle + 90f);  // (Note, depending on direction your sprite faces you may need to +/- 90).
}
