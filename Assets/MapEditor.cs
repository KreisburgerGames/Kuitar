using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.PackageManager;
using UnityEngine;

public class MapEditor : MonoBehaviour
{
    Camera camera;
    public GameObject laneEnds;
    public float laneEdgeMargin = 1f;
    public GameObject notePrefab;
    public GameObject noteParent;
    public float metersPerSecond = 5f;
    public float zoomLevel;
    public float scrollSnapIncrement;
    public float ArrowIncrement = 32f;
    public float offset;
    public string songFolder = "";
    public string mapPath = "";
    public float position;
    private AudioSource audioSource;
    float bpm;
    float secondsPerBeat;
    private Song song;
    public float waveformZoomIncrement = 5f;
    public RectTransform zoomRect;
    public float waveformScrollIncrement = 3f;
    Vector2 originalWaveformPos;
    public Draw draw;
    public RectTransform tracker;
    public RectTransform anchor;
    Vector3 originalWaveformScale;
    Vector3 originalAnchorScale;
    public int drawWidth, drawHeight;
    public float lastPos = 0f;
    bool waveformDownPos = false;
    public List<DummyNote> selectedNotes = new List<DummyNote>();
    public int selectedNoteNumber;
    public GameObject dummyNotePrefab;
    bool selectToStrum = false;
    bool selectedDownStrum = false;
    int i = 1;

    public void OpenEditor()
    {
        
    }

    public void Init()
    {
        if(FindFirstObjectByType<SongListEditor>() != null) Destroy(FindFirstObjectByType<SongListEditor>().gameObject);
        if(FindFirstObjectByType<EditorSong>() != null) Destroy(FindFirstObjectByType<EditorSong>().gameObject);
        song = GetComponent<Song>();
        audioSource = GetComponent<AudioSource>();
        song.folderPath = songFolder;
        StreamReader reader = new StreamReader(songFolder + "/info.json");
        TextAsset jsonFile = new TextAsset(reader.ReadToEnd());
        reader.Close();
        SongLoader songLoad = JsonUtility.FromJson<SongLoader>(jsonFile.text);
        song.songFile = songLoad.songFile;
        song.EditorInit();
        bpm = songLoad.bpm;
        secondsPerBeat = 60f / bpm;
        camera = FindFirstObjectByType<Camera>();
        float edge = camera.ScreenToWorldPoint(new Vector2(camera.pixelWidth, 0)).x;
        offset = edge - laneEdgeMargin;
        laneEnds.transform.position = new Vector2(offset, 0);
        noteParent.transform.position = new Vector2(offset, 0);
        originalWaveformPos = zoomRect.localPosition;
        originalWaveformScale = zoomRect.sizeDelta;
        originalAnchorScale = anchor.localScale;
        draw.Generate();
    }

    public void Ready()
    {
        audioSource.Play();
        audioSource.Pause();
        audioSource.time = 0f;
        LoadNotes();
        draw.audioSource = audioSource;
        draw.Generate();
    }

    void LoadNotes()
    {
        StreamReader reader = new StreamReader(mapPath + "/notes.json");
        TextAsset jsonFile = new TextAsset(reader.ReadToEnd());
        reader.Close();
        Notes notesLoaded = JsonUtility.FromJson<Notes>(jsonFile.text);
        reader = new StreamReader(mapPath + "/difficulty.json");
        jsonFile = new TextAsset(reader.ReadToEnd());
        reader.Close();
        DifficultySettings difficultySettings = JsonUtility.FromJson<DifficultySettings>(jsonFile.text);
        metersPerSecond = difficultySettings.reactionBeats;
        foreach (NoteLoad noteLoad in notesLoaded.notes)
        {
            DummyNote note = Instantiate(notePrefab).GetComponent<DummyNote>();
            note.gameObject.transform.SetParent(noteParent.transform, true);
            note.gameObject.transform.position = new Vector2((noteLoad.beat * secondsPerBeat * -metersPerSecond) + offset, note.gameObject.transform.position.y);
            note.lane = noteLoad.lane;
            note.note = noteLoad.note;
            note.strum = noteLoad.strum;
            note.downStrum = noteLoad.downStrum;
            note.gameObject.name = "Note " + i.ToString();
            note.Init();
            i++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        zoomLevel = anchor.localScale.x;
        noteParent.transform.position = new Vector3(metersPerSecond * audioSource.time, 0, 0);
        if (audioSource.clip != null)
        {
            float audioTime = audioSource.time;
            TrackerGoToSong(audioTime);
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                zoomRect.localPosition = originalWaveformPos;
                anchor.localScale = originalAnchorScale;
                zoomRect.sizeDelta = originalWaveformScale;
                draw.width = drawWidth;
                draw.height = drawHeight;
                draw.Generate();
            }
            if(Input.GetKeyDown(KeyCode.D))
            {
                selectedNotes.Clear();
            }
            if (Mathf.Abs(Input.mouseScrollDelta.y) > 0) Zoom();
            if(Input.GetKeyDown(KeyCode.S)) Save();
            return;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Mathf.Abs(Input.mouseScrollDelta.y) > 0)
            {
                WaveformScroll();
            }
            if(Input.GetKeyDown(KeyCode.Space) && audioSource.isPlaying)
            {
                audioSource.Pause();
                lastPos = audioSource.time;
            }
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                zoomRect.anchoredPosition = new Vector2(zoomRect.anchoredPosition.x, -zoomRect.anchoredPosition.y);
            }
            if (Input.GetKeyDown(KeyCode.Z))
            {
                Undo.PerformUndo();
            }
            if (Input.GetKeyDown(KeyCode.Y))
            {
                Undo.PerformRedo();
            }
            return;
        }
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                zoomRect.anchoredPosition = new Vector2(zoomRect.anchoredPosition.x, -zoomRect.anchoredPosition.y);
            }
            return;
        }
        if (Mathf.Abs(Input.mouseScrollDelta.y) > 0)
        {
            Scroll();
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(audioSource.isPlaying) { audioSource.Pause(); audioSource.time = lastPos; }
            else { lastPos = audioSource.time; audioSource.UnPause(); }
        }
        if(Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Delete))
        {
            DeleteSelectedNotes();
        }
        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectToStrum = true;
            selectedDownStrum = true;
        }
        else if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectToStrum = true;
            selectedDownStrum = false;
        }else if(Input.GetKeyDown(KeyCode.X))
        {
            selectToStrum = false;
        }
        SelectNote();
        if(Input.GetKeyDown(KeyCode.LeftArrow)) IncrementLeft();
        if(Input.GetKeyDown(KeyCode.RightArrow)) IncrementRight();
    }

    void Save()
    {
        File.Delete(mapPath + "/notes.json");
        string json = "{\"notes\": [ ";
        foreach(DummyNote note in noteParent.GetComponentsInChildren<DummyNote>())
        {
            json += " {";
            float beat = (Mathf.Abs(note.transform.localPosition.x) + offset) / metersPerSecond / secondsPerBeat;
            json += "\"beat\" : " + beat.ToString() + ", ";
            int lane = note.lane;
            json += "\"lane\" : " + lane.ToString() + ", ";
            if(note.strum)
            {
                if(note.downStrum) {json += "\"strum\" : true, "; json += "\"downStrum\" : true, ";}
                else {json += "\"strum\" : true, "; json += "\"downStrum\" : false, ";}
            }
            else json += "\"strum\" : false, "; json += "\"downStrum\" : false, ";
            int noteNum = note.note;
            json += "\"note\" : "  + noteNum.ToString() + "}, ";
        }
        json = json.Remove(json.Length - 2, 1);
        json += "] }";
        File.WriteAllText(mapPath + "/notes.json", json);
        print("Saved at " + mapPath + "/notes.json");
    }

    void DeleteSelectedNotes()
    {
        foreach (DummyNote note in selectedNotes)
        {
            note.selected = false;
            Undo.DestroyObjectImmediate(note.gameObject);
        }
        selectedNotes.Clear();
    }

    void SelectNote()
    {
        if(Input.GetKeyDown(KeyCode.C)) selectedNoteNumber = 0;
        else if(Input.GetKeyDown(KeyCode.Alpha1)) selectedNoteNumber = 1;
        else if(Input.GetKeyDown(KeyCode.Alpha2)) selectedNoteNumber = 2;
        else if(Input.GetKeyDown(KeyCode.Alpha3)) selectedNoteNumber = 3;
        else if(Input.GetKeyDown(KeyCode.Alpha4)) selectedNoteNumber = 4;
        else if(Input.GetKeyDown(KeyCode.Alpha5)) selectedNoteNumber = 5;
        else if(Input.GetKeyDown(KeyCode.Alpha6)) selectedNoteNumber = 6;
        else if(Input.GetKeyDown(KeyCode.Alpha7)) selectedNoteNumber = 7;
        else if(Input.GetKeyDown(KeyCode.Alpha8)) selectedNoteNumber = 8;
        else if(Input.GetKeyDown(KeyCode.Alpha9)) selectedNoteNumber = 9;
        else if(Input.GetKeyDown(KeyCode.Alpha0)) selectedNoteNumber = 10;
    }

    public void PlaceNote(int lane)
    {
        DummyNote note = Instantiate(notePrefab).GetComponent<DummyNote>();
        note.gameObject.transform.SetParent(noteParent.transform, true);
        note.gameObject.transform.localPosition = new Vector2((audioSource.time * -metersPerSecond) + offset, note.gameObject.transform.localPosition.y);
        note.lane = lane;
        note.note = selectedNoteNumber;
        if(!selectToStrum) note.strum = false;
        else
        {
            if(selectedDownStrum) { note.strum = true; note.downStrum = true; }
            else { note.strum = true; note.downStrum = false; }
        }
        note.gameObject.name = "Note " + i.ToString();
        i++;
        note.Init();
        Undo.RegisterCreatedObjectUndo(note.gameObject, "Note " + i.ToString());
    }

    void TrackerGoToSong(float audioTime)
    {
        float songPercentage = audioTime / audioSource.clip.length;
        float beatOffset = zoomRect.sizeDelta.x * (secondsPerBeat/audioSource.clip.length);
        float widthOffset = tracker.sizeDelta.x / 2f;
        tracker.anchoredPosition = new Vector2((songPercentage * zoomRect.sizeDelta.x) + beatOffset - widthOffset, tracker.anchoredPosition.y);
    }

    void Zoom()
    {
        float increment = Input.mouseScrollDelta.y * waveformZoomIncrement;
        float newX = anchor.localScale.x + increment;
        newX = Mathf.Clamp(newX, 1f, 30f);
        anchor.localScale = new Vector3(newX, anchor.localScale.y, 1f);
    }

    void IncrementRight()
    {
        float increment = secondsPerBeat/ArrowIncrement * 1;
        if(audioSource.time + increment < 0) audioSource.time = 0;
        if(audioSource.time + increment > audioSource.clip.length) audioSource.time = audioSource.clip.length;
        else audioSource.time += increment;
        lastPos = audioSource.time;
    }

    void IncrementLeft()
    {
        float increment = secondsPerBeat/ArrowIncrement * -1;
        if(audioSource.time + increment < 0) audioSource.time = 0;
        if(audioSource.time + increment > audioSource.clip.length) audioSource.time = audioSource.clip.length;
        else audioSource.time += increment;
        lastPos = audioSource.time;
    }

    void WaveformScroll()
    {
        float increment = Input.mouseScrollDelta.y * waveformScrollIncrement;
        zoomRect.localPosition = (Vector2)zoomRect.localPosition + (Vector2.right * increment);
    }

    void Scroll()
    {
        float increment = secondsPerBeat/scrollSnapIncrement * Input.mouseScrollDelta.y;
        if(audioSource.time + increment < 0) audioSource.time = 0;
        if(audioSource.time + increment > audioSource.clip.length) audioSource.time = audioSource.clip.length;
        else audioSource.time += increment;
        lastPos = audioSource.time;
    }
}
