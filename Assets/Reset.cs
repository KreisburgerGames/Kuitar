using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Reset : MonoBehaviour
{
    public GameObject mapLoaderOBJ;
    private PauseMenuManager pause;
    private bool foundPauseManager = false;

    void Update()
    {
        if(!foundPauseManager && FindFirstObjectByType<PauseMenuManager>() != null)
        {
            pause = FindFirstObjectByType<PauseMenuManager>();
            foundPauseManager = true;
            Init(pause.mapPath, pause.songFileName, pause.bpm, pause.firstBeatOffset, pause.practiceMode, pause.startOffset, pause.songFolder);
        }
    }
    public void Init(string mapPath, string songFile, float bpm, float firstBeatOffset, bool practiceMode, float startOffset, string songFolder)
    {
        Destroy(pause.gameObject);
        MapLoader mapLoader = Instantiate(mapLoaderOBJ).GetComponent<MapLoader>();
        mapLoader.songFolder = songFolder;
        mapLoader.mapPath = mapPath;
        mapLoader.songFileName = songFile;
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
