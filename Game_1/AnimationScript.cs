using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnimationScript : MonoBehaviour
{
    public bool flashingAinme;
    public bool flashingAinmeSlow;
    public bool flashingTextAinme;
    private float val = -1f;
    private Image spriteRenderer;
    private TextMeshProUGUI text;

    void Start()
    {
        spriteRenderer = GetComponent<Image>();
        text = GetComponent<TextMeshProUGUI>();
    }

    private void FixedUpdate()
    {
        if (flashingAinme)
        {
            if (spriteRenderer.color.a <= 0.04f)
            {
                val = 1;
            }
            else if (spriteRenderer.color.a >= 0.94f)
            {
                val = -1;
            }
            spriteRenderer.color += new Color(0, 0, 0, 0.05f * val);
        }

        if (flashingTextAinme)
        {
            if (text.color.a <= 0.04f)
            {
                val = 1;
            }
            else if (text.color.a >= 0.94f)
            {
                val = -1;
            }
            text.color += new Color(0, 0, 0, 0.02f * val);
        }

        if (flashingAinmeSlow)
        {
            if (spriteRenderer.color.a <= 0.04f)
            {
                val = 1;
            }
            else if (spriteRenderer.color.a >= 0.94f)
            {
                val = -1;
            }
            spriteRenderer.color += new Color(0, 0, 0, 0.02f * val);
        }
    }
}
