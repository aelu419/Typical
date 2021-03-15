using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class OneshotPerioder : MonoBehaviour
{
    [FMODUnity.EventRef]
    public string sound;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void FireSound()
    {
        FMODUnity.RuntimeManager.PlayOneShot(sound, transform.position);
    }
}
