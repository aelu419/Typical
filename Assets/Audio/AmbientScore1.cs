using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;

public class AmbientScore1 : MonoBehaviour
{
    //external references of instruments
    [FMODUnity.EventRef]
    public string drone_coarse;
    [FMODUnity.EventRef]
    public string drone_fine;
    public Tambourine tambourine;

    //internal representation of instruments
    EventInstance coarse;
    EventInstance fine;
    Tambourine _tam;

    //note statuses
    float coarse_rest, fine_rest, coarse_t, fine_t;
    float coarse_length_beat, fine_length_beat;
    public AnimationCurve coarse_gain, fine_gain;

    //spatial stuff
    Transform cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
        _tam = Instantiate(tambourine, transform).GetComponent<Tambourine>();
        coarse = FMODUnity.RuntimeManager.CreateInstance(drone_coarse);
        fine = FMODUnity.RuntimeManager.CreateInstance(drone_fine);

        StartCoroutine(Coarse());
        //StartCoroutine(Fine());
    }

    IEnumerator Coarse()
    {
        while (enabled)
        {
            if (coarse_rest <= 0)
            {
                coarse_rest = GetRest();
                //initiate note
                coarse_length_beat = GetLength();
                coarse_t = 0.0f;

                coarse.setParameterByName("Pitch", GetNote());
                coarse.start();
            }
            else
            {
                coarse_rest -= Time.deltaTime / MusicManager.Instance.BeatLength;
                coarse_t += Time.deltaTime / MusicManager.Instance.BeatLength;

                //update note status
                float e_gain = coarse_gain.Evaluate(coarse_t / coarse_length_beat);

                coarse.setParameterByName("Gain", e_gain);
                coarse.set3DAttributes(
                    FMODUnity.RuntimeUtils.To3DAttributes(cam)
                    );
            }
            yield return null;
        }
    }

    public float GetNote()
    {
        return (Random.value * 12 - 6) / 12.0f;
    }

    public float GetLength()
    {
        return 1;
    }

    public float GetRest()
    {
        return 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (coarse_rest <= 0)
        {
            
        }
    }
}
