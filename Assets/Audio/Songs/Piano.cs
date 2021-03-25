using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;

[CreateAssetMenu(menuName = "Typical Customs/Songs/Piano")]
public class Piano : PureAmbient
{
    [FMODUnity.EventRef]
    public string piano;

    public override void Initialize(MonoBehaviour mb)
    {
        base.Initialize(mb);
        player.StartCoroutine(PianoRoutine());
    }

    IEnumerator PianoRoutine()
    {
        while (player.enabled)
        {
            yield return null;
        }
        yield return null;
    }
}
