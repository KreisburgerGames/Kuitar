using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class RemoveNoteCommand : ICommand
{
    GameObject notePrefab;
    GameObject noteParent;
    float metersPerSecond;
    float offset;
    int lane;
    int selectedNoteNumber;
    bool selectToStrum;
    bool selectedDownStrum;
    int i;
    Vector2 position;
    float beat;
    float yValue;
    float secondsPerBeat;
    bool load;

    public RemoveNoteCommand(GameObject notePrefab, GameObject noteParent, float beat, float metersPerSecond, float offset, int lane, int selectedNoteNumber, bool selectToStrum, bool selectedDownStrum, int i, float yValue, float secondsPerBeat, bool isLoad)
    {
        this.notePrefab = notePrefab;
        this.noteParent = noteParent;
        this.beat = beat;
        this.metersPerSecond = metersPerSecond;
        this.offset = offset;
        this.lane = lane;
        this.selectedNoteNumber = selectedNoteNumber;
        this.selectToStrum = selectToStrum;
        this.selectedDownStrum = selectedDownStrum;
        this.i = i;
        this.yValue = yValue;
        this.secondsPerBeat = secondsPerBeat;
        this.load = isLoad;
    }

    public void Execute()
    {
        float loadCompensate = 0f;
        if(load) loadCompensate = .438f;
        position = new Vector2((beat * -metersPerSecond) + offset - loadCompensate, yValue);
        Debug.Log(position);
        PlaceNote.RemoveNote(position);
    }

    public void PerformUndo()
    {
        PlaceNote.Place(notePrefab, noteParent, beat, metersPerSecond, offset, lane, selectedNoteNumber, selectToStrum, selectedDownStrum, i, secondsPerBeat);
    }
}
