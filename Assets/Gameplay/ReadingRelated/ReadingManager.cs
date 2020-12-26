using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

using TMPro;

public class ReadingManager: MonoBehaviour
{
    public GameObject text_holder_prefab;
    private TextAsset script; //the entire script
    private Word[] words;
    public string script_name;

    private VisualManager vManager;
    private PlayerControl player;

    //slope related
    public Vector2 slope_min_max;
    public Perlin perlin_map1, perlin_map2;

    private List<GameObject> loaded_words;

    //cursor related
    public int[] cursor_raw; //first coordinate is index of the word, 
                             //second coordinate is index of letter
    TMP_CharacterInfo cursor_rendered; //the pixel position of the cursor
                                       //set to the boundaries of the next letter
    private char next_letter;
    private bool firstFrame;
    private bool skipped_over_punctuation_last_time;

    private void Awake()
    {
        perlin_map1 = new Perlin(10, 2);
        perlin_map2 = new Perlin(10, 2);
        firstFrame = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        script = Resources.Load(script_name) as TextAsset;
        Debug.Log("Text Loaded as follows:\n" + script.text);

        words = ParseScript(script.text);

        //connect to rest of components
        vManager = GetComponent<VisualManager>();
        player = GameObject.FindGameObjectWithTag("Player")
            .GetComponent<PlayerControl>();

        Vector2 cursor = new Vector2(0, 0);
        //load words, by default load the first 10
        loaded_words = new List<GameObject>();
        (Vector2 cursor, GameObject go) word_loader_temp;
        for (int i = 0; i < Mathf.Min(10, words.Length); i++)
        {
            word_loader_temp = words[i].ToPrefab(text_holder_prefab, cursor);
            cursor = word_loader_temp.cursor; //update cursor
            loaded_words.Add(word_loader_temp.go);
        }

        //search for the first non-empty and non-### word in the script
        if (loaded_words.Count < 2)
            throw new System.Exception("No other word in script");

        int j = 1;
        while(words[j].content.Length <= 0)
        {
            j++;
            if(j == words.Length)
            {
                throw new System.Exception("No other word in script");
            }
        }

        //update cursor according to the search results from above
        cursor_raw = new int[] { j, 0 }; //start at second non-empty word (after the "###")
        UpdateRenderedCursor();
        next_letter = words[j].content.ToCharArray()[0];

        //Debug.Log("starting at " + cursor_raw[0] + "'s " + cursor_raw[1] + "'th letter");
        //Debug.Log("the next letter is: " + next_letter);
    }

    // Update is called once per frame
    void Update()
    {
        //update destination of player on the first frame
        //this is necessary when readingmanager's start runs after
        //player control's start method
        if (firstFrame)
        {
            UpdateRenderedCursor();
            firstFrame = false;
        }

        //deal with word loading within camera scope + buffer region
        GameObject last_loaded_word = loaded_words[loaded_words.Count - 1];
        GameObject first_loaded_word = loaded_words[0];

        // when current last loaded word's end enters the right buffer, load the next word from back
        if (last_loaded_word.GetComponent<TextHolderBehavior>().content.R.x
            < (vManager.CAM.xMax + vManager.BUFFER_SIZE))
        {
            //Debug.Log("load from right");
            int i = last_loaded_word.GetComponent<TextHolderBehavior>().content.index;
            i++;
            if(i < words.Length)
            {
                //load word at i
                (Vector2 cursor, GameObject go) word_loader_temp =
                    words[i].ToPrefab(text_holder_prefab, words[i-1].R);

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
        if(last_loaded_word.GetComponent<TextHolderBehavior>().content.L.x
            > (vManager.CAM.xMax + vManager.BUFFER_SIZE))
        {
            //Debug.Log("load from left");
            int i = first_loaded_word.GetComponent<TextHolderBehavior>().content.index;
            i--;
            if(i >= 0)
            {
                //load word at i to the beginning of loaded_words
                //the word itself should already have been loaded, so the LR are alreaady set
                //load word at i
                (Vector2 cursor, GameObject go) word_loader_temp =
                    words[i].ToPrefab(text_holder_prefab, words[i].L);

                loaded_words.Insert(0, word_loader_temp.go);
            }
            else
            {
               // Debug.Log("Beginning of script reached");
            }
        }

        // when current first loaded word exists the left buffer, unload it
        if (first_loaded_word.GetComponent<TextHolderBehavior>().content.R.x
            < (vManager.CAM.xMin - vManager.BUFFER_SIZE)){
            Debug.Log("unloading " + first_loaded_word.GetComponent<TextHolderBehavior>().content.content);
            loaded_words.Remove(first_loaded_word);
            Destroy(first_loaded_word);
        }

        // handle input
        if (Input.GetKeyDown(next_letter.ToString().ToLower())) //correct key is pressed
        {
            skipped_over_punctuation_last_time = false;
            EventManager.instance.RaiseCorrectKeyPressed();

            // skip unmatching sequence caused by backspacing (see skip_over_puncuation)
            for(int i = 0; i < loaded_words.Count; i++)
            {
                Word this_loaded_word = loaded_words[i].GetComponent<TextHolderBehavior>().content;
                if(this_loaded_word.index > cursor_raw[0])
                {
                    break;
                }
                if(this_loaded_word.typed < this_loaded_word.content.Length - 1)
                {
                    if(this_loaded_word.index == cursor_raw[0])
                    {
                        this_loaded_word.typed = cursor_raw[1];
                    }
                    else
                    {
                        this_loaded_word.typed = this_loaded_word.content.Length;
                    }
                    this_loaded_word.SetCharacterMech();
                }
            }

            do 
            {
                cursor_raw[1]++; //go to next letter
                //update typed portions of the text
                words[cursor_raw[0]].typed++;
                words[cursor_raw[0]].SetCharacterMech();

                //reached the end of the word
                if (words[cursor_raw[0]].content.Length == cursor_raw[1])
                {
                    //currently on the last word of the script
                    if (cursor_raw[0] == words.Length - 1)
                    {
                        EventManager.instance.RaiseScriptEndReached();
                        next_letter = '\0';
                    }
                    //not on the last word of the script
                    else
                    {
                        cursor_raw[1] = 0;
                        int i = cursor_raw[0] + 1;
                        //skip empty words
                        while (words[i].content.Length <= 0)
                        {
                            i++;
                            if (i == words.Length - 1)
                            {
                                //currently on the last word of the script
                                if (cursor_raw[0] == words.Length - 1)
                                {
                                    EventManager.instance.RaiseScriptEndReached();
                                    next_letter = '\0';
                                }
                                break;
                            }
                        }
                        cursor_raw[0] = i;

                        next_letter = words[cursor_raw[0]].content[cursor_raw[1]];
                    }
                }
                else
                {
                    next_letter = words[cursor_raw[0]].content[cursor_raw[1]];
                }
            } while (cursor_raw[0] < words.Length
                && !char.IsLetter(next_letter));

            if (next_letter != '\0')
            {
                //Debug.Log("next letter is " + next_letter);
                UpdateRenderedCursor();
            }
        }
        
        //going backwards
        else if (Input.GetKeyDown(KeyCode.Backspace))
        {
            char next_letter_temp = next_letter;
            bool skipped_through_punctuation = false;

            int[] cursor_raw_temp = new int[] { cursor_raw[0], cursor_raw[1] };

            do
            {
                cursor_raw[1]--;

                //exit from the left
                if (cursor_raw[1] < 0)
                {
                    //Debug.Log("cleared word " + cursor_raw[0]);
                    //clear current word
                    words[cursor_raw[0]].typed = 0;
                    words[cursor_raw[0]].SetCharacterMech();

                    //move onto last word
                    cursor_raw[0]--;
                    //exit when beginning of script is reached
                    if (cursor_raw[0] < 2)
                    {
                        cursor_raw = new int[] { 2, 0 };
                        break;
                    }
                    else
                    {
                        //last word is empty
                        if (words[cursor_raw[0]].content.Length == 0)
                        {
                            next_letter = '\0';
                        }
                        else
                        {
                            cursor_raw[1] = words[cursor_raw[0]].content.Length - 1;

                            words[cursor_raw[0]].typed = Mathf.Min(
                                cursor_raw[1],
                                words[cursor_raw[0]].typed);

                            words[cursor_raw[0]].SetCharacterMech();

                            next_letter = words[cursor_raw[0]].content[cursor_raw[1]];

                            skipped_through_punctuation = 
                                skipped_through_punctuation || !char.IsLetter(next_letter);
                        }
                    }
                }
                //remain on same word
                else
                {
                    words[cursor_raw[0]].typed = Mathf.Min(
                        cursor_raw[1],
                        words[cursor_raw[0]].typed);
                    words[cursor_raw[0]].SetCharacterMech();
                    next_letter = words[cursor_raw[0]].content[cursor_raw[1]];

                    skipped_through_punctuation =
                        skipped_through_punctuation || !char.IsLetter(next_letter);

                }

            } while (!char.IsLetter(next_letter));

            //when the sequence skips over non-letter characters
            //the next letter should be kept the same and the cursor should be moved right by 1
            if (skipped_through_punctuation && !skipped_over_punctuation_last_time)
            {
                //go right by 1
                cursor_raw[1]++;
                words[cursor_raw[0]].typed++;
                words[cursor_raw[0]].SetCharacterMech();

                next_letter = next_letter_temp;

                //Debug.Log(cursor_raw[0] + ", " + cursor_raw[1]);
                //update rendered cursor using the "overshot+1" position
                UpdateRenderedCursor(cursor_raw);

                cursor_raw = cursor_raw_temp;

                skipped_over_punctuation_last_time = true;
            }
            else
            {
                //update cursor as normal
                UpdateRenderedCursor();
            }

            //Debug.Log("backspace sequence ended with " + cursor_raw[0] + ", " + cursor_raw[1]);
        }
        
        //handle lighting
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            // see player controller
        }
        
        //any other key is pressed
        else if (Input.anyKeyDown)
        {
            if (AnyLetterPressed())
                EventManager.instance.RaiseIncorrectKeyPressed();
        }

    }

    //determine if a letter is pressed
    private bool AnyLetterPressed() {
        return
            Input.GetKeyDown(KeyCode.A)
            | Input.GetKeyDown(KeyCode.B)
            | Input.GetKeyDown(KeyCode.C)
            | Input.GetKeyDown(KeyCode.D)
            | Input.GetKeyDown(KeyCode.E)
            | Input.GetKeyDown(KeyCode.F)
            | Input.GetKeyDown(KeyCode.G)
            | Input.GetKeyDown(KeyCode.H)
            | Input.GetKeyDown(KeyCode.I)
            | Input.GetKeyDown(KeyCode.J)
            | Input.GetKeyDown(KeyCode.K)
            | Input.GetKeyDown(KeyCode.L)
            | Input.GetKeyDown(KeyCode.M)
            | Input.GetKeyDown(KeyCode.N)
            | Input.GetKeyDown(KeyCode.O)
            | Input.GetKeyDown(KeyCode.P)
            | Input.GetKeyDown(KeyCode.Q)
            | Input.GetKeyDown(KeyCode.R)
            | Input.GetKeyDown(KeyCode.S)
            | Input.GetKeyDown(KeyCode.T)
            | Input.GetKeyDown(KeyCode.U)
            | Input.GetKeyDown(KeyCode.V)
            | Input.GetKeyDown(KeyCode.W)
            | Input.GetKeyDown(KeyCode.X)
            | Input.GetKeyDown(KeyCode.Y)
            | Input.GetKeyDown(KeyCode.Z);
    }

    //update the rendered cursor position on screen according to a given raw cursor unit by char count
    private void UpdateRenderedCursor(int[] cursor_raw)
    {

        //Debug.Log("setting rendered cursor according to " + cursor_raw[0] + ", " + cursor_raw[1]);

        for (int i = 0; i < loaded_words.Count; i++)
        {
            Word loaded_temp = loaded_words[i].GetComponent<TextHolderBehavior>().content;
            if (loaded_temp.index == cursor_raw[0])
            {
                //Debug.Log("\trendered cursor currently on word: " + loaded_temp.content
                //+"'s " + (cursor_raw[1] + (cursor_raw[1] == 0 ? 0 : 1)) + "'th letter " + 
                //loaded_words[i].GetComponent<TextMeshPro>().text[
                //(cursor_raw[1] + (cursor_raw[1] == 0 ? 0 : 1))]);
                
                //reset tmp text to remove tag influence
                loaded_words[i].GetComponent<TextMeshPro>().text = " " + words[cursor_raw[0]].content;

                cursor_rendered = loaded_words[i].GetComponent<TextMeshPro>()
                    .textInfo.characterInfo[cursor_raw[1] + (cursor_raw[1] == 0 ? 0 : 1)];

                //add in tags again
                words[cursor_raw[0]].SetCharacterMech();
                /*
                cursor_rendered = loaded_words[i].GetComponent<TextMeshPro>()
                    .GetTextInfo(" " + words[cursor_raw[0]].content).characterInfo[
                        cursor_raw[1] + (cursor_raw[1] == 0 ? 0 : 1)
                        ];*/

                //update destination based on the cursor position
                player.destination_override.x =
                    (cursor_rendered.topLeft + loaded_words[i].transform.position).x
                    - player.collider_bounds.width / 2f;
                //Debug.Log("destination override set to" + player.destination_override.x);

                player.destination_override.y = player.destination.y;
                player.destination_override.z = player.destination.z;   
            }
        }
    }

    private void UpdateRenderedCursor()
    {
        UpdateRenderedCursor(this.cursor_raw);
    }

    //get slope of some word by index
    private float GetSlope(int index)
    {
        if (index == 0) return 0;
        float n = 0.75f * perlin_map1.Noise(new VecN(3 * index, index / 10f))
            + 0.25f * perlin_map2.Noise(new VecN(index * 9, index / 30f));
        return Mathf.Lerp(slope_min_max.x, slope_min_max.y, n);
    }

    //parse the script
    //tags with the format <...></...> and <.../> are handled
    //line breaks and spaces are treated the same way
    public Word[] ParseScript(string s)
    {
        List<Word> words = new List<Word>();

        //starting block
        words.Add(new Word(new Tag[] { }, "###", GetSlope(0), 0, 3));

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
                words.Add(new Word(hanging_tags.ToArray(), hanging_word, GetSlope(words.Count), words.Count));
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
                        Word empty = new Word(new Tag[] { this_tag }, "", GetSlope(words.Count), words.Count);
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
