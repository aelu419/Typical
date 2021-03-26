using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Typical Customs / NPC")]
public class NPCScriptable : ScriptableObject
{
    [System.Serializable]
    public class NPCSegment
    {
        public string name;
        [TextArea]
        public string script;
    }

    public Sprite[] sprites;
    public NPCSegment[] segments;
}
