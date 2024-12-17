using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using System;
using Unity.Collections;
using System.IO;

public class End : MonoBehaviour
{
    public Image panel;
    public List<SpriteRenderer> lanes = new List<SpriteRenderer>();
    public float timeToFade;
    public bool isEnd;
    public float fadeTime = 0f;

    [Space]
    public RawImage songImg;
    public TMP_Text songNameText;
    public TMP_Text songArtistText;
    public TMP_Text mapperText;

    [Space]
    public int scoreEarned;
    public TMP_Text rankText;
    public TMP_Text scoreText;
    public float scoreTime = 0f;
    public float scoreLerpTime = 5f;
    private Conductor conductor;

    void Update()
    {
        if(conductor == null)
        {
            try
            {
                conductor = FindFirstObjectByType<Conductor>();
                PauseMenuManager pause = FindFirstObjectByType<PauseMenuManager>();
                Texture2D loadedIMG = new Texture2D(1, 1);
                byte[] pngBytes = File.ReadAllBytes(pause.songFolder + "/" + pause.songCoverFile);
                loadedIMG.LoadImage(pngBytes);
                loadedIMG.Apply();
                songImg.texture = loadedIMG;
                songNameText.text = pause.songName;
                songArtistText.text = pause.songArtist;
                mapperText.text = pause.mapper;
            }
            catch
            {
                return; // Wait
            }
        }
        if(isEnd && fadeTime < timeToFade)
        {
            fadeTime += Time.deltaTime;
            fadeTime = Mathf.Clamp(fadeTime, 0, timeToFade);
            foreach(SpriteRenderer sprite in lanes)
            {
                Color originalColor = sprite.color;
                originalColor.a = Mathf.Lerp(originalColor.a, 0f, fadeTime/timeToFade);
                sprite.color = originalColor;
            }
            foreach(SpriteRenderer sprite in GetComponentsInChildren<SpriteRenderer>())
            {
                Color originalColor = sprite.color;
                originalColor.a = Mathf.Lerp(originalColor.a, 0f, fadeTime/timeToFade);
                sprite.color = originalColor;
            }
            Color panelColor = panel.color;
            panelColor.a = Mathf.Lerp(panelColor.a, 0f, fadeTime/timeToFade);
            panel.color = panelColor;
        }
        if(isEnd)
        {
            scoreTime += Time.deltaTime;
            scoreTime = Mathf.Clamp(scoreTime, 0, scoreLerpTime);
            int newScore = (int)Mathf.Ceil(Mathf.Lerp(0, scoreEarned, scoreTime/scoreLerpTime));
            scoreText.text = "Score: " + newScore.ToString();

            rankText.text = conductor.GetRank(newScore);
            rankText.color = conductor.rankText.color;
        }
    }
}
