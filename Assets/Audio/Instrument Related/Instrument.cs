using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;

public class Instrument : MonoBehaviour
{
    [Range(0, 1)]
    public double volume;
    public int unison, shift;
    [Range(0.000001f, 100)]
    public float gain_spread;
    [Range(0, 100)]
    public double detune;
    public List<int> additional_detunes;
    [Range(0, 30)]
    public float pitch_fade;
    public double modulation;
    [Range(0, 10)]
    public double modulation_amplitude;
    [Range(0, 0.25f)]
    public double noise;
    [Range(0, 1)]
    public double noise_cutoff;
    public double atk, dec, rel;

    [ReadOnly]
    public Note note;
    [ReadOnly]
    public Note override_note = null;
    [ReadOnly]
    public float strength;
    
    private List<MonoWave> playable_waves;
    private List<Oscillator> channels;

    //the current status of the instrument
    [ReadOnly]
    public double t_ms; //time in MILISECOND
    private double ta, td, ts, tr;
    [ReadOnly]
    public AnimationCurve envelope_curve;

    private void Start()
    {
        t_ms = -1.0f;

        playable_waves = new List<MonoWave>();
        channels = new List<Oscillator>();
        float weight_sum = 0;

        for(int i = 0; 
            i < unison + (additional_detunes == null ? 0 : additional_detunes.Count); 
            i++)
        {
            MonoWave temp = GetWave();

            temp.modulation = modulation;
            temp.modulation_amplitude = modulation_amplitude;
            temp.gain = null;
            temp.noise = noise;
            temp.noise_cutoff = noise_cutoff;
            temp.pitch_fade = pitch_fade;
            temp.strength = 1.0f;

            //set the pitch shift for unison notes (constant spread centering note itself)
            if (i < unison)
            {
                float deviation = i - (unison - 1) / 2.0f;
                temp.pitch_shift = deviation * detune;
                temp.phase_shift = (int)deviation * shift * i;
                //weight by bell curve
                temp.polyphonal_gain = Mathf.Exp(-1 * deviation * deviation / gain_spread);
                weight_sum += temp.polyphonal_gain;
            }
            //set the pitch shift for additional detuned notes
            else
            {
                temp.pitch_shift = additional_detunes[i - unison];
                temp.polyphonal_gain = 0.03f;
            }
            
            playable_waves.Add(temp);

            //instantiate one channel for each note
            GameObject go = Instantiate(
                MusicManager.Instance.oscillator_channel,
                transform
                );
            Oscillator o = go.GetComponent<Oscillator>();
            o.wave = temp;
        }
        
        //set polyphonal gain
        for (int i = 0; i < playable_waves.Count; i++)
        {
            if (i < unison)
            {
                playable_waves[i].polyphonal_gain /= weight_sum;
            }
        }
    }

    private void Update()
    {
        if (override_note != null)
        {
            LoadNote(override_note);
            override_note = null;
        }

        t_ms += 1000 * Time.deltaTime;
        if (t_ms >= tr)
        {
            FinishNote();
        }
        else if (note != null)
        {
            strength = 1 - 0.3f * Mathf.PerlinNoise(0, (float)t_ms / 3000.0f);
            foreach (MonoWave w in playable_waves)
            {
                w.strength = strength;
            }
        }
    }

    public MonoWave GetWave()
    {
        MonoWave m1 = new PrimitiveMonoWave(PrimitiveMonoWave.Waveform.SINE);
        return m1;
    }

    private void LoadNote(Note n)
    {
        note = n;
        Debug.Log("New Note Assigned, with tone: " + n.semitone + " and length: " + n.length_beat);

        t_ms = 0;
        //harsher notes have shorter attack duration
        ta = atk * (1 - n.accent);
        td = atk * (1 + n.accent) + dec;

        double ms_length = n.length_beat 
            * MusicManager.Instance.BeatLength 
            * 1000.0f;

        ts = ms_length;
        tr = ts + rel;
        Debug.Log("Note Segments: " + ta + ", " + td + ", " + ts + ", " + tr);

        //ADSR curve
        envelope_curve = AnimationCurve.EaseInOut(0, 0, (float)ta, 1 * n.level);
        envelope_curve.AddKey(new Keyframe((float)td, 0.75f * n.level, 0, 0));
        envelope_curve.AddKey(new Keyframe((float)ts, 0.7f * n.level, 0, 0));
        envelope_curve.AddKey(new Keyframe((float)tr, 0.0f, 0, 0));

        //note general properties
        double f = 440.0f * Mathf.Pow(2.0f, n.semitone / 12.0f);
        for(int i = 0; i < playable_waves.Count; i++)
        {
            if (i < unison)
            {
                playable_waves[i].frequency = f + playable_waves[i].pitch_shift;
            }
            else
            {
                playable_waves[i].frequency = f * 
                    Mathf.Pow(2.0f, (float)playable_waves[i].pitch_shift / 12.0f);
            }
            playable_waves[i].gain = envelope_curve;
            playable_waves[i].t_ms = 0;
            playable_waves[i].t_cycles = 0 + playable_waves[i].phase_shift;
        }
    }

    private void FinishNote()
    {
        Debug.Log("Note Finished Playing");
        
        foreach (MonoWave m in playable_waves)
        {
            m.frequency = 0.0f;
            m.t_ms = 0;
            m.t_cycles = 0;
            m.gain = null;
        }

        t_ms = -1;
        note = null;
    }
}
