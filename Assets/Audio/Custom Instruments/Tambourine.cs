using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tambourine : OneShot
{
    [FMODUnity.EventRef]
    public string event_path;
    [FMODUnity.EventRef]
    public List<string> streaks;
    [Range(0, 1)]
    public float bluntness;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public new void PlayNote()
    {
        FMODUnity.RuntimeManager.PlayOneShot(event_path, MusicManager.Instance.transform.position);
    }

    public void PlayNote(float gain, Vector3 pos)
    {
        FMOD.Studio.EventInstance hit = FMODUnity.RuntimeManager.CreateInstance(event_path);
        hit.setParameterByName("Bluntness", bluntness);
        hit.setVolume(gain);

        transform.position = pos;
        hit.set3DAttributes(
            FMODUnity.RuntimeUtils.To3DAttributes(transform)
            );
        hit.start();
    }

    public void PlayNote(float gain, float bluntness, Vector3 pos)
    {
        FMOD.Studio.EventInstance hit = FMODUnity.RuntimeManager.CreateInstance(event_path);
        hit.setParameterByName("Bluntness", bluntness);
        hit.setVolume(gain);

        transform.position = pos;
        hit.set3DAttributes(
            FMODUnity.RuntimeUtils.To3DAttributes(transform)
            );
        hit.start();
    }

    public void PlayStreak(int index, float gain, Vector3 pos)
    {
        FMOD.Studio.EventInstance hit = FMODUnity.RuntimeManager.CreateInstance(streaks[index]);

        hit.setVolume(gain);

        hit.set3DAttributes(
            FMODUnity.RuntimeUtils.To3DAttributes(transform)
            );
        hit.start();
    }
}
