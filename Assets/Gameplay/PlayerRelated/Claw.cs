using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        base_pos = player.transform.position;
        base_pos.y -= player.charSize / 2f;

        regular_pos = player.transform.position;
        transform.position = Lerp(regular_pos, base_pos, extension);

        claw_anim.SetBool("in_climb", player_anim.GetBool("in_climb"));
    }

    private Vector3 Lerp(Vector3 a, Vector3 b, float t)
    {
        t = Mathf.Clamp(t, 0, 1.0f);
        return b * t + a * (1.0f - t);
    }
}
