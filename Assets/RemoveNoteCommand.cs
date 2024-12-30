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
    bool loaded;
    float noteParentOffset;
    DummyNote noteRef;

    public RemoveNoteCommand(DummyNote noteRef, GameObject notePrefab, GameObject noteParent, float beat, float metersPerSecond, float offset, int lane, int selectedNoteNumber, bool selectToStrum, bool selectedDownStrum, int i, float yValue, float secondsPerBeat, bool loaded, float noteParentOffset)
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
        this.yValue = yValue;
        this.secondsPerBeat = secondsPerBeat;
        this.noteRef = noteRef;
        this.loaded = loaded;
        this.noteParentOffset = noteParentOffset;
    }

    public void Execute()
    {
        PlaceNote.RemoveNote(noteRef);
    }

    public void PerformUndo()
    {
        PlaceNote.Place(notePrefab, noteParent, beat, metersPerSecond, offset, lane, selectedNoteNumber, selectToStrum, selectedDownStrum, i, secondsPerBeat, noteParentOffset, load:loaded);
    }
}
