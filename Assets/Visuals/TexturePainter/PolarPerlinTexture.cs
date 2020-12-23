using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolarPerlinTexture : PerlinTexture
{
    public PolarPerlinTexture(
        int mapResolution, int texResolution, float speed, Colorer colorer = null) :
        base(mapResolution, texResolution, speed, colorer)
    {
        map = new PolarNoise(mapResolution, 1);
        tex = new Texture2D(texResolution, texResolution, TextureFormat.RGBA32, true);

        pix = new Color[texResolution * texResolution];
    }

    override public void FillTexture(float t)
    {
        FillTexture(t, 0, 1);
    }

    override public void FillTexture(float t, float min, float max)
    {
        checkParams(min, max);
        int k = 0;

        float conversion = (float)1 / texResolution;
        for (float i = 0; i < texResolution; i++)
        {
            for (float j = 0; j < texResolution; j++)
            {
                float n = (map as PolarNoise).Noise(
                    i * conversion,
                    j * conversion,
                    t * speed
                    );
                n = min + (max - min) * n;
                if(colorer != null)
                {
                    pix[k] = colorer.Fetch(n);
                }
                else
                {
                    pix[k] = new Color(n, n, n, 1);
                }
                k++;
            }
        }
        tex.SetPixels(pix);
        tex.Apply();
    }
}