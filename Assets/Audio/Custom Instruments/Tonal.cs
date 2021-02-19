using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;

public class Tonal
{
    public int index;
    CustomSong song;
    EventInstance fmod_event;

    //note statuses
    [ReadOnly]
    public float rest, t;
    float length_beat;
    public AnimationCurve envelope;

    public Tonal(int index, CustomSong song, EventInstance fmod_event, AnimationCurve envelope)
    {
        this.index = index;
        this.song = song;
        this.fmod_event = fmod_event;
        this.envelope = envelope;
    }

    public IEnumerator Iterate()
    {
        while (song.enabled)
        {

            //when rest finishes, initiate note
            if (rest <= 0)
            {
                Debug.Log("new note for fmod_event");
                rest = song.GetRest(this);
                length_beat = song.GetLength(this);
                t = 0.0f;

                fmod_event.setParameterByName("Pitch", song.GetNote(this));
                fmod_event.setParameterByName("Gain", envelope.Evaluate(0));
                fmod_event.start();
            }
            //when rest is not finished
            else
            {
                //if there is no current note
                if (t == -1)
                {
                    fmod_event.setParameterByName("Gain", 0.0f);
                    rest -= Time.deltaTime / MusicManager.Instance.BeatLength;
                }
                //if there is a current note
                else
                {
                    t += Time.deltaTime / MusicManager.Instance.BeatLength;
                    //when the current note finishes
                    if (t >= length_beat)
                    {
                        t = -1;
                        song.BroadcastMessage("OnNoteFinished", index);
                        fmod_event.release();
                        continue;
                    }

                    //update note status
                    float e_gain = envelope.Evaluate(t / length_beat);
                    float p, g;
                    fmod_event.getParameterByName("Pitch", out p);
                    fmod_event.getParameterByName("Gain", out g);

                    /*
                    Debug.Log(
                        "Pitch: " + p
                        + " Gain: " + g
                        );*/

                    fmod_event.setParameterByName("Gain", e_gain);
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
