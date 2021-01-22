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
    [ReadOnly] public string cover_type;
    [ReadOnly] public Sprite cover_sprite;
    private static Sprite default_cover_sprite;

    [ReadOnly] public int typed; //number of typed letters in the word
    [ReadOnly] public WORD_TYPES word_mech; //the mechanism that the word block follows

    [ReadOnly] public TextMeshPro tmp;
    private TMP_CharacterInfo[] char_infos;

    //markers for texual contents of the word block
    [ReadOnly] public bool has_typable; //if the word has any typable letter in it
    [ReadOnly] public int first_typable, last_typable;

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

        default_cover_sprite = Resources.Load("DefaultCoverSprite") as Sprite;
    }

    public Word(Tag[] tags, string content, float slope, int index, int typed)
    {
        this.tags = tags;
        this.content = content;
        this.slope = slope;
        this.index = index;
        this.typed = typed;

        has_typable = false;
        first_typable = last_typable = -1;
        for(int i = 0; i < content.Length; i++)
        {
            if (char.IsLetter(content[i]))
            {
                //when first encountering typed letter:
                if (!has_typable)
                {
                    has_typable = true;
                    first_typable = i;
                    last_typable = i;
                }
                //for subsequent typed letters
                else
                {
                    last_typable = i;
                }
            }
        }

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
        tmp.ForceMeshUpdate();
        Vector2 rendered_vals = tmp.GetRenderedValues(false);

        go.GetComponent<WordBlockBehavior>().content = this;
        go.tag = "Word Block";

        BoxCollider2D col = go.GetComponent<BoxCollider2D>();
        if (col == null) throw new System.Exception("prefab loading error: no collider");

        //set slope
        float slope_delta = slope * rendered_vals.x;

        //set collider boundaries
        col.offset = new Vector2(rendered_vals.x / 2f, 0);
        col.size = rendered_vals;

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
        R = new Vector2(lCursor.x + rendered_vals.x, lCursor.y + slope_delta);
        L = new Vector2(lCursor.x, lCursor.y);
        top = lCursor.y + rendered_vals.y / 2f;

        //handle objects that cover the word
        cover_type = "";
        float cover_w = 0;
        foreach(Tag t in tags)
        {
            if (t.type.Equals("O"))
            {
                FetchCover(t, go);
                cover_w = cover_sprite.bounds.size.x;
            }
        }
        R.x += cover_w;

        return (new Vector2(R.x, R.y), go);
    }

    //fetch the cover object prefab according to the object tag
    // - see CoverDispenser and CoverObjectScriptable and their respective objects
    private void FetchCover(Tag t, GameObject parent_obj)
    {
        try
        {
            cover_type = t.GetSpecAt(0);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
            cover_type = "default";
        }

        //fetch sprite for cover object
        cover_sprite = null;

        GameObject cover_child = null;
        foreach (CoverObjectScriptable c in ScriptableObjectManager.Instance.CoverManager.cover_objects)
        {
            if (c.name_.Equals(cover_type))
            {
                cover_child = GameObject.Instantiate(
                    c.prefab, parent_obj.transform
                    );
                break;
            }
        }
        if (cover_child == null)
        {
            Debug.LogError("cover prefab not found for type: " + cover_type);
        }
        else
        {
            cover_sprite = cover_child.GetComponent<SpriteRenderer>().sprite;
            cover_child.tag = "Cover Object";

            //initialize collider
            cover_child.AddComponent<BoxCollider2D>();
            cover_child.GetComponent<BoxCollider2D>().isTrigger = true;

            try
            {
                string par = t.GetSpecAt(1);
                CoverObjectBehaviour cob =
                    cover_child.GetComponent<CoverObjectBehaviour>();
                Debug.Log(cob);
                cob.param = par;
                
            }
            //skip the indexoutofrangeexception
            //because it is likely from having no parameters
            catch(System.IndexOutOfRangeException e)
            {
                //Debug.Log("Object does not have extra parameters");
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
            }

            try
            {
                Debug.Log(cover_child);
                //TODO: set local position
                cover_child.transform.localPosition = new Vector3(
                    cover_sprite.bounds.size.x / 2f,
                    (cover_sprite.bounds.size.y + tmp.GetPreferredValues().y) / 2f,
                    0);
            }
            catch (System.Exception e)
            {
                Debug.Log("error encountered when processing: " + t.ToString());
                throw;
            }
        }
    }

    public void SetRawText()
    {
        if (tmp == null)
        {
            throw new System.Exception("Word is not attached to TMP yet, do not call set text!");
        }
        else
        {
            tmp.text = " " + content;
            tmp.ForceMeshUpdate();
        }
    }

    public TMP_CharacterInfo[] GetCharacterInfos()
    {
        if (tmp == null)
        {
            throw new System.Exception("Word is not attached to TMP yet, do not call get character info!");
        }
        else
        {
            //reset tmp text to remove tag influence
            SetRawText();
            char_infos = tmp.textInfo.characterInfo;

            //add in tags again
            SetCharacterMech();

            return char_infos;
        }
    }

    public TMP_CharacterInfo GetCharacterInfo(int index)
    {
        //fetch character infos if not already available
        if (char_infos == null) GetCharacterInfos();

        else if (index < 0 || index > content.Length)
        {
            throw new System.Exception("index: " + index + " is out of bounds");
        }

        return char_infos[
            index == 0 ? 0 : index + 1];
    }

    public void SetCharacterMech(int index)
    {
        typed = index;
        SetCharacterMech();
    }

    public void SetCharacterMech()
    {
        //the following skips are to make sure the word does not disappear
        //from the left of the screen before unloading. the exact reason is unknown
        //but i suspect it's caused by the submeshes TMP generates when material
        //tags are used within the TMP box

        //skip if none of of the text is typed out
        if (typed <= 0)
        {
            typed = 0;

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
        if (typed >= content.Length)
        {
            typed = content.Length;
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
