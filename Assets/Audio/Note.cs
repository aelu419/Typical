using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note
{

    float[] data; //the data for the sound wave, at 48000hz frequency
    int next_index = 0;

    public bool done = false; //whether the note has finished playing
    
    //every frame the note is played, the oscillator takes a certain segment
    //from the note's data, and plays it
    //when the node data depletes, it is marked as 'done'
    public float[] ReadData(int read_length)
    {
        int terminal = Mathf.Min(data.Length, next_index + read_length);
        float[] result = new float[read_length];
        for(int i = next_index; i < terminal; i++)
        {
            result[i] = data[i];
        }
        if (terminal == data.Length)
        {
            done = true;
        }
        return result;
    }

    public static float[] Sine(int sample_length, int phase_shift, int gain, int frequency)
    {
        if (frequency <= 0.0f)
            throw new UnityException("frequency cannot be negative or 0: " + frequency);
        if (gain < 0.0f)
            throw new UnityException("gain cannot be negative: " + gain);

        float[] result = new float[sample_length];
        //0 gain is basically 0 array
        if (gain == 0)
            return result;

        //return a sine wave
        for(int i = 0; i < sample_length; i++)
        {
            
        }
        return result;
    }
}
