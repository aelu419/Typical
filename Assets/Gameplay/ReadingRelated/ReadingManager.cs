using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class ReadingManager: MonoBehaviour
{
    public GameObject text_holder_prefab;
    private TextAsset script; //the entire script
    private Word[] words;
    public string script_name;

    private VisualManager vManager;
    private int upcoming;

    private List<GameObject> loaded_words;

    private void Awake()
    {
        upcoming = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        script = Resources.Load(script_name) as TextAsset;
        Debug.Log("Text Loaded as follows:\n" + script.text);

        words = parseScript(script.text);

        //connect to rest of components
        vManager = GetComponent<VisualManager>();

        Vector2 cursor = new Vector2(0, 0);
        //load words, by default load the first 30
        loaded_words = new List<GameObject>();
        (Vector2 cursor, GameObject go) word_loader_temp;
        for (int i = 0; i < Mathf.Min(30, words.Length); i++)
        {
            word_loader_temp = words[i].ToPrefab(text_holder_prefab, cursor);
            cursor = word_loader_temp.cursor; //update cursor
            loaded_words.Add(word_loader_temp.go);
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*
        GameObject last_loaded_word = loaded_words[loaded_words.Count - 1];
        GameObject first_loaded_word = loaded_words[0];

        // when current last loaded word's end enters the right buffer, load the next word from back
        if ((last_loaded_word.transform as RectTransform).rect.xMax
            < (vManager.CAM.xMax + vManager.BUFFER_SIZE))
        {
            int i = int.Parse(last_loaded_word.tag.Substring(1));
            i++;
            if(i < words.Length)
            {
                //load word at i
                (Vector2 cursor, GameObject go) word_loader_temp =
                    words[i].ToPrefab(text_holder_prefab, words[i-1].R, "W" + i.ToString());

                loaded_words.Add(word_loader_temp.go);
                last_loaded_word = word_loader_temp.go;
            }
            else
            {
                Debug.Log("End of script is reached, a portal should be spawn to quit the current story");
            }
        }

        // when current last loaded word exists the right buffer,
        // load the word before the current first loaded word,
        // but don't unload the last loaded word as we expect it to come back to view soon
        if((last_loaded_word.transform as RectTransform).rect.xMin
            > (vManager.CAM.xMax + vManager.BUFFER_SIZE)){
            int i = int.Parse(first_loaded_word.tag.Substring(1));
            i--;
            if(i >= 0)
            {
                //load word at i to the beginning of loaded_words
                //the word itself should already have been loaded, so the LR are alreaady set
                //load word at i
                (Vector2 cursor, GameObject go) word_loader_temp =
                    words[i].ToPrefab(text_holder_prefab, words[i].L, "W" + i.ToString());

                loaded_words.Insert(0, word_loader_temp.go);
            }
            else
            {
                Debug.Log("Beginning of script reached");
            }
        }

        // when current first loaded word exists the left buffer, unload it
        if ((first_loaded_word.transform as RectTransform).rect.xMax
            < (vManager.CAM.xMin - vManager.BUFFER_SIZE)){
            loaded_words.RemoveAt(0);
            Destroy(first_loaded_word);
        }*/
    }

    //parse the script
    //tags with the format <...></...> and <.../> are handled
    //line breaks and spaces are treated the same way
    public static Word[] parseScript(string s)
    {
        List<Word> words = new List<Word>();
        Regex tag = new Regex(@"<\s*(\/?)(\s*\w)+\s*(\/?)\s*>");

        Regex open_tag = new Regex(@"<[^\/]+>");
        Regex close_tag = new Regex(@"<\s*(\/).*>");
        Regex self_close_tag = new Regex(@"<.*(\/)>");

        //remove redundant line swaps
        s = s.Replace('\n', ' ');
        //append extra spaces before and after tags just to make sure
        s = Regex.Replace(s, @"<", " <");
        s = Regex.Replace(s, @">", "> ");
        //remove redundant white spaces within tags
        s = Regex.Replace(s, @"<\s+", "<");
        s = Regex.Replace(s, @"\s+>", ">");
        //remove redundant white spaces
        s = Regex.Replace(s, @"\s+", " ");
        s = s.Trim();

        char[] raw = s.ToCharArray();

        MatchCollection tag_matches = tag.Matches(s);

        //if a tag begins on an index, 
        //the corresponding index will be labeled with the length of that tag
        int[] is_tag_beginning = new int[s.Length];

        for(int i = 0; i < is_tag_beginning.Length; i++)
        {
            is_tag_beginning[i] = 0;
        }
        for(int i = 0; i < tag_matches.Count; i++)
        {
            //Debug.Log(s.Substring(tag_matches[i].Index, tag_matches[i].Length));
            is_tag_beginning[tag_matches[i].Index] = tag_matches[i].Length;
        }

        List<Tag> hanging_tags = new List<Tag>();
        string hanging_word = "";
        //iterate through the script
        for(int cursor = 0; cursor < s.Length; cursor++)
        {
            //cursor is at white space
            if(raw[cursor]==' ')
            {
                //terminate cached word and add it to the list
                words.Add(new Word(hanging_tags.ToArray(), hanging_word));
                hanging_word = "";
            }

            //cursor is at beginning of a tag or a close tag
            else if (is_tag_beginning[cursor] > 0)
            {
                //get content of tag
                string tag_content = s.Substring(cursor, is_tag_beginning[cursor]);

                //determine the type of the tag
                Tag.TagAppearanceType t;
                if (open_tag.IsMatch(tag_content))
                {
                    //Debug.Log("open tag: " + tag_content);
                    t = Tag.TagAppearanceType.open;
                }
                else if (close_tag.IsMatch(tag_content))
                {
                    //Debug.Log("close tag: " + tag_content);
                    t = Tag.TagAppearanceType.close;
                }
                else if (self_close_tag.IsMatch(tag_content))
                {
                    //Debug.Log("self closing tag: " + tag_content);
                    t = Tag.TagAppearanceType.self_closing;
                }
                else
                {
                    //deal with miscellaneous tag: throw an error
                    throw new System.Exception("Tag cannot be recognized: " + tag_content);
                }

                //trim brackets and remove slash
                //Debug.Log("before replacement " + tag_content);
                tag_content = tag_content.Replace("/", "");
                tag_content = tag_content.Replace("<", "");
                tag_content = tag_content.Replace(">", "");
                //Debug.Log("after replacement " + tag_content);
                string[] tag_content_list = tag_content.Split(new char[] { ' ' });
                /*
                for(int i = 0; i < tag_content_list.Length; i++)
                {
                    Debug.Log("\t" + tag_content_list[i]);
                }*/

                Tag this_tag = new Tag(tag_content_list);
                //Debug.Log("\t parsed to " + this_tag);

                switch (t)
                {
                    case Tag.TagAppearanceType.open:
                        //deal with open tag: add to tag list
                        hanging_tags.Add(this_tag);
                        break;
                    case Tag.TagAppearanceType.close:
                        //deal with close tag: find nearest open tag and remove it
                        //if cannot find open tag, throw error
                        bool found = false;
                        for(int i = hanging_tags.Count-1; i >= 0; i--)
                        {
                            //Debug.Log("\t finding if matches:" + hanging_tags[i]);
                            if (hanging_tags[i].type.Equals(this_tag.type))
                            {
                                found = true;
                                hanging_tags.RemoveAt(i);
                                break;
                            }
                        }
                        if (!found) throw new System.Exception(
                            "Paired tag not found for:" + tag_content);
                        break;

                    case Tag.TagAppearanceType.self_closing:
                        //deal with self closing tag: append empty word with this tag
                        Word empty = new Word(new Tag[] { this_tag }, "");
                        words.Add(empty);
                        break;

                    default:
                        //deal with miscellaneous tag: throw an error
                        throw new System.Exception("Tag cannot be recognized: " + tag_content);
                }

                //jump to tag end
                cursor += is_tag_beginning[cursor];
            }

            else
            {
                //not white space or tag, just add the character to the cached word
                hanging_word += raw[cursor];
            }
        }

        /*
        for(int i = 0; i < words.Count; i++)
        {
            Debug.Log(words[i].ToString());
        }*/

        return words.ToArray();
    }
}
