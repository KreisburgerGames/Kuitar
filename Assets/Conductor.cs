using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using JetBrains.Annotations;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Conductor : MonoBehaviour
{
    public float songBPM;
    public float secondsPerBeat;
    public float songPos;
    public float songBeatsPos;
    public float dspSongTime;
    public AudioSource song;
    public float firstBeatOffset;
    public float songStartOffset;

    [Header("Controls")]
    public KeyCode H0 = KeyCode.Backspace;
    public KeyCode H1 = KeyCode.Alpha1;
    public KeyCode H2 = KeyCode.Alpha2;
    public KeyCode H3 = KeyCode.Alpha3;
    public KeyCode H4 = KeyCode.Alpha4;
    public KeyCode H5 = KeyCode.Alpha5;
    public KeyCode H6 = KeyCode.Alpha6;
    public KeyCode H7 = KeyCode.Alpha7;
    public KeyCode H8 = KeyCode.Alpha8;
    public KeyCode H9 = KeyCode.Alpha9;
    public KeyCode H10 = KeyCode.Alpha0;
    
    public KeyCode HM0 = KeyCode.Backslash;
    public KeyCode HM1 = KeyCode.Q;
    public KeyCode HM2 = KeyCode.W;
    public KeyCode HM3 = KeyCode.E;
    public KeyCode HM4 = KeyCode.R;
    public KeyCode HM5 = KeyCode.T;
    public KeyCode HM6 = KeyCode.Y;
    public KeyCode HM7 = KeyCode.U;
    public KeyCode HM8 = KeyCode.I;
    public KeyCode HM9 = KeyCode.O;
    public KeyCode HM10 = KeyCode.P;

    public KeyCode LM0 = KeyCode.Return;
    public KeyCode LM1 = KeyCode.A;
    public KeyCode LM2 = KeyCode.S;
    public KeyCode LM3 = KeyCode.D;
    public KeyCode LM4 = KeyCode.F;
    public KeyCode LM5 = KeyCode.G;
    public KeyCode LM6 = KeyCode.H;
    public KeyCode LM7 = KeyCode.J;
    public KeyCode LM8 = KeyCode.K;
    public KeyCode LM9 = KeyCode.L;
    public KeyCode LM10 = KeyCode.Colon;

    public KeyCode L0 = KeyCode.RightShift;
    public KeyCode L1 = KeyCode.Z;
    public KeyCode L2 = KeyCode.X;
    public KeyCode L3 = KeyCode.C;
    public KeyCode L4 = KeyCode.V;
    public KeyCode L5 = KeyCode.B;
    public KeyCode L6 = KeyCode.B;
    public KeyCode L7 = KeyCode.N;
    public KeyCode L8 = KeyCode.M;
    public KeyCode L9 = KeyCode.Comma;
    public KeyCode L10 = KeyCode.Period;
    public KeyCode DownStrum = KeyCode.UpArrow;
    public KeyCode UpStrum = KeyCode.DownArrow;

    public List<Note> notes = new List<Note>();

    public float reactionBeats = 5f;

    private Note previousNote;

    public GameObject notesParent;

    public KeyCode currentH;
    public KeyCode currentHM;
    public KeyCode currentLM;
    public KeyCode currentL;
    public int accuracyRoundingDigits = 3;
    public float missDistance;
    public int score = 0;
    bool isStrumming = false;

    public int roundToOneHundredRange = 95;

    public float startDelay;

    public GameObject perfectHit;
    public GameObject goodHit;
    public GameObject decentHit;
    public int perfectHitScore = 100;
    public int goodHitScore = 75;
    public int decentHitScore = 50;
    public int mehHitScore = 30;
    public float rewindDistance = 2f;

    // Start is called before the first frame update
    void Start()
    {
        //Load the AudioSource attached to the Conductor GameObject
        song = GetComponent<AudioSource>();

        //Calculate the number of seconds in each beat
        secondsPerBeat = 60f / songBPM;

        
        if(firstBeatOffset > 0){
            float clampedOffset = Mathf.Clamp(songStartOffset, 0, rewindDistance);
            song.time = songStartOffset - (rewindDistance * (clampedOffset/firstBeatOffset));
        }
        else if(songStartOffset - rewindDistance > 0)
        {
            song.time = songStartOffset - rewindDistance;
        }
        else
        {
            song.time = 0f;
        }

        //Start the music
        song.Play();

        foreach(Note note in notesParent.GetComponentsInChildren<Note>())
        {
            notes.Add(note);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //determine how many seconds since the song started
        songPos = song.time - firstBeatOffset;

        //determine how many beats since the song started
        songBeatsPos = songPos / secondsPerBeat;

        List<Note> readyNotes = new List<Note>();

        DetermineKeyStrokes();

        foreach (Note note in notes.ToArray())
        {
            if (note.beat <= songBeatsPos + reactionBeats)
            {
                readyNotes.Add(note);
                note.gameObject.SetActive(true);
                note.moving = true;
            }
            else note.gameObject.SetActive(false);
        }
        if(readyNotes.Count == 0) return;
        List<Note> hittingNotes = new List<Note>();
        isStrumming = false;
        if(readyNotes[0].strum)
        {
            isStrumming = true;
            foreach(Note note2 in readyNotes)
            {
                if(previousNote != null)
                {
                    if(note2.beat == previousNote.beat)
                    {
                        hittingNotes.Add(note2);
                        previousNote = note2;
                    }
                    else
                    {
                        break;
                    }
                }
                else {hittingNotes.Add(note2); previousNote = note2; }
            }
            // Enforce same strum direction
            foreach(Note newNote in hittingNotes) newNote.downStrum = hittingNotes[0].downStrum;
        }
        else
        {
            isStrumming = false;
            foreach(Note note2 in readyNotes)
            {
                if(previousNote != null)
                {
                    if(note2.beat == previousNote.beat)
                    {
                        hittingNotes.Add(note2);
                        previousNote = note2;
                    }
                    else
                    {
                        break;
                    }
                }
                else {hittingNotes.Add(note2); previousNote = note2; }
            }
        }
        if(isStrumming)
        {
            if(Input.GetKeyDown(DownStrum) || Input.GetKeyDown(UpStrum))
            {
                // I have no fucking clue why this works but if I don't reverse down strum to upstrum input it doesn't work
                if(hittingNotes[0].downStrum && Input.GetKey(UpStrum) || !hittingNotes[0].downStrum && Input.GetKey(DownStrum))
                {
                    foreach(Note note in hittingNotes)
                    {
                        int hitScore;
                        if(note.lane == 1)
                        {
                            if(currentH == note.note)
                            {
                                hitScore = CalculateScore(GetEndLaneX(note) - note.gameObject.transform.position.x, note.lane);
                                print(hitScore);
                                if(hitScore == 0) { return; }
                                ParticlesAndText(hitScore, note);
                                notes.Remove(note);
                                Destroy(note.gameObject);
                            }
                            else
                            {
                                print("Wrong Note!");
                                WrongNote(note);
                                notes.Remove(note);
                                Destroy(note.gameObject);
                            }
                        }
                        else if(note.lane == 2)
                        {
                            if(currentHM == note.note)
                            {
                                hitScore = CalculateScore(GetEndLaneX(note) - note.gameObject.transform.position.x, note.lane);
                                print(hitScore);
                                if(hitScore == 0) { return; }
                                ParticlesAndText(hitScore, note);
                                notes.Remove(note);
                                Destroy(note.gameObject);
                            }
                            else
                            {
                                print("Wrong Note!");
                                WrongNote(note);
                                notes.Remove(note);
                                Destroy(note.gameObject);
                            }
                        }
                        else if(note.lane == 3)
                        {
                            if(currentLM == note.note)
                            {
                                hitScore = CalculateScore(GetEndLaneX(note) - note.gameObject.transform.position.x, note.lane);
                                print(hitScore);
                                if(hitScore == 0) { return; }
                                ParticlesAndText(hitScore, note);
                                notes.Remove(note);
                                Destroy(note.gameObject);
                            }
                            else
                            {
                                print("Wrong Note!");
                                WrongNote(note);
                                notes.Remove(note);
                                Destroy(note.gameObject);
                            }
                        }
                        else if(note.lane == 4)
                        {
                            if(currentL == note.note)
                            {
                                hitScore = CalculateScore(GetEndLaneX(note) - note.gameObject.transform.position.x, note.lane);
                                print(hitScore);
                                if(hitScore == 0) { return; }
                                ParticlesAndText(hitScore, note);
                                notes.Remove(note);
                                Destroy(note.gameObject);
                            }
                            else
                            {
                                print("Wrong Note!");
                                WrongNote(note);
                                notes.Remove(note);
                                Destroy(note.gameObject);
                            }
                        }
                        
                    }
                }
                else
                {
                    foreach(Note note in hittingNotes)
                    {
                        print("Wrong Direction!");
                        WrongDirection(note);
                        notes.Remove(note);
                        Destroy(note.gameObject);
                    }
                }
            }
        }
        else
        {
            foreach(Note note in hittingNotes)
            {
                int hitScore;
                if(note.lane == 1 && Input.GetKeyDown(H0))
                {
                    if(currentH == note.note)
                    {
                        hitScore = CalculateScore(GetEndLaneX(note) - note.gameObject.transform.position.x, note.lane);
                        print(hitScore);
                        if(hitScore == 0) { return; }
                        ParticlesAndText(hitScore, note);
                        notes.Remove(note);
                        Destroy(note.gameObject);
                    }
                    else
                    {
                        print("Wrong Note!");
                        WrongNote(note);
                        notes.Remove(note);
                        Destroy(note.gameObject);
                    }
                }
                else if(note.lane == 2 && Input.GetKeyDown(HM0))
                {
                    if(currentHM == note.note)
                    {
                        hitScore = CalculateScore(GetEndLaneX(note) - note.gameObject.transform.position.x, note.lane);
                        print(hitScore);
                        if(hitScore == 0) { return; }
                        ParticlesAndText(hitScore, note);
                        notes.Remove(note);
                        Destroy(note.gameObject);
                    }
                    else
                    {
                        print("Wrong Note!");
                        WrongNote(note);
                        notes.Remove(note);
                        Destroy(note.gameObject);
                    }
                }
                else if(note.lane == 3 && Input.GetKeyDown(LM0))
                {
                    if(currentLM == note.note)
                    {
                        hitScore = CalculateScore(GetEndLaneX(note) - note.gameObject.transform.position.x, note.lane);
                        print(hitScore);
                        if(hitScore == 0) { return; }
                        ParticlesAndText(hitScore, note);
                        notes.Remove(note);
                        Destroy(note.gameObject);
                    }
                    else
                    {
                        print("Wrong Note!");
                        WrongNote(note);
                        notes.Remove(note);
                        Destroy(note.gameObject);
                    }
                }
                else if(note.lane == 4 && Input.GetKeyDown(L0))
                {
                    if(currentL == note.note)
                    {
                        hitScore = CalculateScore(GetEndLaneX(note) - note.gameObject.transform.position.x, note.lane);
                        print(hitScore);
                        if(hitScore == 0) { return; }
                        ParticlesAndText(hitScore, note);
                        notes.Remove(note);
                        Destroy(note.gameObject);
                    }
                    else
                    {
                        print("Wrong Note!");
                        WrongNote(note);
                        notes.Remove(note);
                        Destroy(note.gameObject);
                    }
                }
                
            }
        }
        previousNote = null;
    }

    float GetEndLaneX(Note note)
    {
        return GameObject.Find("Lane " + note.lane + " End").gameObject.transform.position.x;
    }

    NoteEndColorManager GetColorManager(Note note)
    {
        return GameObject.Find("Lane " + note.lane + " End").gameObject.GetComponent<NoteEndColorManager>();
    }

    int CalculateScore(float distanceFromNote, int lane)
    {
        decimal dist = (decimal)distanceFromNote;
        decimal distRounded = Math.Round(dist, accuracyRoundingDigits);
        float absDist = Mathf.Abs((float)distRounded);
        if(absDist > missDistance) {print("Miss"); return 0;}
        int hitScore = (int)Math.Round((missDistance - absDist)/missDistance * 100);
        if(hitScore > roundToOneHundredRange) hitScore = 100;
        score += hitScore;
        return hitScore;
    }

    void ParticlesAndText(int hitScore, Note note)
    {
        if(hitScore == 100) { GetColorManager(note).PerfectHit(); Instantiate(perfectHit, note.gameObject.transform.position, Quaternion.identity);}
        else if (hitScore >= goodHitScore) {GetColorManager(note).GoodHit(); Instantiate(goodHit, note.gameObject.transform.position, Quaternion.identity);}
        else if (hitScore >= decentHitScore) {GetColorManager(note).DecentHit(); Instantiate(decentHit, note.gameObject.transform.position, Quaternion.identity);}
        else if (hitScore >= mehHitScore) GetColorManager(note).MehHit();
        else GetColorManager(note).BarelyHit();
        
    }

    void WrongNote(Note note)
    {
        GetColorManager(note).Error();
    }

    void WrongDirection(Note note)
    {
        GetColorManager(note).Error();
    }

    public void MissedNote(Note note)
    {
        GetColorManager(note).Error();
    }

    void DetermineKeyStrokes()
    {
        
        if(Input.GetKey(H10)) currentH = H10;
        else if(Input.GetKey(H9)) currentH = H9;
        else if(Input.GetKey(H8)) currentH = H8;
        else if(Input.GetKey(H7)) currentH = H7;
        else if(Input.GetKey(H6)) currentH = H6;
        else if(Input.GetKey(H5)) currentH = H5;
        else if(Input.GetKey(H4)) currentH = H4;
        else if(Input.GetKey(H3)) currentH = H3;
        else if(Input.GetKey(H2)) currentH = H2;
        else if(Input.GetKey(H1)) currentH = H1;
        else currentH = H0;


        if (Input.GetKey(HM10)) currentHM = HM10;
        else if(Input.GetKey(HM9)) currentHM = HM9;
        else if(Input.GetKey(HM8)) currentHM = HM8;
        else if(Input.GetKey(HM7)) currentHM = HM7;
        else if(Input.GetKey(HM6)) currentHM = HM6;
        else if(Input.GetKey(HM5)) currentHM = HM5;
        else if(Input.GetKey(HM4)) currentHM = HM4;
        else if(Input.GetKey(HM3)) currentHM = HM3;
        else if(Input.GetKey(HM2)) currentHM = HM2;
        else if(Input.GetKey(HM1)) currentHM = HM1;
        else currentHM = HM0;

        if(Input.GetKey(LM10)) currentLM = LM10;
        else if(Input.GetKey(LM9)) currentLM = LM9;
        else if(Input.GetKey(LM8)) currentLM = LM8;
        else if(Input.GetKey(LM7)) currentLM = LM7;
        else if(Input.GetKey(LM6)) currentLM = LM6;
        else if(Input.GetKey(LM5)) currentLM = LM5;
        else if(Input.GetKey(LM4)) currentLM = LM4;
        else if(Input.GetKey(LM3)) currentLM = LM3;
        else if(Input.GetKey(LM2)) currentLM = LM2;
        else if(Input.GetKey(LM1)) currentLM = LM1;
        else currentLM = LM0;

        if(Input.GetKey(L10)) currentL = L10;
        else if(Input.GetKey(L9)) currentL = L9;
        else if(Input.GetKey(L8)) currentL = L8;
        else if(Input.GetKey(L7)) currentL = L7;
        else if(Input.GetKey(L6)) currentL = L6;
        else if(Input.GetKey(L5)) currentL = L5;
        else if(Input.GetKey(L4)) currentL = L4;
        else if(Input.GetKey(L3)) currentL = L3;
        else if(Input.GetKey(L2)) currentL = L2;
        else if(Input.GetKey(L1)) currentL = L1;
        else currentL = L0;
    }
}