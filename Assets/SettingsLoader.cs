using FftSharp;
using NAudio.Mixer;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SettingsLoader : MonoBehaviour
{
    public int screenWidth;
    public int screenHeight;
    public bool fullscreen;
    public int refreshRate;
    public float latency;
    public int frameLimit;
    public int vSync;
    public static SettingsLoader instance;
    public float noteSpacing;
    private bool firstLaunch;
    public GameObject firstLaunchPopup;
    [SerializeField] private AudioMixerGroup e;

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
            latency = PlayerPrefs.GetFloat("Latency");
        }
        else
        {
            PlayerPrefs.SetFloat("Latency", 0f);
            latency = PlayerPrefs.GetFloat("Latency");
        }
        if (PlayerPrefs.HasKey("NoteSpacing"))
        {
            noteSpacing = PlayerPrefs.GetFloat("NoteSpacing");
        }
        else
        {
            PlayerPrefs.SetFloat("NoteSpacing", 5f);
            noteSpacing = PlayerPrefs.GetFloat("NoteSpacing");
        }
        if (PlayerPrefs.HasKey("FrameLimit"))
        {
            frameLimit = PlayerPrefs.GetInt("FrameLimit");
        }
        else
        {
            PlayerPrefs.SetInt("FrameLimit", (int)Screen.mainWindowDisplayInfo.refreshRate.value);
            frameLimit = PlayerPrefs.GetInt("FrameLimit");
        }
        if (PlayerPrefs.HasKey("VSync"))
        {
            vSync = PlayerPrefs.GetInt("VSync");
        }
        else
        {
            PlayerPrefs.SetInt("VSync", 1);
            vSync = PlayerPrefs.GetInt("VSync");
        }
         if (PlayerPrefs.HasKey("FirstLaunch"))
        {
            firstLaunch = false;
        }
        else
        {
            PlayerPrefs.SetInt("FirstLaunch", FromBool(true));
            firstLaunch = true;
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
        if(PlayerPrefs.HasKey("MasterVol"))
        {

        }
        
        Screen.SetResolution(screenWidth, screenHeight, fullScreenMode);
        Application.targetFrameRate = frameLimit;
        QualitySettings.vSyncCount = vSync;

        if(firstLaunch)
        {
            FirstLaunch();
        }
    }

    private void FirstLaunch()
    {
        PlayerPrefs.SetInt("FromSettings", FromBool(false));
        firstLaunchPopup.SetActive(true);
    }

    public void Yes()
    {
        SceneManager.LoadScene("AdjustAudio");
    }

    public void No()
    {
        firstLaunchPopup.SetActive(false);
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
