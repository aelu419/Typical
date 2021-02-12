using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private static EventManager _instance;
    public static EventManager Instance => _instance;

    private void Awake()
    {
        Debug.Log("Event Manager instantiated");
        _instance = this;
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

    public event Action<Vector2> OnPortalOpen;
    public void RaisePortalOpen(Vector2 end)
    {

        Debug.Log("portals are now available");
        if (OnPortalOpen != null)
        {
            Debug.Log("portals are now available");
            OnPortalOpen(end);
        }
    }

    public event Action OnPortalClose;
    public void RaisePortalClose()
    {
        if (OnPortalClose != null)
        {
            Debug.Log("portals are not unavailable");
            OnPortalClose();
        }
    }

    public void ScriptLoaded()
    {
        script_end_reached = false;
    }

    public event Action OnStartExitingScene;
    public void StartExitingScene()
    {
        if (OnStartExitingScene != null)
        {
            OnStartExitingScene();
        }
    }

    public event Action OnStartEnteringScene;
    public void StartEnteringScene()
    {
        if (OnStartEnteringScene != null)
        {
            OnStartEnteringScene();
        }
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
