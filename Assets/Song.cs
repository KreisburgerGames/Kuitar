using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Song : MonoBehaviour
{
    public string folderPath;
    public string songFile;
    public string songName;
    public string artistName;
    public string songCover;
    public string mapper;
    public float bpm;
    public float firstBeatOffset;
    public Texture2D songCoverIMG;
    public GameObject mapLoaderOBJ;
    public GameObject songSelection;
    public RawImage songSelectionIMG;
    private Button button;
    // Start is called before the first frame update
    void Start()
    {
        GetComponentInChildren<RawImage>().texture = songCoverIMG;
        GameObject.Find("Camera/Canvas/Scroll View/Viewport/Content/" + gameObject.name + "/Button/Name").GetComponent<TMP_Text>().text = songName;
        GameObject.Find("Camera/Canvas/Scroll View/Viewport/Content/" + gameObject.name + "/Button/Artist").GetComponent<TMP_Text>().text = artistName;
        GameObject.Find("Camera/Canvas/Scroll View/Viewport/Content/" + gameObject.name + "/Button/Mapper").GetComponent<TMP_Text>().text = mapper;
        button = GameObject.Find("Camera/Canvas/Scroll View/Viewport/Content/" + gameObject.name + "/Button").GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Selected()
    {
        SongList songList = FindFirstObjectByType<SongList>();
        songList.selectedSong = this;
        GameObject songDisplayOBJ = songList.songDisplay;
        SongDisplay songDisplay = songDisplayOBJ.GetComponent<SongDisplay>();
        songDisplay.cover.texture = songCoverIMG;
        songDisplay.songName.text = songName;
        songDisplay.songArtist.text = artistName;
        songDisplay.mapper.text = mapper;
        songDisplay.easy.interactable = Directory.Exists(folderPath + "/easy");
        songDisplay.normal.interactable = Directory.Exists(folderPath + "/normal");
        songDisplay.hard.interactable = Directory.Exists(folderPath + "/hard");
        songDisplay.harder.interactable = Directory.Exists(folderPath + "/harder");
        songDisplay.difficult.interactable = Directory.Exists(folderPath + "/difficult");
        songDisplayOBJ.SetActive(true);
    }

    public void StartMap(float startOffset, string difficulty)
    {
        Destroy(FindFirstObjectByType<Camera>().GetComponent<AudioListener>());
        MapLoader mapLoader = Instantiate(mapLoaderOBJ).GetComponent<MapLoader>();
        mapLoader.coverName = songCover;
        mapLoader.songFolder = folderPath;
        mapLoader.mapPath = folderPath + "/" + difficulty + "/";
        mapLoader.songFileName = songFile;
        mapLoader.songName = songName;
        mapLoader.bpm = bpm;
        mapLoader.firstBeatOffset = firstBeatOffset;
        StreamReader reader = new StreamReader(mapLoader.mapPath + "difficulty.json");
        TextAsset jsonFile = new TextAsset(reader.ReadToEnd());
        DifficultySettings difficultySettings = JsonUtility.FromJson<DifficultySettings>(jsonFile.text);
        print(difficultySettings.reactionBeats + " " + difficultySettings.pixelsPerBeat);
        reader.Close();
        mapLoader.pixelsPerBeat = difficultySettings.pixelsPerBeat;
        mapLoader.reactionBeats = difficultySettings.reactionBeats;
        mapLoader.Init(startOffset);
    }
}
