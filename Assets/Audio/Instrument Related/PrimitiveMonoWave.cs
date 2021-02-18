using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PrimitiveMonoWave : MonoWave
{
    public enum Waveform
    {
        SINE,
        TRIANGLE,
        SAWTOOTH,
        SQUARE,
        NOISE
    }

    public PrimitiveMonoWave(Waveform w) : base()
    {
        switch (w)
        {
            case Waveform.SINE:
                wave_function = (t) => (Mathf.Sin(t));
                break;
            case Waveform.TRIANGLE:
                wave_function = (t) => (Mathf.Asin(Mathf.Sin(t)) * 2.0f / Mathf.PI);
                break;
            case Waveform.SAWTOOTH:
                wave_function = (t) =>
                    {
                        double temp = 0.0f;
                        for (int j = 0; j < 100; j++)
                        {
                            temp += Mathf.Sin((j + 1.0f) * t) / j;
                        }
                        return Mathf.Clamp((float)temp, -1.0f, 1.0f);
                    };
                break;
            case Waveform.SQUARE:
                wave_function = (t) => Mathf.Sin(t) > 0 ? 1.0f : -1.0f;
                break;
            case Waveform.NOISE:
                wave_function = (t) => { return UnityEngine.Random.value; };
                break;
        }
    }
}
