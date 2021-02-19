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
    [ReadOnly]
    public float coarse_rest, fine_rest, coarse_t, fine_t;
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
                Debug.Log("new note for coarse");
                coarse_rest = GetRest();
                //initiate note
                coarse_length_beat = GetLength();
                coarse_t = 0.0f;

                coarse.setParameterByName("Pitch", GetNote());
                coarse.setParameterByName("Gain", coarse_gain.Evaluate(0));
                coarse.start();
            }
            else
            {
                if (coarse_t == -1)
                {
                    coarse.setParameterByName("Gain", 0.0f);
                    coarse_rest -= Time.deltaTime / MusicManager.Instance.BeatLength;
                }
                else
                {
                    coarse_t += Time.deltaTime / MusicManager.Instance.BeatLength;

                    if (coarse_t >= coarse_length_beat)
                    {
                        coarse_t = -1;
                        coarse.release();
                    }

                    //update note status
                    float e_gain = coarse_gain.Evaluate(coarse_t / coarse_length_beat);
                    float p, g;
                    coarse.getParameterByName("Pitch", out p);
                    coarse.getParameterByName("Gain", out g);
                    Debug.Log(
                        "Pitch: " + p
                        + " Gain: " + g
                        );

                    coarse.setParameterByName("Gain", e_gain);
                    coarse.set3DAttributes(
                        FMODUnity.RuntimeUtils.To3DAttributes(cam)
                        );
                }
            }
            yield return null;
        }
    }

    public float GetNote()
    {
        return Mathf.FloorToInt(Random.value * 12.0f);
    }

    public float GetLength()
    {
        return 10;
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
