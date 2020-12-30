using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Portal : MonoBehaviour
{
    public string description, destination;
    public TextMeshPro word_block;
    public Animator portal_animator; //the important parameter is 'open' (bool)

    //obj is instantiated externally
    private void Start()
    {
        if (gameObject.transform.GetChild(0) != null)
        {
            word_block = gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>();
        }

        portal_animator = gameObject.GetComponent<Animator>();
        //portals on the script will have null as animator
        if(portal_animator == null)
        {
            gameObject.AddComponent<Animator>();
            portal_animator = gameObject.GetComponent<Animator>();
            portal_animator.runtimeAnimatorController =
                GameObject.FindGameObjectWithTag("Portal Manager")
                .GetComponent<PortalManager>().portal_animator;

            EventManager.instance.OnPortalOpen += OnScriptPortalOpen;
            EventManager.instance.OnPortalClose += OnScriptPortalClose;
        }
        //portals instantiated from the prefab will have an animator already
        else
        {

        }
    }

    private void OnScriptPortalOpen(Vector2 v)
    {
        portal_animator.SetBool("open", true);
    }
    private void OnScriptPortalClose()
    {
        portal_animator.SetBool("open", true);
    }
}
