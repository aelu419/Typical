using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wobbler : MonoBehaviour
{
    Animator player_anim;
    //[ReadOnly]
    //public float speed;

    public float wobble_magnitude, wobble_speed;

    // Start is called before the first frame update
    void Start()
    {
        player_anim = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float wobble_m;
        if (player_anim.GetBool("in_climb"))
        {
            wobble_m = wobble_magnitude;
        }
        else
        {
            wobble_m = 0;
        }

        transform.rotation = Quaternion.Euler(new Vector3(
            transform.rotation.eulerAngles.x,
            transform.rotation.eulerAngles.y,
            wobble_m * Mathf.Sin(Time.time * wobble_speed)
            ));
    }
}
