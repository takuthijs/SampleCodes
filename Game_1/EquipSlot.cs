using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;
using UniRx;
using UniRx.Triggers;
using System.Linq;

public class EquipSlot : MonoBehaviour, IPointerClickHandler//,IPointerDownHandler, IPointerUpHandler,
{
    public GameObject selectImage;
    public int EquipNum;
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        Debug.Log("クリック中");

        foreach(EquipSlot slot in GameManager.instance.setEquipment.equipSlots)
        {
            slot.selectImage.SetActive(false);
        }
        //選択されたスロットの選択中画像をアクティブにする
        GameManager.instance.setEquipment.equipSlots[EquipNum].selectImage.SetActive(true);
        GameManager.instance.setEquipment.selectEquipSlotsNumber = EquipNum;
        if(GameManager.instance.setEquipment.selectEquipSlotsNumber != -1 && GameManager.instance.setEquipment.selectEquipItemNumber != -1)
        {
            GameManager.instance.setEquipment.setButton.interactable = true;
        }
        else
        {
            GameManager.instance.setEquipment.setButton.interactable = false;
        }
    }

    //public void OnPointerDown(PointerEventData pointerEventData)
    //{
    //}

    ////指を離した時
    //public void OnPointerUp(PointerEventData pointerEventData)
    //{
    //}
}
