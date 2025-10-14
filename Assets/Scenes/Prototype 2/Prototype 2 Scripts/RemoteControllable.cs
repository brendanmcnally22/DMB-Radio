using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class RemoteControllable : MonoBehaviour
{
    public float moveSpeed = 3.5f;
    public float turnSpeed = 180f;

    CharacterController cc;
    float yaw; // local yaw we accumulate while in cam mode

    void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    public void Drive(float horizontal, float vertical, Transform cam)
    {
        if (cc == null || cam == null) return;

        // Camera-facing planar movement (XZ)
        Vector3 fwd = cam.forward; fwd.y = 0f; fwd.Normalize();
        Vector3 right = cam.right; right.y = 0f; right.Normalize();

        Vector3 dir = (fwd * vertical + right * horizontal);
        if (dir.sqrMagnitude > 1f) dir.Normalize();

        cc.SimpleMove(dir * moveSpeed);

        // Face movement direction if moving
        if (dir.sqrMagnitude > 0.0001f)
        {
            Quaternion target = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, target, turnSpeed * Time.deltaTime);
        }
        else
        {
            // If not moving, keep whatever rotation yaw has set
            transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        }
    }

    public void AddYaw(float delta)
    {
        yaw += delta * turnSpeed * 0.1f; // small scale; CameraSystem.mouseSensitivity handles feel
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
    }

    void OnEnable()
    {
        yaw = transform.eulerAngles.y;
    }
}
