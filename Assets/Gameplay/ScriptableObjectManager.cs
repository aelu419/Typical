using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableObjectManager : MonoBehaviour
{
    public ScriptDispenser ScriptManager;
    public CoverDispenser CoverManager;

    private static ScriptableObjectManager _instance;
    public static ScriptableObjectManager Instance => _instance;

    private void Awake()
    {
        _instance = this;
    }
}
