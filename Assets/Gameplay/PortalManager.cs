using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        EventManager.instance.OnPortalOpen += OnPortalOpen;
        EventManager.instance.OnPortalClose += OnPortalClose;
    }

    //open portals according to script and location
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
