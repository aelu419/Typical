using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
[CreateAssetMenu(menuName = "Typical Customs/Dispensers/Script Dispenser")]
public class ScriptDispenser : ScriptableObject
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
