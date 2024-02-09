using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagmaSwordScript : MonoBehaviour
{
    Material myMat;
    Color materialColor;
    // Start is called before the first frame update
    void Start()
    {
        myMat = GetComponent<Renderer>().material;
        materialColor = myMat.GetColor("_EmissionColor");
        myMat.EnableKeyword("_EMISSION");
    }

    // Update is called once per frame
    void Update()
    {
        float val = Mathf.PingPong(Time.time, 2.0F);
        Color color = new Color(materialColor.r * val, materialColor.g * val, materialColor.b * val);
        myMat.SetColor("_EmissionColor", color);
    }
}
