using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongGenerator : MonoBehaviour
{
    Instrument ambient1, ambient2;
    float rest1, rest2; //rest times in SECONDS

    // Start is called before the first frame update
    void Start()
    {
        ambient1 = transform.GetChild(0).GetComponent<Instrument>();
        ambient2 = transform.GetChild(1).GetComponent<Instrument>();

        rest1 = GetRestTime();
        rest2 = GetRestTime();
    }

    //note strength patterns
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
        if (ambient1.note == null)
        {
            if (rest1 <= 0)
            {
                ambient1.override_note = PickNote();
                rest1 = GetRestTime();
            }
        }
        else
        {
            rest1 -= Time.deltaTime;
        }
        if (ambient2.note == null)
        {
            rest2 -= Time.deltaTime;
            if (rest2 <= 0)
            {
                ambient2.override_note = PickNote();
                rest2 = GetRestTime();
            }
        }
        else
        {
            rest2 -= Time.deltaTime;
        }
        if (ambient1.note == null && ambient2.note == null)
        {
            rest1 = 0;
        }
    }

    int[] keys = { 0, 2, 3, 5, 7, 8, 10 };
    private Note PickNote()
    {
        int chosen = keys[Mathf.FloorToInt(Random.value * keys.Length)];
        chosen += 12 * (Mathf.FloorToInt(Random.value * 3) - 1);
        return new Note(chosen, Random.value * 10 + 5);
    }

    private float GetRestTime()
    {
        float result = 1000.0f * (1.5f + Random.value * 1.5f) / 60.0f * MusicManager.Instance.BPM;
        Debug.Log(result);
        return result;
    }

    public float GetError(float range)
    {
        return Random.value * range - range / 2.0f;
    }
}
