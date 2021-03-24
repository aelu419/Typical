using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Typical Customs/Create Story Script")]
public class ScriptObjectScriptable : ScriptableObject
{
    public string name_;
    public ScriptTextSource source;
    public Writer text_writer;
    //public TextAsset text_asset;
    [TextArea]
    public string text;
    public string previous;
    public string [] next;

    public CustomSong music;
    public Vector2 slope_min_max = Vector2.zero;

    public string Text
    {
        get
        {
            if(source == ScriptTextSource.TEXT_ASSET)
            {
                return text;
            }
            else
            {
                return text_writer.Output();
            }
        }
    }
}

public enum ScriptTextSource
{
    TEXT_ASSET,
    SCRIPT
}

