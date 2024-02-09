using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    //public static UIManager instance;

    public TextMeshProUGUI breederName;

    public TextMeshProUGUI rankText;//ランクテキスト
    public TextMeshProUGUI rateText;//レートテキスト
    public TextMeshProUGUI coinText;//コインテキスト
    public TextMeshProUGUI shopCoinText;//ショップのコインテキスト
    public TextMeshProUGUI growCountText;//育成回数テキスト
    public TextMeshProUGUI beetleNameText;//昆虫名テキスト

    public TextMeshProUGUI powerText;//パワーテキスト
    public TextMeshProUGUI inteText;//インテリテキスト
    public TextMeshProUGUI guardText;//ガードテキスト
    public TextMeshProUGUI speedText;//スピードテキスト

    //canvas
    /*
    ランキング画面
    スキル選択画面
    習得中の技一覧
    バトル画面
     */

    //マッチング中画面
    public GameObject matchingImage;
    public TextMeshProUGUI matchingText;

    private void Awake()
    {
        //instance = this;
    }

    //ホームのUIをアップデートする処理
    public void HomeUIUpdate()
    {
        if (GameManager.instance.beetleManager.selectBeetle.rank != 0)
        {
            rankText.text = "ランク:"+GameManager.instance.beetleManager.selectBeetle.rank.ToString();
        }
        else
        {
            rankText.text = "ランク:---";
        }

        breederName.text = "ブリーダー名：" + GameManager.instance.breederName;
        GameManager.instance.breederNameInputValue.text = GameManager.instance.breederName;//デフォルトで入力されているポップアップ内のテキスト

        rateText.text = "レート:"+GameManager.instance.beetleManager.selectBeetle.rate.ToString();
        coinText.text = "<sprite=0> "+ GameManager.instance.coin.ToString("#,0");
        shopCoinText.text = "<sprite=0> " + GameManager.instance.coin.ToString("#,0");
        growCountText.text = "育成回数\n"+GameManager.instance.beetleManager.selectBeetle.growCount + " / 100";
        NameIcon(GameManager.instance.beetleManager.selectBeetle.beetleType);

        powerText.text = "<sprite=0>" + GameManager.instance.beetleManager.selectBeetle.power.ToString();
        inteText.text = "<sprite=1>" + GameManager.instance.beetleManager.selectBeetle.inter.ToString();
        guardText.text = "<sprite=2>" + GameManager.instance.beetleManager.selectBeetle.guard.ToString();
        speedText.text = "<sprite=3>" + GameManager.instance.beetleManager.selectBeetle.speed.ToString();

        //昆虫のタイプに応じてアイコンを表示させる
        StatusIcon(GameManager.instance.beetleManager.selectBeetle.beetleType);
    }

    //ホームに表示されているセットされたスキル表示を更新する
    public void HomeSkillUIUpdate()
    {
        int cost = 0;
        //セレクトビートルのスキルを取得
        BeetleManager.Beetle beetle = GameManager.instance.beetleManager.selectBeetle;

        //取得したスキルによって画像を変える
        for (int i = 0; i < 3; i++)
        {
            //Debug.Log(beetle.selectSkill.Count);
            if (beetle.selectSkill[i].Length < 16)
            {
                //スキルがセットされていなかったら画像を消す
                GameManager.instance.homeSkillImages[i].sprite = null;
                GameManager.instance.homeSkillCostTexts[i].text = "";

                //スキルウィンドウの方もセットする
                GameManager.instance.skillWindowSkillImages[i].sprite = null;
                GameManager.instance.skillWindowSkillCostTexts[i].text = "";
            }
            else
            {
                //スキルがセットされていたらfindしてそのスキルのidの画像を差し込む
                GameManager.SkillInfo skill = GameManager.instance.mySkills.Find(skill => skill.unique_id == GameManager.instance.beetleManager.selectBeetle.selectSkill[i]);
                GameManager.instance.homeSkillImages[i].sprite = GameManager.instance.skillInfo[skill.skillNum - 1].skillImage;
                //コストも設定する
                GameManager.instance.homeSkillCostTexts[i].text = skill.cost.ToString();

                //スキルウィンドウの方もセットする
                GameManager.instance.skillWindowSkillImages[i].sprite = GameManager.instance.skillInfo[skill.skillNum - 1].skillImage; ;
                GameManager.instance.skillWindowSkillCostTexts[i].text = skill.cost.ToString();

                cost += skill.cost;
            }
        }

        GameManager.instance.homeSkillCostText.text = cost + "/" + beetle.skillCostLimit;
        GameManager.instance.skillWindowSkillCostText.text = cost + "/" + beetle.skillCostLimit;
    }


    void NameIcon(GameManager.GrowStatus beetleType)
    {
        switch (beetleType)
        {
            case GameManager.GrowStatus.power:
                beetleNameText.text = "<sprite=0 color=#ff0000> " + GameManager.instance.beetleManager.selectBeetle.beetleName;
                break;
            case GameManager.GrowStatus.inte:
                beetleNameText.text = "<sprite=1 color=#D96B00> " + GameManager.instance.beetleManager.selectBeetle.beetleName;
                break;
            case GameManager.GrowStatus.guard:
                beetleNameText.text = "<sprite=2 color=#008000> " + GameManager.instance.beetleManager.selectBeetle.beetleName;
                break;
            case GameManager.GrowStatus.speed:
                beetleNameText.text = "<sprite=3 color=#0050B0> " + GameManager.instance.beetleManager.selectBeetle.beetleName;
                break;
            default:
                break;
        }
    }

    void StatusIcon(GameManager.GrowStatus beetleType)
    {
        switch (beetleType)
        {
            case GameManager.GrowStatus.power:
                powerText.text = "<sprite=0 color=#ff0000> " + "<color=#FF9800>" + GameManager.instance.beetleManager.selectBeetle.power;
                GameManager.instance.typeSprite.sprite = GameManager.instance.typeIcons[0];
                GameManager.instance.typeSprite.color = new Color(1, 0, 0, 0.5f);
                break;
            case GameManager.GrowStatus.inte:
                inteText.text = "<sprite=1 color=#D96B00> " + "<color=#FF9800>" + GameManager.instance.beetleManager.selectBeetle.inter.ToString();
                GameManager.instance.typeSprite.sprite = GameManager.instance.typeIcons[1];
                GameManager.instance.typeSprite.color = new Color(1, 0.5f, 0, 0.5f);
                break;
            case GameManager.GrowStatus.guard:
                guardText.text = "<sprite=2 color=#008000> " + "<color=#FF9800>" + GameManager.instance.beetleManager.selectBeetle.guard.ToString();
                GameManager.instance.typeSprite.sprite = GameManager.instance.typeIcons[2];
                GameManager.instance.typeSprite.color = new Color(0, 0.56f, 0, 0.5f);
                break;
            case GameManager.GrowStatus.speed:
                speedText.text = "<sprite=3 color=#0050B0> " + "<color=#FF9800>" + GameManager.instance.beetleManager.selectBeetle.speed.ToString();
                GameManager.instance.typeSprite.sprite = GameManager.instance.typeIcons[3];
                GameManager.instance.typeSprite.color = new Color(0, 0.5f, 1, 0.5f);
                break;
            default:
                break;
        }
    }
}
