using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CustomSong : MonoBehaviour
{
    public abstract float GetRest(ContinuousInstrument part);
    public abstract float GetNote(ContinuousInstrument part);
    public abstract float GetLength(ContinuousInstrument part);
}
