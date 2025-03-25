using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    public float beat;
    public string noteStr;
    [Header("Strum set false if 0 note")]
    public bool hammerStart;
    public bool hammerOn;
    public KeyCode prevNote;
    public bool strum;
    public bool downStrum;
    private Vector2 targetPos;
    public int index;
    private float timeToEnd;
    public Conductor conductor;
    [Header("Ignore")]
    public bool moving = false;
    Rigidbody2D rb;
    public bool canHit = false;
    public KeyCode note;
    private Vector2 vel;

    public SpriteRenderer numberSprite;
    public SpriteRenderer bgSprite;

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

    private LineRenderer lineRenderer;
    private Vector2 originalPos;

    public Note hammerDependency;
    public Note hammerRef;

    public float noteVelCalculationEndDist;
    private bool setNoteVel;
    public bool hammerString;
    [SerializeField] private LineRenderer stringLine;

    // 1 = High, 4 = Low
    [Range(1, 4)]
    public int lane;

    public void Init()
    {
        // Now slightly better!
        switch(noteStr)
        {
            case "H0":
                note = conductor.H0; lane = 1; numberSprite.sprite = NoteZeroIMG;
                break;
            case "H1":
                note = conductor.H1; lane = 1; numberSprite.sprite = NoteOneIMG;
                break;
            case "H2":
                note = conductor.H2; lane = 1; numberSprite.sprite = NoteTwoIMG;
                break;
            case "H3":
                note = conductor.H3; lane = 1; numberSprite.sprite = NoteThreeIMG;
                break;
            case "H4":
                note = conductor.H4; lane = 1; numberSprite.sprite = NoteFourIMG;
                break;
            case "H5":
                note = conductor.H5; lane = 1; numberSprite.sprite = NoteFiveIMG;
                break;
            case "H6":
                note = conductor.H6; lane = 1; numberSprite.sprite = NoteSixIMG;
                break;
            case "H7":
                note = conductor.H7; lane = 1; numberSprite.sprite = NoteSevenIMG;
                break;
            case "H8":
                note = conductor.H8; lane = 1; numberSprite.sprite = NoteEightIMG;
                break;
            case "H9":
                note = conductor.H9; lane = 1; numberSprite.sprite = NoteNineIMG;
                break;
            case "H10":
                note = conductor.H10; lane = 1; numberSprite.sprite = NoteTenIMG;
                break;
            case "HM0":
                note = conductor.HM0; lane = 2; numberSprite.sprite = NoteZeroIMG;
                break;
            case "HM1":
                note = conductor.HM1; lane = 2; numberSprite.sprite = NoteOneIMG;
                break;
            case "HM2":
                note = conductor.HM2; lane = 2; numberSprite.sprite = NoteTwoIMG;
                break;
            case "HM3":
                note = conductor.HM3; lane = 2; numberSprite.sprite = NoteThreeIMG;
                break;
            case "HM4":
                note = conductor.HM4; lane = 2; numberSprite.sprite = NoteFourIMG;
                break;
            case "HM5":
                note = conductor.HM5; lane = 2; numberSprite.sprite = NoteFiveIMG;
                break;
            case "HM6":
                note = conductor.HM6; lane = 2; numberSprite.sprite = NoteSixIMG;
                break;
            case "HM7":
                note = conductor.HM7; lane = 2; numberSprite.sprite = NoteSevenIMG;
                break;
            case "HM8":
                note = conductor.HM8; lane = 2; numberSprite.sprite = NoteEightIMG;
                break;
            case "HM9":
                note = conductor.HM9; lane = 2; numberSprite.sprite = NoteNineIMG;
                break;
            case "HM10":
                note = conductor.HM10; lane = 2; numberSprite.sprite = NoteTenIMG;
                break;
            case "LM0":
                note = conductor.LM0; lane = 3; numberSprite.sprite = NoteZeroIMG;
                break;
            case "LM1":
                note = conductor.LM1; lane = 3; numberSprite.sprite = NoteOneIMG;
                break;
            case "LM2":
                note = conductor.LM2; lane = 3; numberSprite.sprite = NoteTwoIMG;
                break;
            case "LM3":
                note = conductor.LM3; lane = 3; numberSprite.sprite = NoteThreeIMG;
                break;
            case "LM4":
                note = conductor.LM4; lane = 3; numberSprite.sprite = NoteFourIMG;
                break;
            case "LM5":
                note = conductor.LM5; lane = 3; numberSprite.sprite = NoteFiveIMG;
                break;
            case "LM6":
                note = conductor.LM6; lane = 3; numberSprite.sprite = NoteSixIMG;
                break;
            case "LM7":
                note = conductor.LM7; lane = 3; numberSprite.sprite = NoteSevenIMG;
                break;
            case "LM8":
                note = conductor.LM8; lane = 3; numberSprite.sprite = NoteEightIMG;
                break;
            case "LM9":
                note = conductor.LM9; lane = 3; numberSprite.sprite = NoteNineIMG;
                break;
            case "LM10":
                note = conductor.LM10; lane = 3; numberSprite.sprite = NoteTenIMG;
                break;
            case "L0":
                note = conductor.L0; lane = 4; numberSprite.sprite = NoteZeroIMG;
                break;
            case "L1":
                note = conductor.L1; lane = 4; numberSprite.sprite = NoteOneIMG;
                break;
            case "L2":
                note = conductor.L2; lane = 4; numberSprite.sprite = NoteTwoIMG;
                break;
            case "L3":
                note = conductor.L3; lane = 4; numberSprite.sprite = NoteThreeIMG;
                break;
            case "L4":
                note = conductor.L4; lane = 4; numberSprite.sprite = NoteFourIMG;
                break;
            case "L5":
                note = conductor.L5; lane = 4; numberSprite.sprite = NoteFiveIMG;
                break;
            case "L6":
                note = conductor.L6; lane = 4; numberSprite.sprite = NoteSixIMG;
                break;
            case "L7":
                note = conductor.L7; lane = 4; numberSprite.sprite = NoteSevenIMG;
                break;
            case "L8":
                note = conductor.L8; lane = 4; numberSprite.sprite = NoteEightIMG;
                break;
            case "L9":
                note = conductor.L9; lane = 4; numberSprite.sprite = NoteNineIMG;
                break;
            case "L10":
                note = conductor.L10; lane = 4; numberSprite.sprite = NoteTenIMG;
                break;
        }
        
        switch(lane)
        {
            case 1:
                transform.position = GameObject.Find("Lanes/Lane 1").transform.position; targetPos = GameObject.Find("Lane Ends/Lane 1 End").transform.position;
                break;
            case 2:
                transform.position = GameObject.Find("Lanes/Lane 2").transform.position; targetPos = GameObject.Find("Lane Ends/Lane 2 End").transform.position;
                break;
            case 3:
                transform.position = GameObject.Find("Lanes/Lane 3").transform.position; targetPos = GameObject.Find("Lane Ends/Lane 3 End").transform.position;
                break;
            case 4:
                transform.position = GameObject.Find("Lanes/Lane 4").transform.position; targetPos = GameObject.Find("Lane Ends/Lane 4 End").transform.position;
                break;
        }

        Vector2 scaleRef = GameObject.Find("Lanes/Lane 1").transform.lossyScale;
        transform.lossyScale.Set(scaleRef.x, scaleRef.y, 1);

        originalPos = transform.position;
        if(hammerOn)
        {
            GetNB(1);
            return;
        }
        if(strum)
        {
            if(downStrum) bgSprite.sprite = DownStrumIMG; else bgSprite.sprite = UpStrumIMG;
        }
    }

    public void CheckString()
    {
        if(!hammerOn) return;
        if(hammerDependency != null)
        {
            hammerString = true;
        }
    }

    public void GetNB(int i)
    {
        try
        {
            Note noteRef = conductor.notes[index - i];
            if(noteRef.lane == lane && noteRef.note != note && noteRef.beat < beat)
            {
                prevNote = noteRef.note;
                noteRef.hammerStart = true;
                noteRef.GetDependency(this);
                hammerRef = noteRef;
            }
            else
            {
                GetNB(i + 1);
            }
        }
        catch(ArgumentOutOfRangeException)
        {
            hammerOn = false;
        }
    }

    public void GetDependency(Note noteBefore)
    {
        hammerDependency = noteBefore;
    }

    void Start()
    {
        conductor = FindFirstObjectByType<Conductor>();
        lineRenderer = GetComponent<LineRenderer>();

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Late")
        {
            conductor.notes.Remove(this);
            conductor.MissedNote(this);
            conductor.passedNotes += 1;
            Destroy(gameObject);
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if(!moving) return;

        if(!setNoteVel)
        {
            rb.velocity = new Vector2(Conductor.instance.noteSpeed, 0);
            setNoteVel = true;
        }

        if(index == 0) print(rb.velocity.x);

        // Messy ass code, but it works well
        if(hammerStart)
        {
            try
            {
                if(!hammerDependency.moving)
                {
                    lineRenderer.SetPosition(0, Vector2.zero);
                    lineRenderer.SetPosition(1, (originalPos - (Vector2)transform.position) * 1/transform.localScale.x);
                }
                else
                {
                    if(!hammerDependency.hammerString)
                        lineRenderer.enabled = false;
                    else
                    {
                        Vector2 targetLocalPos = hammerDependency.gameObject.transform.position - transform.position;

                        lineRenderer.SetPosition(0, Vector2.zero);
                        lineRenderer.SetPosition(1, targetLocalPos * 1/transform.localScale.x);
                    }
                    /*
                    lineRenderer.SetPosition(0, Vector2.zero);
                    lineRenderer.SetPosition(1, Vector2.zero);
                    */
                }
            }
            catch(MissingReferenceException)
            {
                lineRenderer.enabled = false;
            }
        }

        if(hammerOn)
        {
            if(!hammerString)
            {
                try
                {
                    Vector2 targetLocalPos = hammerRef.gameObject.transform.position - transform.position;

                    lineRenderer.SetPosition(0, Vector2.zero);
                    lineRenderer.SetPosition(1, targetLocalPos * 1/transform.localScale.x);
                }
                catch(MissingReferenceException)
                {
                    Vector2 targetLocalPos = targetPos - (Vector2)transform.position;

                    lineRenderer.SetPosition(0, Vector2.zero);
                    lineRenderer.SetPosition(1, targetLocalPos * 1/transform.localScale.x);
                }
            }
            else
            {
                if(!hammerDependency.moving)
                {
                    lineRenderer.SetPosition(0, Vector2.zero);
                    lineRenderer.SetPosition(1, (originalPos - (Vector2)transform.position) * 1/transform.localScale.x);
                }
                else
                {
                    Vector2 targetLocalPos = hammerDependency.gameObject.transform.position - transform.position;

                    lineRenderer.SetPosition(0, Vector2.zero);
                    lineRenderer.SetPosition(1, targetLocalPos * 1/transform.localScale.x);
                }
                try
                {
                    float voidVar = hammerRef.transform.position.x;
                }
                catch(MissingReferenceException)
                {
                    stringLine.enabled = true;
                    stringLine.SetPosition(0, Vector2.zero);
                    stringLine.SetPosition(1, (targetPos - (Vector2)transform.position) * 1/transform.localScale.x);
                }
            }
        }
    }
}
