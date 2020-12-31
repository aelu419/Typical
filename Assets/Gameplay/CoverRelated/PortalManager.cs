using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    [ReadOnly] public Portal[] portals;
    public GameObject portal_prefab;
    public RuntimeAnimatorController portal_animator;
    public static PortalManager instance;
    public float margin;

    private List<PortalData> destinations;

    //portal manager is always initialized to the current portal manager in the scene
    private void Awake()

    {
        instance = this;
    }

    void Start()
    {
        EventManager.instance.OnPortalOpen += OnPortalOpen;
        EventManager.instance.OnPortalClose += OnPortalClose;

        //fetch destinations from current scene data
    }

    //open portals according to script and location
    //beginning marks the left middle position of the collection of portal blocks
    private void OnPortalOpen(Vector2 beginning)
    {
        transform.position = new Vector3(beginning.x, beginning.y, 0);
        Vector2 s = portal_prefab.GetComponent<SpriteRenderer>().size;
        s.y += margin * 2;

        portals = new Portal[destinations.Count];
        for(int i = 0; i < destinations.Count; i++)
        {
            float portional_h = i - destinations.Count / 2f + 0.5f;
            GameObject go = GameObject.Instantiate(
                portal_prefab,
                new Vector3(
                    0,
                    portional_h * s.y,
                    0
                    ),
                Quaternion.identity,
                transform);

            //TODO: set portal data
        }
    }

    //close all portals opened
    private void OnPortalClose()
    {

    }

    //TODO: implement this and link to portal prefab script
    private void SceneTransition(string destination)
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
