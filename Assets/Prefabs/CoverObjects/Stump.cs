using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Stump : MonoBehaviour
{
    //BoxCollider2D box;

    float fall_timer;
    bool engaged;
    int index; //the typed out portion of the word

    TextMeshPro content;
    string orig_text;

    private static string[] SOURCE = { "STOP", "HALT", "NO PASS", "WRONG WAY", "TURN BACK", "STOP", "STOP" };

    public static string GetInitialText()
    {
        return SOURCE[Mathf.FloorToInt(Random.value * SOURCE.Length)];
    }

    // Start is called before the first frame update
    void Start()
    {
        //box = GetComponent<BoxCollider2D>();
        fall_timer = -1.0f;
        engaged = false;
        content = GetComponent<TextMeshPro>();
        orig_text = content.text;
    }

    // Update is called once per frame
    void Update()
    {
        Rect cam = CameraControler.Instance.CAM;
        if (cam.xMin < transform.position.x
            && transform.position.x < cam.xMax)
        {
            if (fall_timer == -1.0f)
            {
                // initialize tmp appearance
                content.text = "<material=\"" + Word.UNTYPED_PLAIN_MAT + "\">" + orig_text + "</material>";

                // initialize position
                transform.localPosition = new Vector3(
                    transform.localPosition.x,
                    Random.value * 10 + cam.height/2 + 5,
                    0
                    );
                fall_timer = 0.0f;
            }
            else
            {
                fall_timer += Time.deltaTime;
            }
        }

        if (engaged)
        {
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                index = Mathf.Min(orig_text.Length, index + 1);
                content.text = "<material=\"" + Word.UNTYPED_PLAIN_MAT + "\">"
                    + orig_text.Substring(0, orig_text.Length - index) + "</material>" +
                    "<material=\"" + Word.TYPED_MAT + "\">"
                    + orig_text.Substring(orig_text.Length - index) + "</material>";
                if (index == orig_text.Length)
                {
                    tag = "Word Block";
                    InputGate.Instance.UnregisterBackspaceBlocker(gameObject);
                    GameObject.FindGameObjectWithTag("Player").
                        GetComponent<PlayerControl>().OverrideCollisionType(gameObject);
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject other = collision.gameObject;
        if (other.CompareTag("Player"))
        {
            engaged = true;
            InputGate.Instance.RegisterBackspaceBlocker(gameObject);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        GameObject other = collision.gameObject;
        if (other.CompareTag("Player"))
        {
            engaged = false;
            InputGate.Instance.UnregisterBackspaceBlocker(gameObject);
        }
    }
}
