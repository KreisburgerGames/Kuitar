using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Threading;
using System.Linq;
using Unity.VisualScripting;

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
    public float arrowIncrement = 32f;
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
    public RectTransform trackerAnchor;
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
    public Scrollbar timeScrollbar;
    public TMP_Text time;
    public RectTransform editorSettings;
    public TMP_InputField noteSpacingInput;
    public TMP_InputField scrollSpeedInput;
    public TMP_InputField arrowIncrementInput;
    public TMP_Text selectedDir;
    public TMP_Text selectedNote;
    public Image waveform;
    public CommandInvoker invoker;
    public float trackerSizeScaler = 100f;
    public RectTransform tracker;
    int i = 1;
    public EditorPauseMenu pause;

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
        draw.Generate(songFolder);
        pause.songFileName = songFolder + "/" + song.songFile;
        pause.songFolder = songFolder;
    }

    public void TimeScrollbar()
    {
        if(audioSource.isPlaying) return;
        float songLength = song.songClip.length;
        float selectedTime = MathF.Round(timeScrollbar.value * songLength, 2);
        song.GetComponent<AudioSource>().time = selectedTime;
    }

    private void UpdateText()
    {
        float selectedTime = MathF.Round(audioSource.time, 2);
        float seconds = MathF.Round(selectedTime % 60, 2);
        int minutes = (int)Mathf.Floor(selectedTime / 60);
        if(seconds < 10) time.text = minutes.ToString() + " : 0" + seconds.ToString();
        else time.text = minutes.ToString() + " : " + seconds.ToString();
    }

    public void Ready()
    {
        audioSource.Play();
        audioSource.Pause();
        audioSource.time = 0f;
        LoadNotes();
        draw.audioSource = audioSource;
        draw.Generate(songFolder);
    }

    void LoadNotes()
    {
        PlaceNote.Clear();
        CommandInvoker.counter = 0;
        StreamReader reader = new StreamReader(mapPath + "/notes.json");
        TextAsset jsonFile = new TextAsset(reader.ReadToEnd());
        reader.Close();
        Notes notesLoaded = JsonUtility.FromJson<Notes>(jsonFile.text);
        foreach (NoteLoad noteLoad in notesLoaded.notes)
        {
            invoker.AddCommand(new PlaceNoteCommand(notePrefab, noteParent, noteLoad.beat * secondsPerBeat, metersPerSecond, offset, noteLoad.lane, noteLoad.note, noteLoad.strum, noteLoad.downStrum, i, secondsPerBeat, load:true));
            i++;
        }
    }

    void SetDirAndNoteText()
    {
        // Direction
        if(!selectToStrum) selectedDir.text = "Pluck";
        else if (selectedDownStrum) selectedDir.text = "Down";
        else selectedDir.text = "Up";

        // Note
        selectedNote.text = selectedNoteNumber.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateText();
        SetDirAndNoteText();
        timeScrollbar.value = audioSource.time / audioSource.clip.length;
        zoomLevel = anchor.localScale.x;
        noteParent.transform.position = new Vector3(metersPerSecond * audioSource.time, 0, 0);
        tracker.gameObject.transform.localScale = new Vector2(trackerSizeScaler/zoomLevel, tracker.gameObject.transform.localScale.y);
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
                editorSettings.anchoredPosition = new Vector2(editorSettings.anchoredPosition.x, -editorSettings.anchoredPosition.y);
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
            timeScrollbar.value = audioSource.time / audioSource.clip.length;
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

    public void NoteSpacingChanged()
    {
        metersPerSecond = float.Parse(noteSpacingInput.text);
        foreach(DummyNote note in FindObjectsOfType<DummyNote>())
        {
            Destroy(note.gameObject);
        }
        LoadNotes();
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void ScrollSpeedChanged()
    {
        scrollSnapIncrement = float.Parse(scrollSpeedInput.text);
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void arrowIncrementChanged()
    {
        arrowIncrement = int.Parse(arrowIncrementInput.text);
        EventSystem.current.SetSelectedGameObject(null);
    }

    void Save()
    {
        if(File.Exists(mapPath + "/notes.json")) File.Delete(mapPath + "/notes.json");
        string json = "{\"notes\": [ ";
        List<DummyNote> notes = new List<DummyNote>();
        foreach(DummyNote note in noteParent.GetComponentsInChildren<DummyNote>())
        {
            notes.Add(note);
        }
        notes = notes.OrderBy(x => x.beat).ToList();
        foreach(DummyNote note in notes)
        {
            json += " {";
            float beat = (Mathf.Abs(note.transform.localPosition.x) + offset + .438f) / metersPerSecond / secondsPerBeat;
            json += "\"beat\" : " + beat.ToString() + ", ";
            int lane = note.lane;
            json += "\"lane\" : " + lane.ToString() + ", ";
            if(note.strum)
            {
                if(note.downStrum) {json += "\"strum\" : true, "; json += "\"downStrum\" : true, ";}
                else {json += "\"strum\" : true, "; json += "\"downStrum\" : false, ";}
            }
            else {json += "\"strum\" : false, "; json += "\"downStrum\" : false, ";}
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
            invoker.AddCommand(new RemoveNoteCommand(notePrefab, noteParent, note.beat * secondsPerBeat, metersPerSecond, offset, note.lane, note.note, note.strum, note.downStrum, i, note.gameObject.transform.localPosition.y, secondsPerBeat, note.load));
            i--;
        }
        selectedNotes.Clear();
    }

    void SelectNote()
    {
        if(EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.tag == "Input") return;
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

    public void PlaceNoteAction(int lane)
    {
        invoker.AddCommand(new PlaceNoteCommand(notePrefab, noteParent, audioSource.time, metersPerSecond, offset, lane, selectedNoteNumber, selectToStrum, selectedDownStrum, i, secondsPerBeat));
        i++;
    }

    void TrackerGoToSong(float audioTime)
    {
        float songPercentage = audioTime / audioSource.clip.length;
        trackerAnchor.anchoredPosition = new Vector2(songPercentage * zoomRect.sizeDelta.x, trackerAnchor.anchoredPosition.y);
    }

    void Zoom()
    {
        float increment = Input.mouseScrollDelta.y * waveformZoomIncrement;
        float newX = anchor.localScale.x + increment;
        newX = Mathf.Clamp(newX, 1f, 30f);
        anchor.localScale = new Vector3(newX, anchor.localScale.y, 1f);
        draw.Generate(songFolder);
    }

    void IncrementRight()
    {
        float increment = secondsPerBeat/arrowIncrement * 1;
        if(audioSource.time + increment < 0) audioSource.time = 0;
        if(audioSource.time + increment > audioSource.clip.length) audioSource.time = audioSource.clip.length;
        else audioSource.time += increment;
        lastPos = audioSource.time;
    }

    void IncrementLeft()
    {
        float increment = secondsPerBeat/arrowIncrement * -1;
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
        float increment = secondsPerBeat * scrollSnapIncrement * Input.mouseScrollDelta.y;
        if(audioSource.time + increment < 0) audioSource.time = 0;
        if(audioSource.time + increment > audioSource.clip.length) audioSource.time = audioSource.clip.length;
        else audioSource.time += increment;
        lastPos = audioSource.time;
    }
}