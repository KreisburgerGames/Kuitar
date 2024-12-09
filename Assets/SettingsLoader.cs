using FftSharp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsLoader : MonoBehaviour
{
    public int screenWidth;
    public int screenHeight;
    public bool fullscreen;
    public float refreshRate;
    public float latency;
    public static SettingsLoader instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if(PlayerPrefs.HasKey("ResWidth"))
        {
            screenWidth = PlayerPrefs.GetInt("ResWidth");
            screenHeight = PlayerPrefs.GetInt("ResHeight");
        }
        else
        {
            PlayerPrefs.SetInt("ResWidth", Screen.mainWindowDisplayInfo.width);
            PlayerPrefs.SetInt("ResHeight", Screen.mainWindowDisplayInfo.height);
            screenWidth = PlayerPrefs.GetInt("ResWidth");
            screenHeight = PlayerPrefs.GetInt("ResHeight");
        }
        if (PlayerPrefs.HasKey("Fullscreen"))
        {
            fullscreen = ToBool(PlayerPrefs.GetInt("Fullscreen"));
        }
        else
        {
            PlayerPrefs.SetInt("Fullscreen", FromBool(true));
            fullscreen = ToBool(PlayerPrefs.GetInt("Fullscreen"));
        }
        if (PlayerPrefs.HasKey("RefreshRate"))
        {
            refreshRate = PlayerPrefs.GetFloat("RefreshRate");
        }
        else
        {
            PlayerPrefs.SetFloat("RefreshRate", (float)Screen.currentResolution.refreshRateRatio.value);
            refreshRate = PlayerPrefs.GetFloat("RefreshRate");
        }
        if (PlayerPrefs.HasKey("Latency"))
        {
            refreshRate = PlayerPrefs.GetFloat("Latency");
        }
        else
        {
            PlayerPrefs.SetFloat("Latency", 200f);
            latency = PlayerPrefs.GetFloat("Latency");
        }

        FullScreenMode fullScreenMode;
        if (fullscreen)
        {
            fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        }
        else
        {
            fullScreenMode = FullScreenMode.Windowed;
        }

        RefreshRate refresh = new RefreshRate();
        refresh.denominator = 1;
        refresh.numerator = (uint)refreshRate;

        Screen.SetResolution(screenWidth, screenHeight, fullScreenMode, refresh);
    }

    public static bool ToBool(int value)
    {
        if(value <= 0)
            return false;
        return true;
    }

    public static int FromBool(bool value)
    {
        if (value)
            return 1;
        return 0;
    }
}
