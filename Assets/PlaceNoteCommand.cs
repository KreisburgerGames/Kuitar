using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlaceNoteCommand : ICommand
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
    float secondsPerBeat;
    bool load;

    public PlaceNoteCommand(GameObject notePrefab, GameObject noteParent, float beat, float metersPerSecond, float offset, int lane, int selectedNoteNumber, bool selectToStrum, bool selectedDownStrum, int i, float secondsPerBeat, bool load=false)
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
        this.beat = beat;
        this.secondsPerBeat = secondsPerBeat;
        this.load = load;
    }

    public void Execute()
    {
        position = PlaceNote.Place(notePrefab, noteParent, beat, metersPerSecond, offset, lane, selectedNoteNumber, selectToStrum, selectedDownStrum, i, secondsPerBeat, load:load);
    }

    public void PerformUndo()
    {
        PlaceNote.RemoveNote(position);
    }
}
