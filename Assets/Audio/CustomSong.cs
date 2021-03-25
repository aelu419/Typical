using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CustomSong : ScriptableObject
{
    public Atonal[] atonals;
    public MonoBehaviour player;

    public abstract void Initialize(MonoBehaviour mb);
}
