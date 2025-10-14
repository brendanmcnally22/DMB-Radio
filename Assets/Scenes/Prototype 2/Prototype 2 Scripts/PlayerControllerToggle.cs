using UnityEngine;

public class PlayerControllerToggle : MonoBehaviour
{
    [Tooltip("Scripts to disable when entering Camera Mode.")]
    public Behaviour[] playerControlScripts;

    public void SetEnabled(bool enabledState)
    {
        foreach (var b in playerControlScripts)
        {
            if (b != null) b.enabled = enabledState;
        }
    }
}
