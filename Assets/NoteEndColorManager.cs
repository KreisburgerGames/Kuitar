using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class NoteEndColorManager : MonoBehaviour
{
    public SpriteRenderer BGSprite;
    private SpriteRenderer noteSprite;
    private Color originalColor;
    private Color originalBGColor;
    private float fadeInSpeed;
    private float fadeOutSpeed;
    private Color hitColor;
    private Color BGHitColor;
    private bool active = false;
    private bool fadeIn = true;
    private int colorRoundingDecimals;

    void Start()
    {
        noteSprite = GetComponent<SpriteRenderer>();
        originalColor = noteSprite.color;
        originalBGColor = BGSprite.color;
        fadeInSpeed = BGColorSettings.settings.fadeInSpeed;
        fadeOutSpeed = BGColorSettings.settings.fadeOutSpeed;
    }

    public void PerfectHit()
    {
        noteSprite.color = originalColor;
        BGSprite.color = originalBGColor;
        hitColor = BGColorSettings.settings.perfectHitColor;
        BGHitColor = BGColorSettings.settings.perfectHitBGColor;
        fadeIn = true;
        active = true;
    }

    public void Error()
    {
        noteSprite.color = originalColor;
        BGSprite.color = originalBGColor;
        hitColor = BGColorSettings.settings.errorHitColor;
        BGHitColor = BGColorSettings.settings.errorHitBGColor;
        fadeIn = true;
        active = true;
    }

    void Update()
    {
        if(!active) return;
        Animate();
    }

    void Animate()
    {
        if(fadeIn)
        {
            noteSprite.color = Color.Lerp(noteSprite.color, hitColor, fadeInSpeed * Time.deltaTime);
            BGSprite.color = Color.Lerp(BGSprite.color, BGHitColor, fadeInSpeed * Time.deltaTime);
            float NSCRGB = noteSprite.color.r + noteSprite.color.g + noteSprite.color.b;
            float BGSRGB = BGSprite.color.r + BGSprite.color.g + BGSprite.color.b;
            if(NSCRGB >= errorMargin(hitColor) && BGSRGB >= errorMargin(BGHitColor)) {print("woo"); fadeIn = false;}
        }
        else
        {
            noteSprite.color = Color.Lerp(noteSprite.color, originalColor, fadeOutSpeed * Time.deltaTime);
            BGSprite.color = Color.Lerp(BGSprite.color, originalBGColor, fadeOutSpeed * Time.deltaTime);
            float NSCRGB = noteSprite.color.r + noteSprite.color.g + noteSprite.color.b;
            float BGSRGB = BGSprite.color.r + BGSprite.color.g + BGSprite.color.b;
            if(NSCRGB >= errorMargin(originalColor) && BGSRGB >= errorMargin(originalBGColor)) {print("done"); active = false;};
        }
    }
    
    float errorMargin(Color color)
    {
        float r = color.r;
        float g = color.g;
        float b = color.b;

        r -= BGColorSettings.settings.lerpErrorMargin;
        g -= BGColorSettings.settings.lerpErrorMargin;
        b -= BGColorSettings.settings.lerpErrorMargin;

        return r + g + b;
    }
}
