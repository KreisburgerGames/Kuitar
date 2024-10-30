using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
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
    bool practiceMode;
    public AudioClip songClip;
    // Start is called before the first frame update
    void Start()
    {
        GetComponentInChildren<RawImage>().texture = songCoverIMG;
        GameObject.Find("Camera/Canvas/Scroll View/Viewport/Content/" + gameObject.name + "/Button/Name").GetComponent<TMP_Text>().text = songName;
        GameObject.Find("Camera/Canvas/Scroll View/Viewport/Content/" + gameObject.name + "/Button/Artist").GetComponent<TMP_Text>().text = artistName;
        GameObject.Find("Camera/Canvas/Scroll View/Viewport/Content/" + gameObject.name + "/Button/Mapper").GetComponent<TMP_Text>().text = mapper;
        button = GameObject.Find("Camera/Canvas/Scroll View/Viewport/Content/" + gameObject.name + "/Button").GetComponent<Button>();
        GetSongClip();
    }

    async void GetSongClip()
    {
        songClip = await LoadClip(folderPath + "/" + songFile);
        print(songClip.length);
    }

    async Task<AudioClip> LoadClip(string path)
    {
        AudioClip clip = null;
        using (UnityEngine.Networking.UnityWebRequest uwr = UnityEngine.Networking.UnityWebRequestMultimedia.GetAudioClip(path, AudioType.OGGVORBIS))
        {
            uwr.SendWebRequest();

            // wrap tasks in try/catch, otherwise it'll fail silently
            try
            {
                while (!uwr.isDone) await Task.Delay(10);

                if (uwr.isNetworkError || uwr.isHttpError) Debug.Log($"{uwr.error}");
                else
                {
                    clip = UnityEngine.Networking.DownloadHandlerAudioClip.GetContent(uwr);
                }
            }
            catch (Exception err)
            {
                Debug.Log($"{err.Message}, {err.StackTrace}");
            }
        }
        return clip;
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
        songList.selectedSongClip = songClip;
        songDisplayOBJ.SetActive(true);
        PracticeModeHandler practiceModeHandler = FindAnyObjectByType<PracticeModeHandler>();
        practiceModeHandler.selectedSong = this;
        practiceModeHandler.selectedSongClip = songClip;
        practiceModeHandler.selectedTime = 0;
        practiceModeHandler.SetTime();
        songList.audioSource.clip = songClip;
        songList.audioSource.Play();
    }

    public void StartMap(float startOffset, string difficulty)
    {
        Destroy(FindFirstObjectByType<Camera>().GetComponent<AudioListener>());
        SongList songList = FindFirstObjectByType<SongList>();
        practiceMode = songList.practiceMode;
        MapLoader mapLoader = Instantiate(mapLoaderOBJ).GetComponent<MapLoader>();
        mapLoader.songFolder = folderPath;
        mapLoader.mapPath = folderPath + "/" + difficulty + "/";
        mapLoader.songFileName = songFile;
        mapLoader.songName = songName;
        mapLoader.bpm = bpm;
        mapLoader.firstBeatOffset = firstBeatOffset;
        StreamReader reader = new StreamReader(mapLoader.mapPath + "difficulty.json");
        TextAsset jsonFile = new TextAsset(reader.ReadToEnd());
        DifficultySettings difficultySettings = JsonUtility.FromJson<DifficultySettings>(jsonFile.text);
        reader.Close();
        mapLoader.pixelsPerBeat = difficultySettings.pixelsPerBeat;
        mapLoader.reactionBeats = difficultySettings.reactionBeats;
        if(!practiceMode) startOffset = 0f;
        mapLoader.practiceMode = practiceMode;
        mapLoader.startOffset = startOffset;
        mapLoader.Init();
    }
}
