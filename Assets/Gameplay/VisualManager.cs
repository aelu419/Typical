using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualManager : MonoBehaviour
{
    [ReadOnly]public Vector2 CAM_VIEW_SIZE; //size of the visible range of the world, in world units
    public float bufferSize = 0.5f; //size of the buffer area around the visible range, in world units

    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        //fetch main camera and get settings
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
