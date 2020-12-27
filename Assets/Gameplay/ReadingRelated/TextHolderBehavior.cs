using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextHolderBehavior : MonoBehaviour
{
    //[ReadOnly] public Vector3 size;
    //[ReadOnly] public Rect rect;
    [ReadOnly] public Word content;
    //private TextMeshPro tmp;
    private Transform cover;
    private SpriteRenderer cover_renderer;

    //private bool collider_updated;
    private BoxCollider2D cover_collider;

    // Start is called before the first frame update
    void Start()
    {
        //tmp = GetComponent<TextMeshPro>();
        //rect = (transform as RectTransform).rect;

        //size = new Vector3(rect.width, rect.height, 1);

        //Debug.Log("Instantiated " + tmp.text);
        cover = transform.GetChild(0);
        cover_renderer = cover.GetComponent<SpriteRenderer>();

        cover_collider = null;

        //collider_updated = false;
    }

    // Update is called once per frame
    void Update()
    {
        //generate cover object
        if(content.cover != null 
            && cover_collider == null)
        {
            cover_renderer.sprite = content.cover;

            cover.gameObject.AddComponent<BoxCollider2D>();
            cover_collider = cover.gameObject.GetComponent<BoxCollider2D>();
            cover.gameObject.tag = "Cover Object";
            cover_collider.isTrigger = true;

            cover.position = new Vector3(
                (content.R.x + content.L.x) / 2f + cover_renderer.size.x / 2f,
                content.top + cover_renderer.size.y / 2f,
                0
                );
        }

        /*
        //update boundaries after rendering, which should shrink by a little
        if(!collider_updated)
        {
            Vector2 rendered_sizes = tmp.GetRenderedValues(false);
            //Debug.Log(rendered_sizes);
            if(rendered_sizes.x <= 0)
            {
                return;
            }

            Vector2 preferred_sizes = tmp.GetPreferredValues();
            content.top = content.L.y + rendered_sizes.y / 2f;

            BoxCollider2D col = gameObject.GetComponent<BoxCollider2D>();
            col.offset = new Vector2(
                rendered_sizes.x / 2f,
                (preferred_sizes.y - rendered_sizes.y)/2f);
            col.size = rendered_sizes;

            collider_updated = true;

        }*/
    }
}
