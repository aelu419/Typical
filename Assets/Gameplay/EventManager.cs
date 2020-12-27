using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager instance;

    private void Awake()
    {
        instance = this;
    }

    public event Action OnCorrectKeyPressed;
    public void RaiseCorrectKeyPressed()
    {
        if (OnCorrectKeyPressed != null)
        {
            OnCorrectKeyPressed();
        }
    }

    public event Action OnIncorrectKeyPressed;
    public void RaiseIncorrectKeyPressed()
    {
        if(OnIncorrectKeyPressed != null)
        {
            OnIncorrectKeyPressed();
        }
    }

    public event Action OnCharacterDeleted;
    public void RaiseCharacterDeleted()
    {
        if(OnCharacterDeleted != null)
        {
            OnCharacterDeleted();
        }
    }

    public bool script_end_reached = false;
    public event Action OnScriptEndReached;
    public void RaiseScriptEndReached()
    {
        if(OnScriptEndReached != null && !script_end_reached)
        {
            Debug.Log("End of script is reached, a portal should be spawn to quit the current story");
            script_end_reached = true;
            OnScriptEndReached();
        }
    }

    public void ScriptLoaded()
    {
        script_end_reached = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
