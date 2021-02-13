using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillator : MonoBehaviour
{
    public float frequency, phase, increment;
    public float sampling_frequency = 48000.0f;

    public float volume = 0.1f;

    public static float[] note_frequencies;

    public ArrayList playing; //the current notes that are being played

    private void Start()
    {
        playing = new ArrayList(32);
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        increment = frequency * 2.0f * Mathf.PI / sampling_frequency;
        for(int i = 0; i < data.Length; i += channels)
        {
            phase += increment;
            //read all currently playing notes here
            

            if (channels == 2)
            {
                data[i + 1] = data[i];
            }

            if (phase > (Mathf.PI * 2))
            {
                phase = 0.0f;
            }
        }

        //trim notes list
        ArrayList p = new ArrayList(32);
        foreach (Note n in playing)
        {
            if (!n.done)
            {
                p.Add(n);
            }
        }
        playing = p;
    }
}
