using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Typical Customs/Story Writer/TextGen1")]
public class TextGen1 : Writer
{
    [TextArea]
    public string input;
    private void OnEnable()
    {
        Output();
    }
    public override string Output()
    {
        string sample_in = input.ToLower().Replace('\n', ' ');
        Markov m = new Markov(sample_in, 2);
        for (int i = 0; i < 10; i++)
        {
            Debug.Log(m.Run(10));
        }
        return "sample output";
    }
}
