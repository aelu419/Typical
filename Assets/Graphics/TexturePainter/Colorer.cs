using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//generally meant to convert a float to a color
public abstract class Colorer
{
    public abstract Color Fetch(float t);
}
