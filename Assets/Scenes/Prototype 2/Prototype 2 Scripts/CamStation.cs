using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider))]
public class CamStation : MonoBehaviour
{
    [Header("References")]
    public CameraSystem cameraSystem;       
    public Canvas promptCanvas;          
    public TMP_Text promptText;            
    [Header("Behavior")]
    public string openMessage = "Press F to access cameras";
    public string closeMessage = "Press F to exit cameras";
    public KeyCode interactKey = KeyCode.F;
    public int defaultFeedIndex = 0;

    bool playerInRange;
    bool monitorOpen = false;

    void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void Start()
    {
        if (promptCanvas) promptCanvas.enabled = false;
        if (promptText) promptText.text = openMessage;
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
            if (!monitorOpen)
            {
                // Open monitor, stay open
                cameraSystem.OpenAtIndex(defaultFeedIndex);
                monitorOpen = true;

                if (promptText) promptText.text = closeMessage;
            }
            else
            {
                // Close monitor, return control
                cameraSystem.CloseMonitor();
                monitorOpen = false;

                if (promptText) promptText.text = openMessage;
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.1f, 0.8f, 1f, 0.25f);
        Gizmos.DrawCube(transform.position, Vector3.one);
        Gizmos.color = new Color(0.1f, 0.8f, 1f, 1f);
        Gizmos.DrawWireCube(transform.position, Vector3.one);
    }
}
