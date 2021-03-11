using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Stump : MonoBehaviour
{
    Collider2D box;
    // Start is called before the first frame update
    void Start()
    {
        box = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = new Vector3(0, box.bounds.size.y, 0);
    }
}
