using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public const float SAMPLING_FREQUENCY = 48000.0f;
    private static MusicManager mm;
    public static MusicManager Instance
    {
        get
        {
            return mm;
        }
    }

    public float timer;
    public float BPM;
    public float beat;

    //available oscillators in the scene
    private List<Oscillator> oscillators_available;
    public List<Oscillator> Oscillators
    {
        get
        {
            return oscillators_available;
        }
    }

    private void OnEnable()
    {
        mm = this;
        oscillators_available = new List<Oscillator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        timer = 0.0f;
        beat = 0.0f;

        EventManager.Instance.OnStartExitingScene += UnregisterOscillators;
    }

    private void UnregisterOscillators()
    {
        oscillators_available.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        beat = timer / 60.0f * BPM;
    }

    public void ResetTimer()
    {
        timer = 0.0f;
        beat = 0.0f;
    }

    public void RegisterOscillator(Oscillator o)
    {
        Debug.Log("trying to register " + o);
        if (!oscillators_available.Contains(o))
        {
            oscillators_available.Add(o);
        }
    }

    public void RegisterNote(Note n)
    {
        if (oscillators_available == null || oscillators_available.Count == 0)
        {
            throw new UnityException("no available oscillators in the scene");
        }

        if (n.entrance_beat >= beat)
        {
            foreach (Oscillator i in oscillators_available)
            {
                if (i.ID == n.oscillator_ID)
                {
                    i.PendNote(n);
                    return;
                }
            }
            throw new UnityException("oscillator with ID:" + n.oscillator_ID + "cannot be found");
        }
        else
        {
            Debug.LogError("note is assigned too late: " + beat + " already passed " + n.entrance_beat);
        }
    }
}
