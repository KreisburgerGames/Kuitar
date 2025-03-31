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
    private List<Song> loadedSongs = new List<Song>();

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
            song.id = songLoad.id;
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
            loadedSongs.Add(song);
        }

        string lp = GetLastPlaylist();
        GetLastSelected(lp);
    }

    private string GetLastPlaylist()
    {
        PlaylistManager playlistManager = FindFirstObjectByType<PlaylistManager>();
        if(PlayerPrefs.HasKey("LastPlaylist"))
        {
            playlistManager.SwitchPlaylist(PlayerPrefs.GetString("LastPlaylist"));
            return PlayerPrefs.GetString("LastPlaylist");
        }
        else
        {
            PlayerPrefs.SetString("LastPlaylist", "OST1");
            playlistManager.ShowOSTOne();
            return "OST1";
        }
    }

    private void SwitchPlaylists(string playlist)
    {
        PlaylistManager playlistManager = FindFirstObjectByType<PlaylistManager>();
        playlistManager.SwitchPlaylist(playlist);
    }
    private void GetLastSelected(string lp)
    {
        if(PlayerPrefs.HasKey("LastSelected"))
        {
            try
            {
                Song song = null;
                foreach(Song check in loadedSongs)
                {
                    if(PlayerPrefs.GetString("LastSelected") == check.id)
                    {
                        song = check;
                        break;
                    }
                }
                if(song.playlist != lp) SwitchPlaylists(song.playlist);
                song.Selected();
            }
            catch(NullReferenceException)
            {
                PlayerPrefs.DeleteKey("LastSelected");
            }
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
