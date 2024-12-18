using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public bool isPaused = false;
    private AudioSource song;
    private bool foundSong = false;

    public string songFileName;
    public string songName;
    public float bpm;
    public float firstBeatOffset;
    public string mapPath;
    public float reactionBeats;
    public bool practiceMode = false;
    public float startOffset;
    public int pixelsPerBeat;
    public string songFolder;
    public string songCoverFile;
    public string songArtist;
    public string mapper;
    public bool end = false;
    public End endRef;
    public float pauseDspTime;
    private Conductor conductor;

    void Start()
    {
        endRef = FindFirstObjectByType<End>();
    }

    void Update()
    {
        if(end) return;
        if(endRef.isEnd)
        {
            if(isPaused)
                Resume();
            end = true;            
        }
        if(!foundSong && FindFirstObjectByType<AudioSource>() != null)
        {
            song = FindFirstObjectByType<AudioSource>();
            foundSong = true;
        }
        if(conductor == null) conductor = FindFirstObjectByType<Conductor>();
        if(Input.GetKeyDown(KeyCode.Escape)) OnPauseButtonPressed();
    }
    public void OnPauseButtonPressed()
    {
        if(isPaused) Resume();
        else Pause();
    }

    public void Restart()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene("Reset");
        Time.timeScale = 1f;
    }

    public void Pause()
    {
        isPaused = true;
        song.Pause();
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
        pauseDspTime = (float)AudioSettings.dspTime;
    }

    public void Resume()
    {
        isPaused = false;
        song.UnPause();
        Time.timeScale = 1f;
        conductor.pauseTime += (float)(AudioSettings.dspTime - pauseDspTime);
        pauseMenu.SetActive(false);
    }

    public void Quit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Select");
    }
}
