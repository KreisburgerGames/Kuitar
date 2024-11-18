using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EditorPauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public bool isPaused = false;
    public AudioSource song;
    private bool foundSong = false;

    public string songFileName;
    public string songFolder;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) OnPauseButtonPressed();
    }
    public void OnPauseButtonPressed()
    {
        if (isPaused) Resume();
        else Pause();
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
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
    }

    public void Quit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Editor Select");
    }
}
