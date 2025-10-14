using UnityEngine;

[System.Serializable]
public class CameraFeed
{
    [Tooltip("Label shown on UI (e.g., 'West Hall')")]
    public string name;

    [Tooltip("The actual scene Camera for this feed. It must render to a RenderTexture.")]
    public Camera camera;

    [Tooltip("Optional: a character you can move while this feed is active.")]
    public RemoteControllable remote;
}
