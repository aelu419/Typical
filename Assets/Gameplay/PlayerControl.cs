using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float charSize; //the height of the main character, in world units
    private SpriteRenderer renderer_; //the sprite renderer assigned to the main character
    [ReadOnly] public Rect collider_bounds;

    private Rigidbody2D rigid;
    private Animator animator;

    private List<GameObject> word_blocks_in_contact;

    //player state machine related 
    private bool in_climb;
    public float climb_speed;
    public float accel;
    public float x_vel_max;
    [ReadOnly] public bool light_toggle;

    [ReadOnly] public Vector3 destination;
    [ReadOnly] public Vector3 destination_override;
    [ReadOnly] public Vector3 relation_to_destination; //negative or positive; 
                                                       //sign change means the player has either arrived or rushed pass the destination
    private float climb_extent; //the initial height difference when initiating a climb

    //connect to other game components
    private VisualManager vManager;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.instance.OnCorrectKeyPressed += CorrectKeyPressed;
        EventManager.instance.OnIncorrectKeyPressed += IncorrectKeyPressed;

        transform.localScale = new Vector3(charSize, charSize, charSize);
        transform.position = new Vector3(1, 5, 0);

        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        vManager = GameObject.FindGameObjectWithTag("General Manager").GetComponent<VisualManager>();

        word_blocks_in_contact = new List<GameObject>();

        destination = new Vector3(50, 0, 0);
        in_climb = false;
        light_toggle = false;
        UpdateRelativePosition();

        BoxCollider2D box = GetComponent<BoxCollider2D>();
        collider_bounds = new Rect(
            box.bounds.min,
            box.bounds.size
            );

        destination_override = new Vector3(-1, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        return;

        BoxCollider2D box = GetComponent<BoxCollider2D>();
        collider_bounds = new Rect(
            box.bounds.min,
            box.bounds.size
            );

        //freeze the character if it is not inside camera range
        if (transform.position.x < vManager.CAM.xMin
            || transform.position.x > vManager.CAM.xMax)
        {
            //Debug.Log("outside camera scope");
            rigid.velocity = Vector2.zero;
            return;
        }

        Vector3 relation_temp = new Vector3(
            relation_to_destination.x,
            relation_to_destination.y,
            relation_to_destination.z);

        UpdateRelativePosition();

        if (!in_climb)
        {
            float x_vel = rigid.velocity.x;

            //stopping distance under constant acceleration
            float stopping_distance_x = rigid.velocity.x * rigid.velocity.x / 2 / accel;

            //change of destination by external scripts
            bool new_order = destination_override.x >= 0;
            if (new_order)
            {
                destination = destination_override;
                destination_override = new Vector3(-1, 0, 0);
            }

            //watch for sign change on each axis, or approximate same-ness
            if (!new_order
                && (Mathf.Sign(relation_to_destination.x) != Mathf.Sign(relation_temp.x)
                || Mathf.Approximately(relation_to_destination.x, 0f)))
            {
                //Debug.Log("reached destination");
                //halt and go back to destination if necessary
                x_vel = 0;
                transform.position = new Vector3(
                    destination.x,
                    transform.position.y,
                    transform.position.z
                    );
            }
            //decelerate when within stopping distance (according to current velocity)
            else if (Mathf.Abs(destination.x - transform.position.x) <= stopping_distance_x)
            {
                //Debug.Log("decelerating");
                float original_sign = Mathf.Sign(x_vel);
                x_vel -= Mathf.Sign(relation_to_destination.x) * -1 * accel * Time.deltaTime;
                //prevent over-decelerating
                if(original_sign != Mathf.Sign(x_vel))
                {
                    x_vel = 0;
                }
            }
            //accelerate before reaching stopping distance
            else
            {
                //Debug.Log("accelerating");
                x_vel += Mathf.Sign(relation_to_destination.x) * -1 * accel * Time.deltaTime;

                //clamp to maximum velocity
                x_vel = Mathf.Min(Mathf.Abs(x_vel), x_vel_max) * Mathf.Sign(x_vel);
            }

            rigid.velocity = new Vector2(x_vel, rigid.velocity.y);

            float max_height = transform.position.y - charSize / 2f;
            bool will_climb = false;
            for (int i = 0; i < word_blocks_in_contact.Count; i++)
            {
                Word curr_word = word_blocks_in_contact[i].GetComponent<TextHolderBehavior>().content;
                //Debug.Log(curr_word.content + " at height " + curr_word.top);

                if (curr_word.top > max_height)
                {
                    will_climb = true;
                }
            }

            //Debug.Log("maximum height at: " + max_height + (will_climb?" will climb":" will not climb"));
            if (will_climb)
            {
                in_climb = true;
                //the 0.1 is to prevent collision detection during overlap
                destination.y = max_height + charSize / 2f + 0.1f;
                climb_extent = destination.y - transform.position.y;
            } else
            {
                destination.y = transform.position.y;
            }
        }
        else
        {
            //while climbing:
            //set horizontal velocity to 0
            rigid.velocity = new Vector2(0, Mathf.Sign(relation_to_destination.y) * -1 * climb_speed);

            //stop climbing when destination is reached
            if (Mathf.Sign(relation_temp.y) != Mathf.Sign(relation_to_destination.y)
                || Mathf.Approximately(relation_to_destination.y, destination.y)) {

                in_climb = false;
                climb_extent = 0;
                rigid.velocity = new Vector2(0, 0);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            light_toggle = true;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            light_toggle = false;
        }

        //TODO: set light_toggle property; setup different animators
        animator.SetFloat("speed", Mathf.Abs(rigid.velocity.x));
        animator.SetBool("in_climb", in_climb);
        animator.SetFloat("climb_extent", climb_extent);

        /*
        //stash destination state
        destination_temp = new Vector3(
            destination.x,
            destination.y,
            destination.z);*/
    }

    private void UpdateRelativePosition() {
        //update relative position
        relation_to_destination = transform.position - destination;
        relation_to_destination.x = Mathf.Approximately(relation_to_destination.x, 0) ? 0 : relation_to_destination.x;
        relation_to_destination.y = Mathf.Approximately(relation_to_destination.y, 0) ? 0 : relation_to_destination.y;
        relation_to_destination.z = Mathf.Approximately(relation_to_destination.z, 0) ? 0 : relation_to_destination.z;
    }

    //handle collisions
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Word Block"))
        {
            //Debug.Log(collision.gameObject.GetComponent<TextHolderBehavior>().content.content);
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Cover Object"))
        {
            Debug.Log("coming into contact with object");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Cover Object"))
        {
            Debug.Log("exiting contact with object");
        }
    }

    private void CorrectKeyPressed()
    {
        //Debug.Log("correct!");
    }
    private void IncorrectKeyPressed()
    {
        //Debug.Log("incorrect!");
    }
}
