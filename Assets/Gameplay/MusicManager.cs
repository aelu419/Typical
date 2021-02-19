using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public const float SAMPLING_FREQUENCY = 44100.0f;
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
    public float BeatLength; //length of 1 beat in SECONDS
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

    /*
    public void PlayOneShot(CustomOneShot oneshot, Vector3 position)
    {

    }*/

    public float BeatToMS(float b)
    {
        return b / BPM * 60.0f * 1000.0f;
    }

    public float MSToBeat(float ms)
    {
        return ms / 1000.0f / 60.0f * BPM;
    }

    public void ResetTimer()
    {
        timer = 0.0f;
        beat = 0.0f;
    }
}
