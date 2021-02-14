using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongGenerator : MonoBehaviour
{
    bool first_update;
    // Start is called before the first frame update
    void Start()
    {
        first_update = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (first_update)
        {
            Debug.Log("registering notes");
            List<Note> notes_to_add = new List<Note>();
            notes_to_add.Add(
                Note.GetRawNote(1, 0, 1, 440, Note.Waveform.sine)
                );
            notes_to_add.Add(
                Note.GetRawNote(1, 0, 1, 440, Note.Waveform.triangular)
                );
            notes_to_add.Add(
                Note.GetRawNote(1, 0, 1, 440, Note.Waveform.sawtooth)
                );
            notes_to_add.Add(
                Note.GetRawNote(1, 0, 1, 440, Note.Waveform.square)
                );

            for (int i = 0; i < notes_to_add.Count; i++)
            {
                notes_to_add[i].entrance_beat = i + 1;
                notes_to_add[i].oscillator_ID = 0;
                MusicManager.Instance.RegisterNote(notes_to_add[i]);
            }
            first_update = false;
        }
    }
}
