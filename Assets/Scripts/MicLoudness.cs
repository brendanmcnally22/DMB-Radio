using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // We need this for our Slider! woo
[RequireComponent(typeof(AudioSource))] // I need to make sure an Audio Source is always attached. This does that. :D Thank you! 
public class MicLoudness : MonoBehaviour
{
    /// <summary>
    // I'km making a script that will measure the players mic audio level in real-time.
    // It Calculates RMS (ROOTMEANSQUARE), the peak (max amplitude), and dB. 
    // I'm also gonna make a UI slider that will show how loud you are.
    // We want to possibly make it be able to open a door. 
    // We can definitely set a threshhold to do that
    /// </summary>

    //Recording Settings
    public int sampleRate = 44100; // Using CD quality sample per seconds 
    public int bufferSeconds = 60; // Rolling buffer length (seconds)
    public int analysisSize = 1024; //number of samples used for loudness analysis per frame

    // Live analysis 
    public float rms; // root mean square, this will be like how overall loud it will be
    public float peak; // max amp in this current analysis block
    public float db; // Decibels, im using logarithmic scale, We converted to decibels, Thank you decibels....

    // UI slider!!! (important for immersion) 
    public Slider meter; 
    public float 

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
