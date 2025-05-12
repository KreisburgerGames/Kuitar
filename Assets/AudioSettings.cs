using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    [SerializeField] private Scrollbar master, music, fx;
    [SerializeField] private AudioMixer masterControl;
    public float minVol, MaxVol;

    void Start()
    {
        masterControl.SetFloat("Master", 1f);
    }
}
