using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyNote : MonoBehaviour
{
    public SpriteRenderer number;
    public int note;
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

    public void Init()
    {
        Bg = GetComponent<SpriteRenderer>();
        if(note == 0) number.sprite = NoteZeroIMG;
        else if(note == 1) number.sprite = NoteOneIMG;
        else if(note == 2) number.sprite = NoteTwoIMG;
        else if(note == 3) number.sprite = NoteThreeIMG;
        else if(note == 4) number.sprite = NoteFourIMG;
        else if(note == 5) number.sprite = NoteFiveIMG;
        else if(note == 6) number.sprite = NoteSixIMG;
        else if(note == 7) number.sprite = NoteSevenIMG;
        else if(note == 8) number.sprite = NoteEightIMG;
        else if(note == 9) number.sprite = NoteNineIMG;
        else if(note == 10) number.sprite = NoteTenIMG;

        if(strum)
        {
            if(!downStrum) Bg.sprite = UpStrumIMG;
            else Bg.sprite = DownStrumIMG;
        }
        else Bg.sprite = regularBGIMG;

        if(lane == 1) transform.position = new Vector2(transform.position.x, GameObject.Find("Lane Ends/Lane 1 End").transform.position.y);
        else if(lane == 2) transform.position = new Vector2(transform.position.x, GameObject.Find("Lane Ends/Lane 2 End").transform.position.y);
        else if(lane == 3) transform.position = new Vector2(transform.position.x, GameObject.Find("Lane Ends/Lane 3 End").transform.position.y);
        else if(lane == 4) transform.position = new Vector2(transform.position.x, GameObject.Find("Lane Ends/Lane 4 End").transform.position.y);
    }
}