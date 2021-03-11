using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stump : MonoBehaviour
{
    BoxCollider2D box;

    float fall_timer = -1.0f;

    // Start is called before the first frame update
    void Start()
    {
        box = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Rect cam = CameraControler.Instance.CAM;
        if (cam.xMin < transform.position.x
            && transform.position.y < cam.xMax)
        {
            if (fall_timer == -1.0f)
            {
                transform.position = new Vector3(
                    transform.position.x,
                    cam.yMax,
                    0
                    );
                fall_timer = 0.0f;
            }
            else
            {
                fall_timer += Time.deltaTime;
            }
        }
        transform.localPosition = new Vector3(0, box.bounds.size.y, 0);
    }
}
