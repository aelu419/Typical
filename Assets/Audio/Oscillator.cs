using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillator : MonoBehaviour
{
    public float frequency, phase, increment;
    public float sampling_frequency = 48000.0f;

    public float gain;

    private void OnAudioFilterRead(float[] data, int channels)
    {
        increment = frequency * 2.0f * Mathf.PI / sampling_frequency;
        
        for(int i = 0; i < data.Length; i += channels)
        {
            phase += increment;
            data[i] = gain * Mathf.Sin(phase);

            if (channels == 2)
            {
                data[i + 1] = data[i];
            }

            if (phase > (Mathf.PI * 2))
            {
                phase = 0.0f;
            }
        }
    }
}
