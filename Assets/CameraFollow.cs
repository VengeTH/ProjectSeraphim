using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // Assign your player here
    public Vector3 offset = new Vector3(0.05f, 0, -10); // Set Z to your camera's depth (e.g., -10)
    public float smoothSpeed = 0.125f;

    // Add these variables for clamping the camera's X position
    public float minXPosition = 2f; // Minimum X position (already set in your script)
    public float maxXPosition = 462.25f; // Maximum X position (new restriction)

    void LateUpdate()
    {
        // Calculate target position
        Vector3 desiredPosition = player.position + offset;

        // Clamp X position between minXPosition and maxXPosition
        desiredPosition.x = Mathf.Clamp(desiredPosition.x, minXPosition, maxXPosition);

        // Lock Y to 1.02 and Z to -10
        desiredPosition.y = 1.02f; // Y is fixed at 1.02
        desiredPosition.z = -10;   // Z is fixed at -10

        // Smoothly interpolate to the clamped position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}