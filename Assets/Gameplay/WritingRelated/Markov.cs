using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Markov
{
    static readonly List<char> ALPHABET = new List<char>("abcedfghijklmnopqrstuvwxyz,.!?;:() ");
    static readonly List<char> END_OF_LINE = new List<char>(".....!?");
    Dictionary<string, int[]> table;
    string[] corpus_words;
    int size;

    public Markov(string corpus, int size)
    {
        this.size = size;
        corpus = corpus.ToLower();
        System.Text.StringBuilder temp = new System.Text.StringBuilder(corpus.Length);
        for(int i = 0; i < corpus.Length; i++)
        {
            if (ALPHABET.Contains(corpus[i]))
                temp.Append(corpus[i]);
            else if (corpus[i].Equals('\n'))
                temp.Append(' ');
        }
        corpus = temp.ToString();

        corpus_words = corpus.Split(' ');

        string arr = "";
        for (int i = 0; i < corpus_words.Length; i++)
        {
            arr += (i == 0 ? "" : ", ")
                + "'" + corpus_words[i] + "'";
        }
        Debug.Log(arr);

        //generate table based on corpus
        table = new Dictionary<string, int[]>();

        for (int i = 0; i < corpus.Length - size - 1; i++)
        {
            string state = corpus.Substring(i, size);
            char next = corpus.Substring(i + size, 1).ToCharArray()[0];
            Debug.Log("state: " + state + "\t next: " + next + " = " + (int)next);

            if (table.ContainsKey(state))
            {
                int[] row = table[state];
                row[ALPHABET.IndexOf(next)]++;
            }
            else
            {
                int[] row = new int[ALPHABET.Count];
                for(int j = 0; j < ALPHABET.Count; j++)
                {
                    row[j] = 0;
                }
                row[ALPHABET.IndexOf(next)] = 1;
                table.Add(state, row);
            }
        }
    }

    //generate sentence of up to length words. the sentence will terminate upon certain puncuations
    public string Run(int length)
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder(length * 10);

        //pick first word from corpus
        string initial_state = "";
        while(initial_state.Length < size)
        {
            initial_state = corpus_words[Mathf.FloorToInt(Random.value * corpus_words.Length)];
        }
        string window = initial_state.Substring(0, size);

        //generate up to length words
        for (int i = 0; i < length; i++)
        {
            //generate new word
            char next;
            do
            {
                next = NextLetter(window);
                result.Append(next);
                window = window.Substring(1) + next;

                //terminate sentence if certain symbols are reached
                if (END_OF_LINE.Contains(next)) break;
            } while (next != ' ');
        }
        //if this is reached, then the sentence has not terminated by puncutation
        //punctuation will be manually added
        result.Remove(result.Length-1, 1);
        result.Append(END_OF_LINE[Mathf.FloorToInt(Random.value * END_OF_LINE.Count)]);

        return result.ToString();
    }

    private char NextLetter(string state)
    {
        if (table.ContainsKey(state))
        {
            int[] row = table[state];

            int[] tally = new int[ALPHABET.Count];
            int sum = 0;
            for(int i = 0; i < ALPHABET.Count; i++)
            {
                tally[i] = row[i] + i > 0 ? tally[i - 1] : 0;
                sum += row[i];
            }
            float index = Random.value * sum;
            for (int i = 0; i < ALPHABET.Count; i++)
            {
                if (index <= tally[i]) { return ALPHABET[i]; }
            }
        }
        else
        {
            //TODO: find nearest key and substitute
        }

        //not found: throw error
        Debug.LogError(state + " has no corresponding next state!");
        return '\0';
    }
}
