using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneshotPerioder : MonoBehaviour
{
    [FMODUnity.EventRef]
    public string sound;
    public bool play;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (play)
        {
            FMODUnity.RuntimeManager.PlayOneShot(sound, transform.position);
            play = false;
        }
    }
}
