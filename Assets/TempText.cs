using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TempText : MonoBehaviour
{
    public float lifetime;
    private float timer;
    private TMP_Text text;

    void Start()
    {
        text = GetComponent<TMP_Text>();
        text.enabled = false;
    }

    public void Popup()
    {
        timer = lifetime;
        text.enabled = true;
    }

    void Update()
    {
        if(!text.enabled) return;
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            text.enabled = false;
        }
    }
}
