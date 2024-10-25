using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGColorSettings : MonoBehaviour
{
    [Header("General")]
    public float fadeInSpeed;
    public float fadeOutSpeed;
    public float lerpErrorMargin = 0.05f;

    [Header("Perfect Hit")]
    public Color perfectHitColor;
    public Color perfectHitBGColor;

    [Header("Good Hit")]
    public Color goodHitColor;
    public Color goodHitBGColor;

    [Header("Decent Hit")]
    public Color decentHitColor;
    public Color decentHitBGColor;

    [Header("Meh Hit")]
    public Color mehHitColor;
    public Color mehHitBGColor;

    [Header("Barely Hit")]
    public Color barelyHitColor;
    public Color barelyHitBGColor;

    [Header("Error")]
    public Color errorHitColor;
    public Color errorHitBGColor;

    public static BGColorSettings settings;

    void Start()
    {
        if(BGColorSettings.settings == null)
        {
            BGColorSettings.settings = this;
        }
        else Destroy(this.gameObject);
    }
}
