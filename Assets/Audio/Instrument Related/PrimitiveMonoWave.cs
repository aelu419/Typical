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
                wave_function = (t) => (Math.Sin(t));
                break;
            case Waveform.TRIANGLE:
                wave_function = (t) => (Math.Asin(Math.Sin(t)) * 2 / Mathf.PI);
                break;
            case Waveform.SAWTOOTH:
                wave_function = (t) =>
                    {
                        double temp = 0.0f;
                        for (int j = 0; j < 30; j++)
                        {
                            temp += Math.Sin((j + 1) * t) / j;
                        }
                        return temp;
                    };
                break;
            case Waveform.SQUARE:
                wave_function = (t) => Math.Sin(t) > 0 ? 1 : -1;
                break;
            case Waveform.NOISE:
                wave_function = (t) => { return UnityEngine.Random.value; };
                break;
        }
    }
}
