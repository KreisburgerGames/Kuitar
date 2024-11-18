using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceNoteCommand : ICommand
{
    GameObject notePrefab;
    GameObject noteParent;
    AudioSource audioSource;
    float metersPerSecond;
    float offset;
    int lane;
    int selectedNoteNumber;
    bool selectToStrum;
    bool selectedDownStrum;
    int i;
    Vector2 position;
    float beat;

    public PlaceNoteCommand(GameObject notePrefab, GameObject noteParent, float beat, float metersPerSecond, float offset, int lane, int selectedNoteNumber, bool selectToStrum, bool selectedDownStrum, int i)
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
    }

    public void Execute()
    {
        position = PlaceNote.Place(notePrefab, noteParent, beat, metersPerSecond, offset, lane, selectedNoteNumber, selectToStrum, selectedDownStrum, i);
    }

    public void PerformUndo()
    {
        PlaceNote.RemoveNote(position);
    }
}
