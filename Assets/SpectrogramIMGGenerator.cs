using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Spectrogram;

public class SpectrogramIMGGenerator : MonoBehaviour
{
    public void GetSpectrum(string folderPath, string songFile)
    {
        print(folderPath + "/" + songFile);
        (double[] audio, int sampleRate) = ReadWavMono(folderPath + "/" + songFile);
        var sg = new SpectrogramGenerator(sampleRate, fftSize: 4096, stepSize: 500, maxFreq: 3000);
        sg.Add(audio);
        sg.SaveImage(folderPath + "/spectrogram.png");
    }

    (double[] audio, int sampleRate) ReadWavMono(string filePath, double multiplier = 16_000)
    {
        using var afr = new NAudio.Wave.AudioFileReader(filePath);
        int sampleRate = afr.WaveFormat.SampleRate;
        int bytesPerSample = afr.WaveFormat.BitsPerSample / 8;
        int sampleCount = (int)(afr.Length / bytesPerSample);
        int channelCount = afr.WaveFormat.Channels;
        var audio = new List<double>(sampleCount);
        var buffer = new float[sampleRate * channelCount];
        int samplesRead = 0;
        while ((samplesRead = afr.Read(buffer, 0, buffer.Length)) > 0)
            audio.AddRange(buffer.Take(samplesRead).Select(x => x * multiplier));
        return (audio.ToArray(), sampleRate);
    }
}
