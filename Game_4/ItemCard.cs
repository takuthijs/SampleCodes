using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ItemCard : MonoBehaviour, IPointerClickHandler
{
    public Image itemImage;
    public int itemId;
    public TextMeshProUGUI brokenCount;

    //ボタンを押した瞬間の処理
    public void OnPointerClick(PointerEventData eventData)
    {
        GameManager.instance.soundManager.PlaySE(2);//装備する音
        GameManager.instance.equipWindow.SetActive(false);
        GameManager.instance.equipItemNum = itemId;
        GameManager.instance.powerObj = GameManager.instance.equipItems[itemId];
        GameManager.instance.equipImage.sprite = GameManager.instance.equipItems[itemId].GetComponent<PowerObj>().equipImage;
    }
}
