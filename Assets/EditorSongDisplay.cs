using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EditorSongDisplay : MonoBehaviour
{
    public RawImage cover;
    public TMP_Text songName;
    public TMP_Text songArtist;
    public TMP_Text mapper;
    public Button easy;
    public Button normal;
    public Button hard;
    public Button harder;
    public Button difficult;
    SongList songList;

    void Start()
    {
        songList = FindFirstObjectByType<SongList>();
    }

    void Update()
    {
        if(songList.selectedDifficulty == "easy") EventSystem.current.SetSelectedGameObject(easy.gameObject);
        else if(songList.selectedDifficulty == "normal") EventSystem.current.SetSelectedGameObject(normal.gameObject);
        else if(songList.selectedDifficulty == "hard") EventSystem.current.SetSelectedGameObject(hard.gameObject);
        else if(songList.selectedDifficulty == "harder") EventSystem.current.SetSelectedGameObject(harder.gameObject);
        else if(songList.selectedDifficulty == "difficult") EventSystem.current.SetSelectedGameObject(difficult.gameObject);
    }
}
