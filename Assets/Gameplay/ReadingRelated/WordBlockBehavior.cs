using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WordBlockBehavior : MonoBehaviour
{
    [ReadOnly] public Word content;

    private bool collider_width_sync;
    //[ReadOnly] public Cover cover;

    // Start is called before the first frame update
    void Start()
    {
        collider_width_sync = false;
    }

    // Update is called once per frame
    void Update()
    {
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
        }

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
