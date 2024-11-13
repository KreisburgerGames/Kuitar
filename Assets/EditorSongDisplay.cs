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
    [Header("Map Info")]
    public TMP_InputField IDInput;
    public TMP_InputField songNameInput;
    public TMP_InputField songCoverInput;
    public TMP_InputField artistNameInput;
    public TMP_InputField songFileInput;
    public TMP_InputField mapperInput;
    public TMP_InputField BPMInput;
    public TMP_InputField reactionBeatsInput;
    SongListEditor songList;

    void Start()
    {
        songList = FindFirstObjectByType<SongListEditor>();
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
