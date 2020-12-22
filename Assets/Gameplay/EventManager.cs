using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager instance;

    private void Awake()
    {
        instance = this;
    }

    public event Action OnCorrectKeyPressed;
    public void RaiseCorrectKeyPressed()
    {
        if (OnCorrectKeyPressed != null)
        {
            OnCorrectKeyPressed();
        }
    }

    public event Action OnIncorrectKeyPressed;
    public void RaiseIncorrectKeyPressed()
    {
        if(OnIncorrectKeyPressed != null)
        {
            OnIncorrectKeyPressed();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
