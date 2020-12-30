using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    [ReadOnly] public Portal[] portals;
    public GameObject portal_prefab;
    public RuntimeAnimatorController portal_animator;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.instance.OnPortalOpen += OnPortalOpen;
        EventManager.instance.OnPortalClose += OnPortalClose;
    }

    //open portals according to script and location
    //beginning marks the left middle position of the collection of portal blocks
    private void OnPortalOpen(Vector2 beginning)
    {

    }

    //close all portals opened
    private void OnPortalClose()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
