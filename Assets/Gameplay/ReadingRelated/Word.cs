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

    [ReadOnly] public int typed; //number of typed letters in the word
    [ReadOnly] public WORD_TYPES word_mech; //the mechanism that the word block follows

    [ReadOnly] TextMeshPro tmp;

    public static string
        TYPED_MAT = "Averia-Regular SDF Typed",
        UNTYPED_PLAIN_MAT = "Averia-Regular Untyped Plain",
        UNTYPED_HIDDEN_MAT = "Averia-Regular Untyped Hidden",
        UNTYPED_REFLECTOR_MAT = "Averia-Regular Untyped Reflector";

    public static Material
        TYPED_MAT_,
        UNTYPED_PLAIN_MAT_,
        UNTYPED_HIDDEN_MAT_,
        UNTYPED_REFLECTOR_MAT_;

    public enum WORD_TYPES
    {
        plain,
        hidden,
        reflector
    }

    //load the word materials
    static Word(){
        TYPED_MAT_ = Resources.Load(
            "Fonts & Materials/" + TYPED_MAT) as Material;
        UNTYPED_PLAIN_MAT_ = Resources.Load(
            "Fonts & Materials/" + UNTYPED_PLAIN_MAT) as Material;
        UNTYPED_HIDDEN_MAT_ = Resources.Load(
            "Fonts & Materials/" + UNTYPED_HIDDEN_MAT) as Material;
        UNTYPED_REFLECTOR_MAT_ = Resources.Load(
            "Fonts & Materials/" + UNTYPED_REFLECTOR_MAT) as Material;
    }

    public Word(Tag[] tags, string content, float slope, int index, int typed)
    {
        this.tags = tags;
        this.content = content;
        this.slope = slope;
        this.index = index;
        this.typed = typed;

        //figure out what the mechanism of the text is, based on the last hanging tag
        //that is mechanism-related

        for(int i = tags.Length - 1; i >= 0; i--)
        {
            if (tags[i].Equals("H"))
            {
                this.word_mech = WORD_TYPES.hidden;
                break;
            }
            else if (tags[i].Equals("R"))
            {
                this.word_mech = WORD_TYPES.reflector;
            }
            //tags depleted, use normal style
            else if (i == 0)
            {
                this.word_mech = WORD_TYPES.plain;
            }
        }
    }

    public Word(Tag[] tags, string content, float slope, int index)
        : this(tags, content, slope, index, 0) { }

    //instantiate a prefab that holds the current words
    //the parameters are horizontal and vertical beginning of text
    //the actual beginnings are determined after considering slope
    public (Vector2, GameObject) ToPrefab(GameObject pre, Vector2 lCursor)
    {
        GameObject go = MonoBehaviour.Instantiate(pre, lCursor, Quaternion.identity);
        tmp = go.GetComponent<TextMeshPro>();
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

        //TODO: handle mechanism tags
        SetCharacterMech();

        //TODO: handle stylistic tags
        string style = "";
        for (int i = tags.Length - 1; i >= 0; i--)
        {
            if (tags[i].type.Equals("I"))
            {
                style += "I";
            }
            if (tags[i].type.Equals("B"))
            {
                style += "B";
            }
        }
        tmp.fontStyle =
            (style.IndexOf("I") != -1 ? FontStyles.Italic : FontStyles.Normal)
            |
            (style.IndexOf("B") != -1 ? FontStyles.Bold : FontStyles.Normal);

        //store dimensions of the text block
        R = new Vector2(lCursor.x + tmp.GetPreferredValues().x, lCursor.y + slope_delta);
        L = new Vector2(lCursor.x, lCursor.y);
        top = lCursor.y + tmp.GetPreferredValues().y / 2f;

        //handle objects that cover the word
        if (content.Length == 0 && (tags.Length == 1 && tags[0].type.Equals("O")))
        {
            if(tags[0].specs.Length == 0)
            {
                //use default object
                cover = Resources.Load<Sprite>("default");
                Debug.Log("loading " + cover.name + " as sprite");
            }
            else
            {
                string cover_object_name = tags[0].specs[0];
                cover = Resources.Load(cover_object_name) as Sprite;
                Debug.Log("loading " + cover.name + " as sprite");
                if (cover == null)
                {
                    throw new System.Exception(cover_object_name + " cannot be found");
                }
            }

        }

        return (new Vector2(R.x, R.y), go);
    }

    public void SetCharacterMech()
    {
        //the following skips are to make sure the word does not disappear
        //from the left of the screen before unloading. the exact reason is unknown
        //but i suspect it's caused by the submeshes TMP generates when material
        //tags are used within the TMP box

        //skip if none of of the text is typed out
        if (typed == 0)
        {
            tmp.text = " " + content;
            switch (word_mech)
            {
                case WORD_TYPES.plain:
                    tmp.fontSharedMaterial = UNTYPED_PLAIN_MAT_;
                    break;
                case WORD_TYPES.hidden:
                    tmp.fontSharedMaterial = UNTYPED_HIDDEN_MAT_;
                    break;
                case WORD_TYPES.reflector:
                    tmp.fontSharedMaterial = UNTYPED_REFLECTOR_MAT_;
                    break;
                default:
                    throw new System.Exception("word type not found");
            }
            return;
        }

        //skip if the entire text is typed out
        if (typed == content.Length)
        {
            tmp.text = " " + content;
            tmp.fontSharedMaterial = TYPED_MAT_;
            return;
        }

        //when the word is half typed out
        string txt_temp = "<material=\"" + TYPED_MAT + "\"> " //the space is for left spacing between words
            + content.Substring(0, typed) + "</material>";
        
        switch (word_mech)
        {
            case WORD_TYPES.plain:
                txt_temp += "<material=\"" + UNTYPED_PLAIN_MAT + "\">"
                    + content.Substring(typed) + "</material>";
                break;
            case WORD_TYPES.hidden:
                txt_temp += "<material=\"" + UNTYPED_HIDDEN_MAT + "\">"
                    + content.Substring(typed) + "</material>";
                break;
            case WORD_TYPES.reflector:
                txt_temp += "<material=\"" + UNTYPED_REFLECTOR_MAT + "\">"
                    + content.Substring(typed) + "</material>";
                break;
            default:
                throw new System.Exception("word type not found");
        }

        tmp.text = txt_temp;

        /*
        //set style of typed characters
        for (int i = 1; i < typed + 1; i++)
        {
            char_info[i].material = TYPED_MAT;
        }
        //set style of untyped characters
        for (int i = typed; i < content.Length + 1; i++)
        {
            switch (word_mech)
            {
                case WORD_TYPES.plain:
                    char_info[i].material = UNTYPED_PLAIN_MAT;
                    break;
                case WORD_TYPES.hidden:
                    char_info[i].material = UNTYPED_HIDDEN_MAT;
                    break;
                case WORD_TYPES.reflector:
                    char_info[i].material = UNTYPED_REFLECTOR_MAT;
                    break;
                default:
                    throw new System.Exception("word type not found");
            }
        }*/
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
