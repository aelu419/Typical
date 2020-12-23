using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextHolderBehavior : MonoBehaviour
{
    [ReadOnly] public Vector3 size;
    [ReadOnly] public Rect rect;
    [ReadOnly] public Word content;
    private TextMeshPro tmp;
    private Transform cover;
    private SpriteRenderer cover_renderer;

    // Start is called before the first frame update
    void Start()
    {
        tmp = GetComponent<TextMeshPro>();
        rect = (transform as RectTransform).rect;

        size = new Vector3(rect.width, rect.height, 1);

        //Debug.Log("Instantiated " + tmp.text);
        cover = transform.GetChild(0);
        cover_renderer = cover.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        cover_renderer.sprite = content.cover;

        cover.position = new Vector3(
            (content.R.x + content.L.x) / 2f,
            content.top + cover_renderer.size.y / 2f,
            0
            );
    }
}
