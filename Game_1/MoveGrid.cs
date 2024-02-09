using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class MoveGrid : MonoBehaviour
{
    [Header("マスの番号")]
    public int gridNum;

    [Header("縦と横の番号")]
    public int ver_num;
    public int hori_num;

    [Header("敵フィールドか味方フィールドか")]
    public bool playerGrid;

    private void OnEnable()
    {
        //移動可能な位置を表す画像を非アクティブにする
        foreach (GameObject grid in GameManager.instance.playerGrids)
        {
            grid.transform.GetChild(0).gameObject.SetActive(false);
        }

        //スキル対象選択可能な位置を表す画像を非アクティブにする
        foreach (GameObject grid in GameManager.instance.playerGrids)
        {
            grid.transform.GetChild(1).gameObject.SetActive(false);
        }

        //スキル対象選択可能な位置を表す画像を非アクティブにする
        foreach (GameObject grid in GameManager.instance.enemyGrids)
        {
            grid.transform.GetChild(1).gameObject.SetActive(false);
        }

        //スキル対象選択可能な位置を表す画像を非アクティブにする
        foreach (GameObject grid in GameManager.instance.enemyGrids)
        {
            grid.transform.GetChild(2).gameObject.SetActive(false);
        }
    }

    public void CommandButtonAction()
    {
        CharacterMove();
        CharacterSkill();
        CharacterAttack();

        //移動可能な位置を表す画像を非アクティブにする
        foreach (GameObject grid in GameManager.instance.playerGrids)
        {
            grid.transform.GetChild(0).gameObject.SetActive(false);
        }

        //スキル対象選択可能な位置を表す画像を非アクティブにする
        foreach (GameObject grid in GameManager.instance.playerGrids)
        {
            grid.transform.GetChild(1).gameObject.SetActive(false);
        }

        //スキル対象選択可能な位置を表す画像を非アクティブにする
        foreach (GameObject grid in GameManager.instance.enemyGrids)
        {
            grid.transform.GetChild(1).gameObject.SetActive(false);
        }

        //スキル対象選択可能な位置を表す画像を非アクティブにする
        foreach (GameObject grid in GameManager.instance.enemyGrids)
        {
            grid.transform.GetChild(2).gameObject.SetActive(false);
        }

        GameManager.instance.playerSkillSelectGrid = gridNum;
        GameManager.instance.playerGrid = playerGrid;
    }

    void CharacterMove()
    {
        if (Skills.instance.slot.GetComponent<Slot>().skillActive && gameObject.transform.GetChild(0).gameObject.activeInHierarchy)
        {
            //重なり順を変更する
            switch (gridNum)
            {
                case 0:case 4:case 8:case 12:
                    Skills.instance.slot.GetComponent<Slot>().charaObj.GetComponent<CharaParam>().canvas.sortingOrder = 2;
                    break;
                case 1:case 5:case 9:case 13:
                    Skills.instance.slot.GetComponent<Slot>().charaObj.GetComponent<CharaParam>().canvas.sortingOrder = 3;
                    break;
                case 2:case 6:case 10:case 14:
                    Skills.instance.slot.GetComponent<Slot>().charaObj.GetComponent<CharaParam>().canvas.sortingOrder = 4;
                    break;
                case 3: case 7:case 11:case 15:
                    Skills.instance.slot.GetComponent<Slot>().charaObj.GetComponent<CharaParam>().canvas.sortingOrder = 5;
                    break;
            }

            //スロットのタイムゲージをリセットする
            Slot slotComponent = Skills.instance.slot.GetComponent<Slot>();
            slotComponent.timeGage = 0;
            slotComponent.fillEffect.SetActive(false);
            slotComponent.skillActive = false;

            //ディクショナリーからキーを削除する
            GameManager.instance.playerCharacterPos.Remove(key:slotComponent.key);

            //ディクショナリーに新しいキーとキャラオブジェを追加する
            GameManager.instance.playerCharacterPos.Add(gridNum, GameManager.instance.foucusChara);

            //押されたら自分のマスに移動する
            GameManager.instance.foucusChara.transform.position = gameObject.transform.position;

        }
    }


    void CharacterSkill()
    {
        if (Skills.instance.slot.GetComponent<Slot>().skillActive && gameObject.transform.GetChild(1).gameObject.activeInHierarchy)
        {
            CharaParam charaParam = GameManager.instance.foucusChara.GetComponent<CharaParam>();

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
    }

    void CharacterAttack()
    {
        if (Skills.instance.slot.GetComponent<Slot>().skillActive && gameObject.transform.GetChild(2).gameObject.activeInHierarchy)
        {
            CharaParam charaParam = GameManager.instance.foucusChara.GetComponent<CharaParam>();

            //スロットのタイムゲージをリセットする
            Slot slotComponent = Skills.instance.slot.GetComponent<Slot>();
            slotComponent.timeGage = 0;
            slotComponent.fillEffect.SetActive(false);
            slotComponent.skillActive = false;

            //相手のHPを減らす処理
            //既に移動されていた場合（Keyがなかった場合）は攻撃が外れたことにする
            if (GameManager.instance.enemyCharacterPos.ContainsKey(gridNum))
            {
                CharaParam enemyCharaParam = GameManager.instance.enemyCharacterPos[gridNum].GetComponent<CharaParam>();
                int damage = GameManager.instance.foucusChara.GetComponent<CharaParam>().Attack - enemyCharaParam.Defence;

                if (damage < 0) damage = Random.Range(0, 2);

                int remainingHP = (int)enemyCharaParam.enemyHpSlider.value - damage;
                if (remainingHP <= 0)
                {
                    remainingHP = 0;
                    //1秒後に消したほうが見栄えがいいかも
                    GameManager.instance.enemyCharacterPos[gridNum].SetActive(false);
                    GameManager.instance.enemyCharacterPos.Remove(gridNum);
                }
                enemyCharaParam.enemyHpSlider.DOValue(remainingHP, 1f);
                enemyCharaParam.enemySliderText.text = remainingHP + "/" + enemyCharaParam.enemyHpSlider.maxValue;
            }
            else
            {
                Debug.Log("攻撃は外れた");
            }  
        }
    }
}
