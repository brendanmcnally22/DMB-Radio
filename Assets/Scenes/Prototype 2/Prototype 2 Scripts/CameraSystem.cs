using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CameraSystem : MonoBehaviour
{
    [Header("Feeds & UI")]
    [Tooltip("All security camera feeds in order.")]
    public List<CameraFeed> feeds = new List<CameraFeed>();

    [Tooltip("UI RawImage that displays the active RenderTexture.")]
    public RawImage feedScreen;

    [Tooltip("UI Text (optional) to show room/camera name.")]
    public Text roomLabel;

    [Header("Mode")]
    [Tooltip("Optional: Your normal player root to disable while in Camera Mode.")]
    public GameObject playerRootToDisable;

    [Tooltip("Show/hide a separate UI canvas while in Camera Mode.")]
    public Canvas cameraUICanvas;

    [Tooltip("Optional: toggle player control scripts instead of whole root.")]
    public PlayerControllerToggle playerControllerToggle;

    [Header("Mouse Look Settings (for remote driving)")]
    public float mouseSensitivity = 2.5f;
    public bool lockCursorInCamMode = true;

    int index = -1;
    bool inCamMode = false;

    void Start()
    {
        // Defensive: ensure all feed cameras target a RenderTexture
        for (int i = 0; i < feeds.Count; i++)
        {
            var f = feeds[i];
            if (f == null) continue;

            if (f.camera == null)
            {
                Debug.LogWarning($"CameraFeed at {i} has no Camera assigned.");
                continue;
            }

            if (f.camera.targetTexture == null)
                Debug.LogWarning($"Camera '{f.camera.name}' has no RenderTexture. Assign one!");
        }

        SetCamMode(false);
    }

    void Update()
    {
        // Toggle Camera Mode
        if (Input.GetKeyDown(KeyCode.Tab))
            SetCamMode(!inCamMode);

        if (!inCamMode) return;

        // Cycle
        if (Input.GetKeyDown(KeyCode.Q)) Prev();
        if (Input.GetKeyDown(KeyCode.E)) Next();

        // Number keys to jump (1-based)
        for (int k = 0; k < Mathf.Min(feeds.Count, 9); k++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + k))
                SetIndex(k);
        }

        // Drive remote, if present
        var active = GetActive();
        if (active != null && active.remote != null && active.camera != null)
        {
            // Mouse look
            float yaw = Input.GetAxis("Mouse X") * mouseSensitivity;
            active.remote.AddYaw(yaw);

            // Movement in camera-facing space
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            active.remote.Drive(h, v, active.camera.transform);
        }
    }

    void SetCamMode(bool value)
    {
        inCamMode = value;

        // UI / player visibility
        if (cameraUICanvas != null) cameraUICanvas.enabled = inCamMode;
        if (playerRootToDisable != null) playerRootToDisable.SetActive(!inCamMode);
        if (playerControllerToggle != null) playerControllerToggle.SetEnabled(!inCamMode);

        // Cursor lock
        if (lockCursorInCamMode)
        {
            Cursor.lockState = inCamMode ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !inCamMode;
        }

        // Select camera on enter / clear on exit
        if (inCamMode)
        {
            if (feeds.Count > 0)
                SetIndex(index < 0 ? 0 : Mathf.Clamp(index, 0, feeds.Count - 1));
            else
                ClearScreen();
        }
        else
        {
            ClearScreen();
        }
    }

    void ClearScreen()
    {
        if (feedScreen) feedScreen.texture = null;
        if (roomLabel) roomLabel.text = "";
    }

    void Next() => SetIndex(Normalize(index + 1));
    void Prev() => SetIndex(Normalize(index - 1));

    int Normalize(int i)
    {
        if (feeds.Count == 0) return -1;
        if (i < 0) return feeds.Count - 1;
        if (i >= feeds.Count) return 0;
        return i;
    }

    void SetIndex(int i)
    {
        if (feeds.Count == 0) { index = -1; ClearScreen(); return; }
        index = Mathf.Clamp(i, 0, feeds.Count - 1);

        var f = feeds[index];
        if (feedScreen != null && f != null && f.camera != null)
            feedScreen.texture = f.camera.targetTexture;

        if (roomLabel != null)
            roomLabel.text = string.IsNullOrEmpty(f.name) ? $"Cam {index + 1}" : f.name;
    }

    CameraFeed GetActive()
    {
        if (index < 0 || index >= feeds.Count) return null;
        return feeds[index];
    }
}
