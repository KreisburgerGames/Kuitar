using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ResolutionSetting : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public List<Resolution> resolutions = new List<Resolution>();
    public List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
    public Toggle fullscreen;

    void Start()
    {
        foreach (Resolution resolution in Screen.resolutions)
        {
            string width = resolution.width.ToString();
            string height = resolution.height.ToString();
            string refreshRate = resolution.refreshRateRatio.ToString();

            TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData();
            optionData.text = width + " x " + height + " at " + refreshRate;

            options.Add(optionData);
            resolutions.Add(resolution);
        }
        dropdown.AddOptions(options);
        dropdown.value = dropdown.options.FindIndex(x => x.text == PlayerPrefs.GetInt("ResWidth").ToString() + " x " + PlayerPrefs.GetInt("ResHeight").ToString() + " at " + Screen.currentResolution.refreshRateRatio.value.ToString());
        fullscreen.isOn = SettingsLoader.ToBool(PlayerPrefs.GetInt("Fullscreen"));
    }

    public void LoadScene(string sceneName)
    {
        PlayerPrefs.SetInt("FromSettings", SettingsLoader.FromBool(true));
        SceneManager.LoadScene(sceneName);
    }

    public void FullscreenToggle(bool fullscreen)
    {
        FullScreenMode fullScreenMode;
        if (fullscreen)
        {
            fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        }
        else
        {
            fullScreenMode = FullScreenMode.Windowed;
        }
        PlayerPrefs.SetInt("Fullscreen", SettingsLoader.FromBool(fullscreen));
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, fullScreenMode, Screen.currentResolution.refreshRateRatio);
    }

    public void ChangeResolution(int index)
    {
        bool fullscreen = SettingsLoader.ToBool(PlayerPrefs.GetInt("Fullscreen"));
        FullScreenMode fullScreenMode;
        if (fullscreen)
        {
            fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        }
        else
        {
            fullScreenMode = FullScreenMode.Windowed;
        }
        Screen.SetResolution(resolutions[index].width, resolutions[index].height, fullScreenMode, resolutions[index].refreshRateRatio);
        PlayerPrefs.SetInt("ResWidth", resolutions[index].width);
        PlayerPrefs.SetInt("ResHeight", resolutions[index].height);
    }
}
