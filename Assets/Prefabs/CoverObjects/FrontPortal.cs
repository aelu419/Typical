using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FrontPortal : MonoBehaviour
{
    TextMeshPro tmp;
    // Start is called before the first frame update
    bool isDisplayingWarning;

    void Start()
    {
        tmp = transform.GetChild(0).GetComponent<TextMeshPro>();
        EventManager.Instance.OnFrontPortalEngage += OnFrontPortalEngage;
        EventManager.Instance.OnFrontPortalDisengage += OnFrontPortalDisengage;
        tmp.enabled = false;
        isDisplayingWarning = false;
    }

    private void OnFrontPortalEngage()
    {
        tmp.enabled = true;
        isDisplayingWarning = true;
    }

    private void OnFrontPortalDisengage()
    {
        tmp.enabled = false;
        isDisplayingWarning = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDisplayingWarning && Input.GetKeyDown(KeyCode.Backspace))
        {
            tmp.enabled = false;
            isDisplayingWarning = false;
            Debug.LogError("IMPLEMENT SCENE TRANSITION");
            EventManager.Instance.TransitionTo(
                ScriptableObjectManager.Instance.ScriptManager.CurrentScript.previous
                );
        }
    }
}
