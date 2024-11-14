using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewSong : MonoBehaviour
{
    private SongListEditor songListEditor;
    
    void Start()
    {
        songListEditor = FindFirstObjectByType<SongListEditor>();
    }

    public void NewSongClicked()
    {
        songListEditor.ToggleNewSong();
    }
}
