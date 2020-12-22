using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float charSize; //the height of the main character, in world units
    //public SpriteRenderer renderer; //the sprite renderer assigned to the main character

    // Start is called before the first frame update
    void Start()
    {

        EventManager.instance.OnCorrectKeyPressed += CorrectKeyPressed;
        EventManager.instance.OnIncorrectKeyPressed += IncorrectKeyPressed;

        transform.localScale = new Vector3(charSize, charSize, charSize);
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
