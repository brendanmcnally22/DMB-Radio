using System.Collections;
using UnityEngine;

public class DoorLerp : MonoBehaviour
{
    public float openAngle = 90f;   // set negative to swing the other way
    public float duration = 0.5f;

    public bool IsOpen { get; private set; }

    Quaternion closedRot, openRot;
    Coroutine swing;

    void Awake()
    {
        closedRot = transform.localRotation;
        openRot = closedRot * Quaternion.Euler(0f, openAngle, 0f);
    }

    public void Toggle() { if (IsOpen) Close(); else Open(); }

    public void Open()
    {
        if (IsOpen) return;
        IsOpen = true;
        StartSwing(openRot);
    }

    public void Close()
    {
        if (!IsOpen) return;
        IsOpen = false;
        StartSwing(closedRot);
    }

    void StartSwing(Quaternion target)
    {
        if (swing != null) StopCoroutine(swing);
        swing = StartCoroutine(Swing(target));
    }

    IEnumerator Swing(Quaternion target)
    {
        Quaternion start = transform.localRotation;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            transform.localRotation = Quaternion.Lerp(start, target, t);
            yield return null;
        }
        transform.localRotation = target;
    }
}
