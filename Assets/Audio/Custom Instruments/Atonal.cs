using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;

public class Atonal : ContinuousInstrument
{
    bool enable;

    public Atonal(
        int index, CustomSong song, string fmod_event_address,
        float noise_velocity, float noise_amplitude, float gain_master) 
        : base(song, fmod_event_address, index, noise_velocity, noise_amplitude, gain_master)
    {
        enable = false;
    }

    public void Start()
    {
        //first frame on playing note
        Debug.Log("Atonal instrument active");
        fmod_event = FMODUnity.RuntimeManager.CreateInstance(fmod_event_address);
        fmod_event.start();
        enable = true;
    }

    public void End()
    {
        Debug.Log("Atonal instrument inactive");
        enable = false;
        fmod_event.setVolume(0.0f);
    }

    public IEnumerator Iterate()
    {
        while (song.player.enabled)
        {
            if (enable)
            {
                //subsequent frames
                float noisy = GetNoisyGain();
                fmod_event.setVolume(noisy);
                fmod_event.set3DAttributes(
                    FMODUnity.RuntimeUtils.To3DAttributes(MusicManager.Instance.transform)
                    );
            }
            yield return null;
        }
        yield return null;
    }
}
