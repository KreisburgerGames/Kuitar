using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;
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

    // Start is called before the first frame update
    void Start()
    {
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
        int i = 1;
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
        if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Space))
        {
            zoomRect.localPosition = originalWaveformPos;
            anchor.localScale = originalAnchorScale;
            zoomRect.sizeDelta = originalWaveformScale;
            draw.width = drawWidth;
            draw.height = drawHeight;
            draw.Generate();
            return;
        }
        if(Mathf.Abs(Input.mouseScrollDelta.y) > 0)
        {
            if(!Input.GetKey(KeyCode.LeftControl)) Scroll();
            else Zoom();
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(audioSource.isPlaying) audioSource.Pause();
            else audioSource.UnPause();
        }
        if(Mathf.Abs(Input.mouseScrollDelta.x) > 0)
        {
            if(Input.GetKey(KeyCode.LeftControl)) WaveformScroll();
        }
        if(Input.GetKeyDown(KeyCode.LeftArrow)) IncrementLeft();
        if(Input.GetKeyDown(KeyCode.RightArrow)) IncrementRight();
        noteParent.transform.position = new Vector3(metersPerSecond * audioSource.time, 0, 0);
        if (audioSource.clip != null)
        {
            float audioTime = audioSource.time;
            TrackerGoToSong(audioTime);
        }
    }

    void TrackerGoToSong(float audioTime)
    {
        float songPercentage = audioTime / audioSource.clip.length;
        float beatOffset = zoomRect.sizeDelta.x * (secondsPerBeat/audioSource.clip.length);
        tracker.anchoredPosition = new Vector2((songPercentage * zoomRect.sizeDelta.x) + beatOffset, tracker.anchoredPosition.y);
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
        float increment = secondsPerBeat/scrollSnapIncrement * 1;
        if(audioSource.time + increment < 0) audioSource.time = 0;
        if(audioSource.time + increment > audioSource.clip.length) audioSource.time = audioSource.clip.length;
        else audioSource.time += increment;
    }

    void IncrementLeft()
    {
        float increment = secondsPerBeat/scrollSnapIncrement * -1;
        if(audioSource.time + increment < 0) audioSource.time = 0;
        if(audioSource.time + increment > audioSource.clip.length) audioSource.time = audioSource.clip.length;
        else audioSource.time += increment;
    }

    void WaveformScroll()
    {
        float increment = Input.mouseScrollDelta.x * waveformScrollIncrement;
        zoomRect.localPosition = (Vector2)zoomRect.localPosition + (Vector2.right * increment);
    }

    void Scroll()
    {
        float increment = secondsPerBeat/scrollSnapIncrement * Input.mouseScrollDelta.y;
        if(audioSource.time + increment < 0) audioSource.time = 0;
        if(audioSource.time + increment > audioSource.clip.length) audioSource.time = audioSource.clip.length;
        else audioSource.time += increment;
    }
}
