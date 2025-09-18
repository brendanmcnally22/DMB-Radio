using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordAudio : MonoBehaviour
{
    // Here we go, We can make a recording script and playback script right here. 
    public AudioClip recordedClip;
    [SerializeField] AudioSource audioSource;
    
public void StartRecording()
    {
        // not adding as many comments this time, just following the yt tutorial
        string device = Microphone.devices[0];
        int sampleRate = 44100; //cd sample rate
        int lengthSec = 3599;

        recordedClip = Microphone.Start(device, false, lengthSec, sampleRate);

    }

    public void PlayRecording()
    {

        audioSource.clip = recordedClip;
        audioSource.Play(); // lol... 
       
    }

    public void StopRecording()
{
        Microphone.End(null);
        
}

    public void EndPlayback()
    {
        audioSource.clip = recordedClip;
        audioSource.Stop();
    }

}