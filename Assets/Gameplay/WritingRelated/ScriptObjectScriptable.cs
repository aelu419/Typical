using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Typical Customs/Create Story Script")]
public class ScriptObjectScriptable : ScriptableObject
{
    public string name_;
    public ScriptTextSource source;
    public Writer text_writer;
    //public TextAsset text_asset;
    [TextArea(20, 20)]
    public string text;
    public string previous;
    public string [] next;

    public CustomSong music;
    public Material background;
    public Vector2 slope_min_max = Vector2.zero;

    [System.NonSerialized]
    private string cleansed_text;
    public string CleansedText { get { return cleansed_text; } }

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

    private static ReadingManager rm = new ReadingManager();

    public void CheckSyntax()
    {
        rm.slope_min_max = slope_min_max;
        rm.perlin_map1 = new Perlin(10, 2);
        rm.perlin_map2 = new Perlin(10, 2);

        List<Word> lst;
        if (source == ScriptTextSource.TEXT_ASSET)
        {
            lst = rm.ParseScript(text);
        }
        else
        {
            lst = rm.ParseScript(text_writer.Output());
        }
        System.Text.StringBuilder sb = new System.Text.StringBuilder(lst.Count * 10);
        foreach (Word w in lst)
            sb.Append(w.ToString());
        Debug.Log(sb.ToString());

        ScriptDispenser.CheckValidity(this);
    }

    public void Cleanse()
    {
        if (source != ScriptTextSource.TEXT_ASSET)
        {
            cleansed_text = "";
            return;
        }
        //extract raw text
        cleansed_text = Regex.Replace(text, @"<[^>]*>", " ");
        cleansed_text = Regex.Replace(cleansed_text, @"\s+", " ");

    }
}

public enum ScriptTextSource
{
    TEXT_ASSET,
    SCRIPT
}

