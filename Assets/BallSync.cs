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

    void Start()
    {
        beep = GetComponent<AudioSource>();
        halfBounceTime = secondsPerBounce / 2;
        currentLatency = PlayerPrefs.GetFloat("Latency") / 1000;
        beepOffset = halfBounceTime + currentLatency;
        if(beepOffset <= halfBounceTime) negBeepOffset = true; else negBeepOffset = false;
        upY = transform.position.y + bounceHeight;
        downY = transform.position.y;
        fromSettings = SettingsLoader.ToBool(PlayerPrefs.GetInt("FromSettings"));
    }

    public void UpdateOffset(float newOffset)
    {
        currentLatency = newOffset / 1000;
        beepOffset = halfBounceTime + currentLatency;
        if(beepOffset <= halfBounceTime) negBeepOffset = true; else negBeepOffset = false;
    }

    private IEnumerator DelayBeep(float delay)
    {
        yield return new WaitForSeconds(delay);
        beep.Play();
    }

    public void Exit()
    {
        if(fromSettings) SceneManager.LoadScene("Settings"); else SceneManager.LoadScene("Main Menu");
    }

    void Update()
    {
        if(!down)
        {
            timer += Time.deltaTime;
            timer = Mathf.Clamp(timer, 0, halfBounceTime);
            transform.position = new Vector2(0, Mathf.Lerp(downY, upY, timer/halfBounceTime));
            if(negBeepOffset)
            {
                if(!beeped)
                {
                    if(timer >= beepOffset)
                    {
                        beep.Play();
                        beeped = true;
                    }
                }
            }
            if(timer == halfBounceTime)
            {
                if(!negBeepOffset) StartCoroutine(DelayBeep(currentLatency));
                timer = 0;
                down = true;
                beeped = false;
            }
        }
        else
        {
            timer += Time.deltaTime;
            timer = Mathf.Clamp(timer, 0, halfBounceTime);
            transform.position = new Vector2(0, Mathf.Lerp(upY, downY, timer/halfBounceTime));
            if(negBeepOffset)
            {
                if(!beeped)
                {
                    if(timer >= beepOffset)
                    {
                        beep.Play();
                        beeped = true;
                    }
                }
            }
            if(timer == halfBounceTime)
            {
                if(!negBeepOffset) StartCoroutine(DelayBeep(currentLatency));
                timer = 0;
                down = false;
                beeped = false;
            }
        }
    }
}
