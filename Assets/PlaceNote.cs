using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public static class PlaceNote
{
    static List<DummyNote> notes;
    public static Vector2 Place(GameObject notePrefab, GameObject noteParent, float beat, float metersPerSecond, float offset, int lane, int selectedNoteNumber, bool selectToStrum, bool selectedDownStrum, int i, float secondsPerBeat)
    {
        DummyNote note = GameObject.Instantiate(notePrefab).GetComponent<DummyNote>();
        note.gameObject.transform.SetParent(noteParent.transform, true);
        note.gameObject.transform.localPosition = new Vector2((beat * -metersPerSecond) + offset, note.gameObject.transform.localPosition.y);
        note.lane = lane;
        note.note = selectedNoteNumber;
        note.beat = beat / secondsPerBeat;
        if(!selectToStrum) note.strum = false;
        else
        {
            if(selectedDownStrum) { note.strum = true; note.downStrum = true; }
            else { note.strum = true; note.downStrum = false; }
        }
        note.gameObject.name = "Note " + i.ToString();
        note.Init();
        if(notes == null)
        {
            notes = new List<DummyNote>();
        }
        notes.Add(note);
        return note.gameObject.transform.localPosition;
    }

    public static void Clear()
    {
        if(notes == null) return;
        notes.Clear();
    }

    public static Vector2 RemoveNote(Vector2 notePos)
    {
        for(int x = 0; x < notes.Count; x++)
        {
            if((Vector2)notes[x].gameObject.transform.localPosition == notePos)
            {
                notes[x].selected = false;
                GameObject.Destroy(notes[x].gameObject);
                Vector2 pos = notes[x].gameObject.transform.localPosition;
                notes.Remove(notes[x]);
                return pos;
            }
        }
        return Vector2.zero;
    }
}
