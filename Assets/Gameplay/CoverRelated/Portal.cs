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

    public string descriptor;


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
            if (descriptor != null && descriptor != "")
            {
                word_block.text = descriptor;
            }
            else
            {
                word_block.text = data.description;
            }
        }

        portal_animator = gameObject.GetComponent<Animator>();

        //DIFFERENTIATE FRONT BACK PORTAL, ASSIGN BY READING MANAGER, NOT HERE!
        if (is_from_cover_prefab)
        {
            portal_animator.SetBool("open", true);
        }
    }

    public void SetDisplay(PortalData pd, KeyCode k)
    {
        Debug.Log(pd + ", " + k);
        data = pd;
        data.control = k;
        descriptor = "[" + k.ToString() + "] " + data.description;
    }

    //transition forward to the next scene indicated by this portal's portal data
    public void OnPortalOpen()
    {
        portal_animator.SetBool("open", true);
        //force update player direction to face right (true)
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>().direction = true;

        if (data.sp == PortalData.SpecialPortals.quit)
        {
            Debug.Log("quitting, please implement progress saving mechanism!");
            Application.Quit();
            return;
        }
        else
        {
            //transition to specific scene
            //set dispenser to display with next script loaded
            EventManager.Instance.TransitionTo(data.destination, true);
        }
    }

}

[System.Serializable]
public class PortalData
{
    public string description;
    public ScriptObjectScriptable destination;
    public static PortalData default_portal_data;
    public SpecialPortals sp;

    private const string QUIT = "_quit";
    private const string MAIN_MENU = "_mainmenu";

    public KeyCode control;

    public enum SpecialPortals
    {
        none,
        quit
    }

    static PortalData()
    {
        default_portal_data = new PortalData(
            "main menu", 
            ScriptableObjectManager.Instance.ScriptManager.scripts[0]
            );
    }

    public static PortalData GetCloneOfDefault()
    {
        return new PortalData(
            "main menu",
            ScriptableObjectManager.Instance.ScriptManager.scripts[0]
            );
    }

    public PortalData(string description, ScriptObjectScriptable destination)
    {
        sp = SpecialPortals.none; //only string destination can result in special portals
        this.description = description;
        this.destination = destination;
    }

    public PortalData(string description, string destination_name)
    {
        sp = SpecialPortals.none;
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
            switch(destination_name)
            {
                case QUIT:
                    sp = SpecialPortals.quit;
                    break;
                default:
                    throw new System.Exception("destination called " + destination_name
                            + " cannot be found");
            }
        }
    }
}
