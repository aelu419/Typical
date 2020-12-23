using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdlePainter : Painter
{
    public IdlePainter():base(null, 0, 0)
    {

    }

    public override void FillTexture(float t) { }

    public override void FillTexture(float t, float min, float max) { checkParams(min, max);  }
}
