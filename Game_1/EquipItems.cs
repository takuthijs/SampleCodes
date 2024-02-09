using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipItems : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public GameManager.GetEquipParam equipParam;
    public Image frameItemImage;
    public Image itemImage;
    public Image selectImage;
    [HideInInspector]public int genarateNum;

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        foreach (EquipItems item in GameManager.instance.setEquipment.equipItems)
        {
            item.selectImage.enabled = false;
        }
        selectImage.enabled = true;

        GameManager.instance.setEquipment.weaponInfo.SetActive(true);
        GameManager.instance.setEquipment.selectEquipItemNumber = equipParam.unique_id;

        GameManager.instance.setEquipment.weaponNameText.text = equipParam.equipName;
        GameManager.instance.setEquipment.hpText.text = equipParam.increaseHP + "%";
        GameManager.instance.setEquipment.attackText.text = equipParam.increaseAttack + "%";
        GameManager.instance.setEquipment.defenceText.text = equipParam.increaseDefence + "%";
        GameManager.instance.setEquipment.skillSpeedText.text = equipParam.increaseSkillTime + "sec";

        GameManager.instance.setEquipment.selectWeaponImage.sprite = equipParam.sprite;

        if (GameManager.instance.setEquipment.selectEquipSlotsNumber != -1 && GameManager.instance.setEquipment.selectEquipItemNumber != -1)
        {
            GameManager.instance.setEquipment.setButton.interactable = true;
        }
        else
        {
            GameManager.instance.setEquipment.setButton.interactable = false;
        }

        GameManager.instance.setEquipment.genarateNum = genarateNum;
    }


    public void SetEquipParam(int unique_id)
    {
        //Debug.Log(unique_id);
        //Debug.Log(equipParam.equipName);
        equipParam = GameManager.instance.getEquipParams.Find(x => x.unique_id == unique_id);
        itemImage.sprite = equipParam.sprite;
        genarateNum = GameManager.instance.generateNum;
        equipParam.genarateNum = genarateNum;
        GameManager.instance.generateNum++;

        //ジャンルが違うものだったら表示しない
        //if (GameManager.instance.selectGenre == "All")
        //{
        //    //引数のジャンルがAllの場合は表示する
        //    gameObject.SetActive(true);
        //}
        //else if (equipParam.genre != GameManager.instance.selectGenre)
        //{
        //    //選択されたタブのジャンル以外のアイテムを非アクティブにする。
        //    gameObject.SetActive(false);
        //}
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
    }

    //指を離した時
    public void OnPointerUp(PointerEventData pointerEventData)
    {
    }
}
