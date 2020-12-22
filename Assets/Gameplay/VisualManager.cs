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
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            cam.transform.position = new Vector3(
                cam.transform.position.x - Time.deltaTime * CAM_SPEED,
                cam.transform.position.y,
                cam.transform.position.z);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            cam.transform.position = new Vector3(
                cam.transform.position.x + Time.deltaTime * CAM_SPEED,
                cam.transform.position.y,
                cam.transform.position.z);
        }
    }
}
