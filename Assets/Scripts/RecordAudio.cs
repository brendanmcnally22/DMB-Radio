using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordAudio : MonoBehaviour
{
    public AudioClip recordedClip;
    [SerializeField] AudioSource audioSource;

    
   public float level01 { get; private set; }   // 0..1 meter value
    [SerializeField] int window = 1024;          // samples for RMS
    float[] _buf;                                // working buffer
   
    void Awake()
    {
        _buf = new float[window];                // ADD
    }

    public void StartRecording()
    {
        string device = Microphone.devices[0];
        int sampleRate = 44100;
        int lengthSec = 3599;

        // loop=true so the clip always has fresh data while recording
        recordedClip = Microphone.Start(device, true, lengthSec, sampleRate);

        
        audioSource.clip = recordedClip;
        audioSource.loop = true;
        audioSource.mute = true;                 // hear nothing, still readable
        audioSource.Play();
    }

    public void PlayRecording()
    {
        audioSource.clip = recordedClip;
        audioSource.mute = false;                // hear playback if you want
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
        // prefer mic clip data if recording
        if (recordedClip && Microphone.IsRecording(null))
        {
            int pos = Microphone.GetPosition(null);
            if (pos > 0 && recordedClip.samples >= window)
            {
                int start = Mathf.Clamp(pos - window, 0, recordedClip.samples - window);
                recordedClip.GetData(_buf, start);
                level01 = RmsTo01(_buf);
                return;
            }
        }

        // fallback: if playing from the audioSource
        if (audioSource && audioSource.isPlaying)
        {
            if (_buf.Length != window) _buf = new float[window];
            audioSource.GetOutputData(_buf, 0);
            level01 = RmsTo01(_buf);
            return;
        }

        level01 = 0f;
    }

    float RmsTo01(float[] data)
    {
        double sum = 0;
        for (int i = 0; i < data.Length; i++) sum += data[i] * data[i];
        float rms = Mathf.Sqrt((float)(sum / data.Length)); // ~0..1
        return Mathf.Clamp01(rms * 4f); // quick boost might tweak this later
    }
 }
