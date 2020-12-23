using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float charSize; //the height of the main character, in world units
    private SpriteRenderer renderer; //the sprite renderer assigned to the main character

    private Rigidbody2D rigid;

    // Start is called before the first frame update
    void Start()
    {

        EventManager.instance.OnCorrectKeyPressed += CorrectKeyPressed;
        EventManager.instance.OnIncorrectKeyPressed += IncorrectKeyPressed;

        transform.localScale = new Vector3(charSize, charSize, charSize);
        transform.position = new Vector3(1, 5, 0);

        rigid = GetComponent<Rigidbody2D>();
        if (rigid == null) throw new System.Exception("no rigid body in player");

    }

    // Update is called once per frame
    void Update()
    {
        rigid.velocity = new Vector2(2f, rigid.velocity.y);
    }

    //handle collisions
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Word Block"))
        {
            Debug.Log("colliding with" + collision.gameObject.name);
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
