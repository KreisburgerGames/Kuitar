using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Threading.Tasks;
using System;

public class SongList : MonoBehaviour
{
    public Transform songsList;
    public GameObject songItem;
    public float songItemOffset = 170f;
    public Song selectedSong;
    public GameObject songDisplay;
    public GameObject mapLoader;
    public float songStartOffset;
    public string selectedDifficulty;
    public bool practiceMode = false;
    public AudioClip selectedSongClip;
    public PracticeModeHandler practiceModeHandler;
    public AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        float offset = 0f;
        foreach(string name in Directory.GetDirectories(Application.streamingAssetsPath + "/" + "Songs"))
        {
            StreamReader reader = new StreamReader(name + "/info.json");
            TextAsset jsonFile = new TextAsset(reader.ReadToEnd());
            reader.Close();
            SongLoader songLoad = JsonUtility.FromJson<SongLoader>(jsonFile.text);
            Song song = Instantiate(songItem).GetComponent<Song>();
            song.songFile = songLoad.songFile;
            song.songName = songLoad.songName;
            song.artistName = songLoad.artistName;
            song.songCover = songLoad.songCover;
            song.mapper = songLoad.mapper;
            song.bpm = songLoad.bpm;
            song.firstBeatOffset = songLoad.firstBeatOffset;
            song.mapLoaderOBJ = mapLoader;
            song.gameObject.transform.SetParent(songsList, false);
            song.gameObject.transform.localPosition = new Vector2(0, offset - 150);
            song.folderPath = name;
            song.gameObject.name = songLoad.id;
            Texture2D loadedIMG = new Texture2D(1, 1);
            byte[] pngBytes = File.ReadAllBytes(song.folderPath + "/" + song.songCover);
            loadedIMG.LoadImage(pngBytes);
            song.songCoverIMG = loadedIMG;
            offset -= songItemOffset;
        }
    }

    public void Easy()
    {
        selectedDifficulty = "easy";
    }

    public void Normal()
    {
        selectedDifficulty = "normal";
    }
    public void Hard()
    {
        selectedDifficulty = "hard";
    }
    public void Harder()
    {
        selectedDifficulty = "harder";
    }
    public void Difficult()
    {
        selectedDifficulty = "difficult";
    }

    public void StartMap()
    {
        practiceMode = practiceModeHandler.practiceToggle.isOn;
        songStartOffset = practiceModeHandler.selectedTime;
        selectedSong.StartMap(songStartOffset, selectedDifficulty);
    }
}
