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

    public float firstDsp;

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
        conductor.firstBeatOffset = 1 + beatsPerPixel;
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
        firstDsp = (float)AudioSettings.dspTime;
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
                    print(h);
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

                    if(h == zeroHue) note.noteStr = laneStr + "0";
                    if(h == oneHue) note.noteStr = laneStr + "1";
                    if(h == twoHue) note.noteStr = laneStr + "2";
                    if(h == threeHue) note.noteStr = laneStr + "3";
                    if(h == fourHue) note.noteStr = laneStr + "4";
                    if(h == fiveHue) note.noteStr = laneStr + "5";
                    if(h == sixHue) note.noteStr = laneStr + "6";
                    if(h == sevenHue) note.noteStr = laneStr + "7";
                    if(h == eightHue) note.noteStr = laneStr + "8";
                    if(h == nineHue) note.noteStr = laneStr + "9";
                    if(h == tenHue) note.noteStr = laneStr + "10";

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
