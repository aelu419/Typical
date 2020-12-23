using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//describes a word (or smallest unit separated by spaces) in the script
public class Word
{
    public Tag[] tags;
    public string content;

    [ReadOnly] public Vector2 L, R; //left and right positions of the cursor associated with this word
    [ReadOnly] public float top;
    [ReadOnly] public float slope;
    [ReadOnly] public int index;
    [ReadOnly] public Sprite cover;

    public enum WORD_TYPES
    {
        plain,
        hidden,
        reflecting
    }

    public Word(Tag[] tags, string content, float slope, int index)
    {
        this.tags = tags;
        this.content = content;
        this.slope = slope;
        this.index = index;
    }

    //instantiate a prefab that holds the current words
    //the parameters are horizontal and vertical beginning of text
    //the actual beginnings are determined after considering slope
    public (Vector2, GameObject) ToPrefab(GameObject pre, Vector2 lCursor)
    {
        GameObject go = MonoBehaviour.Instantiate(pre, lCursor, Quaternion.identity);
        TextMeshPro tmp = go.GetComponent<TextMeshPro>();
        if (tmp == null) throw new System.Exception("prefab loading error: no TMP component");
        tmp.text = " "+content;

        go.GetComponent<TextHolderBehavior>().content = this;
        go.tag = "Word Block";

        BoxCollider2D col = go.GetComponent<BoxCollider2D>();
        if (col == null) throw new System.Exception("prefab loading error: no collider");

        //set left and right boundaries of the word
        float slope_delta = slope * tmp.GetPreferredValues().x;

        //set collider boundaries
        col.offset = new Vector2(tmp.GetPreferredValues().x / 2f, 0);
        col.size = tmp.GetPreferredValues();

        //TODO: handle stylistic/mechanism tags
        for (int i = 0; i < tags.Length; i++)
        {
        }

        R = new Vector2(lCursor.x + tmp.GetPreferredValues().x, lCursor.y + slope_delta);
        L = new Vector2(lCursor.x, lCursor.y);
        top = lCursor.y + tmp.GetPreferredValues().y / 2f;

        //handle objects that cover the word
        if (content.Length == 0 && (tags.Length == 1 && tags[0].type.Equals("O")))
        {
            if(tags[0].specs.Length == 0)
            {
                //use default object
                cover = Resources.Load("default") as Sprite;
            }
            else
            {
                string cover_object_name = tags[0].specs[0];
                cover = Resources.Load(cover_object_name) as Sprite;
                if(cover == null)
                {
                    throw new System.Exception(cover_object_name + " cannot be found");
                }
            }

        }

        return (new Vector2(R.x, R.y), go);
    }

    public override string ToString()
    {
        string tgs = "";
        for(int i = 0; i < tags.Length; i++)
        {
            tgs += " " + tags[i].ToString();
        }
        return content + ": " + tgs;
    }
}
