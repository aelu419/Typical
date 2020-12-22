using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualManager : MonoBehaviour
{
    [ReadOnly]public Vector2 CAM_VIEW_SIZE; //size of the visible range of the world, in world units

    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        //fetch main camera and get settings
        cam = Camera.main;
        CAM_VIEW_SIZE = new Vector2(2f * cam.orthographicSize * cam.aspect, 2f * cam.orthographicSize);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
