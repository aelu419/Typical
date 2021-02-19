using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CustomSong : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public abstract float GetRest(Tonal tonal);
    public abstract float GetNote(Tonal tonal);
    public abstract float GetLength(Tonal tonal);
}
