using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    public GameObject portal_prefab;
    public RuntimeAnimatorController portal_animator;

    private static PortalManager _instance;
    public static PortalManager Instance => _instance;

    public float margin;

    public List<PortalData> destinations;
    public List<GameObject> active_portals;

    private static KeyCode[] alphabet =
    {
        KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E, KeyCode.F,
        KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J, KeyCode.K, KeyCode.L,
        KeyCode.M, KeyCode.N, KeyCode.O, KeyCode.P, KeyCode.Q, KeyCode.R,
        KeyCode.S, KeyCode.T, KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X,
        KeyCode.Y, KeyCode.Z
    };

    private KeyCode[] registeredListening;

    //portal manager is always initialized to the current portal manager in the scene
    private void Awake()

    {
        _instance = this;
    }

    void Start()
    {
        EventManager.Instance.OnPortalOpen += OnPortalOpen;
        EventManager.Instance.OnPortalClose += OnPortalClose;

        //fetch destinations from current scene data
    }

    public PortalData InitializePortalFromTag(Tag t)
    {
        string[] specs = t.Specs;
        if (t.Specs.Length < 2)
        {
            throw new UnityException("portal tag" + t + " lack sufficient specs, need description and destination");
        }
        else
        {
            string description = "";
            for(int i = 0; i < specs.Length - 1; i++)
            {
                description += specs[i] + " ";
            }
            return new PortalData(description, specs[specs.Length - 1]);
        }
    }

    //open portals according to script and location
    //beginning marks the left middle position of the collection of portal blocks
    private void OnPortalOpen(Vector2 beginning)
    {
        foreach (PortalData pd in destinations)
        {
            Debug.Log(pd.description);
        }
        if (destinations == null || destinations.Count == 0)
        {
            Debug.LogError("no destination specified, skipping portal opening procedure");
            return;
        }

        transform.position = new Vector3(beginning.x, beginning.y, 0);
        Vector2 s = portal_prefab.GetComponent<SpriteRenderer>().size;
        float block_raw_height = s.y;
        float block_whole_height = s.y + 2 * margin;

        registeredListening = new KeyCode[destinations.Count];
        active_portals = new List<GameObject>();

        destinations.Reverse();

        for (int i = 0; i < destinations.Count ; i++)
        {
            float portional_h = i - destinations.Count / 2f + 0.5f;
            GameObject go = GameObject.Instantiate(
                portal_prefab,
                new Vector3(
                    transform.position.x + 2 + Random.value * 0.3f - 0.15f,
                    transform.position.y + block_raw_height / 2 
                        + portional_h * block_whole_height + Random.value * 0.2f - 0.1f,
                    transform.position.z
                    ),
                Quaternion.identity,
                transform);

            //TODO: set portal data
            Portal p_ = go.GetComponent<Portal>();
            p_.SetDisplay(destinations[i], alphabet[destinations.Count - i - 1]);
            registeredListening[i] = alphabet[destinations.Count - i - 1];

            active_portals.Add(go);
        }
    }

    //close all portals opened
    private void OnPortalClose()
    {
        //no longer listen to key presses
        registeredListening = new KeyCode[] { };
        if (active_portals != null && active_portals.Count > 0)
        {
            foreach(GameObject go in active_portals)
            {
                Destroy(go);
            }
        }
    }

    // Update is called once per frame
    // listen for keypresses
    void Update()
    {
        if (registeredListening != null && registeredListening.Length > 0)
        {
            for(int i = 0; i < registeredListening.Length; i++)
            {
                if (Input.GetKeyDown(registeredListening[i]))
                {
                    active_portals[i].GetComponent<Portal>().OnPortalOpen();
                    registeredListening = new KeyCode[] { };
                    return;
                }
            }
        }
    }
}
