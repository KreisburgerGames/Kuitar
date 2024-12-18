using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    public float beat;
    public string noteStr;
    [Header("Strum set false if 0 note")]
    public bool strum;
    public bool downStrum;
    private Vector2 targetPos;
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

    public float noteVelCalculationEndDist;

    // 1 = High, 4 = Low
    [Range(1, 4)]
    public int lane;

    public void Init()
    {
        // There HAS to be another way right???
        if(noteStr == "H0") {note = conductor.H0; lane = 1; numberSprite.sprite = NoteZeroIMG; }
        if(noteStr == "H1") {note = conductor.H1; lane = 1; numberSprite.sprite = NoteOneIMG; }
        if(noteStr == "H2") {note = conductor.H2; lane = 1; numberSprite.sprite = NoteTwoIMG; }
        if(noteStr == "H3") {note = conductor.H3; lane = 1; numberSprite.sprite = NoteThreeIMG; }
        if(noteStr == "H4") {note = conductor.H4; lane = 1; numberSprite.sprite = NoteFourIMG; }
        if(noteStr == "H5") {note = conductor.H5; lane = 1; numberSprite.sprite = NoteFiveIMG; }
        if(noteStr == "H6") {note = conductor.H6; lane = 1; numberSprite.sprite = NoteSixIMG; }
        if(noteStr == "H7") {note = conductor.H7; lane = 1; numberSprite.sprite = NoteSevenIMG; }
        if(noteStr == "H8") {note = conductor.H8; lane = 1; numberSprite.sprite = NoteEightIMG; }
        if(noteStr == "H9") {note = conductor.H9; lane = 1; numberSprite.sprite = NoteNineIMG; }
        if(noteStr == "H10") {note = conductor.H10; lane = 1; numberSprite.sprite = NoteTenIMG; }

        if(noteStr == "HM0") {note = conductor.HM0; lane = 2; numberSprite.sprite = NoteZeroIMG; }
        if(noteStr == "HM1") {note = conductor.HM1; lane = 2; numberSprite.sprite = NoteOneIMG; }
        if(noteStr == "HM2") {note = conductor.HM2; lane = 2; numberSprite.sprite = NoteTwoIMG; }
        if(noteStr == "HM3") {note = conductor.HM3; lane = 2; numberSprite.sprite = NoteThreeIMG; }
        if(noteStr == "HM4") {note = conductor.HM4; lane = 2; numberSprite.sprite = NoteFourIMG; }
        if(noteStr == "HM5") {note = conductor.HM5; lane = 2; numberSprite.sprite = NoteFiveIMG; }
        if(noteStr == "HM6") {note = conductor.HM6; lane = 2; numberSprite.sprite = NoteSixIMG; }
        if(noteStr == "HM7") {note = conductor.HM7; lane = 2; numberSprite.sprite = NoteSevenIMG; }
        if(noteStr == "HM8") {note = conductor.HM8; lane = 2; numberSprite.sprite = NoteEightIMG; }
        if(noteStr == "HM9") {note = conductor.HM9; lane = 2; numberSprite.sprite = NoteNineIMG; }
        if(noteStr == "HM10") {note = conductor.HM10; lane = 2; numberSprite.sprite = NoteTenIMG; }

        if(noteStr == "LM0") {note = conductor.LM0; lane = 3; numberSprite.sprite = NoteZeroIMG; }
        if(noteStr == "LM1") {note = conductor.LM1; lane = 3; numberSprite.sprite = NoteOneIMG; }
        if(noteStr == "LM2") {note = conductor.LM2; lane = 3; numberSprite.sprite = NoteTwoIMG; }
        if(noteStr == "LM3") {note = conductor.LM3; lane = 3; numberSprite.sprite = NoteThreeIMG; }
        if(noteStr == "LM4") {note = conductor.LM4; lane = 3; numberSprite.sprite = NoteFourIMG; }
        if(noteStr == "LM5") {note = conductor.LM5; lane = 3; numberSprite.sprite = NoteFiveIMG; }
        if(noteStr == "LM6") {note = conductor.LM6; lane = 3; numberSprite.sprite = NoteSixIMG; }
        if(noteStr == "LM7") {note = conductor.LM7; lane = 3; numberSprite.sprite = NoteSevenIMG; }
        if(noteStr == "LM8") {note = conductor.LM8; lane = 3; numberSprite.sprite = NoteEightIMG; }
        if(noteStr == "LM9") {note = conductor.LM9; lane = 3; numberSprite.sprite = NoteNineIMG; }
        if(noteStr == "LM10") {note = conductor.LM10; lane = 3; numberSprite.sprite = NoteTenIMG; }

        if(noteStr == "L0") {note = conductor.L0; lane = 4; numberSprite.sprite = NoteZeroIMG; }
        if(noteStr == "L1") {note = conductor.L1; lane = 4; numberSprite.sprite = NoteOneIMG; }
        if(noteStr == "L2") {note = conductor.L2; lane = 4; numberSprite.sprite = NoteTwoIMG; }
        if(noteStr == "L3") {note = conductor.L3; lane = 4; numberSprite.sprite = NoteThreeIMG; }
        if(noteStr == "L4") {note = conductor.L4; lane = 4; numberSprite.sprite = NoteFourIMG; }
        if(noteStr == "L5") {note = conductor.L5; lane = 4; numberSprite.sprite = NoteFiveIMG; }
        if(noteStr == "L6") {note = conductor.L6; lane = 4; numberSprite.sprite = NoteSixIMG; }
        if(noteStr == "L7") {note = conductor.L7; lane = 4; numberSprite.sprite = NoteSevenIMG; }
        if(noteStr == "L8") {note = conductor.L8; lane = 4; numberSprite.sprite = NoteEightIMG; }
        if(noteStr == "L9") {note = conductor.L9; lane = 4; numberSprite.sprite = NoteNineIMG; }
        if(noteStr == "L10") {note = conductor.L10; lane = 4; numberSprite.sprite = NoteTenIMG; }
        
        if(lane == 1) transform.position = GameObject.Find("Lanes/Lane 1").transform.position; targetPos = GameObject.Find("Lane Ends/Lane 1 End").transform.position;
        if(lane == 2) transform.position = GameObject.Find("Lanes/Lane 2").transform.position; targetPos = GameObject.Find("Lane Ends/Lane 2 End").transform.position;
        if(lane == 3) transform.position = GameObject.Find("Lanes/Lane 3").transform.position; targetPos = GameObject.Find("Lane Ends/Lane 3 End").transform.position;
        if(lane == 4) transform.position = GameObject.Find("Lanes/Lane 4").transform.position; targetPos = GameObject.Find("Lane Ends/Lane 4 End").transform.position;
        if(strum)
        {
            if(downStrum) bgSprite.sprite = DownStrumIMG; else bgSprite.sprite = UpStrumIMG;
        }
    }

    void Start()
    {
        conductor = FindFirstObjectByType<Conductor>();
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
        float distFromTarget = targetPos.x - transform.position.x - .0219f;
        if(distFromTarget <= 1) return;
        double secondsAvailable = (beat * conductor.secondsPerBeat) - conductor.songPos - Time.deltaTime;
        secondsAvailable = Mathf.Clamp((float)secondsAvailable, 0, 1000);
        vel = new Vector2(distFromTarget / (float)secondsAvailable, 0);
        rb.velocity = vel;
    }
}
