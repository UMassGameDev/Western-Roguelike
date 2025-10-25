using UnityEngine;

/// <summary>
///     This class handles pointing the sprite it's assigned to in the direction of
///     the mouse.
/// </summary>
/// <remarks>
///     This should almost always be assigned to the player.
/// </remarks>
public class PlayerAim : MonoBehaviour
{
    // Updates transform.rotation to point at the mouse every frame:
    void Update()
    {
        Vector3 mousePos = MousePosition();
        Vector3 mouseDir = MouseDirection(mousePos);

        float angle = CalculateAngle(mouseDir);
        transform.rotation = RotateBy(angle);
    }

    //~(Helper Methods)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    // Returns a Vector3 of the mouse's current position.
    Vector3 MousePosition() => Camera.main.ScreenToWorldPoint(Input.mousePosition);
    // Determines the direction of <mousePos> relative to the transform's position.
    Vector3 MouseDirection(Vector3 mousePos) => mousePos - transform.position;

    // Returns the angle made between <dir> and the positive x-axis, in degrees:
    float CalculateAngle(Vector3 dir) => Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    // Returns a quaternion representing a rotation of <angle> degrees:
    Quaternion RotateBy(float angle) => Quaternion.Euler(0f, 0f, angle + 90f);
}
