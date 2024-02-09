using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ShopBattSlot : MonoBehaviour, IPointerClickHandler
{
    public GameManager gameManager;
    public GameObject selectImage;

    public int battNum;
    public string battName;
    public Image battImage;
    //public Image valueImage;//パワーかミートかの画像
    public int battPower;
    public int battMeet;

    public int powerValue;//パワーの価格
    public Button buyButton;//ポップアップじゃない方のボタン
    public TextMeshProUGUI battNameText;
    public TextMeshProUGUI buttonValueText;


    private void OnEnable()
    {
        //購入するバットの価格が足りていない場合はボタンを無効にする
        bool battLevel = gameManager.playerManager.powerLv >= powerValue;
        GameManager.BattStatus battStatus = gameManager.myBattStatuses.Find(battStatus => battStatus.battNumber == battNum);

        //スロットのバットがマックスレベルに到達していた場合はボタンを有効にしない
        bool maxLevel = false;
        if (battStatus == null)
        {
            maxLevel = true;
        }
        else
        {
            maxLevel = battStatus.battlevel < gameManager.battList[battNum - 1].maxLevel;
        }

        if (battLevel && maxLevel)
        {
            buyButton.interactable = true;
        }
        else
        {
            buyButton.interactable = false;
        }

        selectImage.SetActive(false);
    }

    public void BuyButton()
    {
        //最終確認のボタンをアクティブにする
        gameManager.battShopWindow.battPopup.SetActive(true);
        gameManager.battShopWindow.battNum = battNum;
        //gameManager.battShopWindow.popupBeforeLevelText.text = "Level "+gameManager.playerManager.powerLv.ToString("#,0");

        //int afterPower = gameManager.playerManager.powerLv - powerValue;
        //gameManager.battShopWindow.popupAfterLevelText.text = "Level " + afterPower.ToString("#,0");
        //gameManager.battShopWindow.battValue = powerValue;

        clickAction();
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        clickAction();
    }

    private void clickAction()
    {
        //ステータス部分をアクティブにする
        gameManager.battShopWindow.battStatusWindow.SetActive(true);
        //ステータス部分を変える処理を実行する
        gameManager.battShopWindow.battImage.sprite = battImage.sprite;
        gameManager.battShopWindow.battNameText.text = battName;
        //テキストの色を買える処理
        if (battPower > 0)
        {
            gameManager.battShopWindow.powerText.color = new Color(0.02f, 1, 0, 1);
            gameManager.battShopWindow.powerText.text = "+" + battPower.ToString("#,0");
        }
        else
        {
            gameManager.battShopWindow.powerText.color = Color.red;
            gameManager.battShopWindow.powerText.text = battPower.ToString("#,0");
        }


        
        if (battMeet > 0)
        {
            gameManager.battShopWindow.meetText.color = new Color(0.02f, 1, 0, 1);
            gameManager.battShopWindow.meetText.text = "+" + battMeet.ToString("#,0");
        }
        else
        {
            gameManager.battShopWindow.meetText.color = Color.red;
            gameManager.battShopWindow.meetText.text = battMeet.ToString("#,0");
        }

        //選択中の画像をアクティブにして他の選択中画像を非アクティブにする
        foreach (GameObject selectImage in gameManager.battShopSelectImages)
        {
            selectImage.SetActive(false);
        }
        selectImage.SetActive(true);
    }


}
