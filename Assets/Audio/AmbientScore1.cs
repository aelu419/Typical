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


    int[] keys = {-12, -10, -8, -7, -5, -3, -1, 0, 2, 4, 5, 7, 9, 11, 12};
    public override float GetNote(Tonal tonal)
    {
        switch (tonal.index)
        {
            case (COARSE):
                break;
            case (FINE):
                break;
        }
        return keys[Mathf.FloorToInt(Random.value * keys.Length)];
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
        return 2;
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

    IEnumerator TambourineAtBeat(float bluntness, float beat)
    {
        while (MusicManager.Instance.beat <= beat)
        {
            yield return null;
        }

        //fire tambourine
        tambourine.PlayNote(1, bluntness, MusicManager.Instance.transform.position);

        yield return null;
    }

    public void OnNoteFinished(int instrument)
    {
        switch(instrument) {
            case COARSE:
                StartCoroutine(TambourineAtBeat(
                    -0.5f,
                    Mathf.Ceil(MusicManager.Instance.beat + 1)
                    )
                );
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
