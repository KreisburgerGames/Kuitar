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

    void Update()
    {
        if(!foundSong && FindFirstObjectByType<AudioSource>() != null)
        {
            song = FindFirstObjectByType<AudioSource>();
            foundSong = true;
        }
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
    }

    public void Resume()
    {
        isPaused = false;
        song.UnPause();
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
    }

    public void Quit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Select");
    }
}
