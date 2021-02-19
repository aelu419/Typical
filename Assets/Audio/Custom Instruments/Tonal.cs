using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;

public class Tonal : ContinuousInstrument
{
    //note statuses
    [ReadOnly]
    public float rest, t;
    float length_beat;
    public AnimationCurve envelope;

    public Tonal(
        int index, CustomSong song, string fmod_event_address, AnimationCurve envelope, 
        float noise_velocity, float noise_amplitude, float gain_master
        ) : base(song, fmod_event_address, index, noise_velocity, noise_amplitude, gain_master)
    {
        this.envelope = envelope;
    }

    public IEnumerator Iterate()
    {
        while (song.enabled)
        {

            //when rest finishes, initiate note
            if (rest <= 0)
            {
                float note = song.GetNote(this);
                if (note == -1)
                {
                    break;
                }
                Debug.Log("new note for " + index);
                rest = song.GetRest(this);
                length_beat = song.GetLength(this);
                t = 0.0f;

                fmod_event = FMODUnity.RuntimeManager.CreateInstance(fmod_event_address);

                fmod_event.setParameterByName("Pitch", note);
                fmod_event.setParameterByName("Gain", envelope.Evaluate(0));

                Debug.Log("evaluation: " + note + " , " + envelope.Evaluate(0));
                float n_, e_;
                fmod_event.getParameterByName("Pitch", out n_);
                fmod_event.getParameterByName("Gain", out e_);
                Debug.Log("immediately after: " + n_ + ", " + e_);

                fmod_event.start();

            }
            //when rest is not finished
            else
            {
                //if there is no current note
                if (t == -1)
                {
                    //fmod_event.setParameterByName("Gain", 0.0f);
                    rest -= Time.deltaTime / MusicManager.Instance.BeatLength;
                }
                //if there is a current note
                else
                {
                    t += Time.deltaTime / MusicManager.Instance.BeatLength;
                    //when the current note finishes
                    if (t >= length_beat)
                    {
                        Debug.Log("Note finished");
                        t = -1;
                        song.BroadcastMessage("OnNoteFinished", index);
                        fmod_event.release();
                        continue;
                    }

                    //update note status
                    float e_gain = envelope.Evaluate(t / length_beat);
                    /*float p, g;
                    /fmod_event.getParameterByName("Pitch", out p);
                    /fmod_event.getParameterByName("Gain", out g);

                    Debug.Log(
                        "Pitch: " + p
                        + " Gain: " + g
                        );*/

                    fmod_event.setParameterByName("Gain", e_gain * GetNoisyGain());

                    fmod_event.set3DAttributes(
                        FMODUnity.RuntimeUtils.To3DAttributes(MusicManager.Instance.transform)
                        );
                }
            }
            yield return null;
        }
        yield return null;
    }
}
