using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class BattSlot : MonoBehaviour, IPointerClickHandler//IPointerDownHandler, IPointerUpHandler,
{
    public GameManager gameManager;

    public GameObject selectImage;
    public Image battImage;
    public int maxLevel;
    public int battLevel;
    public TextMeshProUGUI levelText;

    public int battNumber;
    public string battName;
    public GameObject battObj;
    public int power;
    public int meet;
    public int value;

    public MyBattWindowStatus myBattWindowStatus;

    //表示した時にセレクトイメージを元に戻す
    private void OnEnable()
    {
        if(gameManager.equipBattNumber != battNumber)
        {
            selectImage.SetActive(false);
        }
        else
        {
            selectImage.SetActive(true);
        }

        if(battLevel >= maxLevel)
        {
            //星マークにする
            levelText.text = "Lv.<sprite=0>";
        }
        else
        {
            levelText.text = "Lv."+battLevel.ToString();
        }
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        gameManager.selectBattNumber = battNumber;
        //フォーカス画像を一旦全て非アクティブにする
        foreach(GameObject foucusImage in gameManager.battSlots)
        {
            foucusImage.GetComponent<BattSlot>().selectImage.SetActive(false);
        }
        selectImage.SetActive(true);

        SetStatus();
    }

    void SetStatus()
    {
        //ステータス画面に反映させる
        myBattWindowStatus.battName.text = gameManager.battList[battNumber - 1].battName;
        myBattWindowStatus.battImage.sprite = gameManager.battList[battNumber - 1].battImage;
        myBattWindowStatus.battPowerText.text = "+" + power;

        //マイナス値だった場合は文字を赤色にしてプラス表記をなくす、そうでない場合は文字を緑色にしてプラス表記にする
        if (power > 0)
        {
            myBattWindowStatus.battPowerText.color = new Color(0.02f, 1, 0, 1);
            myBattWindowStatus.battPowerText.text = "+" + power;
        }
        else
        {
            myBattWindowStatus.battPowerText.color = Color.red;
            myBattWindowStatus.battPowerText.text = power.ToString();
        }

        if (meet > 0)
        {
            myBattWindowStatus.battMeetText.color = new Color(0.02f, 1, 0, 1);
            myBattWindowStatus.battMeetText.text = "+" + meet;
        }
        else
        {
            myBattWindowStatus.battMeetText.color = Color.red;
            myBattWindowStatus.battMeetText.text = meet.ToString();
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
