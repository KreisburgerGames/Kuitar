using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
    public int pixelsPerBeat;
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
        songDisplayOBJ.SetActive(true);
    }

    public void StartMap(float startOffset)
    {
        MapLoader mapLoader = Instantiate(mapLoaderOBJ).GetComponent<MapLoader>();
        mapLoader.coverName = songCover;
        mapLoader.songFolder = folderPath;
        mapLoader.songFileName = songFile;
        mapLoader.songName = songName;
        mapLoader.bpm = bpm;
        mapLoader.firstBeatOffset = firstBeatOffset;
        mapLoader.pixelsPerBeat = pixelsPerBeat;
        mapLoader.Init(startOffset);
    }
}
