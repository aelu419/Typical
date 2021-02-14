using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongGenerator : MonoBehaviour
{
    bool first_update;

    public int phaser;

    // Start is called before the first frame update
    void Start()
    {
        first_update = true;
    }

    public AnimationCurve envelope1, envelope2, envelope3;

    // Update is called once per frame
    void Update()
    {
        if (first_update)
        {
            Debug.Log("registering notes");
            List<Note> notes_to_add = new List<Note>();
            notes_to_add.Add(
                Note.GetRawNote(3, 0, 1, 440, Note.Waveform.sine)
                );
            notes_to_add.Add(
                Note.GetRawNote(3, phaser, 1, 880, Note.Waveform.sine)
                );
            notes_to_add.Add(
                Note.GetRawNote(3, 0, 1, 440, Note.Waveform.triangular)
                );
            notes_to_add.Add(
                Note.GetRawNote(3, phaser, 1, 880, Note.Waveform.triangular)
                );
            notes_to_add.Add(
                Note.GetRawNote(3, 0, 1, 440, Note.Waveform.sawtooth)
                );
            notes_to_add.Add(
                Note.GetRawNote(3, phaser, 1, 880, Note.Waveform.sawtooth)
                );
            /*
            notes_to_add.Add(
                Note.GetRawNote(3, 0, 1, 440, Note.Waveform.square)
                );
            notes_to_add.Add(
                Note.GetRawNote(3, 10, 1, 880, Note.Waveform.square)
                );*/

            for (int i = 0; i < notes_to_add.Count; i += 2)
            {
                notes_to_add[i].entrance_beat = 3 * i / 2.0f + 1;
                notes_to_add[i].oscillator_ID = 0;
                notes_to_add[i].ApplyEnvelope(envelope1);

                notes_to_add[i+1].entrance_beat = 3 * i / 2.0f + 1;
                notes_to_add[i+1].oscillator_ID = 0;
                notes_to_add[i+1].ApplyEnvelope(envelope1);


                MusicManager.Instance.RegisterNote(notes_to_add[i]);
                MusicManager.Instance.RegisterNote(notes_to_add[i + 1]);
            }
            first_update = false;
        }
    }
}
