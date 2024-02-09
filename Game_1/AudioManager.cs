using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [HideInInspector] public static AudioManager instance;

    [Header("Sound Effects")]
    public AudioSource[] SE;

    // Use this for initialization
    void Start()
    {
        instance = this;
    }

    public void PlaySE(int soundToPlay)
    {
        if (soundToPlay < SE.Length)
        {
            SE[soundToPlay].Play();
        }
    }
}
