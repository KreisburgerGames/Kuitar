using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Draw : MonoBehaviour
{
    public Texture2D PaintWaveformSpectrum(AudioClip audio, int width, int height, Color col) {
      Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
      float[] samples = new float[audio.samples];
      float[] waveform = new float[width];
      audio.GetData(samples, 0);
      int packSize = ( audio.samples / width ) + 1;
      int s = 0;
      for (int i = 0; i < audio.samples; i += packSize) {
          waveform[s] = Mathf.Abs(samples[i]);
          s++;
      }
 
      for (int x = 0; x < width; x++) {
          for (int y = 0; y < height; y++) {
              tex.SetPixel(x, y, Color.black);
          }
      }
 
      for (int x = 0; x < waveform.Length; x++) {
          for (int y = 0; y <= waveform[x] * ((float)height * .75f); y++) {
              tex.SetPixel(x, ( height / 2 ) + y, col);
              tex.SetPixel(x, ( height / 2 ) - y, col);
          }
      }
      tex.Apply();
 
      return tex;
  }

    public int width = 500;
    public int height = 100;
    public Color waveformColor = Color.green;

    [SerializeField] Image img;
    public AudioClip clip;

    public void Generate()
    {
        Texture2D texture = PaintWaveformSpectrum(clip, width, height, waveformColor);
        texture.filterMode = FilterMode.Point;
        img.overrideSprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
}
