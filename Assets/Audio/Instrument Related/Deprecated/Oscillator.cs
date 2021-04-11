using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillator : MonoBehaviour
{
    [HideInInspector]
    public MonoWave wave;

    private void OnAudioFilterRead(float[] data, int channels)
    {
        if (wave == null)
        {
            return;
        }
        float[] sample = wave.Bake(data.Length / channels);
        for(int i = 0; i < data.Length; i += channels)
        {
            data[i] = sample[i / channels];
            if (channels == 2)
            {
                data[i + 1] = data[i];
            }
        }
    }
}
