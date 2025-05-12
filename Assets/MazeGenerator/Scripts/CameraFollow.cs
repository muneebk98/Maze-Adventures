using UnityEngine;

public class CameraFollow: MonoBehaviour
{
    [Tooltip("The player or target to follow")]
    public Transform target;

    [Tooltip("Horizontal distance behind the target")]
    public float distance = 4f;
    [Tooltip("Vertical height above the target")]
    public float height = 2f;

    [Tooltip("How fast the camera interpolates to position")]
    public float smoothSpeed = 8f;
    [Tooltip("Mouse X sensitivity for rotation")]
    public float mouseSensitivity = 5f;

    float yaw = 0f;

    void Start()
    {
        // Initialize yaw to current heading
        yaw = transform.eulerAngles.y;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Accumulate mouse X movement into yaw
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;

        // Build a rotation only around Y
        Quaternion rot = Quaternion.Euler(0f, yaw, 0f);

        // Desired position = target + (rotated offset)
        Vector3 offset = new Vector3(0f, height, -distance);
        Vector3 desiredPos = target.position + rot * offset;

        // Move smoothly to that position
        transform.position = Vector3.Lerp(transform.position,
                                          desiredPos,
                                          smoothSpeed * Time.deltaTime);

        // Always look at the target’s head
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}