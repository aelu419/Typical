using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helm : MonoBehaviour
{
    Animator player_anim, anim;

    // Start is called before the first frame update
    void Start()
    {
        player_anim = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("light_toggle", player_anim.GetBool("light_toggle"));
    }
}
