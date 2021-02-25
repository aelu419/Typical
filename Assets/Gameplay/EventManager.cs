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

    private bool portal_opened;
    public event Action<Vector2> OnPortalOpen;
    public void RaisePortalOpen(Vector2 end)
    {
        if (OnPortalOpen != null && !portal_opened)
        {
            portal_opened = true;
            Debug.Log("portals are now available");
            OnPortalOpen(end);
        }
    }

    public bool PortalOpened
    {
        get
        {
            return portal_opened;
        }
    }
    public event Action OnPortalClose;
    public void RaisePortalClose()
    {
        if (OnPortalClose != null && portal_opened)
        {
            portal_opened = false;
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
            Debug.Log("exiting scene");
            OnStartExitingScene();
        }
    }

    public event Action OnStartEnteringScene;
    public void StartEnteringScene()
    {
        if (OnStartEnteringScene != null)
        {
            portal_opened = false;
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
