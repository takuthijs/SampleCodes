using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEditor;

public class Slot : MonoBehaviour, IPointerClickHandler
{
    public int skillTime;//スキルが溜まるまでの時間
    public int skillNum;
    public GameObject charaObj;

    public int key;//現在のグリッドの場所を格納

    [HideInInspector] public float timeGage = 0;
    [HideInInspector] public float startTime = 0;//スクリプトが実行されたタイミングの時間を保存
    //[HideInInspector]public float skillUseTime = 0;//スキルを使用した時点での時間

    public bool skillActive = false;
    public TextMeshProUGUI levelText;
    public Slider hpSlider;
    public TextMeshProUGUI hpText;

    public Slider skillSlider;
    public GameObject fillEffect;

    public GameObject charaSprite;
    public Sprite sprite;

    //コマンド
    public GameObject commandWindow;

    //Todo属性ごとに変えたい
    public GameObject slotBackGround;
    public GameObject backGrow;
    public GameObject bottomGrow;
    public GameObject frame;

    void Start()
    {
        skillSlider.maxValue = skillTime;
        charaSprite.GetComponent<Image>().sprite = sprite;
        //skillUseTime = Time.time;
    }

    void Update()
    {
        //if (!GameManager.instance.buttonPush) return;
        if (timeGage != skillTime)
        {
            if (skillTime > timeGage)
            {
                timeGage += Time.deltaTime;
                skillSlider.value = timeGage;
            }
            else
            {
                timeGage = skillTime;
            }
        }

        if (timeGage == skillTime && !fillEffect.activeInHierarchy)
        {
            FillEffect();
        }
    }

    public void ParamSet(CharaParam charaParam)
    {
        this.levelText.text = charaParam.currentLevel.ToString();
        hpText.text = charaParam.HP + "/" + charaParam.HP;
        hpSlider.maxValue = charaParam.HP;
        hpSlider.value = charaParam.HP;
        this.skillTime = charaParam.skillTime;
        this.skillNum = charaParam.skillNum;
        this.charaObj = charaParam.charaObj;
        this.sprite = charaParam.sprite;
    }


    //スキルが使用可能な時に表示されるエフェクト
    void FillEffect()
    {
        fillEffect.SetActive(true);
        skillActive = true;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        GameManager.instance.FoucusChara(charaObj);//生成時に格納したリストのものをアクティブにした方がいいかも
        Skills.instance.slot = gameObject;
        //自分がアクティブだったら非表示にしてアクティブだったら表示する
        if (!commandWindow.activeInHierarchy)
        {
            //一旦コマンドは全削除
            foreach(GameObject commandWindow in GameManager.instance.commandWindows)
            {
                commandWindow.SetActive(false);
            }
            commandWindow.SetActive(true);
        }
        else
        {
            commandWindow.SetActive(false);
        }

        //一旦移動可能な位置を表す画像を非アクティブにする
        foreach (GameObject grid in GameManager.instance.playerGrids)
        {
            grid.transform.GetChild(0).gameObject.SetActive(false);
        }
        // 一旦スキル発動可能な位置を表す画像を非アクティブにする
        foreach (GameObject grid in GameManager.instance.playerGrids)
        {
            grid.transform.GetChild(1).gameObject.SetActive(false);
        }
        //一旦攻撃可能な位置を表す画像を非アクティブにする
        foreach (GameObject grid in GameManager.instance.enemyGrids)
        {
            grid.transform.GetChild(1).gameObject.SetActive(false);
        }
        //一旦攻撃可能な位置を表す画像を非アクティブにする
        foreach (GameObject grid in GameManager.instance.enemyGrids)
        {
            grid.transform.GetChild(2).gameObject.SetActive(false);
        }
    }

    public void SkillActive()
    {
        GridReset();

        //他のミニゲームウィンドウも条件に入れる(ミニゲームごとの背景でTriggerはブロックされるが念の為)
        if (!NumberTapController.instance.tapScreen.activeInHierarchy && !UIManager.instance.rouletteUI.activeInHierarchy && !UIManager.instance.maxNumberTapUI.activeInHierarchy && !UIManager.instance.smashTapUI.activeInHierarchy)
        {
            Skills.instance.slot = gameObject;
            if (skillActive)
            {
                CharaParam charaParam = charaObj.GetComponent<CharaParam>();

                //ここでキャラに応じたスキルの番号に応じて対象選択の処理を分岐させる
                switch (charaParam.skillNum)
                {
                    case (int)Skills.skillName.heal:
                        //ヒールの対象選択
                        //味方のいるマス
                        for (int i = 0; i < GameManager.instance.playerGrids.Count; i++)
                        {
                            if (GameManager.instance.playerCharacterPos.ContainsKey(i))
                                GameManager.instance.playerGrids[i].transform.GetChild(1).gameObject.SetActive(true);
                        }
                        break;

                    case (int)Skills.skillName.timeExtension:
                        //対象選択はなし
                        //ミニゲーム画面を開く
                        MiniGameSwitch(charaParam);
                        break;

                    case (int)Skills.skillName.shield:
                        //シールドの対象選択
                        //味方の空きマス
                        for (int i = 0; i < GameManager.instance.playerGrids.Count; i++)
                        {
                            if (GameManager.instance.playerCharacterPos.ContainsKey(i) || GameManager.instance.playerShieldPos.ContainsKey(i))
                            {
                                GameManager.instance.playerGrids[i].transform.GetChild(1).gameObject.SetActive(false);
                            }
                            else
                            {
                                GameManager.instance.playerGrids[i].transform.GetChild(1).gameObject.SetActive(true);
                            }
                        }

                        break;

                    case (int)Skills.skillName.shieldBreak:
                        //相手のシールドマスを選択
                        for (int i = 0; i < GameManager.instance.enemyGrids.Count; i++)
                        {
                            if (GameManager.instance.enemyShieldPos.ContainsKey(i))
                            {
                                GameManager.instance.enemyGrids[i].transform.GetChild(1).gameObject.SetActive(true);
                            }
                            else
                            {
                                GameManager.instance.playerGrids[i].transform.GetChild(1).gameObject.SetActive(false);
                            }
                        }
                        break;

                    case (int)Skills.skillName.bomb:
                        //相手マス選択
                        for (int i = 0; i < GameManager.instance.enemyGrids.Count; i++)
                        {
                            GameManager.instance.enemyGrids[i].transform.GetChild(1).gameObject.SetActive(true);
                        }
                        break;
                }
            }
        }
    }


    public void CharaMoveActive()
    {
        GridReset();

        //自分のキャラオブジェが格納されているキーを取得
        key = GameManager.instance.playerCharacterPos.FirstOrDefault(x => x.Value.Equals(charaObj)).Key;

        //キー番号から規則に従ったグリッドの選択位置をアクティブにする
        List<int> canMovePositions = new List<int>();

        for(int i = 0; i< GameManager.instance.playerGrids.Count; i++)
        {
            if(i == key - 1||i == key + 1|| i == key - 3 || i == key + 3 || i == key - 4 || i == key + 4 || i == key - 5 || i == key + 5)
            {
                GameManager.instance.playerGrids[i].transform.GetChild(0).gameObject.SetActive(true);
            }

            if ((key == 3 || key == 7 || key == 11 || key == 15) && (i == key + 1 || i == key -3 || i == key + 5))
            {
                GameManager.instance.playerGrids[i].transform.GetChild(0).gameObject.SetActive(false);
            }

            if (( key == 0 ||key == 4 || key == 8 || key == 12) && (i == key - 1 || i == key + 3 || i == key - 5))
            {
                GameManager.instance.playerGrids[i].transform.GetChild(0).gameObject.SetActive(false);
            }

            if (GameManager.instance.playerCharacterPos.ContainsKey(i) || GameManager.instance.playerShieldPos.ContainsKey(i))
            {
                GameManager.instance.playerGrids[i].transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }

    void MiniGameSwitch(CharaParam charaParam)
    {
        switch (charaParam.skillGameNum)
        {
            //Todo 増えてきたらenumに変更
            case 0://ナンバータップ
                NumberTapController.instance.NumberScreenActive(GameManager.instance.foucusChara);
                break;

            case 1://ルーレット
                UIManager.instance.rouletteUI.SetActive(true);

                RouletteController.instance.cost = GameManager.instance.foucusChara.GetComponent<CharaParam>().skillCost;
                RouletteController.instance.buttonPush = true;

                break;
            case 2://最大数字のタップ
                UIManager.instance.maxNumberTapUI.SetActive(true);

                MaxNumberTapController.instance.skillCost = GameManager.instance.foucusChara.GetComponent<CharaParam>().skillCost;
                MaxNumberTapController.instance.successCount = 0;
                MaxNumberTapController.instance.NumbersGenerate();

                break;
            case 3://連打ゲーム
                UIManager.instance.smashTapUI.SetActive(true);

                SmashTapController.instance.skillCost = GameManager.instance.foucusChara.GetComponent<CharaParam>().skillCost;
                SmashTapController.instance.SetCount();

                break;
        }
    }

    public void PlayerAttack()
    {
        GridReset();

        for (int i = 0; i < GameManager.instance.enemyGrids.Count; i++)
        {
            if (GameManager.instance.enemyCharacterPos.ContainsKey(i))
            {
                GameManager.instance.enemyGrids[i].transform.GetChild(2).gameObject.SetActive(true);
            }
        }
        List<int> horiNums = new List<int>();
        foreach (int key in GameManager.instance.enemyCharacterPos.Keys)
        {
            horiNums.Add(GameManager.instance.playerGrids[key].GetComponent<MoveGrid>().hori_num);
        }
        //シールドの配置も追加する
        foreach (int key in GameManager.instance.enemyShieldPos.Keys)
        {
            horiNums.Add(GameManager.instance.playerGrids[key].GetComponent<MoveGrid>().hori_num);
        }

        //verNumsのナンバーが全て揃っていたら
        if (horiNums.OrderBy(x => x).SequenceEqual(Enumerable.Range(0, 4)))
        {
            //数字が全て揃っている場合、対象には攻撃できないため、大将ボタンは非アクティブにする
            GameManager.instance.enemyGeneralButton.interactable = false;
        }
        else
        {
            //大将ボタンをアクティブにする
            GameManager.instance.enemyGeneralButton.interactable = true;
        }
    }

    public void GridReset()
    {
        //一旦コマンドは全非アクティブ
        foreach (GameObject commandWindow in GameManager.instance.commandWindows)
        {
            commandWindow.SetActive(false);
        }
        //一旦移動可能な位置を表す画像を非アクティブにする
        foreach (GameObject grid in GameManager.instance.playerGrids)
        {
            grid.transform.GetChild(0).gameObject.SetActive(false);
        }
        // 一旦スキル発動可能な位置を表す画像を非アクティブにする
        foreach (GameObject grid in GameManager.instance.playerGrids)
        {
            grid.transform.GetChild(1).gameObject.SetActive(false);
        }
        //一旦攻撃可能な位置を表す画像を非アクティブにする
        foreach (GameObject grid in GameManager.instance.enemyGrids)
        {
            grid.transform.GetChild(1).gameObject.SetActive(false);
        }
        //一旦攻撃可能な位置を表す画像を非アクティブにする
        foreach (GameObject grid in GameManager.instance.enemyGrids)
        {
            grid.transform.GetChild(2).gameObject.SetActive(false);
        }
    }

}
