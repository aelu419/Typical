using System;
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

    public bool back_portal_opened;

    public bool BackPortalOpened
    {
        get
        {
            return back_portal_opened;
        }
    }

    public event Action<Vector2> OnBackPortalOpen;
    public void RaiseBackPortalOpen(Vector2 end)
    {
        if (OnBackPortalOpen != null && !back_portal_opened)
        {
            back_portal_opened = true;
            Debug.Log("portals are now available");
            OnBackPortalOpen(end);
        }
    }

    public event Action OnBackPortalClose;
    public void RaiseBackPortalClose()
    {
        if (OnBackPortalClose != null && back_portal_opened)
        {
            back_portal_opened = false;
            Debug.Log("portals are not unavailable");
            OnBackPortalClose();
        }
    }

    public event Action OnFrontPortalEngage;
    public void RaiseFrontPortalEngaged()
    {
        if (OnFrontPortalEngage != null)
        {
            OnFrontPortalEngage();
        }
    }

    public event Action OnFrontPortalDisengage;
    public void RaiseFrontPortalDisengaged()
    {
        if (OnFrontPortalDisengage != null)
        {
            OnFrontPortalDisengage();
        }
    }

    public void ScriptLoaded()
    {
        back_portal_opened = false;
        script_end_reached = false;
    }

    public void TransitionTo(ScriptObjectScriptable next, bool from_front)
    {
        if (ScriptableObjectManager.Instance.ScriptManager.SetNext(next))
        {
            back_portal_opened = !from_front;
            ScriptableObjectManager.Instance.ScriptManager.load_mode = from_front;
            StartExitingScene();
        }
        else
        {
            Debug.LogError("Next scene is not set!");
        }
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
