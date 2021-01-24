using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Writer : ScriptableObject
{
    public ScriptObjectScriptable input;
    
    public string Input()
    {
        return input.Text;
    }

    public abstract string Output();
}
