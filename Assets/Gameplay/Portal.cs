using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Portal
{
    public string description, destination;
    public TextMeshPro word_block;
    public Animator portal_animator; //the important parameter is 'open' (bool)

    //obj is instantiated externally
    public void instance(GameObject obj)
    {
        word_block = obj.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>();
        portal_animator = obj.GetComponent<Animator>();
    }
}
