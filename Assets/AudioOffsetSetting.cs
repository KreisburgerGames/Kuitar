using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AudioOffsetSetting : MonoBehaviour
{
    private Scrollbar scrollbar;
    public float minLatency;
    public float maxLatency;
    public TMP_Text latencyText;
    public BallSync ballSync;

    void Start()
    {
        scrollbar = GetComponent<Scrollbar>();
        float currentLatency = PlayerPrefs.GetFloat("Latency");
        float range = -minLatency + maxLatency;
        scrollbar.value = Mathf.Lerp(0, 1, (currentLatency + maxLatency)/range);
        latencyText.text = currentLatency.ToString() + " ms";
    }

    public void LatencyChanged(float value)
    {
        float selectedLatency = Mathf.Round(Mathf.Lerp(minLatency, maxLatency, value));
        latencyText.text = selectedLatency.ToString() + " ms";
        PlayerPrefs.SetFloat("Latency", selectedLatency);
        if(ballSync != null)
        {
            ballSync.UpdateOffset(selectedLatency);
        }
    }
}
