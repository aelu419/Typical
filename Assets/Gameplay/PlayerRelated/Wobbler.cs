using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Wobbler : MonoBehaviour
{
    Animator player_anim, torso_anim;
    //[ReadOnly]
    //public float speed;

    public float wobble_magnitude, wobble_speed;

    float time;

    // Start is called before the first frame update
    void Start()
    {
        player_anim = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
        torso_anim = GetComponent<Animator>();
        time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.isPlaying)
        {
            float speed = player_anim.GetFloat("speed");
            torso_anim.SetFloat("speed", speed);
            torso_anim.SetBool("in_climb", player_anim.GetBool("in_climb"));
        }
        else
        {

        }

        time += Time.deltaTime * wobble_speed;

        transform.rotation = Quaternion.Euler(new Vector3(
            transform.rotation.eulerAngles.x,
            transform.rotation.eulerAngles.y,
            wobble_magnitude * Mathf.Sin(time)
            ));
    }
}
