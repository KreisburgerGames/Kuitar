using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public static class PlaceNote
{
    static List<DummyNote> notes;
    public static DummyNote Place(GameObject notePrefab, GameObject noteParent, float beat, float metersPerSecond, float offset, int lane, int selectedNoteNumber, bool selectToStrum, bool selectedDownStrum, int i, float secondsPerBeat, float noteParentOffset, bool load=false, bool latencySet=false)
    {
        DummyNote note = GameObject.Instantiate(notePrefab).GetComponent<DummyNote>();
        note.gameObject.transform.SetParent(noteParent.transform, true);
        note.gameObject.transform.localPosition = new Vector2((beat * secondsPerBeat * -metersPerSecond) + offset, note.gameObject.transform.localPosition.y);
        if(!load) note.gameObject.transform.localPosition -= Vector3.right * noteParentOffset;
        note.lane = lane;
        note.note = selectedNoteNumber;
        note.beat = beat;
        note.latencySet = latencySet;
        if(load) note.load = true;
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
        notes = notes.OrderBy(x => x.beat).ToList();
        return note;
    }

    public static void Clear()
    {
        if(notes == null) return;
        notes.Clear();
    }

    public static DummyNote RemoveNote(DummyNote note)
    {
        for(int x = 0; x < notes.Count; x++)
        {
            if(notes[x] == note)
            {
                notes[x].selected = false;
                GameObject.Destroy(notes[x].gameObject);
                Vector2 pos = notes[x].gameObject.transform.localPosition;
                notes.Remove(notes[x]);
                return note;
            }
        }
        notes = notes.OrderBy(x => x.beat).ToList();
        return null;
    }
}
