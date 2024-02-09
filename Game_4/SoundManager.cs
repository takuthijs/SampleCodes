using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("BGM")]
    public AudioSource[] BGM;
    [Header("Sound Effects")]
    public AudioSource[] SE;
    [Header("Material Sound")]
    public AudioSource[] MS;

    public void PlayBGM(int soundToPlay)
    {
        if (soundToPlay < BGM.Length)
        {
            BGM[soundToPlay].Play();
        }
    }

    public void PlaySE(int soundToPlay)
    {
        if (soundToPlay < SE.Length)
        {
            SE[soundToPlay].Play();
        }
    }

    public void PlayMaterialSound(int soundToPlay)
    {
        if (soundToPlay < MS.Length)
        {
            MS[soundToPlay].Play();
        }
    }
}
