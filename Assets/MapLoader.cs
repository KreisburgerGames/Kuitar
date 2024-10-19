using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapLoader : MonoBehaviour
{
    public GameObject notePrefab;
    public Texture2D map;
    public GameObject conductorPrefab;
    public float beatsPerPixel = 0.25f;
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

    List<Note> notes= new List<Note>();

    private IEnumerator LoadLevel(string sceneName)
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
        foreach(var note in notes)
        {
            note.conductor = conductor;
            note.gameObject.transform.SetParent(GameObject.Find("Notes").transform);
            note.Init();
        }
    }

    void Start ()
    {
        GenerateMap();
    }

    void GenerateMap ()
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

                if(a != 0f)
                {
                    print("hue = " + h);
                    Note note = Instantiate(notePrefab).GetComponent<Note>();
                    note.beat = x * beatsPerPixel;
                    int lane = y + 1;
                    note.lane = lane;
                    if(a == downStrumAlpha) {note.strum = true; note.downStrum = true;}
                    else if(a == upStrumAlpha) {note.strum = true; note.downStrum = false;}
                    else {note.strum = false;}
                    string laneStr = null;
                    if(lane == 1) laneStr = "L";
                    else if(lane == 2) laneStr = "LM";
                    else if(lane == 3) laneStr = "HM";
                    else if(lane == 4) laneStr = "H";

                    if(h >= zeroHue && h < oneHue || h == 360f) note.noteStr = laneStr + "0";
                    else if(h >= oneHue && h < twoHue) note.noteStr = laneStr + "1";
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
        StartMap();
    }
    void StartMap()
    {
        StartCoroutine(LoadLevel("Game"));
    }
}
