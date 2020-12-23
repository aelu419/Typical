using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float charSize; //the height of the main character, in world units
    private SpriteRenderer renderer; //the sprite renderer assigned to the main character

    private Rigidbody2D rigid;

    private List<GameObject> word_blocks_in_contact;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.instance.OnCorrectKeyPressed += CorrectKeyPressed;
        EventManager.instance.OnIncorrectKeyPressed += IncorrectKeyPressed;

        transform.localScale = new Vector3(charSize, charSize, charSize);
        transform.position = new Vector3(1, 5, 0);

        rigid = GetComponent<Rigidbody2D>();
        if (rigid == null) throw new System.Exception("no rigid body in player");

        word_blocks_in_contact = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        rigid.velocity = new Vector2(2f, rigid.velocity.y);

        float max_height = transform.position.y - charSize / 2f;
        bool will_climb = false;
        for(int i = 0; i < word_blocks_in_contact.Count; i++)
        {
            Word curr_word = word_blocks_in_contact[i].GetComponent<TextHolderBehavior>().content;
            Debug.Log(curr_word.content + " at height " + curr_word.top);

            if(curr_word.top > max_height)
            {
                will_climb = true;
            }
        }

        Debug.Log("maximum height at: " + max_height);
        if (will_climb)
        {
            transform.position = new Vector3(
                transform.position.x,
                max_height + charSize / 2f + 0.1f,
                transform.position.z);
        }
    }

    //handle collisions
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Word Block"))
        {
            word_blocks_in_contact.Add(collision.gameObject);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Word Block"))
        {
            word_blocks_in_contact.Remove(collision.gameObject);
        }
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
