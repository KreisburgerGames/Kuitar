using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Threading.Tasks;
using System;
using Unity.VisualScripting;
using System.Threading;
using TMPro;
using UnityEngine.SceneManagement;

public class SongListEditor : MonoBehaviour
{
    public Transform songsList;
    public GameObject songItem;
    public float songItemOffset = 170f;
    public EditorSong selectedSong;
    public GameObject songDisplay;
    public string selectedDifficulty;
    private GameObject tempSong;
    public Color unselectedColor;
    public Color selectedColor;
    public Color disabledColor;
    public Image easy;
    public Image normal;
    public Image hard;
    public Image harder;
    public Image difficult;
    public GameObject easyAdd;
    public GameObject normalAdd;
    public GameObject hardAdd;
    public GameObject harderAdd;
    public GameObject difficultAdd;
    public GameObject easyRemove;
    public GameObject normalRemove;
    public GameObject hardRemove;
    public GameObject harderRemove;
    public GameObject difficultRemove;
    private EditorSongDisplay editorSongDisplay;
    public GameObject newSongPrefab;
    public GameObject newSongScreen;
    public TMP_InputField newFolderInput;
    public GameObject deleteConfirmScreen;
    public string currentMapPath;
    public GameObject deleteDifficulty;
    public string difficultyToDelete;

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    // Start is called before the first frame update
    void Start()
    {
        editorSongDisplay = songDisplay.GetComponent<EditorSongDisplay>();
        LoadSongs();
    }
    
    void Update()
    {
        ColorButtons();
        CheckDeletions();
    }

    public void ToggleDeleteMap()
    {
        if(deleteDifficulty.activeSelf) deleteDifficulty.SetActive(false); else deleteDifficulty.SetActive(true);
    }

    public void DeleteSelectedMap()
    {
        StartCoroutine(OnDelete());
    }

    private IEnumerator OnDelete()
    {
        FindFirstObjectByType<EditorSongDisplay>().gameObject.SetActive(false);
        Destroy(selectedSong.gameObject);
        selectedSong = null;
        if(File.Exists(currentMapPath + ".meta")) File.Delete(currentMapPath + ".meta");
        DirectoryInfo directory = new DirectoryInfo(currentMapPath);
        File.SetAttributes(currentMapPath, FileAttributes.Normal);
        directory.Delete(true);
        while(Directory.Exists(currentMapPath)) Thread.Sleep(0);
        currentMapPath = "";
        ToggleDeleteConfrim();
        foreach(EditorSong song in FindObjectsOfType<EditorSong>()) Destroy(song.gameObject);
        yield return new WaitForEndOfFrame();
        Thread.Sleep(10);
        LoadSongs();
    }

    public void ToggleDeleteConfrim()
    {
        if(deleteConfirmScreen.activeSelf) deleteConfirmScreen.SetActive(false); else deleteConfirmScreen.SetActive(true);
    }

    public void ToggleNewSong()
    {
        if(newSongScreen.activeSelf) newSongScreen.SetActive(false); else newSongScreen.SetActive(true);
    }

    async public void NewSong()
    {
        string folderPath = Application.streamingAssetsPath + "/Songs/" + newFolderInput.text;
        if(Directory.Exists(folderPath)) return;
        Directory.CreateDirectory(folderPath);
        string infoJsonTemplate = "{\"songName\" : \"Song Name\", \"songCover\" : \"cover.png\", \"artistName\" : \"Song Artist\", \"songFile\" : \"song.ogg\", \"mapper\" : \"Mapper\", \"bpm\" : 100, \"firstBeatOffset\" : 0, \"id\" : \"NewSong.YourUniqueID\"}";
        await File.WriteAllTextAsync(folderPath + "/info.json", infoJsonTemplate);
        string mapPath = folderPath + "/easy";
        await Task.Run(() => Directory.CreateDirectory(mapPath));
        string difficultyJsonTemplate = "{\"pixelsPerBeat\" : 32, \"reactionBeats\" : 8}";
        await File.WriteAllTextAsync(mapPath + "/difficulty.json", difficultyJsonTemplate);
        string notesJsonTemplate = "{\"notes\" : []}";
        await File.WriteAllTextAsync(mapPath + "/notes.json", notesJsonTemplate);
        ToggleNewSong();
        foreach(EditorSong song in FindObjectsOfType<EditorSong>()) Destroy(song.gameObject);
        StartCoroutine(WaitLoad());
    }

    void ColorButtons()
    {
        if(easy.gameObject.GetComponent<Button>().interactable) { if(selectedDifficulty == "easy") easy.color = selectedColor; else easy.color = unselectedColor; easyAdd.SetActive(false);} else {easy.color = disabledColor; easyAdd.SetActive(true);}
        if(normal.gameObject.GetComponent<Button>().interactable) { if(selectedDifficulty == "normal") normal.color = selectedColor; else normal.color = unselectedColor; normalAdd.SetActive(false);} else {normal.color = disabledColor; normalAdd.SetActive(true);}
        if(hard.gameObject.GetComponent<Button>().interactable) { if(selectedDifficulty == "hard") hard.color = selectedColor; else hard.color = unselectedColor; hardAdd.SetActive(false);} else {hard.color = disabledColor; hardAdd.SetActive(true);}
        if(harder.gameObject.GetComponent<Button>().interactable) { if(selectedDifficulty == "harder") harder.color = selectedColor; else harder.color = unselectedColor; harderAdd.SetActive(false);} else {harder.color = disabledColor; harderAdd.SetActive(true);}
        if(difficult.gameObject.GetComponent<Button>().interactable) { if(selectedDifficulty == "difficult")difficult.color = selectedColor; else difficult.color = unselectedColor; difficultAdd.SetActive(false);} else {difficult.color = disabledColor; difficultAdd.SetActive(true);}
    }

    void CheckDeletions()
    {
        if(easy.gameObject.GetComponent<Button>().interactable)
        {
            bool onlyDifficulty = true;
            if(normal.gameObject.GetComponent<Button>().interactable) onlyDifficulty = false;
            if(hard.gameObject.GetComponent<Button>().interactable) onlyDifficulty = false;
            if(harder.gameObject.GetComponent<Button>().interactable) onlyDifficulty = false;
            if(difficult.gameObject.GetComponent<Button>().interactable) onlyDifficulty = false;
            if(!onlyDifficulty) easyRemove.SetActive(true); else easyRemove.SetActive(false);
        } else easyRemove.SetActive(false);
        if(normal.gameObject.GetComponent<Button>().interactable)
        {
            bool onlyDifficulty = true;
            if(easy.gameObject.GetComponent<Button>().interactable) onlyDifficulty = false;
            if(hard.gameObject.GetComponent<Button>().interactable) onlyDifficulty = false;
            if(harder.gameObject.GetComponent<Button>().interactable) onlyDifficulty = false;
            if(difficult.gameObject.GetComponent<Button>().interactable) onlyDifficulty = false;
            if(!onlyDifficulty) normalRemove.SetActive(true); else normalRemove.SetActive(false);
        } else normalRemove.SetActive(false);
        if(hard.gameObject.GetComponent<Button>().interactable)
        {
            bool onlyDifficulty = true;
            if(normal.gameObject.GetComponent<Button>().interactable) onlyDifficulty = false;
            if(easy.gameObject.GetComponent<Button>().interactable) onlyDifficulty = false;
            if(harder.gameObject.GetComponent<Button>().interactable) onlyDifficulty = false;
            if(difficult.gameObject.GetComponent<Button>().interactable) onlyDifficulty = false;
            if(!onlyDifficulty) hardRemove.SetActive(true); else hardRemove.SetActive(false);
        } else hardRemove.SetActive(false);
        if(harder.gameObject.GetComponent<Button>().interactable)
        {
            bool onlyDifficulty = true;
            if(normal.gameObject.GetComponent<Button>().interactable) onlyDifficulty = false;
            if(hard.gameObject.GetComponent<Button>().interactable) onlyDifficulty = false;
            if(easy.gameObject.GetComponent<Button>().interactable) onlyDifficulty = false;
            if(difficult.gameObject.GetComponent<Button>().interactable) onlyDifficulty = false;
            if(!onlyDifficulty) harderRemove.SetActive(true); else harderRemove.SetActive(false);
        } else harderRemove.SetActive(false);
        if(difficult.gameObject.GetComponent<Button>().interactable)
        {
            bool onlyDifficulty = true;
            if(normal.gameObject.GetComponent<Button>().interactable) onlyDifficulty = false;
            if(hard.gameObject.GetComponent<Button>().interactable) onlyDifficulty = false;
            if(harder.gameObject.GetComponent<Button>().interactable) onlyDifficulty = false;
            if(easy.gameObject.GetComponent<Button>().interactable) onlyDifficulty = false;
            if(!onlyDifficulty) difficultRemove.SetActive(true); else difficultRemove.SetActive(false);
        } else difficultRemove.SetActive(false);
    }

    async public void AddDifficulty(string difficulty)
    {
        string newPath = selectedSong.folderPath + "/" + difficulty;
        await Task.Run(() => Directory.CreateDirectory(newPath));
        string difficultyJsonTemplate = "{\"pixelsPerBeat\" : 32, \"reactionBeats\" : 8}";
        await File.WriteAllTextAsync(newPath + "/difficulty.json", difficultyJsonTemplate);
        string notesJsonTemplate = "{\"notes\" : []}";
        await File.WriteAllTextAsync(newPath + "/notes.json", notesJsonTemplate);
        selectedSong.Selected();
    }

    public void DeleteDifficulty(string difficulty)
    {
        difficultyToDelete = difficulty;
        ToggleDeleteMap();
    }

    public void DeleteDifficultyConfirm()
    {
        string newPath = selectedSong.folderPath + "/" + difficultyToDelete;
        print(newPath);
        if(File.Exists(newPath + ".meta")) File.Delete(newPath + ".meta");
        DirectoryInfo directory = new DirectoryInfo(newPath);
        File.SetAttributes(newPath, FileAttributes.Normal);
        directory.Delete(true);
        while (Directory.Exists(newPath)) Thread.Sleep(0);
        if(easy.gameObject.GetComponent<Button>().interactable && difficultyToDelete != "easy")  Easy();
        else if(normal.gameObject.GetComponent<Button>().interactable && difficultyToDelete != "normal") Normal();
        else if(hard.gameObject.GetComponent<Button>().interactable && difficultyToDelete != "hard") Hard();
        else if(harder.gameObject.GetComponent<Button>().interactable && difficultyToDelete != "harder") Harder();
        else if(difficult.gameObject.GetComponent<Button>().interactable && difficultyToDelete != "difficult") Difficult();
        selectedSong.Selected();
        ToggleDeleteMap();
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
        foreach(NewSong newSongRef in FindObjectsOfType<NewSong>())
        {
            Destroy(newSongRef.gameObject);
        }
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
            if(File.Exists(name + "/" + songLoad.songCover))
            {
                Texture2D loadedIMG = new Texture2D(1, 1);
                byte[] pngBytes = File.ReadAllBytes(song.folderPath + "/" + song.songCover);
                loadedIMG.LoadImage(pngBytes);
                song.songCoverIMG = loadedIMG;
            }
            offset -= songItemOffset;
            song.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, song.gameObject.GetComponent<RectTransform>().anchoredPosition.y);
        }
        GameObject newSong = Instantiate(newSongPrefab);
        newSong.transform.SetParent(songsList, false);
        newSong.transform.localPosition = new Vector2(0, offset - 150);
        newSong.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, newSong.gameObject.GetComponent<RectTransform>().anchoredPosition.y);
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
        selectedSong.Selected();
        Destroy(tempSong);
    }
    
    private IEnumerator WaitLoad()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.01f);
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
        selectedSong.SetReactionBeats(this, editorSongDisplay);
    }
    public void Hard()
    {
        selectedDifficulty = "hard";
        selectedSong.SetReactionBeats(this, editorSongDisplay);
    }
    public void Harder()
    {
        selectedDifficulty = "harder";
        selectedSong.SetReactionBeats(this, editorSongDisplay);
    }
    public void Difficult()
    {
        selectedDifficulty = "difficult";
        selectedSong.SetReactionBeats(this, editorSongDisplay);
    }

    public void OpenEditor()
    {
        DontDestroyOnLoad(selectedSong.gameObject);
        DontDestroyOnLoad(gameObject);
        StartCoroutine(OpenEditorTask());
    }

    private IEnumerator OpenEditorTask()
    {
        
        SceneManager.LoadScene("Editor");
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.01f);
        MapEditor editor = FindFirstObjectByType<MapEditor>();
        editor.songFolder = selectedSong.folderPath;
        editor.mapPath = selectedSong.folderPath + "/" + selectedDifficulty;
        editor.Init();
    }
}
