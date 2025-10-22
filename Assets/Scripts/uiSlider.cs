using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class uiSlider : MonoBehaviour
{
    public RecordAudio mic;   
    public Slider slider;    

    void Update()
    {
        if (mic && slider) slider.value = mic.level01; // live update 0..1
    }
}