using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "Typical Customs/Create Cover Dispenser")]
public class CoverDispenser : ScriptableSingleton<CoverDispenser>
{
    [SerializeField]
    public CoverObjectScriptable[] cover_objects;
}
