using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
[CreateAssetMenu(menuName = "Typical Customs/Dispensers/Script Dispenser")]
public class ScriptDispenser : ScriptableObject
{
    public ScriptObjectScriptable[] init_scripts;
    [System.NonSerialized]
    public static ScriptObjectScriptable[] scripts;

    public ScriptObjectScriptable main_menu_no_save;
    public ScriptObjectScriptable main_menu_has_save;
    public ScriptObjectScriptable tutorial;

    private static ScriptObjectScriptable _current;

    public TextAsset parseable;


    public const string
        MAINMENU = "_mainmenu",
        QUIT = "_quit",
        SAVE = "_save",
        TUTORIAL = "_tutorial";

    public static bool first_load = true; //the first time for a script to be loaded, this is set once per game session

    //true: enter from the front of the script
    //false: enter from the back of the script, and set all words as typed out
    //(see ReadingManager for implementation)
    public bool load_mode;

    public ScriptObjectScriptable CurrentScript {
        get
        {
            if (first_load && _current == null)
            {
                //during first load, debug all the scripts by trying to parse each of them
                foreach (ScriptObjectScriptable s in scripts)
                {
                    GameObject.FindGameObjectWithTag("General Manager").GetComponent<ReadingManager>().ParseScript(
                        s.text
                        );
                }

                //Debug.Log("loading for the first time");
                if (GameSave.PassedTutorial)
                {
                    //fetch main menu
                    if (GameSave.HasSavedScene)
                    {
                        _current = main_menu_has_save;
                    }
                    else
                    {
                        _current = main_menu_no_save;
                    }
                }
                else
                {
                    _current = tutorial;
                }
            }
            return _current;
        }
    }

    public string Previous
    {
        get
        {
            ScriptObjectScriptable p = Fetch(CurrentScript.previous);
            if (p != null)
            {
                return p.name_;
            }
            else
            {
                if (GameSave.PassedTutorial && _current.name_.Equals(TUTORIAL))
                {
                    return MAINMENU;
                }
                else
                {
                    return null;
                }
            }
        }
    }

    public void SetCurrentScript(string destination)
    {
        _current = Fetch(destination);
    }

    public ScriptObjectScriptable LoadSaved()
    {
        string s = GameSave.SavedScene;
        if (s.Equals("")) { return null; }
        for(int i = 0; i < scripts.Length; i++)
        {
            if (scripts[i].name_.Equals(s))
            {
                Debug.Log("found game save on " + s);
                return scripts[i];
            }
        }
        Debug.LogError("saved scene called \"" + s + "\" cannot be found");
        return null;
    }

    public ScriptObjectScriptable Fetch(string name)
    {
        if (name.Equals(MAINMENU))
        {
            if (GameSave.HasSavedScene)
            {
                return main_menu_has_save;
            }
            else
            {
                return main_menu_no_save;
            }
        }
        else if (name.Equals(SAVE))
        {
            return LoadSaved();
        }
        else
        {
            foreach (ScriptObjectScriptable s in scripts)
            {
                if (s.name_.Equals(name))
                {
                    return s;
                }
            }
            return null;
        }
    }

    public static void CheckValidity(ScriptObjectScriptable sos)
    {
        OnLoad();
        List<PortalData> portals = new List<PortalData>();
        foreach (string n in sos.next)
        {
            portals.Add(PortalData.Fetch(n));
        }
        portals.Add(PortalData.Fetch(sos.previous));

        foreach (PortalData p in portals)
        {
            bool found = false;
            foreach (ScriptObjectScriptable s in scripts)
            {
                if (p.destination.Equals(s.name_))
                {
                    found = true;
                }
            }
            if (!found)
            {
                throw new System.Exception(p.destination + " cannot be found!");
            }
        }
    }

    /*
    private List<ScriptObjectScriptable> Parse()
    {
        List<ScriptObjectScriptable> s = new List<ScriptObjectScriptable>();
        string p = parseable.text;
        string[] lines = p.Split('\n');

        for (int i = 0; i < lines.Length;)
        {
            string header = Extract(lines[i++]);
            string previous = Extract(lines[i++]);
            List<string> nexts = new List<string>();
            while (lines[i][0] == '[')
            {
                nexts.Add(Extract(lines[i++]));
            }

            string script = "";
            while (i < lines.Length)
            {
                if (lines[i].Length == 0)
                {
                    i++;
                    if (i >= lines.Length) break;
                }
                else if (lines[i][0] == '[')
                {
                    break;
                }
                script += " " + lines[i];
                i++;
            }
            
            ScriptObjectScriptable chapter = CreateInstance<ScriptObjectScriptable>();

            chapter.name_ = header;
            chapter.previous = previous;
            chapter.next = nexts.ToArray();
            chapter.text = script;

            s.Add(chapter);
        }
        return s;
    }

    private string Extract (string full)
    {
        //Debug.Log(full);
        int start = full.IndexOf('[');
        int end = full.IndexOf(']');
        return full.Substring(start + 1, end - start - 1);
    }*/

    public static event System.Action OnLoad;

    public void LoadScripts()
    {
        ScriptObjectScriptable[] fwd = Resources.LoadAll<ScriptObjectScriptable>("PlotFwd/");
        ScriptObjectScriptable[] bwd = Resources.LoadAll<ScriptObjectScriptable>("PlotBwd/");
        List<ScriptObjectScriptable> all = new List<ScriptObjectScriptable>();
        all.AddRange(fwd);
        all.AddRange(bwd);
        all.AddRange(init_scripts);

        scripts = all.ToArray();
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Append("loaded scripts:\n\t");
        foreach (ScriptObjectScriptable s in scripts)
        {
            sb.Append(s.name_);
        }
        Debug.Log(sb);
    }

    private void OnEnable()
    {
        LoadScripts();
        OnLoad = LoadScripts;
        /*List<ScriptObjectScriptable> all = Parse();
        all.AddRange(init_scripts);
        scripts = all.ToArray();

        foreach (ScriptObjectScriptable s in scripts)
        {
            Debug.Log("script loaded: " + s.name_);
        }*/

        load_mode = true;
        //_current = LoadSaved();
    }
}
