using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
[FilePath("cover_dispenser.json", FilePathAttribute.Location.ProjectFolder)]
[CreateAssetMenu(menuName = "Typical Customs/Dispensers/Cover Dispenser")]
public class CoverDispenser : ScriptableObject
{
    public CoverObjectScriptable[] cover_objects;

    private static CoverDispenser _instance;
    public static CoverDispenser instance
    {
        get { return _instance; }
    }

    private void OnEnable()
    {
        _instance = this;
    }
}
