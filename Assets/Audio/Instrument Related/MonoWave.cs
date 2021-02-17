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
    public double frequency, pitch_shift, 
        modulation, modulation_amplitude,
        noise, noise_cutoff;
    public AnimationCurve gain;
    public float polyphonal_gain;
    public float pitch_fade;
    public int phase_shift;

    public float strength;

    protected Func<double, double> wave_function;

    public double t_cycles, t_ms;

    public MonoWave()
    {
        t_cycles = 0.0;
        t_ms = 0.0;
        strength = 1.0f;
    }

    public Func<double, double> Multiply(double m)
    {
        Func<double, double> w_ = wave_function;
        
        wave_function = (t) => (wave_function(t) * m);
        return w_;
    }


    public double[] Bake(int length)
    {
        double[] baked = new double[length];

        if (frequency == 0)
        {
            t_cycles = 0;
            t_ms = 0;
            strength = 1.0f;
            return baked;
        }

        System.Random rand = new System.Random();

        double inc_cycles = (frequency
            - pitch_fade * (1.0 - gain.Evaluate((float)t_ms))) 
            * 2.0f * Mathf.PI / MusicManager.SAMPLING_FREQUENCY;
        double inc_ms = 1000 / MusicManager.SAMPLING_FREQUENCY;

        for (int i = 0; i < length; i++)
        {
            float curr_gain = strength * gain.Evaluate((float)
                    (t_ms) //overall gain
                    );
            baked[i] =
                (wave_function(
                    t_cycles //the current progress of the note in RADIAN
                    + SampleToRadian(phase_shift) //phase shift in RADIAN
                    + curr_gain * modulation_amplitude
                        * Math.Sin(modulation * t_cycles 
                        / frequency) //modulation in RADIAN
                    )
                    + (rand.NextDouble() - 0.5) * noise * Mathf.Max(0, curr_gain - (float)noise_cutoff)
                )
                * curr_gain
                * polyphonal_gain
                ;
            t_cycles += inc_cycles;
            t_ms += inc_ms;
        }

        return baked;
    }

    private double SampleToRadian(int sample)
    {

        return (sample / MusicManager.SAMPLING_FREQUENCY //second
            * (frequency + pitch_shift)) //number of iterations passed (double hz)
            % 1.0f //percent of the current iteration passed
            * 2 * Mathf.PI; //iteration -> radian
    }
}
