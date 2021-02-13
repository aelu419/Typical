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

    private void OnEnable()
    {
        mm = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
