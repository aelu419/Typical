using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Painter
{
    public float speed;
    public float freq;

    public Texture2D tex;
    protected Color[] pix;
    public Colorer colorer;

    public Painter(Texture2D tex, float speed, float freq, Colorer colorer = null)
    {
        this.speed = speed;
        this.freq = freq;

        this.tex = tex;
        pix = new Color[tex.width * tex.height];

        this.colorer = colorer;
    }

    public Painter(int resolution, float speed, float freq, Colorer colorer = null)
    {
        this.speed = speed;
        this.freq = freq;

        this.tex = new Texture2D(resolution, resolution, TextureFormat.RGBA32, true);
        pix = new Color[tex.width * tex.height];

        this.colorer = colorer;
    }

    public abstract void FillTexture(float t);

    public abstract void FillTexture(float t, float min, float max);

    public void checkParams(float min, float max) {
        if (max <= min)
        {
            throw new System.ArgumentException("max cannot be smaller than min: " + max + " <= " + min);
        }
    }
}
