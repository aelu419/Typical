using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class LArm : MonoBehaviour
{

    public GameObject player, claw;
    public SpriteRenderer sprite_;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 p = player.transform.position;
        Vector3 c = new Vector3(
            claw.transform.position.x,
            claw.transform.position.y,
            0
            );
        Vector3 m = (p + c) / 2;
        Vector3 r = p - c;

        sprite_.size = new Vector2(r.magnitude / 2, sprite_.size.y);
        transform.position = m;

        if (r.magnitude > 0.1)
        {
            float angle = Mathf.Atan2(r.y, r.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(
                new Vector3(
                    0,
                    0,
                    angle
                    ));
        }
    }
}
