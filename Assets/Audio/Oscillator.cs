using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillator : MonoBehaviour
{
    public float volume = 0.1f;

    public ArrayList playing; //the current notes that are being played
    public LinkedList<Note> to_play; //notes that will be played in the future

    public int ID;

    private void Start()
    {
        playing = new ArrayList(32);
        to_play = new LinkedList<Note>();

        MusicManager.Instance.RegisterOscillator(this);
    }

    public void PendNote(Note n)
    {
        if (to_play.Count == 0)
        {
            to_play.AddFirst(n);
        }
        else
        {
            LinkedListNode<Note> start = to_play.First;
            while (true)
            {
                if (start == null)
                {
                    to_play.AddLast(n);
                    break;
                }
                if (start.Value.entrance_beat >= n.entrance_beat)
                {
                    to_play.AddBefore(start, n);
                    break;
                }
                start = start.Next;
            }
        }
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        //sample from currently playing notes
        float[] sampler = new float[data.Length / channels];
        foreach(Note n in playing)
        {
            float[] read = n.ReadData(sampler.Length);
            for(int i = 0; i < sampler.Length; i++)
            {
                sampler[i] += read[i];
            }
        }

        //transfer sampler into audio filter data
        for(int i = 0; i < data.Length; i += channels)
        {
            data[i] = volume * sampler[i/channels];
            if (channels == 2)
            {
                data[i + 1] = data[i];
            }
        }

        //transfer upcoming note to playing
        LinkedListNode<Note> upcoming = to_play.First;
        while (upcoming != null)
        {
            if (upcoming.Value.entrance_beat <= MusicManager.Instance.beat)
            {
                Debug.Log("now playing: " + upcoming.Value.type);
                //load note into playing
                playing.Add(upcoming.Value);
                to_play.Remove(upcoming);
                break;
            }
            upcoming = upcoming.Next;
        }

        //trim notes list
        ArrayList p = new ArrayList(32);
        foreach (Note n in playing)
        {
            if (!n.done)
            {
                p.Add(n);
            }
        }
        
        playing = p;
    }
}
