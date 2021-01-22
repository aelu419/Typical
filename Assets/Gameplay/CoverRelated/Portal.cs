using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Portal : MonoBehaviour
{
    public PortalData data;
    public bool is_from_cover_prefab;
    public TextMeshPro word_block;
    public Animator portal_animator; //the important parameter is 'open' (bool)

    //obj is instantiated externally
    private void Start()
    {
        //portal gameobject contains child object ONLY when it is spawn
        //to the right of the script
        //itself, meaning that the child obj is the textmesh
        if(transform.childCount != 0)
        {
            word_block = gameObject.
                transform.GetChild(0).gameObject.GetComponent<TextMeshPro>();
            word_block.text = data.description;
        }

        portal_animator = gameObject.GetComponent<Animator>();

        if (is_from_cover_prefab)
        {
            EventManager.instance.OnPortalOpen += OnScriptPortalOpen;
            EventManager.instance.OnPortalClose += OnScriptPortalClose;
        }
    }

    //TODO: implement transition to another scene, link to portal manager
    private void OnPortalOpen()
    {
        Debug.LogError("implement transition!");
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

[System.Serializable]
public class PortalData
{
    public string description;
    public ScriptObjectScriptable destination;
    public static PortalData default_portal_data;

    static PortalData()
    {
        default_portal_data = new PortalData(
            "roll credit", 
            ScriptableObjectManager.Instance.ScriptManager.scripts[0]
            );
    }

    public static PortalData GetCloneOfDefault()
    {
        return new PortalData(
            "roll credit",
            ScriptableObjectManager.Instance.ScriptManager.scripts[0]
            );
    }

    public PortalData(string description, ScriptObjectScriptable destination)
    {
        this.description = description;
        this.destination = destination;
    }

    public PortalData(string description, string destination_name)
    {
        this.description = description;
        foreach(ScriptObjectScriptable d in 
            ScriptableObjectManager.Instance.ScriptManager.scripts)
        {
            if (d.name_.Equals(destination_name))
            {
                this.destination = d;
            }
        }
        if(this.destination == null)
        {
            throw new System.Exception("destination called " + destination_name
                + " cannot be found");
        }
    }
}
