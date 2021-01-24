using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypingStatusRecord : MonoBehaviour
{
    [ReadOnly] public float timer;
    [ReadOnly] public int correct_presses;
    [ReadOnly] public int incorrect_presses;
    [ReadOnly] public float accuracy;
    [ReadOnly] public float speed;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0f;
        correct_presses = 0;
        incorrect_presses = 0;

        accuracy = 1.0f;
        speed = 0;

        EventManager.Instance.OnCorrectKeyPressed += OnCorrectKeyPressed;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        speed = timer == 0 ? 0f : correct_presses / timer;
    }

    private void OnCorrectKeyPressed()
    {
        correct_presses++;
        accuracy = (float)correct_presses / (incorrect_presses + correct_presses);
    }

    private void OnIncorrectKeyPressed()
    {
        incorrect_presses++;
        accuracy = (float)correct_presses / (incorrect_presses + correct_presses);
    }

    private void OnCharacterDeleted()
    {
        correct_presses--;

        accuracy = 
            correct_presses + incorrect_presses == 0 ? 
                1.0f : accuracy = (float)correct_presses / (incorrect_presses + correct_presses);
    }
}
