using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordAudio : MonoBehaviour
{
    public AudioClip recordedClip;
    [SerializeField] AudioSource audioSource;

    public DoorLerp door;                // Lerp-based door
    public float openThreshold = 0.35f;
    public float closeThreshold = 0.15f;
    public float minOpenTime = 0.75f;
    float reopenAt;

    public float level01 { get; private set; }
    [SerializeField] int window = 1024;
    float[] _buf;

    string deviceName;                   // store the mic we started

    void Awake()
    {
        _buf = new float[window];
    }

    public void StartRecording()
    {
        if (Microphone.devices.Length == 0) { Debug.LogWarning("No microphone found."); return; }

        deviceName = Microphone.devices[0];
        int sampleRate = 44100;
        int lengthSec = 3599;

        recordedClip = Microphone.Start(deviceName, true, lengthSec, sampleRate);

        if (audioSource)
        {
            audioSource.clip = recordedClip;
            audioSource.loop = true;
            audioSource.mute = true;
            audioSource.Play();
        }
    }

    public void PlayRecording()
    {
        if (!audioSource || !recordedClip) return;
        audioSource.clip = recordedClip;
        audioSource.mute = false;
        audioSource.Play();
    }

    public void StopRecording()
    {
        if (!string.IsNullOrEmpty(deviceName)) Microphone.End(deviceName);
        else Microphone.End(null);
    }

    public void EndPlayback()
    {
        if (audioSource) audioSource.Stop();
    }

    void Update()
    {
        float lvl = 0f;

        // read live mic
        if (!string.IsNullOrEmpty(deviceName) && Microphone.IsRecording(deviceName) && recordedClip)
        {
            int pos = Microphone.GetPosition(deviceName);
            if (pos > 0 && recordedClip.samples >= window)
            {
                int start = Mathf.Clamp(pos - window, 0, recordedClip.samples - window);
                recordedClip.GetData(_buf, start);
                lvl = RmsTo01(_buf);
            }
        }
        // or read current playback
        else if (audioSource && audioSource.isPlaying)
        {
            if (_buf.Length != window) _buf = new float[window];
            audioSource.GetOutputData(_buf, 0);
            lvl = RmsTo01(_buf);
        }

        level01 = lvl;

        if (!door) return;

        if (!door.IsOpen)
        {
            if (level01 >= openThreshold)
            {
                door.Open();
                reopenAt = Time.time + minOpenTime;
            }
        }
        else
        {
            if (Time.time >= reopenAt && level01 <= closeThreshold)
            {
                door.Close();
            }
        }
    }

    float RmsTo01(float[] data)
    {
        double sum = 0;
        for (int i = 0; i < data.Length; i++) sum += data[i] * data[i];
        float rms = Mathf.Sqrt((float)(sum / data.Length));
        return Mathf.Clamp01(rms * 4f); // adjust boost as needed
    }
}
