using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SetListCardEventTrigger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler,IPointerClickHandler
{
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        SetListCharacterCard setListCharacterCard = gameObject.GetComponent<SetListCharacterCard>();
        UIManager.instance.cardInfoUI.GetComponent<SetCharacterInfo>().SetCharacterinfo(setListCharacterCard.unique_id);
        UIManager.instance.cardInfoUI.SetActive(true);
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
    }

    //指を離した時
    public void OnPointerUp(PointerEventData pointerEventData)
    {
    }
}