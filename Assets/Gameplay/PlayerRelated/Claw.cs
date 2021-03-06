using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Claw : MonoBehaviour
{
    PlayerControl player;
    Animator player_anim, claw_anim;

    [ReadOnly] public Vector3 base_pos, regular_pos;
    [ReadOnly] public float extension;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        player_anim = player.GetComponent<Animator>();
        claw_anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!player.in_climb)
        {
            base_pos = player.transform.position;
            base_pos.y -= player.charSize / 2f;
            claw_anim.SetBool("in_climb", false);
        }
        else
        {
            claw_anim.SetBool("in_climb", !player_anim.GetBool("climb_done"));
        }
        transform.position = Lerp(player.transform.position, base_pos, extension + 0.5f);
    }

    private Vector3 Lerp(Vector3 a, Vector3 b, float t)
    {
        t = Mathf.Clamp(t, 0, 1.0f);
        return b * t + a * (1.0f - t);
    }
}
