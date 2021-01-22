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
    public List<Word> words;
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

    [ReadOnly] public char next_letter; //the next letter to be typed out
    private int first_typable_word; //the first word in the script that contains typable letters
    private int last_typable_word; //the last word in the script that contains typable letters

    private void Awake()
    {
        perlin_map1 = new Perlin(10, 2);
        perlin_map2 = new Perlin(10, 2);
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("script dispenser instance = " + 
            ScriptableObjectManager.Instance.ScriptManager.name);
        words = ParseScript(
            ScriptableObjectManager.Instance.ScriptManager.CurrentScript.Text
            );

        //pick out the portals
        List<PortalData> ports = new List<PortalData>();
        Debug.Log(words);
        for(int i = 0; i < words.Count; i++)
        {
            Word w = words[i];
            Tag[] t = w.tags;
            foreach (Tag t_ in t)
            {
                if (t_.appearance == Tag.TagAppearanceType.self_closing
                    && t_.type.Equals("P"))
                {
                    try
                    {
                        ports.Add(new PortalData(
                            t_.GetSpecAt(0),
                            t_.GetSpecAt(1)));
                    }
                    catch(System.Exception e)
                    {
                        Debug.LogError("defunct portal tag: " + t_.ToString());
                        Debug.LogError("\t" + e.Message);
                    }
                    finally
                    {
                        words.Remove(w);
                        i--;
                    }
                }
            }
        }
        if(ports.Count == 0)
        {
            ports.Add(PortalData.GetCloneOfDefault());
        }

        PortalManager.instance.destinations = ports;

        /*
        Debug.Log("the parsed script is:");
        for(int i = 0; i < words.Count; i++)
        {
            Debug.Log("\t" + words[i]);
        }*/

        //connect to rest of components
        vManager = GetComponent<VisualManager>();
        player = GameObject.FindGameObjectWithTag("Player")
            .GetComponent<PlayerControl>();

        Vector2 cursor = new Vector2(0, 0);
        //load words, by default load the first 10
        loaded_words = new List<GameObject>();
        (Vector2 cursor, GameObject go) word_loader_temp;
        for (int i = 0; i < Mathf.Min(10, words.Count); i++)
        {
            word_loader_temp = words[i].ToPrefab(text_holder_prefab, cursor);
            cursor = word_loader_temp.cursor; //update cursor
            loaded_words.Add(word_loader_temp.go);
        }

        if (loaded_words.Count < 3)
            throw new System.Exception("No other word in script");

        //search for the last typable word in script
        for (int i = words.Count - 1; i >= 0; i--)
        {
            if (words[i].has_typable)
            {
                last_typable_word = i;
                break;
            }
            if (i == 0)
            {
                throw new System.Exception("No typable word in script");
            }
        }

        //search for the fist typable word in script
        for (int i = 0; i < words.Count; i++)
        {
            if (words[i].has_typable)
            {
                first_typable_word = i;
                break;
            }
        }

        EventManager.instance.ScriptLoaded();

        //cursor is initialized to the first typable letter in the first typable word
        cursor_raw = new int[] { first_typable_word, words[first_typable_word].first_typable };
        next_letter = words[first_typable_word].content[words[first_typable_word].first_typable];

        //update player position
        player.spawn_root = words[0].GetCharacterInfo(2).topRight;
        UpdateRenderedCursor();

        //Debug.Log("starting at " + cursor_raw[0] + "'s " + cursor_raw[1] + "'th letter");
        //Debug.Log("the next letter is: " + next_letter);
    }

    // Update is called once per frame
    void Update()
    {
        /*
         * code below deal with loading words in both play mode and edit mode
         */

        //deal with word loading within camera scope + buffer region
        GameObject last_loaded_word = loaded_words[loaded_words.Count - 1];
        GameObject first_loaded_word = loaded_words[0];

        // when current last loaded word's end enters the right buffer, load the next word from back
        if (last_loaded_word.GetComponent<WordBlockBehavior>().content.R.x
            < (vManager.CAM.xMax + vManager.BUFFER_SIZE))
        {
            //Debug.Log("load from right");
            int i = last_loaded_word.GetComponent<WordBlockBehavior>().content.index;
            i++;
            if(i < words.Count)
            {
                //load word at i
                (Vector2 cursor, GameObject go) word_loader_temp =
                    words[i].ToPrefab(text_holder_prefab, words[i-1].R);

                loaded_words.Add(word_loader_temp.go);
                last_loaded_word = word_loader_temp.go;
            }
            else
            {
            }
        }

        // when current last loaded word exists the right buffer,
        // or if the first loaded word comes into the screen
        // load the word before the current first loaded word,
        // but don't unload the last loaded word as we expect it to come back to view soon
        if(last_loaded_word.GetComponent<WordBlockBehavior>().content.L.x
            > (vManager.CAM.xMax + vManager.BUFFER_SIZE)
            || first_loaded_word.GetComponent<WordBlockBehavior>().content.R.x
            >= vManager.CAM.xMin)
        {
            //Debug.Log("load from left");
            int i = first_loaded_word.GetComponent<WordBlockBehavior>().content.index;
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
        if (first_loaded_word.GetComponent<WordBlockBehavior>().content.R.x
            < (vManager.CAM.xMin - vManager.BUFFER_SIZE)){
            //Debug.Log("unloading " + first_loaded_word.GetComponent<WordBlockBehavior>().content.content);
            loaded_words.Remove(first_loaded_word);
            Destroy(first_loaded_word);
        }

        /*
         * code from now on deals with player input under play mode
         */

        // handle input
        if (next_letter != '\0' && Input.GetKeyDown(next_letter.ToString().ToLower())) //correct key is pressed
        {
            EventManager.instance.RaiseCorrectKeyPressed();

            // skip unmatching sequence caused by backspacing (see skip_over_puncuation)
            for (int i = 0; i < loaded_words.Count; i++)
            {
                Word this_loaded_word = loaded_words[i].GetComponent<WordBlockBehavior>().content;
                if (this_loaded_word.index > cursor_raw[0])
                {
                    break;
                }
                if (this_loaded_word.typed < this_loaded_word.content.Length - 1)
                {
                    if (this_loaded_word.index == cursor_raw[0])
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
                    if (cursor_raw[0] == last_typable_word)
                    {
                        //move on to the portal
                        cursor_raw[0]++;
                        cursor_raw[1] = 0;

                        next_letter = '\0';

                        break;
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
                            if (i == words.Count - 1)
                            {
                                //currently on the last word of the script
                                if (cursor_raw[0] == words.Count - 1)
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
            } while (cursor_raw[0] < words.Count
                && !char.IsLetter(next_letter));

            if (next_letter == '\0')
            {
                EventManager.instance.RaisePortalOpen(
                    new Vector2(
                        words[words.Count - 1].R.x,
                        words[words.Count - 1].top
                        )
                    );
                //Debug.Log("next letter is " + next_letter);
            }

            UpdateRenderedCursor();
        }

        //going backwards
        else if (Input.GetKeyDown(KeyCode.Backspace))
        {
            //do not do anything if currently on the first typable letter of the entire script
            //cursor somehow went beyond the beginning of the typable scripts
            if (cursor_raw[0] < first_typable_word
                && cursor_raw[1] <= words[first_typable_word].first_typable)
            {
                cursor_raw[0] = first_typable_word;
                cursor_raw[1] = words[first_typable_word].first_typable;

                words[first_typable_word].SetCharacterMech(0);

                for (int i = 0; i < first_typable_word; i++)
                {
                    words[i].SetCharacterMech(words[i].content.Length);
                }
                words[first_typable_word].SetCharacterMech(words[first_typable_word].first_typable);
            }

            else
            {
                //if currently exiting from the last word on the script (the portal marker)
                //broadcast event to notice the portal manager
                if(cursor_raw[0] == words.Count - 1)
                {
                    EventManager.instance.RaisePortalClose();
                }

                EventManager.instance.RaiseCharacterDeleted();
                int[] cursor_override = new int[] { -1, -1 };

                //in the span of the current word:
                //skip through the first untypable sequence encountered, 
                //until first typable letter on the left is reached
                //or the beginning of the word is reached
                if (cursor_raw[1] > 0)
                {
                    while (cursor_raw[1] > 0)
                    {
                        cursor_raw[1]--;
                        if (char.IsLetter(words[cursor_raw[0]].content[cursor_raw[1]]))
                        {
                            break;
                        }
                        else
                        {
                            if (cursor_raw[1] < words[cursor_raw[0]].first_typable ||
                                cursor_raw[1] <= 0)
                            {
                                cursor_raw[1] = -1;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    //Debug.Log("exiting directly off of word beginning");
                    cursor_raw[1] = -1;
                }

                //when exiting current word from the left
                if (cursor_raw[1] < 0)
                {
                    //Debug.Log("exiting from " + cursor_raw[0]);
                    //clear current word
                    words[cursor_raw[0]].SetCharacterMech(0);
                    //skip through non-typable words
                    //int[] cursor_temp = new int[] { cursor_raw[0], cursor_raw[1] };

                    while (cursor_raw[0] > 0)
                    {
                        cursor_raw[0]--;
                        if (words[cursor_raw[0]].has_typable)
                        {
                            Debug.Log("backspace sequence stops at " + words[cursor_raw[0]]);
                            break;
                        }
                        //clear when skipping through words
                        words[cursor_raw[0]].SetCharacterMech(0);
                    }

                    //reached either the start of the script or a typable word:
                    //for the case of typable word
                    Word stopped = words[cursor_raw[0]];
                    if (stopped.has_typable)
                    {
                        //if the word block ends with a typable letter,
                        //then the letter itself is skipped
                        if (stopped.last_typable == stopped.content.Length - 1)
                        {
                            cursor_raw[1] = stopped.last_typable;
                            next_letter = stopped.content[cursor_raw[1]];
                            stopped.SetCharacterMech(cursor_raw[1]);
                        }
                        //if the word block ends with some punctuation instead
                        else
                        {
                            stopped.SetCharacterMech(stopped.last_typable + 1);
                            cursor_raw[1] = stopped.last_typable + 1;
                            //next letter remains unchanged because only punctuations have been escaped
                        }

                    }
                }
                //if cursor does not exit from any word
                else
                {
                    next_letter = words[cursor_raw[0]].content[cursor_raw[1]];
                    words[cursor_raw[0]].SetCharacterMech(cursor_raw[1]);
                }

                //cursor somehow went beyond the beginning of the typable scripts
                if (cursor_raw[0] < first_typable_word)
                {
                    cursor_raw[0] = first_typable_word;
                    cursor_raw[1] = words[first_typable_word].first_typable;

                    for (int i = 0; i < first_typable_word; i++)
                    {
                        words[i].SetCharacterMech(words[i].content.Length);
                    }
                    words[first_typable_word].SetCharacterMech(words[first_typable_word].first_typable);
                }

                UpdateRenderedCursor();
            }


            /*
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
            }*/

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
        Word current = words[cursor_raw[0]];

        if (current.L == null)
        {
            throw new System.Exception("word is already unloaded!");
        }
        else
        {
            cursor_rendered = words[cursor_raw[0]].GetCharacterInfo(cursor_raw[1]);

            //update destination based on the cursor position
            player.destination_override.x =
                cursor_rendered.topLeft.x + words[cursor_raw[0]].L.x
                - player.collider_bounds.width / 2f;

            //Debug.Log(cursor_raw[0] + ", " + cursor_raw[1] + " -- " + player.destination_override.x);

            player.destination_override.y = player.destination.y;
            player.destination_override.z = player.destination.z;
        }

        //--------deprecated: raw tmp handling is delegated to the word obj itself
        /*
        for (int i = 0; i < loaded_words.Count; i++)
        {
            Word loaded_temp = loaded_words[i].GetComponent<WordBlockBehavior>().content;
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

                //cursor_rendered = loaded_words[i].GetComponent<TextMeshPro>()
                    //.GetTextInfo(" " + words[cursor_raw[0]].content).characterInfo[
                        //cursor_raw[1] + (cursor_raw[1] == 0 ? 0 : 1)
                        //];

                //update destination based on the cursor position
                player.destination_override.x =
                    cursor_rendered.topLeft.x + words[cursor_raw[0]].L.x
                    - player.collider_bounds.width / 2f;
                Debug.Log("destination override set to " + player.destination_override.x);

                player.destination_override.y = player.destination.y;
                player.destination_override.z = player.destination.z;   
            }
        }*/
    }

    private void UpdateRenderedCursor()
    {
        UpdateRenderedCursor(this.cursor_raw);
    }

    //return if the marker falls within a word block, horizontally
    public bool IsBetween(float marker, Word w)
    {
        //Debug.Log(marker + " in? " + w.L.x + " to " + w.R.x);
        return marker >= w.L.x && marker <= w.R.x;
    }

    //return if the marker falls within bounds 1 and 2
    public bool IsBetween(float marker, float bound1, float bound2)
    {
        float min = Mathf.Min(bound1, bound2);
        float max = Mathf.Max(bound2, bound1);
        return marker >= min && marker <= max;
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
    public List<Word> ParseScript(string s)
    {
        List<Word> words = new List<Word>();

        //starting block
        //the first block has no slope and is entirely typed out
        words.Add(new Word(new Tag[] { }, "###", 0, 0, 3));

        Regex tag = new Regex(@"<\s*(\/?)(\s*[^>])+\s*(\/?)\s*>");

        Regex open_tag = new Regex(@"<[^\/]+>");
        Regex close_tag = new Regex(@"<\s*(\/).*>");
        Regex self_close_tag = new Regex(@"<.*(\/)>");

        //append lamp at start
        s = "<O lamp/>" + s;

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
        //add portal at end
        s += " <O portal/>";

        Debug.Log(s);

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
            //cursor is at white space, or at last character of the script
            if(raw[cursor]==' ')
            {
                //terminate cached word and add it to the list
                words.Add(new Word(hanging_tags.ToArray(), hanging_word, GetSlope(words.Count), words.Count));
                hanging_word = "";
            }
            else if (cursor == raw.Length - 1)
            {
                hanging_word += raw[cursor];
                words.Add(new Word(hanging_tags.ToArray(), hanging_word, GetSlope(words.Count), words.Count));
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

                Tag this_tag = new Tag(tag_content_list, t);
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
                        //self closing tags do not change the slope
                        Word empty = new Word(new Tag[] { this_tag }, "", 0, words.Count);
                        //in this case the last word should also end with 0 slope
                        words[words.Count - 1].slope = 0;

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

        return words;
    }

}
