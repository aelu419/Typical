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
    public TextAsset text_asset;
    public ScriptObjectScriptable previous;

    public string Text
    {
        get
        {
            if(source == ScriptTextSource.TEXT_ASSET)
            {
                return text_asset.text;
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

