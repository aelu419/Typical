using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControler : MonoBehaviour
{
    [ExecuteAlways]
    [ReadOnly] public Rect CAM; //the range of the camera as a rectangle, in world units
    public float BUFFER_SIZE;

    public float CAM_SPEED;
    private Camera cam;
    private ReadingManager rManager;
    private GameObject player;

    private static CameraControler _instance;
    public static CameraControler Instance
    {
        get { return _instance; }
    }

    // Start is called before the first frame update
    void Start()
    {
        _instance = this;
        //fetch main camera and get settings
        cam = Camera.main;
        player = PlayerControl.Instance.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        float cam_h = 2f * cam.orthographicSize;
        float cam_w = cam_h * cam.aspect;

        cam.transform.position = new Vector3(
            player.transform.position.x,
            player.transform.position.y,
            -10);

        CAM = new Rect(
            cam.transform.position.x - cam_w / 2f, 
            cam.transform.position.y - cam_h / 2f,
            cam_w, cam_h);

        /*
        Vector3 cam_translation = new Vector3(
            Input.GetAxis("Horizontal") * CAM_SPEED * Time.deltaTime,
            Input.GetAxis("Vertical") * CAM_SPEED * Time.deltaTime,
            0
            );

        cam.transform.Translate(cam_translation);*/
    }
}
