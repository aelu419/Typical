using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ImageBehaviour : CoverObjectBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
        if (sr != null && param != null && param != "")
        {
            Sprite spr = Resources.Load<Sprite>("Misc/" + param);
            if (spr)
            {
                sr.sprite = spr;
            }
            else
            {
                throw new ArgumentException(param + " is not associated with any image");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
