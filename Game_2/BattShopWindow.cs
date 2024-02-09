using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class BattShopWindow : MonoBehaviour
{
    public GameManager gameManager;

    public GameObject battShopWindow;
    public GameObject battStatusWindow;
    public GameObject battPopup;

    //バットのステータステキスト
    public Image battImage;
    public TextMeshProUGUI battNameText;
    public TextMeshProUGUI powerText;
    public TextMeshProUGUI meetText;

    public int battNum;
    public int battValue;

    //バットのポップアップテキスト
    public TextMeshProUGUI popupBeforeLevelText;
    public TextMeshProUGUI popupAfterLevelText;

    private void OnEnable()
    {
        battStatusWindow.SetActive(false);
        battPopup.SetActive(false);
    }


    public void AddOrLevelUpBatt()
    {
        ////レベルを引く
        //gameManager.playerManager.powerLv -= battValue;
        //gameManager.powBtns[0].level -= battValue;
        if (!gameManager.networkConection) return;

        gameManager.adMobScript.ShowAd(() =>
        {
            //購入したバットを持っているか判定
            bool isBattNumberFound = gameManager.myBattStatuses.Any(battStatus => battStatus.battNumber == battNum);
            if (isBattNumberFound)
            {
                //持っている場合はレベルを追加してそれぞれの能力値をあげる
                GameManager.BattStatus battStatus = gameManager.myBattStatuses.Find(battStatus => battStatus.battNumber == battNum);
                battStatus.battlevel++;
                battStatus.power += gameManager.battList[battNum - 1].power;
                if (gameManager.battList[battNum - 1].meet > 0)
                {
                    battStatus.meet += gameManager.battList[battNum - 1].meet;
                }

                GameObject battSlotObj =  gameManager.battSlots.Find(battSlot => battSlot.GetComponent<BattSlot>().battNumber == battNum);
                BattSlot battSlot = battSlotObj.GetComponent<BattSlot>();
                battSlot.battLevel += gameManager.battList[battNum - 1].battlevel;
                battSlot.power += gameManager.battList[battNum - 1].power;
                if (gameManager.battList[battNum - 1].meet > 0)
                {
                    battSlot.meet += gameManager.battList[battNum - 1].meet;
                }
            }
            else
            {
                //新しいバットの場合はバットを追加する
                GameObject battSlotObj = Instantiate(gameManager.battSlotEntity, gameManager.battSlotParent);
                BattSlot battSlot = battSlotObj.GetComponent<BattSlot>();
                battSlot.gameManager = gameManager;
                battSlot.myBattWindowStatus = gameManager.myBattWindowStatus;
                battSlot.battNumber = battNum;
                battSlot.selectImage.SetActive(false);
                battSlot.maxLevel = gameManager.battList[battNum - 1].maxLevel;//バットのマックスレベル;
                battSlot.battLevel = 1;
                battSlot.battName = gameManager.battList[battNum-1].battName;//バットの名前
                battSlot.battNumber = gameManager.battList[battNum - 1].battNumber;//バットの番号
                battSlot.battObj = gameManager.battList[battNum - 1].battObj;//バットのゲームオブジェクト
                battSlot.battImage.sprite = gameManager.battList[battNum - 1].battImage;//バットの画像
                battSlot.power = gameManager.battList[battNum - 1].power;//バットのパワー
                battSlot.meet = gameManager.battList[battNum - 1].meet;//バットのミート

                gameManager.battSlots.Add(battSlotObj);
                gameManager.battSlots = gameManager.battSlots.OrderBy(battSlot => battSlot.GetComponent<BattSlot>().battNumber).ToList();

                //ゲームマネージャーに生成したバットを追加する
                GameManager.BattStatus battStatus = new GameManager.BattStatus();
                battStatus.battNumber = battSlot.battNumber;
                battStatus.battName = battSlot.battName;
                battStatus.battObj = battSlot.battObj;
                battStatus.battImage = battSlot.battImage.sprite;
                battStatus.battlevel = 1;
                battStatus.power = battSlot.power;
                battStatus.meet = battSlot.meet;
                battStatus.speed = gameManager.battList[battNum - 1].speed;
                battStatus.value = gameManager.battList[battNum - 1].value;

                gameManager.myBattStatuses.Add(battStatus);

                //追加後にバットナンバー順にソートする
                gameManager.myBattStatuses = gameManager.myBattStatuses.OrderBy(battStatus => battStatus.battNumber).ToList();
            }

            //セーブする
            string json = JsonUtility.ToJson(new SerializableBattStatuses(gameManager.myBattStatuses));
            //Debug.Log(json);
            PlayerPrefs.SetString(GameManager.KEY_SAVE_MYBATT, json);
            PlayerPrefs.SetInt(GameManager.KEY_SAVE_Power, gameManager.playerManager.powerLv);
            PlayerPrefs.Save();

            //ボトムのボタンの表示を更新する
            gameManager.powBtns[0].lb_level.text = "Lv." + gameManager.playerManager.powerLv.ToString();
            gameManager.powBtns[0].lb_price.text = "<sprite=0>"+(gameManager.playerManager.powerLv * 10).ToString();

            //ポップアップをショップのウィンドウを非アクティブにする
            battShopWindow.SetActive(false);
            battPopup.SetActive(false);


            Firebase.Analytics.FirebaseAnalytics.LogEvent("BattReword");

            //広告をロード
            gameManager.adMobScript.LoadAd();
        });
    }

}

//JSONに変換可能なクラスを作成 (ラッパークラス)
[System.Serializable]
public class SerializableBattStatuses
{
    public List<GameManager.BattStatus> battStatuses;

    public SerializableBattStatuses(List<GameManager.BattStatus> battStatuses)
    {
        this.battStatuses = battStatuses;
    }
}


