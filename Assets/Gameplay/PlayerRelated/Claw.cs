using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Claw : MonoBehaviour
{
    public GameObject player;
    PlayerControl player_ctrl;
    Animator player_anim, claw_anim;

    [ReadOnly] public Vector3 base_pos, regular_pos;
    [ReadOnly] public float extension;

    // Start is called before the first frame update
    void Start()
    {
        player_ctrl = player.GetComponent<PlayerControl>();
        player_anim = player.GetComponent<Animator>();
        claw_anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        bool in_climb = player_anim.GetBool("in_climb");
        claw_anim.SetBool("in_climb", in_climb);
        if (!in_climb 
            && !player_anim.GetCurrentAnimatorStateInfo(1).IsName("FinishClimb") 
            && extension == 0)
        {
            base_pos = player.transform.position;
            base_pos.y -= player_ctrl.charSize / 2f;
        }
        transform.position = Lerp(player.transform.position, base_pos, extension + 0.5f);
    }

    private Vector3 Lerp(Vector3 a, Vector3 b, float t)
    {
        t = Mathf.Clamp(t, 0, 1.0f);
        return b * t + a * (1.0f - t);
    }
}
