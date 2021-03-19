using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[ExecuteAlways]
public class NPCBehaviour : MonoBehaviour
{

    public Transform bubble, bubble_head, bubble_tail, bubble_text;
    SpriteRenderer sprite;
    public bool engaged;

    public Vector2 margin;
    public float normalized_bubble_x;
    public float bubble_y;

    public float float_speed, float_magnitude;

    string[] script;
    int index;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();

        //engaged = true;
        if (Application.isPlaying)
        {
            Disengage();
        }
        else
        {
            Engage();
        }
    }

    // Update is called once per frame
    void Update()
    {

        bubble.localPosition = new Vector3(
            0, 
            (Mathf.Sin(Time.time * float_speed) + 1 ) / 2 * float_magnitude,
            0);

        if (engaged)
        {
            Vector2 right_up = sprite.bounds.size;
            SpriteRenderer tail_sprite = bubble_tail.GetComponent<SpriteRenderer>();

            bubble_tail.localPosition = (Vector3)(right_up / 2)
                + tail_sprite.bounds.size / 2;
            
            Vector3 sbubble = new Vector3(
                sprite.bounds.size.x * normalized_bubble_x,
                bubble_y,
                0
                );

            RectTransform text = bubble_text as RectTransform;
            text.localPosition = new Vector3(
                0,
                right_up.y / 2 + tail_sprite.bounds.size.y + margin.y + sbubble.y / 2,
                0
                );
            
            text.sizeDelta = sbubble;

            sbubble.x += margin.x * 2;
            sbubble.y += margin.y * 2;

            SpriteRenderer head_sprite = bubble_head.GetComponent<SpriteRenderer>();
            head_sprite.size = sbubble;

            bubble_head.localPosition = new Vector3(
                0,
                right_up.y / 2 + tail_sprite.bounds.size.y + sbubble.y / 2,
                0);

            if (Input.GetKeyDown(KeyCode.Return))
            {
                PlayerControl.Instance.SendMessage("OnTalkToNPC");
                NextLine();
            }
        }   
    }

    //initialize the npc behaviour based on which npc it is associated to
    public void Initialize(string identifier)
    {
        foreach (NPCScriptable n in ScriptableObjectManager.Instance.NPCManager.npcs)
        {
            if (n.name_.Equals(identifier))
            {
                script = n.script.text.Split('\n');
                index = -1;
                NextLine();
            }
        }
    }

    private void NextLine()
    {
        index++;
        if (index < script.Length)
        {
            bubble_text.GetComponent<TextMeshPro>().text = script[index] + "\n[Enter]";
        }
        else
        {
            bubble_text.GetComponent<TextMeshPro>().text = "...";
        }
    }

    public void Engage()
    {
        PlayerControl.Instance.SendMessage("OnReachNPC");
        bubble.gameObject.SetActive(true);
        engaged = true;
    }

    public void Disengage()
    {
        //Debug.LogError("Implemenet Disengage");
        bubble.gameObject.SetActive(false);
        engaged = false;
    }
}
