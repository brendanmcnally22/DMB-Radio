using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform playerCamera;
    public float mouseSensitivity = 100f;
    public float verticalClamp = 85f;
    public float lookSmooth = 0.06f;

    private float cameraPitchRotation;
    private float cameraYawRotation;
    private float pitchVelocity;
    private float yawVelocity;
    private bool hasInitializedLook;

    [Header("Movement Settings")]
    [SerializeField] public float moveSpeed = 10.0f;
    public float moveAcceleration = 15.0f;

    private Rigidbody playerRigidbody;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerRigidbody = GetComponent<Rigidbody>();
        playerRigidbody.freezeRotation = true;
        playerRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void LateUpdate()
    {
        HandleCamera();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleCamera()
    {
        // Initialize yaw/pitch once so there’s no snap
        if (!hasInitializedLook)
        {
            cameraYawRotation = transform.eulerAngles.y;
            cameraPitchRotation = playerCamera.localEulerAngles.x;
            if (cameraPitchRotation > 180f) cameraPitchRotation -= 360f; // normalize
            hasInitializedLook = true;
        }

        // Mouse input
        float deltaTime = Time.unscaledDeltaTime;
        float mouseXInput = Input.GetAxis("Mouse X") * mouseSensitivity * deltaTime;
        float mouseYInput = Input.GetAxis("Mouse Y") * mouseSensitivity * deltaTime;

        // Accumulate angles
        cameraYawRotation += mouseXInput;
        cameraPitchRotation -= mouseYInput;
        cameraPitchRotation = Mathf.Clamp(cameraPitchRotation, -verticalClamp, verticalClamp);

        // Smooth interpolation
        float smoothedYaw = Mathf.SmoothDampAngle(
            transform.eulerAngles.y,
            cameraYawRotation,
            ref yawVelocity,
            lookSmooth,
            Mathf.Infinity,
            deltaTime
        );

        float smoothedPitch = Mathf.SmoothDampAngle(
            playerCamera.localEulerAngles.x,
            cameraPitchRotation,
            ref pitchVelocity,
            lookSmooth,
            Mathf.Infinity,
            deltaTime
        );


        // Apply
        transform.rotation = Quaternion.Euler(0f, smoothedYaw, 0f);
        playerCamera.localRotation = Quaternion.Euler(smoothedPitch, 0f, 0f);
    }

    void HandleMovement()
    {
        // Get Input
        float inputHorizontal = Input.GetAxisRaw("Horizontal");
        float inputVertical = Input.GetAxisRaw("Vertical");


        // Calculate direction
        Vector3 moveDirection = (transform.right * inputHorizontal + transform.forward * inputVertical).normalized;
        Vector3 targetVelocity = moveDirection * moveSpeed;


        // Get current and target velocity
        Vector3 currentXZVelocity = new Vector3(playerRigidbody.velocity.x, 0f, playerRigidbody.velocity.z);
        Vector3 targetXZVelocity = new Vector3(targetVelocity.x, 0f, targetVelocity.z);


        // Interpalate between current and target velocity
        Vector3 smoothedXZVelocity = Vector3.Lerp(
            currentXZVelocity,
            targetXZVelocity,
            moveAcceleration * Time.fixedDeltaTime
        );


        // Apply
        playerRigidbody.velocity = new Vector3(smoothedXZVelocity.x, playerRigidbody.velocity.y, smoothedXZVelocity.z);
        playerRigidbody.angularVelocity = Vector3.zero;
    }
}
