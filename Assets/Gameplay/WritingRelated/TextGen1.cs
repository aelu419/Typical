using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Typical Customs/Story Writer/TextGen1")]
public class TextGen1 : Writer
{
    public override string Output()
    {
        Markov m = new Markov(input, 4);
        string str = "";
        for (int i = 0; i < 10; i++)
        {
            try
            {
                str += m.Run(10) + ' ';
                
            }catch(System.Exception _)
            {

            }
        }
        //add scoring, selecting, and concatenating
        return str;
    }
}
