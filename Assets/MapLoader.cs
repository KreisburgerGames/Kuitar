using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
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
    

    private IEnumerator LoadLevel(string sceneName, AudioClip song)
    {
        // Start loading the scene
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
        foreach(var note in notes)
        {
            note.conductor = conductor;
            note.gameObject.transform.SetParent(GameObject.Find("Notes").transform);
            note.Init();
        }
    }

    async void Start ()
    {
        string path = songFolder + "/";
        int i = 1;
        foreach(var file in System.IO.Directory.GetFiles(path))
        {
            if (file.EndsWith(".png"))
            {
                if(file == "cover.png") return;
                else
                {
                    byte[] pngBytes = System.IO.File.ReadAllBytes(file);
                    Texture2D mapToAdd = new Texture2D(4000, 4)
                    {
                        name = songName + " " + i
                    };
                    mapToAdd.LoadImage(pngBytes);
                    maps.Add(mapToAdd);
                }
            }
            i++;
        }
        path = songFolder + "/" + songFileName;
        print(path);
        await LoadClip(path);
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
                if(uwr.isDone) {GenerateMap(clip);}
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
                        print("a = " + a + " x = " + x.ToString());
                        Note note = Instantiate(notePrefab).GetComponent<Note>();
                        note.beat = ((float)x / pixelsPerBeat) + xOffset;
                        int lane = y + 1;
                        note.lane = lane;
                        if (a == downStrumAlpha) { note.strum = true; note.downStrum = true; }
                        else if (a == upStrumAlpha) { note.strum = true; note.downStrum = false; }
                        else { note.strum = false; }
                        string laneStr = null;
                        if (lane == 1) laneStr = "L";
                        else if (lane == 2) laneStr = "LM";
                        else if (lane == 3) laneStr = "HM";
                        else if (lane == 4) laneStr = "H";

                        if (h >= zeroHue && h < oneHue || h == 360f) note.noteStr = laneStr + "0";
                        else if (h >= oneHue && h < twoHue) note.noteStr = laneStr + "1";
                        else if (h >= twoHue && h < threeHue) note.noteStr = laneStr + "2";
                        else if (h >= threeHue && h < fourHue) note.noteStr = laneStr + "3";
                        else if (h >= fourHue && h < fiveHue) note.noteStr = laneStr + "4";
                        else if (h >= fiveHue && h < sixHue) note.noteStr = laneStr + "5";
                        else if (h >= sixHue && h < sevenHue) note.noteStr = laneStr + "6";
                        else if (h >= sevenHue && h < eightHue) note.noteStr = laneStr + "7";
                        else if (h >= eightHue && h < nineHue) note.noteStr = laneStr + "8";
                        else if (h >= nineHue && h < tenHue) note.noteStr = laneStr + "9";
                        else if (h >= tenHue) note.noteStr = laneStr + "10";

                        notes.Add(note);
                        DontDestroyOnLoad(note);
                    }
                }
            }
            xOffset += map.width;
        }
        StartMap(song);
    }
    void StartMap(AudioClip song)
    {
        StartCoroutine(LoadLevel("Game", song));
    }
}
