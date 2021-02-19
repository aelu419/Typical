using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

//this class represents a single raw wave (or a simple compound of such)
//that will be fed to a *single* oscillator
//instruments can manipulate multiple monowaves at the same time
//for unison and such effects.
public class MonoWave
{
    //private static int POP_FILTER_ITERATIONS = 3;

    public float pitch_shift,
        modulation, modulation_amplitude,
        noise, noise_cutoff,
        pop_cutoff = 1;
    public AnimationCurve semitone;
    public AnimationCurve envelope_gain;
    public AnimationCurve note_gain;
    public float polyphonal_gain;
    public float pitch_fade;
    public int phase_shift;

    public float strength;

    protected Func<float, float> wave_function;

    public float t_cycles, t_ms;

    System.Random rand;

    public const float MS_PER_SAMPLE = 
        1000.0f / MusicManager.SAMPLING_FREQUENCY;
    public const float UNIT_RADIAN_PER_SAMPLE = 
        2.0f * Mathf.PI / MusicManager.SAMPLING_FREQUENCY; 

    public MonoWave()
    {
        Initialize();
    }

    public void Initialize()
    {
        rand = new System.Random();
        t_cycles = 0.0f;
        t_ms = 0.0f;
        strength = 1.0f;

        envelope_gain = null;
        note_gain = null;
        semitone = null;
    }

    public Func<float, float> Multiply(float m)
    {
        Func<float, float> w_ = wave_function;
        wave_function = (t) => (wave_function(t) * m);
        return w_;
    }


    public float[] Bake(int length)
    {
        float[] baked = new float[length];

        if (semitone == null || note_gain == null)
        {
            //Initialize();
            return baked;
        }

        float semi = semitone.Evaluate(t_ms); //the current note tone
        float n_gain = note_gain.Evaluate(t_ms); //the current note inflections
        float e_gain = note_gain.Evaluate(t_ms); //the current ADSR level

        float frequency = 
            440.0f * Mathf.Pow(2.0f, semi / 12.0f)
            //+ pitch_shift
            //- pitch_fade * (1.0 - e_gain * n_gain)
            ;

        Debug.Log(frequency + " " + n_gain + " " + e_gain);

        //# hz per sample
        float RADIAN_PER_SAMPLE = (float)(frequency * UNIT_RADIAN_PER_SAMPLE);

        float phase_shift_sample = RADIAN_PER_SAMPLE * phase_shift;
        float curr_gain;

        for (int i = 0; i < length; i++)
        {
            curr_gain = strength * e_gain * n_gain; //overall amplitude of wave

            //Debug.Log(curr_gain + " at fr: " + frequency);

            baked[i] = 0.1f * Mathf.Sin(t_cycles);
                /*
                wave_function(
                    t_cycles //the current progress of the note in RADIAN
                    //+ phase_shift_sample //phase shift in RADIAN
                    //+ curr_gain * modulation_amplitude
                        //* Mathf.Sin(modulation * t_cycles 
                        /// frequency) //modulation in RADIAN
                    //)
                    //+ (rand.NextDouble() - 0.5) * noise * Mathf.Max(0.0f, curr_gain - noise_cutoff)
                )
                * curr_gain
                * polyphonal_gain
                ;*/

            t_cycles += RADIAN_PER_SAMPLE;
            t_ms += MS_PER_SAMPLE;
            n_gain = note_gain.Evaluate(t_ms);
            e_gain = envelope_gain.Evaluate(t_ms);

            if (float.IsNaN(n_gain) || float.IsNaN(e_gain))
            {
                Initialize();
                break;
            }
        }

        /*
        //pop filtering
        for(int j = 0; j < POP_FILTER_ITERATIONS; j++)
        {
            for (int i = 1; i < baked.Length - 1; i++)
            {
                if (Mathf.Abs(baked[i] - baked[i + 1]) > pop_cutoff
                    || Mathf.Abs(baked[i - 1] - baked[i]) > pop_cutoff)
                {
                    baked[i] = (baked[i - 1] + baked[i + 1]) / 2.0f;
                }
            }
        }*/

        return baked;
    }
}
