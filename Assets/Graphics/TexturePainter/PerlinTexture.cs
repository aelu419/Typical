using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PerlinTexture : Painter
{
    protected int mapResolution;
    protected int texResolution;

    protected Perlin map;

    //perlin noise doesn't really care about frequency, using map resolution seems more intuitive
    public PerlinTexture(int mapRes, int texRes, float speed, Colorer colorer) :
        base(texRes, speed, 1, colorer) {
        this.mapResolution = mapRes;
        this.texResolution = texRes;
    }

}