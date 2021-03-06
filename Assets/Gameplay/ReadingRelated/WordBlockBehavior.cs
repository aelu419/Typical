using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WordBlockBehavior : MonoBehaviour
{
    [ReadOnly] public Word content;
    [ReadOnly] public string word_status;

    private bool collider_width_sync;
    [ReadOnly]
    public float light_intensity;
    //[ReadOnly] public Cover cover;

    [ReadOnly]
    //list of font materials currently rendered on this word block
    public Material[] mats;//, mats_;
    [ReadOnly]
    public int typed;

    private PlayerControl player;

    //private event System.Action engage, disengage;
    //private bool engaged;

    // Start is called before the first frame update
    void Start()
    {
        light_intensity = -1;
        collider_width_sync = false;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();

        //EventManager.Instance.OnCorrectKeyPressed += ListenForEngageChange;

        //engaged = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (content != null)
        {
            word_status = content.ToString();
            /*if (content.is_npc)
            {
                engage += GetComponentInChildren<NPCBehaviour>().Engage;
                disengage += GetComponentInChildren<NPCBehaviour>().Disengage;
            }*/


            //detect light
            float x_diff = ((content.L + content.R) / 2).x - player.transform.position.x;
            float l_range = player.head_light_controller.current_setting.w;
            if (player.head_light_controller.light_)
            {
                //player facing right
                if (player.direction)
                {
                    light_intensity = x_diff >= 0 && x_diff < l_range ?
                        GetLightIntensity(x_diff, l_range)
                        : -1;
                }
                else
                {
                    light_intensity = x_diff <= 0 && x_diff * -1 < l_range ?
                        GetLightIntensity(x_diff * -1, l_range)
                        : -1;
                }
            }
            else
            {
                light_intensity = -1;
            }

            if (content.word_mech == Word.WORD_TYPES.hidden)
            {
                mats = content.tmp.fontMaterials;
                foreach (Material m in mats)
                {
                    Color temp = m.GetColor("_FaceColor");
                    //Debug.Log(m.name);
                    //Debug.Log("vs:  " + Word.UNTYPED_HIDDEN_MAT_.name);
                    if (m.name.Equals(Word.UNTYPED_HIDDEN_MAT_.name + " (Instance)"))
                    {
                        //empty
                        if (content.typed == 0)
                        {
                            m.SetColor(
                                "_FaceColor", new Color(
                                    temp.r, temp.g, temp.b,
                                    light_intensity == -1 ? 0 : light_intensity
                                    )
                                );
                        }
                        //partially typed out
                        else
                        {
                            m.SetColor(
                                "_FaceColor", new Color(
                                    temp.r, temp.g, temp.b,
                                    0.5f
                                    )
                                );
                        }
                    }
                    else
                    {
                        m.SetColor(
                            "_FaceColor", new Color(
                                    temp.r, temp.g, temp.b,
                                    1
                                )
                            );
                    }

                }
                content.tmp.fontMaterials = mats;
            }
        }
        /*
        //has cover obj
        if (!collider_width_sync && transform.childCount != 0)
        {
            BoxCollider2D b2 = transform.GetChild(0).GetComponent<BoxCollider2D>();
            if(b2 != null)
            {
                BoxCollider2D b1 = GetComponent<BoxCollider2D>();
                b1.size = new Vector2(
                    b2.size.x,
                    b1.size.y
                   );
                b1.offset = new Vector2(
                    b1.size.x/2f,
                    b1.offset.y
                    );

                collider_width_sync = true;
            }
        }*/


        /*
        //generate cover object
        if(content.cover_type != null 
            && !content.cover_type.Equals("") 
            && cover == null)
        {
            cover = new Cover(transform.GetChild(0).gameObject, content);
        }*/
    }

    /*
    private void ListenForEngageChange()
    {
        Debug.Log("engage change");
        //in the middle of typing the current word
        if (content.has_typable && content.typed >= 1 && content.typed <= content.content.Length)
        {
            if (!engaged)
            {
                Debug.Log("word " + content + " is engaged");
                engaged = true;
                if (engage != null) engage();
            }
        } else
        {
            if (engaged)
            {
                Debug.Log("word " + content + " is disengaged");
                engaged = false;
                if (disengage != null) disengage();
            }
        }
    }*/

    private float GetLightIntensity(float dist, float light_range)
    {
        float t = dist / light_range;
        return 0.5f - 0.5f * t;
    }

    /*
    public class Cover
    {
        private string type;
        private GameObject object_;
        private SpriteRenderer renderer_;
        private BoxCollider2D collider_;
        private Word reference;

        public Cover(GameObject obj, Word reference)
        {
            this.object_ = obj;
            this.reference = reference;
            this.type = reference.cover_type;

            //set object parameters
            object_.tag = "Cover Object";

            //set sprite renderer
            renderer_ = obj.GetComponent<SpriteRenderer>();
            renderer_.sprite = reference.cover_sprite;

            //set behavior of the gameobject

            
        }

    }*/
}
