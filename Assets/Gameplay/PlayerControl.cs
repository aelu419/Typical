using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        EventManager.instance.OnCorrectKeyPressed += CorrectKeyPressed;
        EventManager.instance.OnIncorrectKeyPressed += IncorrectKeyPressed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void CorrectKeyPressed()
    {
        Debug.Log("correct!");
    }
    private void IncorrectKeyPressed()
    {
        Debug.Log("incorrect!");
    }
}
