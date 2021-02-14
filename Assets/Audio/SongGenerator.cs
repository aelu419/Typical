using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongGenerator : MonoBehaviour
{
    bool first_update;

    // Start is called before the first frame update
    void Start()
    {
        first_update = true;
    }

    public AnimationCurve envelope1, envelope2, envelope3;

    //strength patterns
    public float[][] patterns = {
        new float[]{1.0f },
        new float[]{1.0f, 0.8f},
        new float[]{1.0f, 0.8f, 0.8f},
        new float[]{1.0f, 0.8f, 0.9f, 0.8f},
        new float[]{1.0f, 0.8f, 0.9f, 0.8f, 0.75f}
    };

    // Update is called once per frame
    void Update()
    {
        if (first_update)
        {
            Debug.Log("registering notes");
            for(int i = 0; i < patterns.Length; i++)
            {
                for(int j = 0; j <= i; j++)
                {
                    PlayNote(
                        j%2 == 0? 0 : -5,
                        1.0f / (i + 1),
                        patterns[i][j],
                        1.0f + 2 * i + (j + 1) * (1.0f / (i + 1))
                        );
                }
            }
            first_update = false;
        }
    }

    public void PlayNote(float note, float length, float gain, float at)
    {
        Note t = Note.GetRawNote(
                length,
                0,
                gain,
                440.0f * Mathf.Pow(2, note / 12.0f) + GetError(0.25f),
                Note.Waveform.triangular
            );

        t.entrance_beat = at;
        t.oscillator_ID = 0;
        t.ApplyEnvelope(envelope1);
        MusicManager.Instance.RegisterNote(t);
    }

    public float GetError(float range)
    {
        return Random.value * range - range / 2.0f;
    }
}
