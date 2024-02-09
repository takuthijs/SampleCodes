using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainObjectsLoader : MonoBehaviour
{
    [SerializeField] private GameObject MainObjects;
    void Awake()
    {
        GameObject mainObjects = GameObject.FindWithTag("MainObjects");

        if (mainObjects == null)
        {
            DontDestroyOnLoad(Instantiate(MainObjects));
        }
        else
        {
            DontDestroyOnLoad(mainObjects);
        }
    }
}
