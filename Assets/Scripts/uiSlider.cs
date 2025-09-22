using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class uiSlider : MonoBehaviour
{
    public RecordAudio mic;   // drag your RecordAudio here
    public Slider slider;     // drag your UI Slider here

    void Update()
    {
        if (mic && slider) slider.value = mic.level01; // live update 0..1
    }
}