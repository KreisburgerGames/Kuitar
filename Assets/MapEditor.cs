using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;

public class MapEditor : MonoBehaviour
{
    Camera camera;
    public GameObject laneEnds;
    public float laneEdgeMargin = 1f;
    public GameObject notePrefab;
    public GameObject noteParent;
    public float metersPerSecond = 5f;
    public float zoomLevel;
    public float snapIncrement;
    public float offset;
    public string songFolder = "";
    public string mapPath = "";
    // Start is called before the first frame update
    void Start()
    {
        camera = FindFirstObjectByType<Camera>();
        float edge = camera.ScreenToWorldPoint(new Vector2(camera.pixelWidth, 0)).x;
        offset = edge - laneEdgeMargin;
        laneEnds.transform.position = new Vector2(offset, 0);
    }

    // Update is called once per frame
    void Update()
    {
        noteParent.transform.position = new Vector2(offset, 0);
    }
}
