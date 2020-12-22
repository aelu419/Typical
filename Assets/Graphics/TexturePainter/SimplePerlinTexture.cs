using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SimplePerlinTexture : PerlinTexture
{
    public SimplePerlinTexture(
        int mapResolution, int texResolution, float speed, Colorer colorer = null) : 
        base(mapResolution, texResolution, speed, colorer)
    {
        map = new Perlin(mapResolution, 3);
        tex = new Texture2D(texResolution, texResolution, TextureFormat.RGBA32, true);

        pix = new Color[texResolution * texResolution];
    }

    override public void FillTexture(float t)
    {
        int k = 0;

        float conversion = (float)mapResolution / texResolution;
        for (float i = 0; i < texResolution; i++)
        {
            for (float j = 0; j < texResolution; j++)
            {
                float n = map.Noise(new VecN(new float[]
                {
                    i * conversion,
                    j * conversion,
                    t * speed
                }));
                pix[k] = new Color(n, n, n, 1);
                k++;
            }
        }
        tex.SetPixels(pix);
        tex.Apply();
    }

    override public void FillTexture(float t, float min, float max)
    {
        checkParams(min, max);

        int k = 0;

        float conversion = (float)mapResolution / texResolution;
        for (float i = 0; i < texResolution; i++)
        {
            for (float j = 0; j < texResolution; j++)
            {
                float n = map.Noise(new VecN(new float[]
                {
                    i * conversion,
                    j * conversion,
                    t * speed
                }));
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
