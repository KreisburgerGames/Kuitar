using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class DummyNote : MonoBehaviour, IPointerClickHandler
{
    public SpriteRenderer number;
    public int note;
    public bool hammerOn;
    public DummyNote noteBefore;
    public int lane;
    public Sprite NoteZeroIMG;
    public Sprite NoteOneIMG;
    public Sprite NoteTwoIMG;
    public Sprite NoteThreeIMG;
    public Sprite NoteFourIMG;
    public Sprite NoteFiveIMG;
    public Sprite NoteSixIMG;
    public Sprite NoteSevenIMG;
    public Sprite NoteEightIMG;
    public Sprite NoteNineIMG;
    public Sprite NoteTenIMG;
    public Sprite DownStrumIMG;
    public Sprite UpStrumIMG;
    public Sprite regularBGIMG;
    public bool strum;
    public bool downStrum;
    SpriteRenderer Bg;
    MapEditor mapEditor;
    public GameObject selectedHighlight;
    public bool selected = false;
    public float beat;
    public int index;
    public Transform parent;
    public Vector2 position;
    public bool history = false;
    public bool load = false;
    public DummyNote hammerDependency;
    public bool latencySet;
    [SerializeField] private LineRenderer lineRenderer;

    public void Init()
    {
        Bg = GetComponent<SpriteRenderer>();
        switch (note)
        {
            case 0:
                number.sprite = NoteZeroIMG;
                break;
            case 1:
                number.sprite = NoteOneIMG;
                break;
            case 2:
                number.sprite = NoteTwoIMG;
                break;
            case 3:
                number.sprite = NoteThreeIMG;
                break;
            case 4:
                number.sprite = NoteFourIMG;
                break;
            case 5:
                number.sprite = NoteFiveIMG;
                break;
            case 6:
                number.sprite = NoteSixIMG;
                break;
            case 7:
                number.sprite = NoteSevenIMG;
                break;
            case 8:
                number.sprite = NoteEightIMG;
                break;
            case 9:
                number.sprite = NoteNineIMG;
                break;
            case 10:
                number.sprite = NoteTenIMG;
                break;
        }

        if(strum)
        {
            if(!downStrum) Bg.sprite = UpStrumIMG;
            else Bg.sprite = DownStrumIMG;
        }
        else Bg.sprite = regularBGIMG;
        
        switch(lane)
        {
            case 1:
                transform.position = new Vector2(transform.position.x, GameObject.Find("Lane Ends/Lane 1 End").transform.position.y);
                break;
            case 2:
                transform.position = new Vector2(transform.position.x, GameObject.Find("Lane Ends/Lane 2 End").transform.position.y);
                break;
            case 3:
                transform.position = new Vector2(transform.position.x, GameObject.Find("Lane Ends/Lane 3 End").transform.position.y);
                break;
            case 4:
                transform.position = new Vector2(transform.position.x, GameObject.Find("Lane Ends/Lane 4 End").transform.position.y);
                break;
        }

        Vector2 scaleRef = GameObject.Find("Lane Ends/Lane 1 End").transform.lossyScale;
        transform.lossyScale.Set(scaleRef.x, scaleRef.y, 1f);

        mapEditor = FindFirstObjectByType<MapEditor>();
    }

    void Update()
    {
        if(history) return;
        selected = false;
        if (mapEditor == null) mapEditor = FindFirstObjectByType<MapEditor>();
        if(CheckSlelected()) selectedHighlight.SetActive(true); else selectedHighlight.SetActive(false);
    }

    private bool CheckSlelected()
    {
        foreach (DummyNote note in mapEditor.selectedNotes)
        {
            if (note == this)
            {
                return true;
            }
        }
        return false;
    }

    public void GetNB(int i)
    {
        DummyNote noteRef = mapEditor.loadedNotes[index - i];
        if (noteRef.lane == lane && noteRef.note != note && noteRef.beat < beat)
        {
            noteBefore = noteRef;
        }
        else
        {
            GetNB(i + 1);
        }
    }

    public void CheckHammers()
    {
        if(hammerOn)
        {
            GetNB(1);
            noteBefore.hammerDependency = this;
            Vector2 localDiff = noteBefore.gameObject.transform.position - transform.position;

            lineRenderer.SetPosition(0, Vector2.zero);
            lineRenderer.SetPosition(1, localDiff * 1/transform.localScale.x);
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }

    public void MakeHammer(DummyNote previousNote)
    {
        hammerOn = true;
        noteBefore = previousNote;
        noteBefore.hammerDependency = this;
        Vector2 localDiff = noteBefore.gameObject.transform.position - transform.position;

        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, Vector2.zero);
        lineRenderer.SetPosition(1, localDiff * 1/transform.localScale.x);
    }

    public void UnmakeHammer()
    {
        noteBefore.hammerDependency = null;
        hammerOn = false;
        noteBefore = null;
        lineRenderer.SetPosition(0, Vector2.zero);
        lineRenderer.SetPosition(1, Vector2.zero);
        lineRenderer.enabled = false;
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if(Input.GetKey(KeyCode.LeftShift))
        {
            if(!selected) mapEditor.selectedNotes.Add(this);
            else mapEditor.selectedNotes.Remove(this);
        }
        else
        {
            bool wasSelectedSingle = CheckSlelected() && mapEditor.selectedNotes.Count == 1;
            mapEditor.selectedNotes.Clear();
            if (wasSelectedSingle) return;
            else mapEditor.selectedNotes.Add(this);
        }
    }

}
