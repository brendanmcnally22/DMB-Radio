using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordAudio : MonoBehaviour
{
    public AudioClip recordedClip;
    [SerializeField] AudioSource audioSource;

    public DoorController door;          // assign in Inspector
    public float openThreshold = 0.35f;  // 0..1 meter value to open
    public float closeThreshold = 0.15f; // 0..1 to close (hysteresis)
    public float minOpenTime = 0.75f;    // seconds door must stay open
    float reopenAt;                      // internal timer
    
    public float level01 { get; private set; } // 0..1 meter value
    [SerializeField] int window = 1024;
    float[] _buf;

    void Awake()
    {
        _buf = new float[window];
    }

    public void StartRecording()
    {
        string device = Microphone.devices[0];
        int sampleRate = 44100;
        int lengthSec = 3599;

        recordedClip = Microphone.Start(device, true, lengthSec, sampleRate);

        audioSource.clip = recordedClip;
        audioSource.loop = true;
        audioSource.mute = true; // silent monitoring
        audioSource.Play();
    }

    public void PlayRecording()
    {
        audioSource.clip = recordedClip;
        audioSource.mute = false;
        audioSource.Play();
    }

    public void StopRecording()
    {
        Microphone.End(null);
    }

    public void EndPlayback()
    {
        audioSource.Stop();
    }

    void Update()
    {
        // --- compute current loudness into lvl ---
        float lvl = 0f;

        if (recordedClip && Microphone.IsRecording(null))
        {
            int pos = Microphone.GetPosition(null);
            if (pos > 0 && recordedClip.samples >= window)
            {
                int start = Mathf.Clamp(pos - window, 0, recordedClip.samples - window);
                recordedClip.GetData(_buf, start);
                lvl = RmsTo01(_buf);
            }
        }
        else if (audioSource && audioSource.isPlaying)
        {
            if (_buf.Length != window) _buf = new float[window];
            audioSource.GetOutputData(_buf, 0);
            lvl = RmsTo01(_buf);
        }

        level01 = lvl;
        // ----------------------------------------

        // --- drive the door with hysteresis + hold ---
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
        // --------------------------------------------
    }

    float RmsTo01(float[] data)
    {
        double sum = 0;
        for (int i = 0; i < data.Length; i++) sum += data[i] * data[i];
        float rms = Mathf.Sqrt((float)(sum / data.Length)); // ~0..1
        return Mathf.Clamp01(rms * 4f); // quick boost; tweak as needed
    }
}
