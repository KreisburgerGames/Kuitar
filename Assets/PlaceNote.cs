using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlaceNote
{
    static List<DummyNote> notes;
    public static Vector2 Place(GameObject notePrefab, GameObject noteParent, float beat, float metersPerSecond, float offset, int lane, int selectedNoteNumber, bool selectToStrum, bool selectedDownStrum, int i)
    {
        DummyNote note = GameObject.Instantiate(notePrefab).GetComponent<DummyNote>();
        note.gameObject.transform.SetParent(noteParent.transform, true);
        note.gameObject.transform.localPosition = new Vector2((beat * -metersPerSecond) + offset, note.gameObject.transform.localPosition.y);
        note.position = note.gameObject.transform.localPosition;
        note.lane = lane;
        note.note = selectedNoteNumber;
        note.beat = beat;
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

    public static void RemoveNote(Vector2 notePos)
    {
        for(int x = 0; x < notes.Count; x++)
        {
            if(notes[x].gameObject.transform.localPosition == (Vector3)notePos)
            {
                notes[x].selected = false;
                GameObject.Destroy(notes[x].gameObject);
                notes.Remove(notes[x]);
            }
        }
    }
}
