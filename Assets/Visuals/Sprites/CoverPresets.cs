using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//settings for all the objects that may appear above text blocks
public class CoverPresets
{
    public enum COVER_TYPES
    {
        lamp
    }

    public static Dictionary<string, string> FetchCoverSpecs(COVER_TYPES type)
    {
        Dictionary<string, string> specs = new Dictionary<string, string>();

        //TODO: parse json here

        return specs;
    }
}
