using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualManager : MonoBehaviour
{
    [ExecuteAlways]
    [ReadOnly] public Rect CAM; //the range of the camera as a rectangle, in world units
    public float BUFFER_SIZE;

    public float CAM_SPEED;
    private Camera cam;
    private ReadingManager rManager;

    // Start is called before the first frame update
    void Start()
    {
        //fetch main camera and get settings
        cam = Camera.main;
        CAM = cam.rect;
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 cam_translation = new Vector3(
            Input.GetAxis("Horizontal") * CAM_SPEED * Time.deltaTime,
            Input.GetAxis("Vertical") * CAM_SPEED * Time.deltaTime,
            0
            );

        cam.transform.Translate(cam_translation);
    }
}
