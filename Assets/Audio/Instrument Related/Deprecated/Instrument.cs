using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;
/*
public class Instrument : MonoBehaviour
{
    [Range(0, 1)]
    public float volume;
    public int unison, shift;
    [Range(0.000001f, 100)]
    public float gain_spread;
    [Range(0, 100)]
    public float detune;
    public List<float> additional_detunes;
    [Range(0, 30)]
    public float pitch_fade;
    public float modulation;
    [Range(0, 10)]
    public float modulation_amplitude;
    [Range(0, 0.25f)]
    public float noise;
    [Range(0, 1)]
    public float noise_cutoff;
    [Range(0, 1)]
    public float pop_cutoff;
    public float atk, dec, rel;

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
            temp.noise = noise;
            temp.noise_cutoff = noise_cutoff;
            temp.pop_cutoff = pop_cutoff;
            temp.pitch_fade = pitch_fade;
            temp.strength = 1.0f;

            //set the pitch shift for unison notes (constant spread centering note itself)
            if (i < unison)
            {
                float deviation = i - (unison - 1) / 2.0f;
                temp.pitch_shift = deviation * detune;
                Debug.Log(temp.pitch_shift);
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

        if (note != null)
        {
            strength = 1 - 0.3f * Mathf.PerlinNoise(0, (float)t_ms / 3000.0f);
            foreach (MonoWave w in playable_waves)
            {
                w.strength = strength;
            }
            t_ms += 1000 * Time.deltaTime;
            //Debug.Log(t_ms + " out of " + tr);
            if (t_ms >= tr + 100)
            {
                FinishNote();
            }
        }
    }

    public MonoWave GetWave()
    {
        MonoWave m1 = new PrimitiveMonoWave(PrimitiveMonoWave.Waveform.SINE);
        return m1;
    }

    public static void PrintKeys(AnimationCurve c) {
        string result = "";
        for (int i = 0; i < c.keys.Length; i++)
        {
            result += c.keys[i].time + " - " + c.keys[i].value + "\t";
        }
        Debug.Log(result);
    }


    private void LoadNote(Note n)
    {
        note = n;
        Debug.Log("New Note Assigned, with tone: " + n.semitone.Evaluate(0) + " and length: " + n.length_beat);
        PrintKeys(n.semitone);
        PrintKeys(n.level);

        t_ms = 0;
        //harsher notes have shorter attack duration
        ta = atk * (1 - n.accent);
        td = ta + dec;

        double ms_length = n.length_beat 
            * MusicManager.Instance.BeatLength 
            * 1000.0f;

        ts = ms_length;
        tr = ts + rel;
        //Debug.Log("Note Segments: " + ta + ", " + td + ", " + ts + ", " + tr);

        //ADSR curve
        envelope_curve = AnimationCurve.Constant(float.MinValue, float.MaxValue, 0);
        envelope_curve.AddKey(0, 0);
        envelope_curve.AddKey(new Keyframe((float)ta, 1, 0, 0));
        envelope_curve.AddKey(new Keyframe((float)td, 0.75f, 0, 0));
        envelope_curve.AddKey(new Keyframe((float)ts, 0.7f, 0, 0));
        envelope_curve.AddKey(new Keyframe((float)tr, 0.0f, 0, 0));

        //note general properties
        for(int i = 0; i < playable_waves.Count; i++)
        {
            playable_waves[i].semitone = n.semitone;
            playable_waves[i].envelope_gain = envelope_curve;
            playable_waves[i].note_gain = n.level;
            playable_waves[i].t_ms = 0;
            playable_waves[i].t_cycles = 0 + playable_waves[i].phase_shift;
        }
    }

    private void FinishNote()
    {
        Debug.Log("Note Finished Playing");
        
        foreach (MonoWave m in playable_waves)
        {
            m.Initialize();
        }

        t_ms = -1;
        note = null;
    }
}
*/