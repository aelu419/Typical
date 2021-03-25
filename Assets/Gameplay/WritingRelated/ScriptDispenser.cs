using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Typical Customs/Dispensers/Script Dispenser")]
public class ScriptDispenser : ScriptableObject
{
    public ScriptObjectScriptable[] scripts;

    public ScriptObjectScriptable main_menu_no_save;
    public ScriptObjectScriptable main_menu_has_save;
    public ScriptObjectScriptable tutorial;

    private static ScriptObjectScriptable _current;

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
            if (first_load)
            {
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

    private void OnEnable()
    {
        //GameSave.ClearSave();
        load_mode = true;
        _current = LoadSaved();
    }
}
