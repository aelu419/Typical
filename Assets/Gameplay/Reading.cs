using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reading : MonoBehaviour
{
    //for saving user progress
    public string section;
    public int sectionLineNum;
    public int universalLineNum;

    public float accuracy = 1.0f;
    private int typedLetters = 0;
    private int totalTypedLetters = 0;
    public float speed = 1.0f; //letters per second
    private float timeSinceLastCorrect = 0.0f;

    //input related
    private ToBeTyped upcomingLetter;
    private string[] scriptLines;

    //display related
    public string rawString; //unformated, i.e. without differentiation of material
    private string displayString; //material tag added, ready to give to textmesh
    UnityEngine.UI.Text mesh;

    void Awake()
    {
        TextAsset scriptText = Resources.Load("script") as TextAsset;
        scriptLines = scriptText.text.Split('\n');
    }
    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<UnityEngine.UI.Text>();
        

        if(scriptLines != null && scriptLines.Length > 0)
        {
            //read lines until there is one line with a letter and is not a section tag
            universalLineNum = -1;
            ParseNextLine();
        }
        else
        {
            Debug.Log("there is no script!");
        }
    }


    public void KeyInput()
    {
        //cheat stroke
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.RightShift))
        {
            Debug.Log("cheat code pressed");

            //using the cheat code does not build typing speed or accuracy:
            timeSinceLastCorrect += Time.deltaTime;

            //skip to next line directly
            ParseNextLine();
        }
        else if (upcomingLetter != null && Input.GetKeyDown(upcomingLetter.getLetter().ToString())) {
            //correct press:
            //updates typing speed
            float totalSeconds = typedLetters++ / speed;
            totalSeconds += timeSinceLastCorrect;
            timeSinceLastCorrect = 0.0f;
            speed = typedLetters / totalSeconds;
            //updates typing accuracy
            accuracy = (float)(typedLetters) / (float)++totalTypedLetters;

            //update to suit next input
            UpdateDisplay(upcomingLetter.getPosInDisplay());
            upcomingLetter = upcomingLetter.getNext();

            //broadcast event
            EventManager.instance.RaiseCorrectKeyPressed();

            //update graphics
            if(upcomingLetter != null)
            {
                //there is a next letter
            }
            else
            {
                //there is no next letter (end of line reached)
                //then load next line
                ParseNextLine();
            }
        }
        else
        {
            //incorrect
            if (Input.anyKeyDown)
            {
                totalTypedLetters++;
                accuracy = (float)typedLetters / (float)totalTypedLetters;
                EventManager.instance.RaiseIncorrectKeyPressed();
            }

            //incorrect OR no input
            timeSinceLastCorrect += Time.deltaTime;
        }
    }

    void ParseNextLine()
    {
        universalLineNum++;

        //see if the reader completed all the scripts
        if (universalLineNum >= scriptLines.Length)
        {
            Debug.Log("You reached the end!");
            TriggerEnding();
            return;
        }


        //the current line to be parsed
        string line = scriptLines[universalLineNum];

        char[] rawArr = line.ToLower().ToCharArray();//lower is required by input key conventions
        //parse tag, tags are lines that begin with [
        if (rawArr[0] == '[')
        {
            upcomingLetter = null;
            section = line.Substring(line.IndexOf('[')+1, line.Length-3);
            sectionLineNum = 0; //set to section beginning
            Debug.Log("you are proceeding to a new section called "+section+"!");

            ParseNextLine();
            return;
        }

        //set up linked list for input
        ToBeTyped head = null;
        ToBeTyped curr = null;

        //head -> temp1 -> ... -> curr, all the way to the last letter of the sentence
        for(int i = 0; i < rawArr.Length; i++)
        {
            if (isLetter(rawArr[i]))
            {
                if(head == null)
                {
                    head = new ToBeTyped(i, rawArr[i].ToString());
                    curr = head;
                }
                else
                {
                    ToBeTyped temp = new ToBeTyped(i, rawArr[i].ToString());
                    curr.setNext(temp);
                    curr = temp;
                }
            }
        }

        if(head == null)
        {
            Debug.Log("somehow there is no single letter in this line, proceeding to the next");
            upcomingLetter = null;
            ParseNextLine();
        }
        else
        {
            Debug.Log("this new line starts with " + head.getLetter());
            upcomingLetter = head;
            rawString = scriptLines[universalLineNum];
            UpdateDisplay(-1);
        }
    }

    void UpdateDisplay(int i)
    {
        //i is the next letter, so whatever is before i will be solid, 
        //and whatever is after i will be half transparent
        displayString = "<b>" + rawString.Substring(0, i+1) + "</b>" + rawString.Substring(i+1);

        //Debug.Log(i+": "+ displayString);
        mesh.text = displayString;
    }

    //return if the given char is a upper or lower case letter in the latin alphabet
    bool isLetter(char i)
    {
        int iVal = (int)i;
        if( (iVal >= 'A' && iVal <= 'Z')
            || (iVal >= 'a' && iVal <= 'z'))
        {
            return true;
        }
        return false;
    }

    void TriggerEnding()
    {
        Debug.Log("You beat the game!");
    }

    // Update is called once per frame
    void Update()
    {
        KeyInput();
    }

    private class ToBeTyped
    {
        int posInDisplay;
        ToBeTyped next;
        string letter;

        public ToBeTyped (int posInDisplay, string letter)
        {
            this.posInDisplay = posInDisplay;
            this.letter = letter;
        }

        public int getPosInDisplay()
        {
            return posInDisplay;
        }

        public void setNext(ToBeTyped next)
        {
            this.next = next;
        }

        public ToBeTyped getNext()
        {
            return next;
        }

        public string getLetter()
        {
            return letter;
        }
    }
}