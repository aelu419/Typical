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

    // Start is called before the first frame update
    void Start()
    {
        tmp = GetComponent<TextMeshPro>();
        rect = (transform as RectTransform).rect;

        size = new Vector3(rect.width, rect.height, 1);

        //Debug.Log("Instantiated " + tmp.text);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
