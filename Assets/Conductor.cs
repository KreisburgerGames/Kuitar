using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Burst.Intrinsics;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class Conductor : MonoBehaviour
{
    public float songBPM;
    public float secondsPerBeat;
    public double songPos;
    public double songBeatsPos;
    public double dspSongTime;
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
    public KeyCode L6 = KeyCode.N;
    public KeyCode L7 = KeyCode.M;
    public KeyCode L8 = KeyCode.Comma;
    public KeyCode L9 = KeyCode.Period;
    public KeyCode L10 = KeyCode.Slash;
    public KeyCode DownStrum = KeyCode.UpArrow;
    public KeyCode UpStrum = KeyCode.DownArrow;
    private List<KeyCode> HKeys;
    private List<KeyCode> HMKeys;
    private List<KeyCode> LMKeys;
    private List<KeyCode> LKeys;
    public List<Note> notes = new List<Note>();

    public float reactionBeats = 5f;

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
    bool notesReady = false;
    private GameObject noteLate;
    public float noteMissDist = 1f;
    private Camera camera;
    public float laneEdgeMargin;
    private bool canEnd = false;
    public int combo = 0;
    public int multiplier = 1;
    public List<Combo> combos = new List<Combo>();
    private float scoreLerped;
    public float scoreLerpSpeed;
    public TMP_Text comboText;
    public TMP_Text multiplierText;
    public TMP_Text scoreText;
    public TMP_Text accuracyText;
    public TMP_Text rankText;
    public int passedNotes = 0;
    public int bestPossibleScore;
    private int unmultipliedScore = 0;
    public List<AccuracyRank> ranks = new List<AccuracyRank>();
    public PauseMenuManager pauseMenu;
    public double time = 0f;
    public float noteTimingOffset = 0.5f;
    private End end;
    public double pauseDspTime;
    public bool started = false;
    public float offset;
    // Is this enough varibles for u pookie?

    void Start()
    {
        notes = notes.OrderBy(x => x.beat).ToList();
        HKeys = new List<KeyCode>()
        {
            H1, H2, H3, H4, H5, H6, H7, H8, H9, H10
        };
        HMKeys = new List<KeyCode>()
        {
            HM1, HM2, HM3, HM4, HM5, HM6, HM7, HM8, HM9, HM10
        };
        LMKeys = new List<KeyCode>()
        {
            LM1, LM2, LM3, LM4, LM5, LM6, LM7, LM8, LM9, LM10
        };
        LKeys = new List<KeyCode>()
        {
            L1, L2, L3, L4, L5, L6, L7, L8, L9, L10
        };
        song = GetComponent<AudioSource>();

        secondsPerBeat = 60f / songBPM;
        
        if(firstBeatOffset > 0){
            float clampedOffset = Mathf.Clamp(songStartOffset, 0, rewindDistance - 1f);
            offset = songStartOffset - ((rewindDistance - 1f) * (clampedOffset/firstBeatOffset));
            song.time = offset;
        }
        else if(songStartOffset - rewindDistance > 0)
        {
            offset = songStartOffset - rewindDistance - 1f;
            song.time = offset;
        }
        else
        {
            offset = 0f;
            song.time = offset;
        }

        pauseMenu = FindAnyObjectByType<PauseMenuManager>();
        
        StartCoroutine(PlaySong());

        AudioSettings.GetDSPBufferSize(out int bufflen, out int numbuff);
        foreach(Note note in notesParent.GetComponentsInChildren<Note>())
        {
            notes.Add(note);
            note.Init();
            note.gameObject.SetActive(false);
        }
        noteLate = GameObject.Find("Note Late");
        noteTimingOffset = secondsPerBeat/2f;

        end = FindAnyObjectByType<End>();
        end.panel.gameObject.SetActive(false);
        print(song.clip.loadType);
    }

    private void ChangeIndexes()
    {
        for (int i = 0; i < notes.Count; i++)
        {
            notes[i].index--;
        }
    }

    IEnumerator PlaySong()
    {
        yield return new WaitUntil(() => song.clip.loadState == AudioDataLoadState.Loaded);
        float latency = PlayerPrefs.GetFloat("Latency") / 1000f;
        float delay = 1f;
        double schedule = AudioSettings.dspTime + delay;
        dspSongTime = schedule - latency;
        song.PlayScheduled(schedule);
        yield return new WaitForSeconds(delay);
        started = true;
    }

    public double GetBeats()
    {
        return songBeatsPos;
    }

    void Update()
    {
        if(end.isEnd || !started) return;
        int currentSample = song.timeSamples;
        int sampleRate = song.clip.frequency;
        
        time = (double)currentSample / sampleRate;

        if(song.time > 0) canEnd = true;
        if(canEnd && song.time == 0f) 
        {
            end.scoreEarned = score;
            end.unmultipliedScore = unmultipliedScore;
            end.panel.gameObject.SetActive(true);
            end.isEnd = true;
        }
        if(camera == null)
        {
            camera = FindFirstObjectByType<Camera>();
            if(camera == null) return;
            noteLate.transform.position = new Vector2(camera.ScreenToWorldPoint(new Vector2(camera.pixelWidth, 0)).x + noteMissDist, noteLate.transform.position.y);
            GameObject lanes = GameObject.FindGameObjectWithTag("Lanes");
            GameObject laneEnds = GameObject.FindGameObjectWithTag("Lane Ends");
            lanes.transform.position = new Vector2(camera.ScreenToWorldPoint(new Vector2(0, 0)).x + laneEdgeMargin, 0);
            laneEnds.transform.position = new Vector2(camera.ScreenToWorldPoint(new Vector2(camera.pixelWidth, 0)).x - laneEdgeMargin, 0);
        }

        if(pauseMenu.isPaused) return;

        songPos = AudioSettings.dspTime - dspSongTime - pauseDspTime + offset;

        songBeatsPos = songPos / secondsPerBeat;

        List<Note> readyNotes = new List<Note>();

        DetermineKeyStrokes();
        StartCoroutine(UpdateHUD());

        foreach (Note note in notes.ToArray())
        {
            if (note.beat <= songBeatsPos + reactionBeats + (noteTimingOffset/secondsPerBeat))
            {
                readyNotes.Add(note);
                note.gameObject.SetActive(true);
                note.moving = true;
            }
            else note.gameObject.SetActive(false);
        }
        List<Note> hittingNotes = new List<Note>();
        isStrumming = false;
        bool hammerOn = false;
        Note previousNote = null;
        // gl reading this code, I can barley read this fucking thing
        if (readyNotes.Count > 0 && readyNotes[0].strum)
        {
            isStrumming = true;
            foreach(Note note2 in readyNotes)
            {
                float checkhitScore = CalculateScore(GetEndLaneX(note2) - note2.gameObject.transform.position.x, note2.lane, false);
                if (checkhitScore == 0) break;
                if (previousNote != null)
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
        else if(readyNotes.Count > 0)
        {
            isStrumming = false;
            foreach(Note note2 in readyNotes)
            {
                
                float checkhitScore = CalculateScore(GetEndLaneX(note2) - note2.gameObject.transform.position.x, note2.lane, false);
                if (checkhitScore == 0) break;
                if (previousNote != null)
                {
                    if(note2.beat == previousNote.beat)
                    {
                        hittingNotes.Add(note2);
                        previousNote = note2;
                        if(readyNotes[0].hammerOn)
                        {
                            hammerOn = true;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                else if(checkhitScore != 0) {hittingNotes.Add(note2); previousNote = note2; if(readyNotes[0].hammerOn)
                {
                    hammerOn = true;
                }}
            }
        }
        else hittingNotes.Clear();
        if (hittingNotes.Count > 0) notesReady = true; else notesReady = false;
        bool strum = Input.GetKeyDown(DownStrum) || Input.GetKeyDown(UpStrum);
        if (strum && !notesReady)
        {
            EmptyStrum();
        }
        if(hittingNotes.Count > 0)
        {
            bool strumNote = hittingNotes[0].strum;
            if (!strumNote && strum)
            {
                EmptyStrum();
            }
            else if(strumNote && Input.GetKeyDown(H0)) GetColorManagerFromString("1").Error();
            else if (strumNote && Input.GetKeyDown(HM0)) GetColorManagerFromString("2").Error();
            else if (strumNote && Input.GetKeyDown(LM0)) GetColorManagerFromString("3").Error();
            else if (strumNote && Input.GetKeyDown(L0)) GetColorManagerFromString("4").Error();
        }
        if(Input.GetKeyDown(H0) && !notesReady) GetColorManagerFromString("1").Error();
        else if (Input.GetKeyDown(HM0) && !notesReady) GetColorManagerFromString("2").Error();
        else if (Input.GetKeyDown(LM0) && !notesReady) GetColorManagerFromString("3").Error();
        else if (Input.GetKeyDown(L0) && !notesReady) GetColorManagerFromString("4").Error();
        if (isStrumming)
        {
            Strum(hittingNotes);
        }
        else if(!hammerOn)// Lmao copy paste code go brrrrrrrrr
        {
            Pluck(hittingNotes);
        }
        else
        {
            HammerOn(hittingNotes);
        }
    }

    private void Strum(List<Note> hittingNotes)
    {
        if(Input.GetKeyDown(DownStrum) || Input.GetKeyDown(UpStrum))
        {
            // I have no fucking clue why this works but if I don't reverse down strum to upstrum input it doesn't work
            if(hittingNotes.Count > 0 && hittingNotes[0].downStrum && Input.GetKey(UpStrum) || hittingNotes.Count > 0 && !hittingNotes[0].downStrum && Input.GetKey(DownStrum))
            {
                foreach(Note note in hittingNotes)
                {
                    int hitScore;
                    float checkHitScore = CalculateScore(GetEndLaneX(note) - note.gameObject.transform.position.x, note.lane, false);
                    if(checkHitScore == 0) continue;
                    if (note.lane == 1)
                    {
                        if(currentH == note.note)
                        {
                            hitScore = CalculateScore(GetEndLaneX(note) - note.gameObject.transform.position.x, note.lane, true);
                            print(hitScore);
                            ParticlesAndText(hitScore, note);
                            notes.Remove(note);
                            ChangeIndexes();
                            Destroy(note.gameObject);
                            passedNotes += 1;
                        }
                        else
                        {
                            print("Wrong Note!");
                            WrongNote(note);
                            notes.Remove(note);
                            ChangeIndexes();
                            Destroy(note.gameObject);
                            passedNotes += 1;
                        }
                    }
                    else if(note.lane == 2)
                    {
                        if(currentHM == note.note)
                        {
                            hitScore = CalculateScore(GetEndLaneX(note) - note.gameObject.transform.position.x, note.lane, true);
                            print(hitScore);
                            ParticlesAndText(hitScore, note);
                            notes.Remove(note);
                            ChangeIndexes();
                            Destroy(note.gameObject);
                            passedNotes += 1;
                        }
                        else
                        {
                            print("Wrong Note!");
                            WrongNote(note);
                            notes.Remove(note);
                            ChangeIndexes();
                            Destroy(note.gameObject);
                            passedNotes += 1;
                        }
                    }
                    else if(note.lane == 3)
                    {
                        if(currentLM == note.note)
                        {
                            hitScore = CalculateScore(GetEndLaneX(note) - note.gameObject.transform.position.x, note.lane, true);
                            print(hitScore);
                            ParticlesAndText(hitScore, note);
                            notes.Remove(note);
                            ChangeIndexes();
                            Destroy(note.gameObject);
                            passedNotes += 1;
                        }
                        else
                        {
                            print("Wrong Note!");
                            WrongNote(note);
                            notes.Remove(note);
                            ChangeIndexes();
                            Destroy(note.gameObject);
                            passedNotes += 1;
                        }
                    }
                    else if(note.lane == 4)
                    {
                        if(currentL == note.note)
                        {
                            hitScore = CalculateScore(GetEndLaneX(note) - note.gameObject.transform.position.x, note.lane, true);
                            print(hitScore);
                            ParticlesAndText(hitScore, note);
                            notes.Remove(note);
                            ChangeIndexes();
                            Destroy(note.gameObject);
                            passedNotes += 1;
                        }
                        else
                        {
                            print("Wrong Note!");
                            WrongNote(note);
                            notes.Remove(note);
                            ChangeIndexes();
                            Destroy(note.gameObject);
                            passedNotes += 1;
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
                    ChangeIndexes();
                    Destroy(note.gameObject);
                    passedNotes += 1;
                }
            }
        }
    }

    private void Pluck(List<Note> hittingNotes)
    {
        foreach(Note note in hittingNotes)
        {
            int hitScore;
            float checkHitScore = CalculateScore(GetEndLaneX(note) - note.gameObject.transform.position.x, note.lane, false);
            if (checkHitScore == 0) continue;
            if (note.lane == 1 && Input.GetKeyDown(H0))
            {
                if(currentH == note.note)
                {
                    hitScore = CalculateScore(GetEndLaneX(note) - note.gameObject.transform.position.x, note.lane, true);
                    print(hitScore);
                    ParticlesAndText(hitScore, note);
                    notes.Remove(note);
                    ChangeIndexes();
                    Destroy(note.gameObject);
                    passedNotes += 1;
                }
                else
                {
                    print("Wrong Note!");
                    WrongNote(note);
                    notes.Remove(note);
                    ChangeIndexes();
                    Destroy(note.gameObject);
                    passedNotes += 1;
                }
            }
            else if(note.lane == 2 && Input.GetKeyDown(HM0))
            {
                if(currentHM == note.note)
                {
                    hitScore = CalculateScore(GetEndLaneX(note) - note.gameObject.transform.position.x, note.lane, true);
                    print(hitScore);
                    ParticlesAndText(hitScore, note);
                    notes.Remove(note);
                    ChangeIndexes();
                    Destroy(note.gameObject);
                    passedNotes += 1;
                }
                else
                {
                    print("Wrong Note!");
                    WrongNote(note);
                    notes.Remove(note);
                    ChangeIndexes();
                    Destroy(note.gameObject);
                    passedNotes += 1;
                }
            }
            else if(note.lane == 3 && Input.GetKeyDown(LM0))
            {
                if(currentLM == note.note)
                {
                    hitScore = CalculateScore(GetEndLaneX(note) - note.gameObject.transform.position.x, note.lane, true);
                    print(hitScore);
                    ParticlesAndText(hitScore, note);
                    notes.Remove(note);
                    ChangeIndexes();
                    Destroy(note.gameObject);
                    passedNotes += 1;
                }
                else
                {
                    print("Wrong Note!");
                    WrongNote(note);
                    notes.Remove(note);
                    ChangeIndexes();
                    Destroy(note.gameObject);
                    passedNotes += 1;
                }
            }
            else if(note.lane == 4 && Input.GetKeyDown(L0))
            {
                if(currentL == note.note)
                {
                    hitScore = CalculateScore(GetEndLaneX(note) - note.gameObject.transform.position.x, note.lane, true);
                    print(hitScore);
                    ParticlesAndText(hitScore, note);
                    notes.Remove(note);
                    ChangeIndexes();
                    Destroy(note.gameObject);
                    passedNotes += 1;
                }
                else
                {
                    print("Wrong Note!");
                    WrongNote(note);
                    notes.Remove(note);
                    ChangeIndexes();
                    Destroy(note.gameObject);
                    passedNotes += 1;
                }
            }
            else if(Input.GetKeyDown(H0)) GetColorManagerFromString("1").Error();
            else if(Input.GetKeyDown(HM0)) GetColorManagerFromString("2").Error();
            else if(Input.GetKeyDown(LM0)) GetColorManagerFromString("3").Error();
            else if(Input.GetKeyDown(L0)) GetColorManagerFromString("4").Error();
        }
    }

    private void HammerOn(List<Note> hittingNotes)
    {
        foreach(Note note in hittingNotes)
        {
            int hitScore;
            float checkHitScore = CalculateScore(GetEndLaneX(note) - note.gameObject.transform.position.x, note.lane, false);
            if (checkHitScore == 0) continue;
            if (note.lane == 1 && note.prevNote != H0 && !Input.GetKey(note.prevNote))
            {
                if(currentH == note.note)
                {
                    hitScore = CalculateScore(GetEndLaneX(note) - note.gameObject.transform.position.x, note.lane, true);
                    print(hitScore);
                    ParticlesAndText(hitScore, note);
                    notes.Remove(note);
                    ChangeIndexes();
                    Destroy(note.gameObject);
                    passedNotes += 1;
                }
                else
                {
                    print("Wrong Note!");
                    WrongNote(note);
                    notes.Remove(note);
                    ChangeIndexes();
                    Destroy(note.gameObject);
                    passedNotes += 1;
                }
            }
            else if (note.lane == 1 && note.prevNote == H0 && currentH != H0)
            {
                if(currentH == note.note)
                {
                    hitScore = CalculateScore(GetEndLaneX(note) - note.gameObject.transform.position.x, note.lane, true);
                    print(hitScore);
                    ParticlesAndText(hitScore, note);
                    notes.Remove(note);
                    ChangeIndexes();
                    Destroy(note.gameObject);
                    passedNotes += 1;
                }
                else
                {
                    print("Wrong Note!");
                    WrongNote(note);
                    notes.Remove(note);
                    ChangeIndexes();
                    Destroy(note.gameObject);
                    passedNotes += 1;
                }
            }
            else if(note.lane == 2 && note.prevNote != HM0  && !Input.GetKey(note.prevNote))
            {
                if(currentHM == note.note)
                {
                    hitScore = CalculateScore(GetEndLaneX(note) - note.gameObject.transform.position.x, note.lane, true);
                    print(hitScore);
                    ParticlesAndText(hitScore, note);
                    notes.Remove(note);
                    ChangeIndexes();
                    Destroy(note.gameObject);
                    passedNotes += 1;
                }
                else
                {
                    print("Wrong Note!");
                    WrongNote(note);
                    notes.Remove(note);
                    ChangeIndexes();
                    Destroy(note.gameObject);
                    passedNotes += 1;
                }
            }
            else if (note.lane == 2 && note.prevNote == HM0 && currentHM != HM0)
            {
                if(currentHM == note.note)
                {
                    hitScore = CalculateScore(GetEndLaneX(note) - note.gameObject.transform.position.x, note.lane, true);
                    print(hitScore);
                    ParticlesAndText(hitScore, note);
                    notes.Remove(note);
                    ChangeIndexes();
                    Destroy(note.gameObject);
                    passedNotes += 1;
                }
                else
                {
                    print("Wrong Note!");
                    WrongNote(note);
                    notes.Remove(note);
                    ChangeIndexes();
                    Destroy(note.gameObject);
                    passedNotes += 1;
                }
            }
            else if(note.lane == 3 && note.prevNote != LM0 && !Input.GetKey(note.prevNote))
            {
                if(currentLM == note.note)
                {
                    hitScore = CalculateScore(GetEndLaneX(note) - note.gameObject.transform.position.x, note.lane, true);
                    print(hitScore);
                    ParticlesAndText(hitScore, note);
                    notes.Remove(note);
                    ChangeIndexes();
                    Destroy(note.gameObject);
                    passedNotes += 1;
                }
                else
                {
                    print("Wrong Note!");
                    WrongNote(note);
                    notes.Remove(note);
                    ChangeIndexes();
                    Destroy(note.gameObject);
                    passedNotes += 1;
                }
            }
            else if (note.lane == 3 && note.prevNote == LM0 && currentLM != LM0)
            {
                if(currentLM == note.note)
                {
                    hitScore = CalculateScore(GetEndLaneX(note) - note.gameObject.transform.position.x, note.lane, true);
                    print(hitScore);
                    ParticlesAndText(hitScore, note);
                    notes.Remove(note);
                    ChangeIndexes();
                    Destroy(note.gameObject);
                    passedNotes += 1;
                }
                else
                {
                    print("Wrong Note!");
                    WrongNote(note);
                    notes.Remove(note);
                    ChangeIndexes();
                    Destroy(note.gameObject);
                    passedNotes += 1;
                }
            }
            else if(note.lane == 4 && note.prevNote != L0 && !Input.GetKey(note.prevNote))
            {
                if(currentL == note.note)
                {
                    hitScore = CalculateScore(GetEndLaneX(note) - note.gameObject.transform.position.x, note.lane, true);
                    print(hitScore);
                    ParticlesAndText(hitScore, note);
                    notes.Remove(note);
                    ChangeIndexes();
                    Destroy(note.gameObject);
                    passedNotes += 1;
                }
                else
                {
                    print("Wrong Note!");
                    WrongNote(note);
                    notes.Remove(note);
                    ChangeIndexes();
                    Destroy(note.gameObject);
                    passedNotes += 1;
                }
            }
            else if (note.lane == 4 && note.prevNote == L0 && currentL != L0)
            {
                if(currentL == note.note)
                {
                    hitScore = CalculateScore(GetEndLaneX(note) - note.gameObject.transform.position.x, note.lane, true);
                    print(hitScore);
                    ParticlesAndText(hitScore, note);
                    notes.Remove(note);
                    ChangeIndexes();
                    Destroy(note.gameObject);
                    passedNotes += 1;
                }
                else
                {
                    print("Wrong Note!");
                    WrongNote(note);
                    notes.Remove(note);
                    ChangeIndexes();
                    Destroy(note.gameObject);
                    passedNotes += 1;
                }
            }
            if(Input.GetKeyDown(H0)) GetColorManagerFromString("1").Error();
            if(Input.GetKeyDown(HM0)) GetColorManagerFromString("2").Error();
            if(Input.GetKeyDown(LM0)) GetColorManagerFromString("3").Error();
            if(Input.GetKeyDown(L0)) GetColorManagerFromString("4").Error();     
        }
    }

    private IEnumerator UpdateHUD()
    {
        comboText.text = "Combo: " + combo.ToString();
        multiplierText.text = "Multiplier: x" + multiplier.ToString();

        scoreLerped = Mathf.Lerp(scoreLerped, score, scoreLerpSpeed * Time.deltaTime);
        int scoreRounded = (int)MathF.Round(scoreLerped);
        scoreText.text = "Score: " + scoreRounded.ToString();

        bestPossibleScore = passedNotes * 100;
        if(bestPossibleScore == 0)
        {
            accuracyText.text = "Accuracy: 100%";
        }
        else
        {
            float accuracy = MathF.Round((float)unmultipliedScore / bestPossibleScore * 100, 2);
            accuracyText.text = $"Accuracy: {accuracy}%";
            rankText.text = GetRank(accuracy, false);
        }
        
        yield return null;
    }

    float GetEndLaneX(Note note)
    {
        return GameObject.Find("Lane " + note.lane + " End").gameObject.transform.position.x;
    }

    NoteEndColorManager GetColorManager(Note note)
    {
        return GameObject.Find("Lane " + note.lane + " End").gameObject.GetComponent<NoteEndColorManager>();
    }

    NoteEndColorManager GetColorManagerFromString(string lane)
    {
        return GameObject.Find("Lane " + lane + " End").gameObject.GetComponent<NoteEndColorManager>();
    }

    public void ResetMultiplier()
    {
        multiplier = 1;
        combo = 0;
        ComboMultiplierHander();
    }

    public void ComboMultiplierHander()
    {
        for (int i = 0; i < combos.Count; i++)
        {
            if(combo >= combos[i].notesUntilUpgrade)
            {
                try
                {
                    if(combo >= combos[i + 1].notesUntilUpgrade) continue;
                    else multiplier = combos[i].multiplier;
                }
                catch(IndexOutOfRangeException)
                {
                    multiplier = combos[i].multiplier;
                }
            }
        }
    }

    public Color rankColor;

    public string GetRank(float accuracy, bool end)
    {
        for (int i = 0; i < ranks.Count; i++)
        {
            if(accuracy >= ranks[i].accuracyNeeded)
            {
                try
                {
                    if(accuracy >= ranks[i + 1].accuracyNeeded) continue;
                    if(!end)
                        rankText.color = ranks[i].color;
                    else
                        rankColor = ranks[i].color;
                    return ranks[i].rank;
                }
                catch(ArgumentOutOfRangeException)
                {
                    if(!end)
                        rankText.color = ranks[i].color;
                    else
                        rankColor = ranks[i].color;
                    return ranks[i].rank;
                }
            }
        }
        return "SS";
    }

    int CalculateScore(float distanceFromNote, int lane, bool hit)
    {
        decimal dist = (decimal)distanceFromNote;
        decimal distRounded = Math.Round(dist, accuracyRoundingDigits);
        float absDist = Mathf.Abs((float)distRounded);
        if(absDist > missDistance) {return 0;}
        // I feel so smart writing this
        int hitScore = (int)Math.Round((missDistance - absDist)/missDistance * 100);
        if(hitScore > roundToOneHundredRange) hitScore = 100;
        if(hit)
        {
            combo += 1;
            score += hitScore * multiplier;
            unmultipliedScore += hitScore;
            ComboMultiplierHander();
        }
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
        ResetMultiplier();
    }

    void WrongDirection(Note note)
    {
        GetColorManager(note).Error();
        ResetMultiplier();
    }

    public void MissedNote(Note note)
    {
        GetColorManager(note).Error();
        ResetMultiplier();
    }

    public void EmptyStrum()
    {
        foreach(NoteEndColorManager colorManager in GameObject.FindObjectsOfType<NoteEndColorManager>())
        {
            colorManager.Error();
        }
        ResetMultiplier();
    }

    void DetermineKeyStrokes()
    {
        for (int i = 9; i >= 0; i--)
        {
            if(Input.GetKey(HKeys[i]))
            {
                currentH = HKeys[i];
                break;
            }
            if(i == 0) currentH = H0;
        }
        for (int i = 9; i >= 0; i--)
        {
            if(Input.GetKey(HMKeys[i]))
            {
                currentHM = HMKeys[i];
                break;
            }
            if(i == 0) currentHM = HM0;
        }
        for (int i = 9; i >= 0; i--)
        {
            if(Input.GetKey(LMKeys[i]))
            {
                currentLM = LMKeys[i];
                break;
            }
            if(i == 0) currentLM = LM0;
        }
        for (int i = 9; i >= 0; i--)
        {
            if(Input.GetKey(LKeys[i]))
            {
                currentL = LKeys[i];
                break;
            }
            if(i == 0) currentL = L0;
        }
    }
}