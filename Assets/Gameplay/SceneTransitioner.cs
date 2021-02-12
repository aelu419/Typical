using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransitioner : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Animator>().SetBool("exitTrigger", false);
        EventManager.Instance.OnStartExitingScene += OnStartExitingScene;
    }

    private void OnStartExitingScene()
    {
        Debug.Log("Exit scene");
        GetComponent<Animator>().SetBool("exitTrigger", true);
        StartCoroutine(BroadCastExitFinished());
    }

    IEnumerator BroadCastExitFinished()
    {
        yield return new WaitForSeconds(1f);
        Debug.Log("scene exit complete");
        EventManager.Instance.StartEnteringScene();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
