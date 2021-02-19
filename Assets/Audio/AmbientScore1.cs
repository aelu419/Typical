using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;

public class AmbientScore1 : CustomSong
{
    //external references of instruments
    [FMODUnity.EventRef]
    public string drone_coarse;
    [Range(0, 1)]
    public float coarse_fluctuation_amplitude;
    [Range(0, 5)]
    public float coarse_flucuation_speed;
    [Range(0, 1)]
    public float coarse_gain_master;

    [FMODUnity.EventRef]
    public string drone_fine;
    [Range(0, 1)]
    public float fine_fluctuation_amplitude;
    [Range(0, 5)]
    public float fine_flucuation_speed;
    [Range(0, 1)]
    public float fine_gain_master;

    [FMODUnity.EventRef]
    public string whisper;
    [Range(0, 1)]
    public float whisper_fluctuation_amplitude;
    [Range(0, 5)]
    public float whisper_flucuation_speed;
    [Range(0, 1)]
    public float whisper_gain_master;

    [FMODUnity.EventRef]
    public string ambient;
    [Range(0, 1)]
    public float ambient_fluctuation_amplitude;
    [Range(0, 5)]
    public float ambient_flucuation_speed;
    [Range(0, 1)]
    public float ambient_gain_master;

    public Tambourine tambourine;
    [Range(0, 5)]
    public float tambourine_gain_modifier;

    //internal representation of instruments
    Tonal coarse_instrument, fine_instrument;
    Atonal whisper_instrument, ambient_instrument;
    Tambourine _tam;

    //ID of instruments under the song
    const int COARSE = 0, FINE = 1, WHISPER = 2, AMBIENT = 3;

    //note ADSR (or other) envelopes
    public AnimationCurve coarse_gain, fine_gain;

    // Start is called before the first frame update
    void Start()
    {
        _tam = Instantiate(tambourine, transform).GetComponent<Tambourine>();
        
        coarse_instrument = new Tonal(COARSE, this, drone_coarse, coarse_gain, 
            coarse_flucuation_speed, coarse_fluctuation_amplitude, coarse_gain_master);

        fine_instrument = new Tonal(FINE, this, drone_fine, fine_gain,
            fine_flucuation_speed, fine_fluctuation_amplitude, fine_gain_master);

        whisper_instrument = new Atonal(WHISPER, this, whisper,
            whisper_flucuation_speed, whisper_fluctuation_amplitude, whisper_gain_master);

        ambient_instrument = new Atonal(AMBIENT, this, ambient,
            ambient_flucuation_speed, ambient_fluctuation_amplitude, ambient_gain_master);

        StartCoroutine(coarse_instrument.Iterate());
        StartCoroutine(fine_instrument.Iterate());

        StartCoroutine(whisper_instrument.Iterate());
        StartCoroutine(ambient_instrument.Iterate());
        whisper_instrument.Start();
        ambient_instrument.Start();
    }


    int[] keys = {-12, -10, -8, -7, -5, -3, -1, 0, 2, 4, 5, 7, 9, 11, 12};
    public override float GetNote(ContinuousInstrument part)
    {
        return keys[Mathf.FloorToInt(Random.value * keys.Length)];
    }

    public override float GetLength(ContinuousInstrument part)
    {
        return Mathf.Ceil(Random.value * 20) + 7;
    }

    public override float GetRest(ContinuousInstrument part)
    {
        return Mathf.Ceil(Random.value * 10) + 3;
    }

    int tambourine_t = 0;

    IEnumerator TambourineAtBeat(float bluntness, float beat, float gain)
    {
        while (MusicManager.Instance.beat <= beat)
        {
            yield return null;
        }

        //fire tambourine
        if (tambourine_t % 2 == 0)
        {
            //play single note
            tambourine.PlayNote(gain, bluntness);
        }
        else
        {
            //play streak
            if (tambourine_t == 1)
            {
                tambourine.PlayStreak(0, gain);
            }
            else
            {
                tambourine.PlayStreak(1, gain);
            }
        }
        tambourine_t++;

        yield return null;
    }

    public void OnNoteFinished(int instrument)
    {
        switch(instrument) {
            case COARSE:
                StartCoroutine(TambourineAtBeat(
                    -0.5f,
                    Mathf.Ceil(MusicManager.Instance.beat + 1),
                    coarse_gain_master * tambourine_gain_modifier
                    )
                );
                break;
            case FINE:
                StartCoroutine(TambourineAtBeat(
                    0.5f,
                    Mathf.Ceil(MusicManager.Instance.beat + 1),
                    fine_gain_master * tambourine_gain_modifier
                    )
                );
                break;
            case WHISPER:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
