using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Portal : MonoBehaviour
{
    public string description, destination;
    public bool is_from_cover_prefab;
    public TextMeshPro word_block;
    public Animator portal_animator; //the important parameter is 'open' (bool)

    //obj is instantiated externally
    private void Start()
    {
        try
        {
            word_block = gameObject.
                transform.GetChild(0).gameObject.GetComponent<TextMeshPro>();
        }
        //error is thrown when there is no gameobject child to the portal
        //which is a normal occurrence from the portalcover prefab (not the portal prefab)
        catch(UnityException e)
        {
            Debug.Log(e);
        }

        portal_animator = gameObject.GetComponent<Animator>();

        if (is_from_cover_prefab)
        {
            EventManager.instance.OnPortalOpen += OnScriptPortalOpen;
            EventManager.instance.OnPortalClose += OnScriptPortalClose;
        }
    }

    private void OnScriptPortalOpen(Vector2 v)
    {
        Debug.Log("opening script end portal");
        portal_animator.SetBool("open", true);
    }
    private void OnScriptPortalClose()
    {
        Debug.Log("closing script end portal");
        portal_animator.SetBool("open", false);
    }
}
