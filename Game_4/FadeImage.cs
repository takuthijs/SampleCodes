using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FadeImage : MonoBehaviour
{
    public CanvasGroup canvasGroup;

    void Start()
    {
        canvasGroup.alpha = 1;

        //1秒かけてalphaを1にする
        canvasGroup.DOFade(0, 1f);
    }
}
