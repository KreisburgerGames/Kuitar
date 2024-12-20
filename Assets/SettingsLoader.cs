using FftSharp;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class SettingsLoader : MonoBehaviour
{
    public int screenWidth;
    public int screenHeight;
    public bool fullscreen;
    public int refreshRate;
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
        if (PlayerPrefs.HasKey("Latency"))
        {
            refreshRate = PlayerPrefs.GetInt("Latency");
        }
        else
        {
            PlayerPrefs.SetFloat("Latency", 250f);
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
        
        Screen.SetResolution(screenWidth, screenHeight, fullScreenMode);
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
