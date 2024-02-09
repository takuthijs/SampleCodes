using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SetCharaCardEventTrigger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler,IPointerClickHandler
{
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        UIManager.instance.cardListUI.SetActive(true);
        //GameManager.instance.ResetCharaList();
        //GameManager.instance.SetCharaList();
        GameManager.instance.selectCardNumber = gameObject.GetComponent<SetCharacterCard>().cardNumber;
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
    }

    //指を離した時
    public void OnPointerUp(PointerEventData pointerEventData)
    {
    }
}