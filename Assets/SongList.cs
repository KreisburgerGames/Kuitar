using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Threading.Tasks;
using System;
using UnityEngine.SceneManagement;

public class SongList : MonoBehaviour
{
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
        foreach(string name in Directory.GetDirectories(Application.streamingAssetsPath + "/" + "Songs"))
        {
            print(name);
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
            song.folderPath = name;
            song.gameObject.name = songLoad.id;
            Texture2D loadedIMG = new Texture2D(1, 1);
            byte[] pngBytes = File.ReadAllBytes(song.folderPath + "/" + song.songCover);
            if(songLoad.playlist == null || songLoad.playlist == "")
            {
                song.playlist = "Custom";
            }
            else song.playlist = songLoad.playlist;
            Offset offset = GameObject.Find("Camera/Canvas/" + song.playlist + "/Viewport/Content").GetComponent<Offset>();
            song.gameObject.transform.SetParent(GameObject.Find("Camera/Canvas/" + song.playlist + "/Viewport/Content").transform, false);
            song.gameObject.transform.localPosition = new Vector2(0, offset.offset - 150);
            song.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, song.gameObject.GetComponent<RectTransform>().anchoredPosition.y);
            loadedIMG.LoadImage(pngBytes);
            song.songCoverIMG = loadedIMG;
            offset.offset -= songItemOffset;
        }
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
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
