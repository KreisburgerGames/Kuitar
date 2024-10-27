using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System;

public class PracticeModeHandler : MonoBehaviour
{
    public SongList songList;
    public TMP_Text time;
    public Scrollbar timeSelect;
    public Toggle practiceToggle;
    public AudioClip selectedSongClip;
    public Song selectedSong;
    public int selectedTime = 0;

    public void TogglePracticeMode()
    {
        songList.practiceMode = practiceToggle.isOn;
        timeSelect.interactable = practiceToggle.isOn;
    }

    public void SetTime()
    {
        float songLength = selectedSongClip.length;
        selectedTime = (int)Mathf.Round(timeSelect.value * songLength);
        int seconds = selectedTime % 60;
        int minutes = (int)Mathf.Floor(selectedTime / 60);
        if(seconds < 10) time.text = minutes.ToString() + " : 0" + seconds.ToString();
        else time.text = minutes.ToString() + " : " + seconds.ToString();
        songList.audioSource.time = selectedTime;
    }
}
