using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note
{
    public float semitone; //measured away from A4
    public float length_beat;

    public float level = 1;
    public float accent = 0;

    public Note(float semitone, float length)
    {
        this.semitone = semitone;
        this.length_beat = length;
    }

    public Note(float semitone, float length, float level, float accent) : this(semitone, length)
    {
        this.level = level;
        this.accent = accent;
    }
}
