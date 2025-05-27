using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    [SerializeField] private Scrollbar master, music, fx;
    [SerializeField] private AudioMixer masterControl;
    public float minVol, maxVol;

    void Start()
    {
        master.value = Mathf.InverseLerp(minVol, maxVol, PlayerPrefs.GetFloat("MasterVol"));
        music.value = Mathf.InverseLerp(minVol, maxVol, PlayerPrefs.GetFloat("MusicVol"));
        fx.value = Mathf.InverseLerp(minVol, maxVol, PlayerPrefs.GetFloat("FXVol"));
    }

    public void MasterChanged(float scrollbarValue)
    {
        float newVolume = Mathf.Lerp(minVol, maxVol, scrollbarValue);
        PlayerPrefs.SetFloat("MasterVol", newVolume);
        masterControl.SetFloat("Master", newVolume);
    }

    public void MusicChanged(float scrollbarValue)
    {
        float newVolume = Mathf.Lerp(minVol, maxVol, scrollbarValue);
        PlayerPrefs.SetFloat("MusicVol", newVolume);
        masterControl.SetFloat("Music", newVolume);
    }

    public void FXChanged(float scrollbarValue)
    {
        float newVolume = Mathf.Lerp(minVol, maxVol, scrollbarValue);
        PlayerPrefs.SetFloat("FXVol", newVolume);
        masterControl.SetFloat("HitFX", newVolume);
    }
}
