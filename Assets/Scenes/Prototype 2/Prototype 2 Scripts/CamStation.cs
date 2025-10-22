using UnityEngine;
using UnityEngine.UI; // or TMPro if you prefer TMP
using TMPro;

[RequireComponent(typeof(Collider))]
public class CamStation : MonoBehaviour
{
    [Header("References")]
    public CameraSystem cameraSystem;       // drag your CameraSystem object
    public Canvas promptCanvas;             // small world-space canvas with "Press F..."
    public TMP_Text promptText;                 // or TMP_Text if using TextMeshPro

    [Header("Behavior")]
    public string message = "Press F to access cameras";
    public KeyCode interactKey = KeyCode.F;
    public int defaultFeedIndex = 0;        // which cam to show first at this station

    bool playerInRange;

    void Reset()
    {
        // Make the collider a trigger by default
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void Start()
    {
        if (promptCanvas) promptCanvas.enabled = false;
        if (promptText) promptText.text = message;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;
        if (promptCanvas) promptCanvas.enabled = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
        if (promptCanvas) promptCanvas.enabled = false;
    }

    void Update()
    {
        if (!playerInRange || cameraSystem == null) return;

        if (Input.GetKeyDown(interactKey))
        {
            cameraSystem.OpenAtIndex(defaultFeedIndex);
            if (promptCanvas) promptCanvas.enabled = false; // hide while open
        }
    }

    // Optional: call this from a UI "Close" button inside your monitor
    public void Close()
    {
        if (cameraSystem) cameraSystem.CloseMonitor();
    }

    // Draw a little gizmo so it’s easy to see in scene view
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.1f, 0.8f, 1f, 0.25f);
        Gizmos.DrawCube(transform.position, Vector3.one);
        Gizmos.color = new Color(0.1f, 0.8f, 1f, 1f);
        Gizmos.DrawWireCube(transform.position, Vector3.one);
    }
}
