using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // This [] is an attribute, attributes mostly provide additional info in the editor.
    [SerializeField, Tooltip("Distance moved per frame in meters per second.")]
    private float moveSpeed = 5f;
    [SerializeField, Tooltip("Move Speed multiplier when the player is touching a cactus.")]
    private float cactusDebuff = 0.5f;

    private Rigidbody2D playerRB;
    private Vector3 moveDir;

    // List of trigger colliders objects overlapped with the player 
    private List<Collider2D> currentColliders = new List<Collider2D>();

    // The player is slowed down when colliding with a cactus
    private bool onCactus = false;

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

        // Check for overlap with cacti to update onCactus variable
        onCactus = false;
        foreach (Collider2D collider in currentColliders)
        {
            if (collider.gameObject.GetComponent<Cactus>() != null)
            {
                onCactus = true;
            }
        }

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
    // Moves the sprite by <effectiveMovementSpeed> * <dt> every frame:
    void FixedUpdate()
    {
        // Calculate effective movement speed for if there are movement buffs/debuffs applied
        float effectiveMovementSpeed = moveSpeed;
        if (onCactus) { effectiveMovementSpeed *= cactusDebuff; }

        float moveSpeedDt = effectiveMovementSpeed * Time.fixedDeltaTime;
        playerRB.MovePosition(playerRB.position + (Vector2)(moveDir * moveSpeedDt));
    }

    // Track all currently overlapped trigger colliders
    private void OnTriggerEnter2D(Collider2D collider) { currentColliders.Add(collider); }
    private void OnTriggerExit2D(Collider2D collider) { currentColliders.Remove(collider); }

    //~(Helper Methods)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    [System.Obsolete("Obsolete if using multiple direction sprites for player.")]
    // Returns the angle made between <moveDir> and the positive x-axis, in degrees:
    float CalculateAngle(Vector3 moveDir) => Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;

    [System.Obsolete("Obsolete if using multiple direction sprites for player.")]
    // Returns a quaternion representing a rotation of <angle> degrees:
    Quaternion RotateBy(float angle) => Quaternion.Euler(0f, 0f, angle + 90f);  // (Note, depending on direction your sprite faces you may need to +/- 90).
}
