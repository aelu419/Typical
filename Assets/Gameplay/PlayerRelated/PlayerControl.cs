using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public static PlayerControl Instance
    {
        get { return _instance; }
    }

    private static PlayerControl _instance;

    public PlayerSFXLibrary sfx_lib;

    public float charSize; //the height of the main character, in world units
    [ReadOnly] public Rect collider_bounds;

    //player state machine related 
    public float climb_threshold; //the threshold for height difference, above it triggers climbing
    [ReadOnly] public bool in_climb;
    //private float climb_extent; //the initial height difference when initiating a climb

    private bool light_toggle;

    //movement related
    public float climb_speed, accel, x_vel_max;
    [ReadOnly] public Vector3 destination;
    [ReadOnly] public Vector3 destination_override;
    [ReadOnly] public Vector3 relation_to_destination; //negative or positive; 
                                                       //sign change means the player has either 
                                                       //arrived or rushed pass the destination
    public bool direction; //true when facing right
    //private Vector3 relation_temp;
    //private ContactPoint2D[] cp;

    //connect to other game components
    private CameraControler cControler;
    //private ReadingManager rManager;
    //private SpriteRenderer renderer_; //the sprite renderer assigned to the main character
    private Rigidbody2D rigid;

    private Animator animator;

    private BoxCollider2D box;

    [ReadOnly]
    public HeadLightControl head_light_controller;

    [ReadOnly] public List<GameObject> word_blocks_in_contact;
    //[ReadOnly] public string word_blocks_in_contact_str;

    //private float stuck_time = 0.0f; //to deal with really weird situations

    public float light_progress; //0 is shut off, 1 is up
    //private SpriteRenderer torso;

    private event System.Action on_first_frame;

    void Awake()
    {
        destination = new Vector3(-1, 0, 0);
        destination_override = new Vector3(-1, 0, 0);
        word_blocks_in_contact = new List<GameObject>();

        on_first_frame = null;


        _instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

        //register events
        EventManager.Instance.OnProgression += OnProgression;
        EventManager.Instance.OnRegression += OnRegression;

        //connect to rest of the game
        rigid = GetComponent<Rigidbody2D>();

        animator = GetComponent<Animator>();
        animator.SetBool("in_climb", false);
        //torso = transform.GetChild(1).GetComponent<SpriteRenderer>();
        head_light_controller = transform.GetChild(0).GetComponent<HeadLightControl>();

        cControler = CameraControler.Instance;

        box = GetComponent<BoxCollider2D>();

        //set character state
        in_climb = false;
        light_toggle = false;

        //set coordinate related fields
        transform.localScale = new Vector3(charSize, charSize, charSize);
        UpdateRelativePosition();
        /*relation_temp = new Vector3(
            relation_to_destination.x,
            relation_to_destination.y,
            relation_to_destination.z
            );*/
        
        collider_bounds = new Rect(
            box.bounds.min,
            box.bounds.size
            );

    }

    public void SpawnAtRoot(Vector2 spawn_root)
    {
        on_first_frame += () =>
        {
            transform.position = new Vector3(
               spawn_root.x,
               spawn_root.y + charSize / 2f,
               0
               );

            light_progress = 0;
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (on_first_frame != null)
        {
            on_first_frame();
            on_first_frame = null;
        }
        //bool accelerating = false;
        //float hor_spd_temp = rigid.velocity.x;

        //basic variables for the rest of the method
        collider_bounds = new Rect(
            box.bounds.min,
            box.bounds.size
            );
        //Debug.Log(collider_bounds.yMin + " to " + collider_bounds.yMax);

        /*
        relation_temp = new Vector3(
            relation_to_destination.x,
            relation_to_destination.y,
            relation_to_destination.z);*/

        UpdateRelativePosition();

        /*
        word_blocks_in_contact_str = "";
        for(int i = 0; i < word_blocks_in_contact.Count; i++)
        {
            word_blocks_in_contact_str += i + ": " + word_blocks_in_contact[i].content + " ";
        }*/

        //control the motion of the player:

        /* ---deprecated---
        //freeze the character if it is not inside camera range
        if (transform.position.x < cControler.CAM.xMin
            || transform.position.x > cControler.CAM.xMax)
        {
            //Debug.Log("outside camera scope");
            rigid.velocity = Vector2.zero;
            return;
        }
        */

        if (!in_climb)
        {
            rigid.gravityScale = 1.0f;
            if (!Approximately(relation_to_destination.x, 0))
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

                //decide first if should decelerate or accelerate
                bool should_decel =
                    //the player is going in the right direction
                    Mathf.Sign(rigid.velocity.x) != Mathf.Sign(relation_to_destination.x)
                    //and the destination is within stopping distance
                    && Mathf.Abs(relation_to_destination.x) <= stopping_distance_x;

                if (should_decel)
                {

                    //plain decel
                    //Debug.Log("decelerating");
                    float original_sign = Mathf.Sign(x_vel);
                    x_vel -= Mathf.Sign(relation_to_destination.x) * -1 * accel * Time.deltaTime;
                    //prevent over-decelerating
                    if (original_sign != Mathf.Sign(x_vel))
                    {
                        x_vel = 0;
                    }
                }
                else
                {
                    //accelerate accordingly
                    float dvdt = Mathf.Sign(relation_to_destination.x) * -1 * accel * Time.deltaTime;
                    //Debug.Log("accelerating " + dvdt);
                    //accelerating = true;
                    x_vel += dvdt;

                    //clamp to maximum velocity
                    x_vel = Mathf.Min(Mathf.Abs(x_vel), x_vel_max) * Mathf.Sign(x_vel);
                }

                rigid.velocity = new Vector2(x_vel, rigid.velocity.y);

                float yMax = transform.position.y - charSize / 2f;
                for(int i = 0; i < word_blocks_in_contact.Count; i++)
                {
                    //WordBlockBehavior block_content = word_blocks_in_contact[i].GetComponent<WordBlockBehavior>();
                    float block_top = word_blocks_in_contact[i].GetComponent<BoxCollider2D>().bounds.max.y;

                    float hdiff = block_top - yMax;
                    if (hdiff > 0.0f)
                    {
                        //teleport for small height gaps
                        if (hdiff < climb_threshold)
                        {
                            transform.position = new Vector3(
                                transform.position.x,
                                block_top + charSize / 2f + 0.1f,
                                transform.position.z
                            );
                        }
                        //climb for large height gaps
                        else
                        {
                            yMax = block_top + charSize / 2f + 0.1f;
                            in_climb = true;
                        }
                    }
                }

                if (in_climb)
                {
                    destination.y = yMax;
                    //climb_extent = yMax - transform.position.y;
                    animator.SetBool("in_climb", true);
                }
                else
                {
                    destination.y = transform.position.y;
                }
            }
        }
        else
        {
            //while climbing:
            rigid.gravityScale = 0.0f;
            //actual climbing
            if (animator.GetCurrentAnimatorStateInfo(1).IsName("Climb"))
            {
                //Debug.Log("climbing...");
                if (relation_to_destination.y <= 0)
                {
                    rigid.velocity = new Vector2(0, climb_speed);
                }
                else
                {
                    in_climb = false;
                    animator.SetBool("in_climb", false);
                    rigid.velocity = Vector2.zero;
                    //climb_extent = 0;
                }
            }
            else
            {
                rigid.velocity = Vector2.zero;
            }

            /*
            if (relation_to_destination.y < 0)
            {
                if (climb_buffer_ <= climb_buffer)
                {
                    rigid.velocity = new Vector2(0, 0);
                    climb_buffer_ += Time.deltaTime;
                }
                else
                {
                    rigid.velocity = new Vector2(0, climb_speed);
                }
            }
            else
            {
                //stop climbing when destination is reached
                rigid.velocity = new Vector2(0, 0);
                if (climb_buffer_ > 0)
                {
                    climb_buffer_ -= Time.deltaTime;
                }
                else
                {
                    in_climb = false;
                    climb_extent = 0;
                }
            }*/
        }

        //glitch jump when stuck
        /*
        if (accelerating && 
            (relation_temp.x == relation_to_destination.x 
            || Approximately(hor_spd_temp, 0)))
        {
            stuck_time += Time.deltaTime;
            if(stuck_time > 0.5f)
            {
                in_climb = true;
                destination.y = rigid.position.y + 0.1f;
                Debug.Log("glitch jumped");
                //TODO: do a special glitch jump animation :)
                //animator.SetBool("glitch_jump", true);

                //rigid.MovePosition(new Vector2(
                    //rigid.position.x,
                    //rigid.position.y + 0.1f));
            }
        }
        else
        {
            stuck_time = 0.0f;
        }*/

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Debug.Log("light on");
            light_toggle = true;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            //Debug.Log("light off");
            light_toggle = false;
        }

        animator.SetFloat("speed", Mathf.Abs(rigid.velocity.x));
        if (animator.GetBool("light_toggle") != light_toggle)
        {
            //light toggle status changed
            //play oneshot squeaking sound for helmet raising/lowering
            if (light_toggle)
            {
                FMODUnity.RuntimeManager.PlayOneShot(sfx_lib.helm_open, transform.position);
            }
            else
            {
                FMODUnity.RuntimeManager.PlayOneShot(sfx_lib.helm_close, transform.position);
            }
            
        }
        animator.SetBool("light_toggle", light_toggle);

        head_light_controller.light_ = light_toggle;
        head_light_controller.lerp_state = light_progress;

        transform.rotation = Quaternion.Euler(0, direction ? 0 : 180f, 0);
    }


    //update the stored relative position of the player to the cursor
    private void UpdateRelativePosition() {
        relation_to_destination = transform.position - destination;
        relation_to_destination.x = Approximately(relation_to_destination.x, 0) ? 0 : relation_to_destination.x;
        relation_to_destination.y = Approximately(relation_to_destination.y, 0) ? 0 : relation_to_destination.y;
        relation_to_destination.z = Approximately(relation_to_destination.z, 0) ? 0 : relation_to_destination.z;
    }

    private bool Approximately(float a, float b)
    {
        return Mathf.Approximately(a, b);
        //return Mathf.Abs(a - b) <= 0.05;
    }

    public void OnReachNPC()
    {
        FMODUnity.RuntimeManager.PlayOneShot(sfx_lib.npc_encounter, transform.position);
    }

    public void OnTalkToNPC()
    {
        FMODUnity.RuntimeManager.PlayOneShot(sfx_lib.npc_talk, transform.position);
    }

    //handle collisions
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.LogError("IMPLEMENT COLLISION ANIMATION, collision speed: " + collision.relativeVelocity.y);
        if (collision.relativeVelocity.y > 0)
        {
            StartCoroutine(CameraControler.Instance.Shake(collision.relativeVelocity.y, 0.1f));
        }

        if (collision.gameObject.CompareTag("Word Block"))
        {
            //Debug.Log(collision.gameObject.GetComponent<WordBlockBehavior>().content.content);
            word_blocks_in_contact.Add(collision.gameObject);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Word Block"))
        {
            word_blocks_in_contact.RemoveAll(
                (GameObject go) => go.Equals(collision.gameObject)
            );
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject other = collision.gameObject;
        if (other.CompareTag("Cover Object"))
        {
            Debug.Log("coming into contact with cover object");
            NPCBehaviour n = other.GetComponent<NPCBehaviour>();
            if (n != null)
            {
                n.SendMessage("Engage");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject other = collision.gameObject;
        if (other.CompareTag("Cover Object"))
        {
            Debug.Log("exiting contact with cover object");
            NPCBehaviour n = other.GetComponent<NPCBehaviour>();
            if (n != null)
            {
                n.SendMessage("Disengage");
            }
        }
    }

    public void OverrideCollisionType(GameObject go)
    {
        if (go.CompareTag("Word Block"))
        {
            word_blocks_in_contact.Add(go);
        }
    }

    private void OnProgression()
    {
        direction = true;
    }
    private void OnRegression()
    {
        direction = false;
    }
}
