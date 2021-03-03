using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FrontPortal : MonoBehaviour
{
    TextMeshPro tmp;
    // Start is called before the first frame update
    void Start()
    {
        tmp = transform.GetChild(0).GetComponent<TextMeshPro>();
        EventManager.Instance.OnFrontPortalEngage += OnFrontPortalEngage;
        EventManager.Instance.OnFrontPortalDisengage += OnFrontPortalDisengage;
        tmp.enabled = false;
    }

    private void OnFrontPortalEngage()
    {
        tmp.enabled = true;
    }

    private void OnFrontPortalDisengage()
    {
        tmp.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (tmp.enabled && Input.GetKeyDown(KeyCode.Backspace))
        {
            Debug.LogError("IMPLEMENT SCENE TRANSITION");
            
        }
    }
}
