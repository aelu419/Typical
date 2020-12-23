using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothPalettedColorer : PalettedColorer
{

    public SmoothPalettedColorer(Color[] colors, float[] frequencies):
        base(colors, frequencies)
    { }

    override public Color Fetch(float t)
    {
        if (t <= markers[0])
        {
            float cursor_ = Fade(t);
            return new Color(
                    colors[0].r * cursor_,
                    colors[0].g * cursor_,
                    colors[0].b * cursor_,
                    colors[0].a * cursor_
                );
        }
        for (int i = 0; i < markers.Length - 1; i++)
        {
            if (t > markers[i] && t <= markers[i + 1])
            {
                float cursor_ = Fade((t - markers[i]) / (markers[i + 1] - markers[i]));
                return new Color(
                        Mathf.Lerp(colors[i].r, colors[i+1].r, cursor_),
                        Mathf.Lerp(colors[i].g, colors[i + 1].g, cursor_),
                        Mathf.Lerp(colors[i].b, colors[i + 1].b, cursor_),
                        Mathf.Lerp(colors[i].a, colors[i + 1].a, cursor_)
                    );
            }
        }

        throw new System.Exception(t + " does not fall within range [0,1]");
    }
    new public Color FetchRandom() { return Fetch(Random.value); }

    private float Fade (float t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }
}
