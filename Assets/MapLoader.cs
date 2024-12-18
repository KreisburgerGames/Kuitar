using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

public class MapLoader : MonoBehaviour
{
    public GameObject notePrefab;
    public List<Texture2D> maps = new List<Texture2D>();
    public GameObject conductorPrefab;
    public int pixelsPerBeat;
    [Range(0f, 1f)]
    public float downStrumAlpha = 0.5f;
    [Range(0f, 1f)]
    public float upStrumAlpha = 0.75f;
    public int zeroHue = 0;
    public int oneHue = 30;
    public int twoHue = 60;
    public int threeHue = 90;
    public int fourHue = 120;
    public int fiveHue = 150;
    public int sixHue = 180;
    public int sevenHue = 210;
    public int eightHue = 240;
    public int nineHue = 270;
    public int tenHue = 300;
    private int xOffset = 0;
    List<Note> notes= new List<Note>();
    public string songFolder;
    public string songFileName;
    public string songName;
    public float bpm;
    public float firstBeatOffset;
    public string mapPath;
    public float reactionBeats;
    public bool practiceMode = false;
    public float startOffset;
    public float noteRewindTime = 2f;
    public float laneEdgeMargin = 1f;

    // Thanks random unity form guy
    private IEnumerator LoadLevel(string sceneName, AudioClip song, float bpm, float firstBeatOffset)
    {
        // Start loading the scene
        foreach(AudioSource source in FindObjectsOfType<AudioSource>())
        {
            Destroy(source);
        }
        
        AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        // Wait until the level finish loading
        while (!asyncLoadLevel.isDone)
            yield return null;
        // Wait a frame so every Awake and Start method is called
        yield return new WaitForEndOfFrame();
        GameObject conductorGO = (GameObject)Instantiate(conductorPrefab, SceneManager.GetSceneByName("Game"));
        Conductor conductor = conductorGO.GetComponent<Conductor>();
        conductor.notesParent = GameObject.Find("Notes");
        conductor.gameObject.GetComponent<AudioSource>().clip = song;
        conductor.songStartOffset = startOffset;
        conductor.songBPM = bpm;
        conductor.reactionBeats = reactionBeats;
        conductor.firstBeatOffset = firstBeatOffset;
        conductor.rewindDistance = noteRewindTime;
        conductor.laneEdgeMargin = laneEdgeMargin;
        conductor.comboText = GameObject.FindGameObjectWithTag("Combo Text").GetComponent<TMP_Text>();
        conductor.multiplierText = GameObject.FindGameObjectWithTag("Multiplier Text").GetComponent<TMP_Text>();
        conductor.scoreText = GameObject.FindGameObjectWithTag("Score Text").GetComponent<TMP_Text>();
        conductor.accuracyText = GameObject.FindGameObjectWithTag("Accuracy Text").GetComponent<TMP_Text>();
        conductor.rankText = GameObject.FindGameObjectWithTag("Rank Text").GetComponent<TMP_Text>();
        SetPauseManager(FindFirstObjectByType<PauseMenuManager>());
        foreach(var note in notes)
        {
            note.conductor = conductor;
            note.gameObject.transform.SetParent(GameObject.Find("Notes").transform);
            note.Init();
            note.gameObject.SetActive(false);
        }
        if(SceneManager.GetActiveScene().name == "Select") SceneManager.UnloadSceneAsync("Select");
        else SceneManager.UnloadSceneAsync("Reset");
    }

    private void SetPauseManager(PauseMenuManager pause)
    {
        // Pause... :kek:
        pause.mapPath = mapPath;
        pause.songFileName = songFileName;
        pause.songName = songName;
        pause.bpm = bpm;
        pause.firstBeatOffset = firstBeatOffset;
        StreamReader reader = new StreamReader(mapPath + "difficulty.json");
        TextAsset jsonFile = new TextAsset(reader.ReadToEnd());
        DifficultySettings difficultySettings = JsonUtility.FromJson<DifficultySettings>(jsonFile.text);
        reader.Close();
        pause.pixelsPerBeat = difficultySettings.pixelsPerBeat;
        pause.reactionBeats = difficultySettings.reactionBeats;
        pause.startOffset = startOffset;
        pause.practiceMode = practiceMode;
        pause.songFolder = songFolder;
        reader = new StreamReader(songFolder + "/info.json");
        jsonFile = new TextAsset(reader.ReadToEnd());
        reader.Close();
        SongLoader songLoad = JsonUtility.FromJson<SongLoader>(jsonFile.text);
        pause.songCoverFile = songLoad.songCover;
        pause.songArtist = songLoad.artistName;
        pause.mapper = songLoad.mapper;
    }

    void Start()
    {
        if(System.IO.File.Exists(mapPath + "notes.json")) return;
        int i = 1;
        foreach(var file in System.IO.Directory.GetFiles(mapPath))
        {
            // I accidentally made an error here before and kept trying to load cover art as the map, that will destroy your memory PLEASE DONT DO IT
            if (file.EndsWith(".png"))
            {
                byte[] pngBytes = System.IO.File.ReadAllBytes(file);
                Texture2D mapToAdd = new Texture2D(4000, 4)
                {
                    name = songName + " " + i
                };
                mapToAdd.LoadImage(pngBytes);
                maps.Add(mapToAdd);
                i++;
            }
        }
        DontDestroyOnLoad(gameObject);
    }

    // INIT????
    public async void Init()
    {
        string path = songFolder + "/" + songFileName;
        await LoadClip(path);
    }

    // thanks other random unity form guy
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
                    if(uwr.isDone)
                    {
                        if(System.IO.File.Exists(mapPath + "notes.json")) LoadNotes(clip);
                        else GenerateMap(clip);
                    }
                }
            }
            catch (Exception err)
            {
                Debug.Log($"{err.Message}, {err.StackTrace}");
            }
        }
        return clip;
    }

    void GenerateMap(AudioClip song)
    {
        string json = "{\"notes\": [ ";
        foreach (Texture2D map in maps)
        {
            for (int x = 0; x < map.width; x++)
            {
                for (int y = 0; y < map.height; y++)
                {
                    float h, s, v, a;
                    Color.RGBToHSV(map.GetPixel(x, y), out h, out s, out v);
                    a = map.GetPixel(x, y).a;
                    a = (float)Math.Round(a, 2);
                    h = (float)Math.Round(h * 360);
                    
                    if (a > .1f)
                    {
                        json += " {";
                        json += "\"beat\" : " + (((float)x / pixelsPerBeat) + xOffset).ToString() + ", ";
                        int lane = 4 - y + 1;
                        json += "\"lane\" : " + lane.ToString() + ", ";
                        if (a == downStrumAlpha) { json += "\"strum\" : true, "; json += "\"downStrum\" : true, ";}
                        else if (a == upStrumAlpha) { json += "\"strum\" : true, "; json += "\"downStrum\" : false, "; }
                        else { json +="\"strum\" : false, "; json += "\"downStrum\" : false, "; }

                        if (h >= zeroHue && h < oneHue || h == 360f) json += "\"note\" : 0 }, ";
                        else if (h >= oneHue && h < twoHue) json += "\"note\" : 1 }, ";
                        else if (h >= twoHue && h < threeHue) json += "\"note\" : 2 }, ";
                        else if (h >= threeHue && h < fourHue) json += "\"note\" : 3 }, ";
                        else if (h >= fourHue && h < fiveHue) json += "\"note\" : 4 }, ";
                        else if (h >= fiveHue && h < sixHue) json += "\"note\" : 5 }, ";
                        else if (h >= sixHue && h < sevenHue) json += "\"note\" : 6 }, ";
                        else if (h >= sevenHue && h < eightHue) json += "\"note\" : 7 }, ";
                        else if (h >= eightHue && h < nineHue) json += "\"note\" : 8 }, ";
                        else if (h >= nineHue && h < tenHue) json += "\"note\" : 9 }, ";
                        else if (h >= tenHue) json += "\"note\" : 10 }, ";
                    }
                }
            }
            xOffset += map.width;
        }
        json = json.Remove(json.Length - 2, 1);
        json += "] }";
        System.IO.File.WriteAllText(mapPath + "/notes.json", json);
        LoadNotes(song);
    }
    
    public void LoadNotes(AudioClip song)
    {
        StreamReader reader = new StreamReader(mapPath + "/notes.json");
        TextAsset jsonFile = new TextAsset(reader.ReadToEnd());
        reader.Close();
        Notes notesLoaded = JsonUtility.FromJson<Notes>(jsonFile.text);
        foreach (NoteLoad noteLoad in notesLoaded.notes)
        {
            float fixedRewindTime;
            bool beginning = false;
            if(startOffset - noteRewindTime < 0) fixedRewindTime = 0; else fixedRewindTime = noteRewindTime;
            if (fixedRewindTime == 0) beginning = true;
            if((noteLoad.beat * (60f/bpm)) - (reactionBeats * (60f / bpm)) >= startOffset - (60f/bpm) - fixedRewindTime || !practiceMode || beginning)
            {
                Note note = Instantiate(notePrefab).GetComponent<Note>();
                note.beat = noteLoad.beat;
                note.lane = noteLoad.lane;
                note.strum = noteLoad.strum;
                note.downStrum = noteLoad.downStrum;
                string laneStr;
                if (note.lane == 1) laneStr = "H";
                else if (note.lane == 2) laneStr = "HM";
                else if (note.lane == 3) laneStr = "LM";
                else laneStr = "L";
                note.noteStr = laneStr + noteLoad.note.ToString();
                notes.Add(note);
                DontDestroyOnLoad(note);
            }
        }
        StartMap(song); 
    }

    void StartMap(AudioClip song)
    {
        StartCoroutine(LoadLevel("Game", song, bpm, firstBeatOffset));
    }
}
