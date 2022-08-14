using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sounds : MonoBehaviour
{
    public List<AudioClip> clips;
    private AudioSource Player;
    private static Sounds IN;

    void Start()
    {
        Player = GetComponent<AudioSource>();
        IN = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal static void Play(string v)
    {
        
        AudioClip clip=null;
        IN.clips.ForEach((c) =>
        {
            if (c.name.Contains(v))
                clip = c;
        });
        if (clip)
        {
            //IN.Player.Stop();
            IN.Player.clip = clip;
            
            IN.Player.Play();
        }
    }
}
