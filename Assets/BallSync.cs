using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BallSync : MonoBehaviour
{
    public float bounceHeight;
    public float secondsPerBounce;
    private float halfBounceTime;
    private bool down;
    private float timer;
    private float upY;
    private float downY;
    private AudioSource beep;
    public AudioOffsetSetting audioOffset;
    private float beepOffset;
    private bool beeped = false;
    bool negBeepOffset;
    private float currentLatency;
    public bool fromSettings;
    public SpriteRenderer topColor, bottomColor;
    public float fadeSpeed = 5f;

    void Start()
    {
        beep = GetComponent<AudioSource>();
        halfBounceTime = secondsPerBounce / 2;
        currentLatency = PlayerPrefs.GetFloat("Latency") / 1000f;
        beepOffset = halfBounceTime + currentLatency;
        if(beepOffset <= halfBounceTime) negBeepOffset = true; else negBeepOffset = false;
        upY = transform.position.y + bounceHeight;
        downY = transform.position.y;
        fromSettings = SettingsLoader.ToBool(PlayerPrefs.GetInt("FromSettings"));
    }

    public void UpdateOffset(float newOffset)
    {
        currentLatency = newOffset / 1000f;
        beepOffset = halfBounceTime + currentLatency;
        if(beepOffset <= halfBounceTime) negBeepOffset = true; else negBeepOffset = false;
    }

    private IEnumerator DelayBeep(float delay, bool top)
    {
        yield return new WaitForSeconds(delay);
        beep.Play();
        if (top) topColor.color = Color.green;
        else bottomColor.color = Color.green;
    }

    public void Exit()
    {
        if(fromSettings) SceneManager.LoadScene("Settings"); else SceneManager.LoadScene("Main Menu");
    }

    void Update()
    {
        Color.RGBToHSV(topColor.color, out float th, out float ts, out float tv);
        Color.RGBToHSV(bottomColor.color, out float bh, out float bs, out float bv);
        ts = Mathf.Lerp(ts, 0, fadeSpeed * Time.deltaTime);
        tv= Mathf.Lerp(tv, 1, fadeSpeed * Time.deltaTime);
        bs = Mathf.Lerp(bs, 0, fadeSpeed * Time.deltaTime);
        bv= Mathf.Lerp(bv, 1, fadeSpeed * Time.deltaTime);
        topColor.color = Color.HSVToRGB(th, ts, tv);
        bottomColor.color = Color.HSVToRGB(bh, bs, bv);
        if (!down)
        {
            timer += Time.deltaTime;
            timer = Mathf.Clamp(timer, 0, halfBounceTime);
            transform.position = new Vector2(0, Mathf.Lerp(downY, upY, timer / halfBounceTime));
            if (negBeepOffset)
            {
                if (!beeped)
                {
                    if (timer >= beepOffset)
                    {
                        beep.Play();
                        topColor.color = Color.green;
                        beeped = true;
                    }
                }
            }
            if (timer == halfBounceTime)
            {
                if (!negBeepOffset) StartCoroutine(DelayBeep(currentLatency, true));
                timer = 0;
                down = true;
                beeped = false;
            }
        }
        else
        {
            timer += Time.deltaTime;
            timer = Mathf.Clamp(timer, 0, halfBounceTime);
            transform.position = new Vector2(0, Mathf.Lerp(upY, downY, timer / halfBounceTime));
            if (negBeepOffset)
            {
                if (!beeped)
                {
                    if (timer >= beepOffset)
                    {
                        beep.Play();
                        bottomColor.color = Color.green;
                        beeped = true;
                    }
                }
            }
            if (timer == halfBounceTime)
            {
                if (!negBeepOffset) StartCoroutine(DelayBeep(currentLatency, false));
                timer = 0;
                down = false;
                beeped = false;
            }
        }
    }
}
