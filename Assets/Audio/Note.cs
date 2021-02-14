using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note
{
    const int PRECISION = 10; //the number fourier series terms used in approxination
    float[] data; //the data for the sound wave, at 48000hz frequency
    int next_index = 0;

    public bool done = false; //whether the note has finished playing

    public float entrance_beat;
    public int oscillator_ID;
    public string type;

    public enum Waveform
    {
        sine,
        triangular,
        sawtooth,
        square
    }

    public Note(float[] data)
    {
        this.data = data;
    }
    
    //every frame the note is played, the oscillator takes a certain segment
    //from the note's data, and plays it
    //when the node data depletes, it is marked as 'done'
    public float[] ReadData(int read_length)
    {
        int terminal = Mathf.Min(data.Length, next_index + read_length);
        float[] result = new float[read_length];
        for(int i = next_index; i < terminal; i++)
        {
            result[i - next_index] = data[i];
        }
        if (terminal >= data.Length)
        {
            Debug.Log(this + " is done");
            done = true;
        }

        next_index = terminal;
        return result;
    }

    public static Note GetRawNote(float beats, int phase_shift, float gain, float frequency, Waveform w)
    {
        if (frequency <= 0.0f)
            throw new UnityException("frequency cannot be negative or 0: " + frequency);
        if (gain < 0.0f)
            throw new UnityException("gain cannot be negative: " + gain);

        int sample_length = Mathf.CeilToInt(
            beats / MusicManager.Instance.BPM * 60.0f * MusicManager.SAMPLING_FREQUENCY
            );

        float[] result = new float[sample_length];
        //0 gain is basically 0 array
        if (gain == 0)
            return new Note(result);

        float inc = frequency * 2.0f * Mathf.PI / MusicManager.SAMPLING_FREQUENCY;
        //return a sine wave
        float t = phase_shift * inc;
        
        switch(w)
        {
            case (Waveform.sine):
                for (int i = 0; i < sample_length; i++)
                {
                    result[i] = Mathf.Sin(t) * gain;
                    t += inc;
                }
                        
                break;
            case (Waveform.triangular):
                //abs of sawtooth
                for(int i = 0; i < sample_length; i++)
                {
                    result[i] = gain * (Mathf.Asin(Mathf.Sin(t)) * 2 / Mathf.PI);
                    t += inc;
                }
                break;
            case (Waveform.sawtooth):
                for (int j = 1; j <= PRECISION; j++)
                {
                    for (int i = 0; i < sample_length; i++)
                    {
                        result[i] += Mathf.Sin(j * t) / j / 2;
                        t += inc;
                    }
                }
                for (int i = 0; i < sample_length; i++)
                {
                    result[i] *= gain;
                }
                break;
            case (Waveform.square):
                for (int j = 0; j < PRECISION; j++)
                {
                    for (int i = 0; i < sample_length; i++)
                    {
                        result[i] += gain * Mathf.Sin((2 * j + 1.0f) * t) / (2.0f * j + 1.0f);
                        t += inc;
                    }
                }
                break;
        }

        Note n = new Note(result);

        switch (w)
        {
            case (Waveform.sine):
                n.type = "sine";
                break;
            case (Waveform.triangular):
                n.type = "triangular";
                break;
            case (Waveform.sawtooth):
                n.type = "sawtooth";
                break;
            case (Waveform.square):
                n.type = "square";
                break;
        }

        return n;
    }
}
