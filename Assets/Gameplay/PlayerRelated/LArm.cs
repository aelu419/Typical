using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class LArm : MonoBehaviour
{

    public Transform player, claw;
    public SpriteRenderer sprite_;
    [FMODUnity.EventRef]
    public string continuous;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 p = player.position;
        Vector3 c = claw.position;
        Vector3 m = c;
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
