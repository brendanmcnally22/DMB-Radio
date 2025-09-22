using System.Collections;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public bool IsOpen { get; private set; }
    public Transform doorPivot;
    public float openAngle = 90f;
    public float rotateDuration = 0.35f;
    public AnimationCurve ease = AnimationCurve.EaseInOut(0, 0, 1, 1);

    Quaternion closedRot, openRot;
    Coroutine spin;

    void Awake()
    {
        if (!doorPivot) doorPivot = transform;
        closedRot = doorPivot.localRotation;
        openRot = closedRot * Quaternion.Euler(0f, openAngle, 0f);
    }

    public void Open()
    {
        if (IsOpen) return;
        IsOpen = true;
        StartSpin(true);
    }

    public void Close()
    {
        if (!IsOpen) return;
        IsOpen = false;
        StartSpin(false);
    }

    public void Toggle()
    {
        if (IsOpen) Close(); else Open();
    }

    void StartSpin(bool open)
    {
        if (spin != null) StopCoroutine(spin);
        spin = StartCoroutine(RotateDoor(open ? openRot : closedRot));
    }

    IEnumerator RotateDoor(Quaternion target)
    {
        Quaternion from = doorPivot.localRotation;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / rotateDuration;
            doorPivot.localRotation = Quaternion.Slerp(from, target, ease.Evaluate(t));
            yield return null;
        }
        doorPivot.localRotation = target;
        spin = null;
    }
}
