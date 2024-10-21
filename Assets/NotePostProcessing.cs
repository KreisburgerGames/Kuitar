using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class NotePostProcessing : MonoBehaviour
{
    public float targetIntensity;
    public float fadeInSpeed;
    public float fadeOutSpeed;
    public PostProcessVolume vol;
    private Bloom bloom;

    private bool fadeIn = true;
    // Start is called before the first frame update
    void Start()
    {
        vol.profile.TryGetSettings<Bloom>(out bloom);
    }

    // Update is called once per frame
    void Update()
    {
        if(bloom != null)
        {
            if(fadeIn)
            {
                bloom.intensity.Override(Mathf.Lerp(bloom.intensity.value, targetIntensity, fadeInSpeed * Time.deltaTime));
                if(Math.Round(bloom.intensity.value, 2) == targetIntensity) fadeIn = false;
            }
            else bloom.intensity.Override(Mathf.Lerp(bloom.intensity.value, 0, fadeOutSpeed * Time.deltaTime));
        }
    }
}
