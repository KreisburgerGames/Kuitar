using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NotePlacer : MonoBehaviour, IPointerClickHandler
{
    public MapEditor mapEditor;
    public int lane;

    public void OnPointerClick(PointerEventData eventData)
    {
        mapEditor.PlaceNote(lane);
    }

}
