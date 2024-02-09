using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonAnimation : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        //ボタンのアニメーション処理を入れる
        transform.DOScale(Vector3.one * 0.9f, 0.1f)
            .OnComplete(() =>
            {
                // 0.2秒かけて元の倍率(1倍)に戻す
                transform.DOScale(1, 0.1f);
            });
    }
}
