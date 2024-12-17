using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using System;
using Unity.Collections;
using System.IO;
using UnityEngine.Rendering.PostProcessing;

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
    public int unmultipliedScore;
    public TMP_Text rankText;
    public TMP_Text scoreText;
    public float scoreTime = 0f;
    public float scoreLerpTime = 5f;
    private Conductor conductor;

    public Image menu;
    public Image restart;
    public float rankAlpha = 0f;
    private PauseMenuManager pause;

    public TMP_Text accuracyText;

    public PostProcessVolume vol;
    private Bloom bloom;
    private float originalBloomStrength;
    public float endBloomStrength;

    void Start()
    {
        pause = FindFirstObjectByType<PauseMenuManager>();
        vol = FindFirstObjectByType<PostProcessVolume>();
        vol.profile.TryGetSettings(out bloom);
        originalBloomStrength = bloom.intensity;
    }

    public void Restart()
    {
        pause.Restart();
    }

    public void Menu()
    {
        pause.Quit();
    }

    void Update()
    {
        if(conductor == null)
        {
            try
            {
                conductor = FindFirstObjectByType<Conductor>();
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
            Color originalColorS = Color.white;
            originalColorS.a = Mathf.Lerp(1f, 0f, fadeTime/timeToFade);
            foreach(SpriteRenderer sprite in lanes)
            {
                sprite.color = originalColorS;
            }

            Color originalColor = songImg.color;
            originalColor.a = Mathf.Lerp(0f, 1f, fadeTime/timeToFade);
            songImg.color = originalColor;

            songNameText.color = originalColor;

            songArtistText.color = originalColor;

            mapperText.color = originalColor;

            scoreText.color = originalColor;

            rankAlpha = originalColor.a;

            menu.color = originalColor;

            restart.color = originalColor;

            accuracyText.color = originalColor;

            originalColor = menu.gameObject.GetComponentInChildren<TMP_Text>().color;
            originalColor.a = Mathf.Lerp(0f, 1f, fadeTime/timeToFade);
            menu.gameObject.GetComponentInChildren<TMP_Text>().color = originalColor;

            restart.gameObject.GetComponentInChildren<TMP_Text>().color = originalColor;

            Color panelColor = panel.color;
            panelColor.a = Mathf.Lerp(0f, 1f, fadeTime/timeToFade);
            panel.color = panelColor;

            bloom.intensity.Override(Mathf.Lerp(originalBloomStrength, endBloomStrength, fadeTime/timeToFade));
        }
        if(isEnd && scoreTime < scoreLerpTime)
        {
            scoreTime += Time.deltaTime;
            scoreTime = Mathf.Clamp(scoreTime, 0, scoreLerpTime);
            int newScore = (int)Mathf.Ceil(Mathf.Lerp(0, scoreEarned, scoreTime/scoreLerpTime));
            int newUScore = (int)Mathf.Ceil(Mathf.Lerp(0, unmultipliedScore, scoreTime/scoreLerpTime));
            scoreText.text = "Score: " + newScore.ToString();

            float lerpedAccuracy = MathF.Round((float)newUScore/conductor.bestPossibleScore * 100, 2);
            rankText.text = conductor.GetRank(lerpedAccuracy, true);
            Color newColor = conductor.rankColor;
            newColor.a = rankAlpha;
            rankText.color = newColor;
            
            accuracyText.text = "Accuracy: " + lerpedAccuracy.ToString() + "%";
        }
    }
}
