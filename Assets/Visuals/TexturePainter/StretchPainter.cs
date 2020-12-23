using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StretchPainter : Painter
{
    Perlin map;
    public StretchPainter(Texture2D tex, float speed, float freq) : 
        base(tex, speed, freq)
    {
        map = new Perlin(10, 2);
    }

    public StretchPainter(int res, float speed, float freq) : 
        this(
            new Texture2D(res, res, TextureFormat.RGBA32, true),
            speed, freq
            ) { }

    public override void FillTexture(float t, float min, float max)
    {
        checkParams(min, max);

        float t_ = t * speed;

        int cursor = 0;

        //for some reason u and v should be flipped
        for (float i = 0; i < tex.width; i++)
        {
            //standard u from uv
            float v = i / tex.width;
            float noi = map.Noise(new VecN(new float[] { v, t_ }));
            //convert to deviation from horizontal center
            v = Mathf.Abs(v - 0.5f);
            for (float j = 0; j < tex.width; j++)
            {
                //standard v from uv
                float u = j / tex.height;
                //convert to deviation from vertical center
                u = Mathf.Abs(u - 0.5f);

                //for wavy texture on the floating bars
                float wavy =
                    (1 - u) //waves are higher near the horizontal center
                    * (1 - v) //waves are higher near the vertical center
                    / 0.25f //normalization of the previous two terms
                    * Mathf.Sin(u * Mathf.PI * 2); //wave itself

                //"stickiness" of the stretching action between the waves
                wavy *= wavy; //to have more stickiness, replace with pow(wavy, n) for increasing n

                //generating horizontal bars
                float b = Mathf.Sin(
                    v * freq //frequency of horizontal bars
                    + wavy //apply wavy pattern
                    - t_ - noi //time and noise influences
                    );

                b = (max - min) * b + min;
                if(colorer != null)
                {
                    pix[cursor] = colorer.Fetch(b);
                }
                else
                {
                    pix[cursor] = new Color(b, b, b, 1);
                }
                cursor++;
            }
        }

        tex.SetPixels(pix);
        tex.Apply();
    }

    public override void FillTexture(float t)
    {
        FillTexture(t, 0, 1);
    }
}
