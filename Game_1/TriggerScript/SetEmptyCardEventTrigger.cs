using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SetEmptyCardEventTrigger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler,IPointerClickHandler
{
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        UIManager.instance.cardListUI.SetActive(true);
        GameManager.instance.selectCardNumber = gameObject.GetComponent<EmptyCardData>().cardNumber;
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
    }

    //指を離した時
    public void OnPointerUp(PointerEventData pointerEventData)
    {
    }
}