using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    Vector2 originalWaveformScale;
    public Draw draw;
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
        draw.Generate();
    }

    public void Ready()
    {
        audioSource.Play();
        audioSource.Pause();
        audioSource.time = 0f;
        LoadNotes();
        FindFirstObjectByType<Draw>().clip = audioSource.clip;
        FindFirstObjectByType<Draw>().Generate();
    }

    void LoadNotes()
    {
        StreamReader reader = new StreamReader(mapPath + "/notes.json");
        TextAsset jsonFile = new TextAsset(reader.ReadToEnd());
        reader.Close();
        Notes notesLoaded = JsonUtility.FromJson<Notes>(jsonFile.text);
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
        if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Space))
        {
            zoomRect.localPosition = originalWaveformPos;
            zoomRect.sizeDelta = originalWaveformScale;
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
        noteParent.transform.position = new Vector3((metersPerSecond * audioSource.time) + offset, 0, 0);
    }

    void Zoom()
    {
        float increment = Input.mouseScrollDelta.y * waveformZoomIncrement;
        zoomRect.sizeDelta = new Vector2(zoomRect.sizeDelta.x + increment, zoomRect.sizeDelta.y);
        draw.Generate();
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
