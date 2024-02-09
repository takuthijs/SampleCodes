using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class MonsterPoopEventTrigger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        int nowgold = GameManager.instance.gold;
        GameManager.instance.gold += 100000;
        //指定したupdateNumberまでカウントアップ・カウントダウンする
        DOTween.To(() => nowgold, (n) => nowgold = n, nowgold + 100000, 1)
            .OnUpdate(() => GameManager.instance.goldText.text = nowgold.ToString("#,0"));

        GameManager.instance.monsterPoops.Remove(gameObject);
        Destroy(gameObject);
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
    }

    //指を離した時
    public void OnPointerUp(PointerEventData pointerEventData)
    {
    }
}
