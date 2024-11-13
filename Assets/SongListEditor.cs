using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Threading.Tasks;
using System;
using Unity.VisualScripting;

public class SongListEditor : MonoBehaviour
{
    public Transform songsList;
    public GameObject songItem;
    public float songItemOffset = 170f;
    public EditorSong selectedSong;
    public GameObject songDisplay;
    public string selectedDifficulty;
    private GameObject tempSong;

    // Start is called before the first frame update
    void Start()
    {
        LoadSongs();
    }

    public EditorSong loadSong(string path)
    {
        StreamReader reader = new StreamReader(path + "/info.json");
        TextAsset jsonFile = new TextAsset(reader.ReadToEnd());
        reader.Close();
        SongLoader songLoad = JsonUtility.FromJson<SongLoader>(jsonFile.text);
        EditorSong song = Instantiate(new GameObject()).AddComponent<EditorSong>();
        song.songFile = songLoad.songFile;
        song.songName = songLoad.songName;
        song.artistName = songLoad.artistName;
        song.songCover = songLoad.songCover;
        song.mapper = songLoad.mapper;
        song.bpm = songLoad.bpm;
        song.firstBeatOffset = songLoad.firstBeatOffset;
        song.folderPath = path;
        Texture2D loadedIMG = new Texture2D(1, 1);
        byte[] pngBytes = File.ReadAllBytes(song.folderPath + "/" + song.songCover);
        loadedIMG.LoadImage(pngBytes);
        song.songCoverIMG = loadedIMG;
        tempSong = song.gameObject;
        return song;
    }

    public void LoadSongs()
    {
        float offset = 0f;
        foreach(string name in Directory.GetDirectories(Application.streamingAssetsPath + "/" + "Songs"))
        {
            StreamReader reader = new StreamReader(name + "/info.json");
            TextAsset jsonFile = new TextAsset(reader.ReadToEnd());
            reader.Close();
            SongLoader songLoad = JsonUtility.FromJson<SongLoader>(jsonFile.text);
            EditorSong song = Instantiate(songItem).GetComponent<EditorSong>();
            song.songFile = songLoad.songFile;
            song.songName = songLoad.songName;
            song.artistName = songLoad.artistName;
            song.songCover = songLoad.songCover;
            song.mapper = songLoad.mapper;
            song.bpm = songLoad.bpm;
            song.firstBeatOffset = songLoad.firstBeatOffset;
            song.gameObject.transform.SetParent(songsList, false);
            song.gameObject.transform.localPosition = new Vector2(0, offset - 150);
            song.folderPath = name;
            song.gameObject.name = songLoad.id;
            Texture2D loadedIMG = new Texture2D(1, 1);
            byte[] pngBytes = File.ReadAllBytes(song.folderPath + "/" + song.songCover);
            loadedIMG.LoadImage(pngBytes);
            song.songCoverIMG = loadedIMG;
            offset -= songItemOffset;
            song.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, song.gameObject.GetComponent<RectTransform>().anchoredPosition.y);
        }
    }

    public void Save()
    {
        EditorSongDisplay display = songDisplay.GetComponent<EditorSongDisplay>();
        File.Delete(selectedSong.folderPath + "/info.json");
        string json = "{\"songName\" : \"";
        json += display.songNameInput.text;
        json += "\", \"songCover\" : \"";
        json += display.songCoverInput.text;
        json += "\", \"artistName\" : \"";
        json += display.artistNameInput.text;
        json += "\", \"songFile\" : \"";
        json += display.songFileInput.text;
        json += "\", \"mapper\" : \"";
        json += display.mapperInput.text;
        json += "\", \"bpm\" : ";
        json += display.BPMInput.text;
        json += ", \"firstBeatOffset\" : ";
        json += "0";
        json += ", \"id\" : \"";
        json += display.IDInput.text;
        json += "\"}";
        File.WriteAllText(selectedSong.folderPath + "/info.json", json);
        print("Saved at " + selectedSong.folderPath + "/info.json");
        File.Delete(selectedSong.folderPath + "/" + selectedDifficulty + "/difficulty.json");
        json = "{\"pixelsPerBeat\" : 32, \"reactionBeats\" : ";
        json += display.reactionBeatsInput.text;
        json += "}";
        File.WriteAllText(selectedSong.folderPath + "/" + selectedDifficulty + "/difficulty.json", json);
        print(selectedSong.folderPath + "/" + selectedDifficulty + "/difficulty.json");
        foreach(EditorSong song in FindObjectsOfType<EditorSong>()) Destroy(song.gameObject);
        StartCoroutine(WaitLoad());
        loadSong(selectedSong.folderPath).Selected();
        Destroy(tempSong);
    }
    
    private IEnumerator WaitLoad()
    {
        yield return new WaitForSeconds(1);
        LoadSongs();
    }

    public void Easy()
    {
        selectedDifficulty = "easy";
        selectedSong.SetReactionBeats(this, songDisplay.GetComponent<EditorSongDisplay>());
    }

    public void Normal()
    {
        selectedDifficulty = "normal";
        selectedSong.SetReactionBeats(this, songDisplay.GetComponent<EditorSongDisplay>());
    }
    public void Hard()
    {
        selectedDifficulty = "hard";
        selectedSong.SetReactionBeats(this, songDisplay.GetComponent<EditorSongDisplay>());
    }
    public void Harder()
    {
        selectedDifficulty = "harder";
        selectedSong.SetReactionBeats(this, songDisplay.GetComponent<EditorSongDisplay>());
    }
    public void Difficult()
    {
        selectedDifficulty = "difficult";
        selectedSong.SetReactionBeats(this, songDisplay.GetComponent<EditorSongDisplay>());
    }

    public void OpenEditor()
    {
        
    }
}
