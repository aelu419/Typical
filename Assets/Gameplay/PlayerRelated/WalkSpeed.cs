using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkSpeed : MonoBehaviour
{
    [Range(0, 6)]
    public float vMin, vMax;
    public AnimationCurve lerp;

    [ReadOnly]
    public float playback;

    Animator player_anim, anim;
    // Start is called before the first frame update
    void Start()
    {
        player_anim = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
        anim = GetComponent<Animator>();

        if (vMax <= vMin || vMin < 0)
        {
            throw new UnityException("velocity threshold incorrect, must be 0 < vMin < vMax");
        }
    }

    // Update is called once per frame
    void Update()
    {
        float v = player_anim.GetFloat("speed");
        v = Mathf.Clamp(v, vMin, vMax) - vMin;
        v /= (vMax - vMin);
        playback = v;
        v = lerp.Evaluate(v);

        

        anim.speed = Mathf.Approximately(v, 0) ? 1 : v;
    }
}
