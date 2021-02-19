using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneShot : MonoBehaviour
{
    public List<float> pending;

    private void OnEnable()
    {
        pending = new List<float>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (pending == null)
        {
            return;
        }

        foreach(float b in pending)
        {
            if (b <= MusicManager.Instance.beat)
            {
                if (MusicManager.Instance.beat - b >= 1)
                {
                    PlayNote();
                }
                else
                {
                    Debug.LogError("note expired");
                }
                pending.Remove(b);
            }
        } 
    }

    public void PlayNote() { }
}
