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

    private bool calculated = false;

    // 1 = High, 4 = Low
    [Range(1, 4)]
    public int lane;

    void Start()
    {
        if(noteStr == "H0") {note = conductor.H0; lane = 1; strum = false;}
        if(noteStr == "H1") {note = conductor.H1; lane = 1;}
        if(noteStr == "H2") {note = conductor.H2; lane = 1;}
        if(noteStr == "H3") {note = conductor.H3; lane = 1;}
        if(noteStr == "H4") {note = conductor.H4; lane = 1;}
        if(noteStr == "H5") {note = conductor.H5; lane = 1;}
        if(noteStr == "H6") {note = conductor.H6; lane = 1;}
        if(noteStr == "H7") {note = conductor.H7; lane = 1;}
        if(noteStr == "H8") {note = conductor.H8; lane = 1;}
        if(noteStr == "H9") {note = conductor.H9; lane = 1;}
        if(noteStr == "H10") {note = conductor.H10; lane = 1;}

        if(noteStr == "HM0") {note = conductor.HM0; lane = 2; strum = false;}
        if(noteStr == "HM1") {note = conductor.HM1; lane = 2;}
        if(noteStr == "HM2") {note = conductor.HM2; lane = 2;}
        if(noteStr == "HM3") {note = conductor.HM3; lane = 2;}
        if(noteStr == "HM4") {note = conductor.HM4; lane = 2;}
        if(noteStr == "HM5") {note = conductor.HM5; lane = 2;}
        if(noteStr == "HM6") {note = conductor.HM6; lane = 2;}
        if(noteStr == "HM7") {note = conductor.HM7; lane = 2;}
        if(noteStr == "HM8") {note = conductor.HM8; lane = 2;}
        if(noteStr == "HM9") {note = conductor.HM9; lane = 2;}
        if(noteStr == "HM10") {note = conductor.HM10; lane = 2;}

        if(noteStr == "LM0") {note = conductor.LM0; lane = 3; strum = false;}
        if(noteStr == "LM1") {note = conductor.LM1; lane = 3;}
        if(noteStr == "LM2") {note = conductor.LM2; lane = 3;}
        if(noteStr == "LM3") {note = conductor.LM3; lane = 3;}
        if(noteStr == "LM4") {note = conductor.LM4; lane = 3;}
        if(noteStr == "LM5") {note = conductor.LM5; lane = 3;}
        if(noteStr == "LM6") {note = conductor.LM6; lane = 3;}
        if(noteStr == "LM7") {note = conductor.LM7; lane = 3;}
        if(noteStr == "LM8") {note = conductor.LM8; lane = 3;}
        if(noteStr == "LM9") {note = conductor.LM9; lane = 3;}
        if(noteStr == "LM10") {note = conductor.LM10; lane = 3;}

        if(noteStr == "L0") {note = conductor.L0; lane = 4; strum = false;}
        if(noteStr == "L1") {note = conductor.L1; lane = 4;}
        if(noteStr == "L2") {note = conductor.L2; lane = 4;}
        if(noteStr == "L3") {note = conductor.L3; lane = 4;}
        if(noteStr == "L4") {note = conductor.L4; lane = 4;}
        if(noteStr == "L5") {note = conductor.L5; lane = 4;}
        if(noteStr == "L6") {note = conductor.L6; lane = 4;}
        if(noteStr == "L7") {note = conductor.L7; lane = 4;}
        if(noteStr == "L8") {note = conductor.L8; lane = 4;}
        if(noteStr == "L9") {note = conductor.L9; lane = 4;}
        if(noteStr == "L10") {note = conductor.L10; lane = 4;}
        
        if(lane == 1) transform.position = GameObject.Find("Lane 1").transform.position; targetPos = GameObject.Find("Lane 1 End").transform.position;
        if(lane == 2) transform.position = GameObject.Find("Lane 2").transform.position; targetPos = GameObject.Find("Lane 1 End").transform.position;
        if(lane == 3) transform.position = GameObject.Find("Lane 3").transform.position; targetPos = GameObject.Find("Lane 1 End").transform.position;
        if(lane == 4) transform.position = GameObject.Find("Lane 4").transform.position; targetPos = GameObject.Find("Lane 1 End").transform.position;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(!moving) return;
        if(!calculated)
        {
            float secondsAvailable = (beat * conductor.secondsPerBeat) - conductor.songPos;
            vel = new Vector2((targetPos.x - transform.position.x) / secondsAvailable, 0);
            calculated = true;
        }
        rb.velocity = vel;
    }
}
