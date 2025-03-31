using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EditorSong : MonoBehaviour
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
    bool practiceMode;
    public AudioClip songClip;
    public bool editor = false;
    public bool waitingForClip = false;
    public bool selected = false;
    public string id;
    // Start is called before the first frame update
    void Start()
    {
        if(editor) {return;}
        GetComponentInChildren<RawImage>().texture = songCoverIMG;
        GameObject.Find("Camera/Canvas/Scroll View/Viewport/Content/" + gameObject.name + "/Button/Name").GetComponent<TMP_Text>().text = songName;
        GameObject.Find("Camera/Canvas/Scroll View/Viewport/Content/" + gameObject.name + "/Button/Artist").GetComponent<TMP_Text>().text = artistName;
        GameObject.Find("Camera/Canvas/Scroll View/Viewport/Content/" + gameObject.name + "/Button/Mapper").GetComponent<TMP_Text>().text = mapper;
        GetSongClip();
    }

    void Update()
    {
        if(waitingForClip && songClip != null)
        {
            GetComponent<AudioSource>().clip = songClip;
            GetComponent<MapEditor>().Ready();
            waitingForClip = false;
        }
    }

    public void EditorInit()
    {
        GetSongClip();
        waitingForClip = true;
    }

    async void GetSongClip()
    {
        string path = folderPath + "/" + songFile;

        if(SystemInfo.operatingSystemFamily != OperatingSystemFamily.Windows)
        {
            path = UnityEngine.Networking.UnityWebRequest.EscapeURL(path);
            path = "file:///" + path;
        }
        
        songClip = await LoadClip(path);
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
        PlayerPrefs.SetString("EditorLS", id);
        SongListEditor songList = FindFirstObjectByType<SongListEditor>();
        songList.selectedSong = this;
        songList.currentMapPath = folderPath;
        GameObject songDisplayOBJ = songList.songDisplay;
        EditorSongDisplay songDisplay = songDisplayOBJ.GetComponent<EditorSongDisplay>();
        songDisplay.cover.texture = songCoverIMG;
        songDisplay.songName.text = songName;
        songDisplay.songArtist.text = artistName;
        songDisplay.mapper.text = mapper;
        print(folderPath + "/easy");
        songDisplay.easy.interactable = Directory.Exists(folderPath + "/easy");
        songDisplay.normal.interactable = Directory.Exists(folderPath + "/normal");
        songDisplay.hard.interactable = Directory.Exists(folderPath + "/hard");
        songDisplay.harder.interactable = Directory.Exists(folderPath + "/harder");
        songDisplay.difficult.interactable = Directory.Exists(folderPath + "/difficult");
        StreamReader reader = new StreamReader(folderPath + "/info.json");
        TextAsset jsonFile = new TextAsset(reader.ReadToEnd());
        reader.Close();
        SongLoader songLoad = JsonUtility.FromJson<SongLoader>(jsonFile.text);
        songDisplay.IDInput.text = songLoad.id;
        songDisplay.songNameInput.text = songLoad.songName;
        songDisplay.songCoverInput.text = songLoad.songCover;
        songDisplay.artistNameInput.text = songLoad.artistName;
        songDisplay.songFileInput.text = songLoad.songFile;
        songDisplay.mapperInput.text = songLoad.mapper;
        songDisplay.BPMInput.text = songLoad.bpm.ToString();
        SetReactionBeats(songList, songDisplay);
        songDisplayOBJ.SetActive(true);
    }

    public void SetReactionBeats(SongListEditor songList, EditorSongDisplay display)
    {
        DifficultySettings settings;
        if(songList.selectedDifficulty == "easy")
        {
            StreamReader reader = new StreamReader(folderPath + "/easy/difficulty.json");
            TextAsset jsonFile = new TextAsset(reader.ReadToEnd());
            settings = JsonUtility.FromJson<DifficultySettings>(jsonFile.text);
            reader.Close();
        }
        else if(songList.selectedDifficulty == "normal")
        {
            StreamReader reader = new StreamReader(folderPath + "/normal/difficulty.json");
            TextAsset jsonFile = new TextAsset(reader.ReadToEnd());
            settings = JsonUtility.FromJson<DifficultySettings>(jsonFile.text);
            reader.Close();
        }
        else if(songList.selectedDifficulty == "hard")
        {
            StreamReader reader = new StreamReader(folderPath + "/hard/difficulty.json");
            TextAsset jsonFile = new TextAsset(reader.ReadToEnd());
            settings = JsonUtility.FromJson<DifficultySettings>(jsonFile.text);
            reader.Close();
        }
        else if(songList.selectedDifficulty == "harder")
        {
            StreamReader reader = new StreamReader(folderPath + "/harder/difficulty.json");
            TextAsset jsonFile = new TextAsset(reader.ReadToEnd());
            settings = JsonUtility.FromJson<DifficultySettings>(jsonFile.text);
            reader.Close();
        }
        else
        {
            StreamReader reader = new StreamReader(folderPath + "/difficult/difficulty.json");
            TextAsset jsonFile = new TextAsset(reader.ReadToEnd());
            settings = JsonUtility.FromJson<DifficultySettings>(jsonFile.text);
            reader.Close();
        }
        display.reactionBeatsInput.text = settings.reactionBeats.ToString();
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
