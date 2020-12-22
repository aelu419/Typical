using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//describes a word (or smallest unit separated by spaces) in the script
public class Word
{
    public Tag[] tags;
    public string content;

    public Word(Tag[] tags, string content)
    {
        this.tags = tags;
        this.content = content;
    }

    public override string ToString()
    {
        string tgs = "";
        for(int i = 0; i < tags.Length; i++)
        {
            tgs += " " + tags[i].ToString();
        }
        return content + ":" + tgs;
    }
}
