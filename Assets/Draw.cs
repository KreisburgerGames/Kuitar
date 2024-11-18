using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Draw : MonoBehaviour
{
    public Texture2D PaintWaveformSpectrum(AudioSource aud, int width, int height, Color waveformColor, string folderPath){
        
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        
        if(File.Exists(folderPath + "/spectrogram.png"))
        {
            byte[] pngBytes = File.ReadAllBytes(folderPath + "/spectrogram.png");
            tex.LoadImage(pngBytes);
            return tex;
        }
        int halfheight = height / 2;
        float heightscale = (float)height * 0.75f;

        // get the sound data
        
        float[] waveform = new float[width];

        int samplesize = aud.clip.samples * aud.clip.channels;
        float[] samples = new float[samplesize];
        aud.clip.GetData(samples, 0);

        int packsize = (samplesize / width);
        for (int w = 0; w < width; w++)
        {
            waveform[w] = Mathf.Abs(samples[w * packsize]);
        }

        // map the sound data to texture
        // 1 - clear
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tex.SetPixel(x, y, Color.black);
            }
        }

        // 2 - plot
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < waveform[x] * heightscale; y++)
            {
                tex.SetPixel(x, halfheight + y, waveformColor);
                tex.SetPixel(x, halfheight - y, waveformColor);
            }
        }

        tex.Apply();

        return tex;
  }

    public int width = 500;
    public int height = 100;
    public Color waveformColor = Color.green;

    [SerializeField] Image img;
    public AudioSource audioSource;

    public void Generate(string folderPath)
    {
        if (audioSource.clip == null) return;
        Texture2D texture = PaintWaveformSpectrum(audioSource, width, height, waveformColor, folderPath);
        texture.filterMode = FilterMode.Trilinear;
        img.overrideSprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
}
