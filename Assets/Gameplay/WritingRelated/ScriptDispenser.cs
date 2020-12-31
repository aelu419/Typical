using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "Typical Customs/Create Script Dispenser")]
public class ScriptDispenser : ScriptableSingleton<ScriptDispenser>
{
    public int index;
    public ScriptObjectScriptable[] scripts;

    public ScriptObjectScriptable CurrentScript {
        get
        {
            return scripts[index];
        }
    }
}
