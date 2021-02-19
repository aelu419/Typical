using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;

public class AmbientScore1 : CustomSong
{
    //external references of instruments
    [FMODUnity.EventRef]
    public string drone_coarse;
    [FMODUnity.EventRef]
    public string drone_fine;
    public Tambourine tambourine;

    //internal representation of instruments
    EventInstance coarse_event, fine_event;
    Tonal coarse_instrument, fine_instrument;
    Tambourine _tam;

    const int COARSE = 0, FINE = 1;

    //note ADSR (or other) envelopes
    public AnimationCurve coarse_gain, fine_gain;

    // Start is called before the first frame update
    void Start()
    {
        
        coarse_event = FMODUnity.RuntimeManager.CreateInstance(drone_coarse);
        fine_event = FMODUnity.RuntimeManager.CreateInstance(drone_fine);

        _tam = Instantiate(tambourine, transform).GetComponent<Tambourine>();
        coarse_instrument = new Tonal(COARSE, this, coarse_event, coarse_gain);
        fine_instrument = new Tonal(FINE, this, fine_event, fine_gain);

        StartCoroutine(coarse_instrument.Iterate());
        StartCoroutine(fine_instrument.Iterate());
        //StartCoroutine(Fine());
    }

    

    public override float GetNote(Tonal tonal)
    {
        switch (tonal.index)
        {
            case (COARSE):
                break;
            case (FINE):
                break;
        }
        return Mathf.FloorToInt(Random.value * 12.0f);
    }

    public override float GetLength(Tonal tonal)
    {
        switch (tonal.index)
        {
            case (COARSE):
                break;
            case (FINE):
                break;
        }
        return 10;
    }

    public override float GetRest(Tonal tonal)
    {
        switch (tonal.index)
        {
            case (COARSE):
                break;
            case (FINE):
                break;
        }
        return 1;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
