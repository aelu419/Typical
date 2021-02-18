using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note
{
    public AnimationCurve semitone; //measured away from A4
    public AnimationCurve level;
    public float accent;
    public float length_beat;

    public Note(float semitone, float length)
    {
        this.semitone = AnimationCurve.Constant(
            -100, 
            MusicManager.Instance.BeatToMS(length) + 100, 
            semitone
            );

        /*
        this.semitone.AddKey(-1, 0);
        this.semitone.AddKey(0, semitone);
        this.semitone.AddKey(MusicManager.Instance.BeatToMS(length), semitone);
        this.semitone.AddKey(MusicManager.Instance.BeatToMS(length) + 1, 0);*/

        this.level = AnimationCurve.Constant(
            -100,
            MusicManager.Instance.BeatToMS(length) + 100,
            1
            );

        /*this.level.AddKey(-1, 0);
        this.level.AddKey(0, 1);
        this.level.AddKey(MusicManager.Instance.BeatToMS(length), 1);
        this.level.AddKey(MusicManager.Instance.BeatToMS(length) + 1, 0);*/

        //Debug.Log(this.level.Evaluate(0));

        this.accent = 0;
        this.length_beat = length;
    }

    public Note(float semitone, float length, float level, float accent) : this(semitone, length)
    {
        this.level = AnimationCurve.Constant(
            -100,
            MusicManager.Instance.BeatToMS(length) + 100,
            level
            );

        /*
        this.level.AddKey(-1, 0);
        this.level.AddKey(0, level);
        this.level.AddKey(MusicManager.Instance.BeatToMS(length), level);
        this.level.AddKey(MusicManager.Instance.BeatToMS(length) + 1, 0);*/

        this.accent = accent;
    }

    public Note(Note[] notes)
    {semitone = new AnimationCurve();
        level = new AnimationCurve();
        if (notes.Length == 0)
        {
            accent = 0;
            length_beat = 0.00001f;
            return;
        }
        accent = notes[0].accent;
        length_beat = notes[0].length_beat;

        semitone.AddKey(-100, notes[0].semitone.Evaluate(0));
        level.AddKey(-100, 0);
        level.AddKey(0, notes[0].level.Evaluate(0));

        for(int i = 1; i < notes.Length - 1; i++)
        {
            semitone.AddKey(
                MusicManager.Instance.BeatToMS(length_beat), 
                notes[i].semitone.Evaluate(0)
                );
            semitone.AddKey(
                MusicManager.Instance.BeatToMS(length_beat + notes[i].length_beat),
                notes[i].semitone.Evaluate(0)
                );

            level.AddKey(MusicManager.Instance.BeatToMS(length_beat),
                notes[i].level.Evaluate(0));

            length_beat += notes[i].length_beat;
        }

        length_beat += notes[notes.Length - 1].length_beat;
        Note last = notes[notes.Length - 1];

        semitone.AddKey(length_beat + 100, last.semitone.Evaluate(0));
        level.AddKey(length_beat, last.level.Evaluate(0));
        level.AddKey(length_beat + 100, 0);
    }
}
