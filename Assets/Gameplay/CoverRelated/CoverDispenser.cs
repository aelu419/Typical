using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Typical Customs/Create Cover Dispenser")]
public class CoverDispenser : ScriptableObject
{
    public static CoverDispenser instance;
    public CoverObjectScriptable[] cover_objects;

    private void OnEnable()
    {
        instance = this;
    }
}
