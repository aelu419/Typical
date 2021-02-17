using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public const double SAMPLING_FREQUENCY = 48000.0f;
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
    [ReadOnly]
    public double BeatLength; //length of 1 beat in SECONDS
    public float beat;
    public GameObject oscillator_channel;

    private void OnEnable()
    {
        mm = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        timer = 0.0f;
        beat = 0.0f;
        BeatLength = 1.0f / (BPM / 60.0f); 
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
}
