using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ThirdPersonController : MonoBehaviour
{
    public Transform cam;
    public float sens = 2f, clamp = 80f, dist = 4f, speed = 6f, accel = 12f;
    public Vector3 camOffset = new Vector3(0, 1.6f, 0);

    Rigidbody rb;
    float yaw, pitch;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        yaw = transform.eulerAngles.y;
    }

    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            yaw += Input.GetAxis("Mouse X") * sens;
            pitch -= Input.GetAxis("Mouse Y") * sens;
            pitch = Mathf.Clamp(pitch, -clamp, clamp);
        }
    }

    void LateUpdate()
    {
        if (!cam) return;
        Vector3 pivot = transform.position + camOffset;
        Quaternion rot = Quaternion.Euler(pitch, yaw, 0);
        cam.position = pivot + rot * new Vector3(0, 0, -dist);
        cam.rotation = rot;
    }

    void FixedUpdate()
    {
        if (!cam) return;
        float x = Input.GetAxisRaw("Horizontal"), z = Input.GetAxisRaw("Vertical");
        Vector3 f = cam.forward; f.y = 0; f.Normalize();
        Vector3 r = cam.right; r.y = 0; r.Normalize();
        Vector3 dir = (r * x + f * z); if (dir.sqrMagnitude > 1) dir.Normalize();

        Vector3 cur = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        Vector3 tgt = dir * speed;
        Vector3 lerp = Vector3.Lerp(cur, tgt, accel * Time.fixedDeltaTime);
        rb.velocity = new Vector3(lerp.x, rb.velocity.y, lerp.z);

        if (dir.sqrMagnitude > 0.0001f)
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, Quaternion.LookRotation(dir, Vector3.up), 10f * Time.fixedDeltaTime));
    }
}
