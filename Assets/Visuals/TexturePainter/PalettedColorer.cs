using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalettedColorer : Colorer
{
    protected float[] markers; //the ending mark of each color
                     //when random.value falls between two markers
                     //then the color between those two markers
                     //will be chosen
    protected Color[] colors;

    public PalettedColorer(Color[] colors, float[] frequencies)
    {
        this.colors = colors;
        float summary = 0;
        for(int i = 0; i < frequencies.Length; i++)
        {
            if(frequencies[i] <= 0)
            {
                throw new System.Exception("frequency must be greater than 0");
            }
            summary += frequencies[i];
        }
        if(summary == 0)
        {
            throw new System.Exception("Frequency cannot add up to 0 or negative");
        }

        float cursor = 0;
        markers = new float[frequencies.Length];
        for(int i = 0; i < frequencies.Length; i++)
        {
            cursor += frequencies[i] / summary;
            markers[i] = cursor;
        }
    }

    override public Color Fetch(float t)
    {
        if(t <= markers[0])
        {
            return colors[0];
        }
        for(int i = 0; i < markers.Length - 1; i++)
        {
            if(t > markers[i] && t <= markers[i + 1])
            {
                return colors[i+1];
            }
        }

        throw new System.Exception(t + " does not fall within range [0,1]");
    }

    public Color FetchRandom() { return Fetch(Random.value); }
}
