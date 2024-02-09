using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class Skills : MonoBehaviour
{
    public static Skills instance;

    // スキル一覧
    public List<Action> skills = new List<Action>();
    [HideInInspector]public GameObject slot;

    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        skills.Add(MissSkill());
        skills.Add(HealSkill());
        skills.Add(TimeExtension());
        skills.Add(ShieldGenerate());
        skills.Add(Bomb());
    }

    //EventTriggerからスキルを使用
    public void SkillExe(int skillnum)
    {
        skills[skillnum]();
    }

    public enum skillName
    {
        miss,//ミス
        heal,//回復
        timeExtension,//時間の伸ばし
        shield,//味方マスにシールド配置
        bomb,//広範囲に自分の攻撃の3割ダメージ
        shieldBreak,//相手マスのシールドを壊す
        lineBreak,//自分のver_numに応じた選択した範囲に自分の攻撃の7割ダメージ
    }

    //ミス
    Action MissSkill()
    {
        return () =>
        {
            Debug.Log("ミス!!!");
            Slot slotComponent = slot.GetComponent<Slot>();
            if (slotComponent.skillActive)
            {
                slotComponent.timeGage = 0;
                slotComponent.fillEffect.SetActive(false);
                slotComponent.skillActive = false;

                foreach (GameObject grid in GameManager.instance.enemyGrids)
                {
                    grid.transform.GetChild(1).gameObject.SetActive(false);
                }

                GameManager.instance.playerSlider.DOValue(GameManager.instance.playerCurrentHP - 100, 1f);
                GameManager.instance.playerCurrentHP = GameManager.instance.playerCurrentHP - 100;
                if (GameManager.instance.playerCurrentHP < 0)
                {
                    GameManager.instance.playerCurrentHP = 0;
                }
                GameManager.instance.PlayerHPText.text = GameManager.instance.playerCurrentHP + " / " + GameManager.instance.playerHP;
                GameManager.instance.ShakeGameobject(GameManager.instance.playerHpGage);
            }
        };
    }

    //スキル機能
    //回復
    Action HealSkill()
    {
        return () =>
        {
            Debug.Log("回復発動");
            Slot slotComponent = slot.GetComponent<Slot>();
            if (slotComponent.skillActive)
            {
                slotComponent.timeGage = 0;
                slotComponent.fillEffect.SetActive(false);
                slotComponent.skillActive = false;

                //個別の選択したHPを回復する
                CharaParam charaParam = slot.GetComponent<Slot>().charaObj.GetComponent<CharaParam>();
                int afterHealHP = (int)slotComponent.hpSlider.value + Mathf.FloorToInt(charaParam.HP * 0.1f);
                slotComponent.hpSlider.DOValue(afterHealHP, 1f);
                //GameManager.instance.playerCurrentHP = GameManager.instance.playerCurrentHP + 100;
                //if (GameManager.instance.playerCurrentHP > GameManager.instance.playerHP)
                //{
                //    GameManager.instance.playerCurrentHP = GameManager.instance.playerHP;
                //}

                slot.GetComponent<Slot>().hpText.text = afterHealHP + " / " + charaParam.HP;
            }
        };
    }

    //時間伸ばし
    Action TimeExtension()
    {
        return () =>
        {
            Debug.Log("時間のばしスキル発動");
            Slot slotComponent = slot.GetComponent<Slot>();
            if (slotComponent.skillActive)
            {
                slotComponent.timeGage = 0;
                slotComponent.fillEffect.SetActive(false);
                slotComponent.skillActive = false;

                GameManager.instance.currentTime += 5;
            }
        };
    }

    //時間伸ばし
    Action ShieldGenerate()
    {
        return () =>
        {
            Debug.Log("シールド生成");
            Slot slotComponent = slot.GetComponent<Slot>();
            if (slotComponent.skillActive)
            {
                slotComponent.timeGage = 0;
                slotComponent.fillEffect.SetActive(false);
                slotComponent.skillActive = false;

                GameObject shieldObj = Instantiate(GameManager.instance.MonsterEntity, GameManager.instance.characterParent.transform);
                shieldObj.GetComponent<CharaParam>().SetShieldParam(GameManager.instance.foucusChara.GetComponent<CharaParam>().Defence);

                //自分のシールドリストにAddする
                GameManager.instance.playerShieldPos.Add(GameManager.instance.playerSkillSelectGrid, shieldObj);

                //重なり順を変更する
                switch (GameManager.instance.playerSkillSelectGrid)
                {
                    case 0:
                    case 4:
                    case 8:
                    case 12:
                        shieldObj.GetComponent<CharaParam>().canvas.sortingOrder = 2;
                        break;
                    case 1:
                    case 5:
                    case 9:
                    case 13:
                        shieldObj.GetComponent<CharaParam>().canvas.sortingOrder = 3;
                        break;
                    case 2:
                    case 6:
                    case 10:
                    case 14:
                        shieldObj.GetComponent<CharaParam>().canvas.sortingOrder = 4;
                        break;
                    case 3:
                    case 7:
                    case 11:
                    case 15:
                        shieldObj.GetComponent<CharaParam>().canvas.sortingOrder = 5;
                        break;
                }

                //シールドを選択したグリッドの位置にする
                shieldObj.transform.position = GameManager.instance.playerGrids[GameManager.instance.playerSkillSelectGrid].transform.position;

            }
        };
    }

    //爆弾
    Action Bomb()
    {
        return () =>
        {
            Debug.Log("爆弾スキル");
            Slot slotComponent = slot.GetComponent<Slot>();
            if (slotComponent.skillActive)
            {
                slotComponent.timeGage = 0;
                slotComponent.fillEffect.SetActive(false);
                slotComponent.skillActive = false;

                int key = GameManager.instance.playerSkillSelectGrid;

                //スキル対象選択可能な位置を表す画像を非アクティブにする
                foreach (GameObject grid in GameManager.instance.enemyGrids)
                {
                    grid.transform.GetChild(1).gameObject.SetActive(false);
                }

                //ここに選択したマスの中心から正方形の形の範囲にいるキャラクターにダメージを与える
                for (int i = 0; i < GameManager.instance.enemyGrids.Count; i++)
                {
                    if (i == key || i == key - 1 || i == key + 1 || i == key - 3 || i == key + 3 || i == key - 4 || i == key + 4 || i == key - 5 || i == key + 5)
                    {
                        //マスの上下端だった場合はしたの処理をスキップ
                        if ((key == 3 || key == 7 || key == 11 || key == 15) && (i == key + 1 || i == key - 3)) continue;
                        if ((key == 0 || key == 4 || key == 8 || key == 12) && (i == key - 1 || i == key + 3 )) continue;

                        //当てはまったマス上にいるキャラクターにダメージを与える
                        if (GameManager.instance.enemyCharacterPos.ContainsKey(i))
                        {
                            CharaParam charaParam = GameManager.instance.enemyCharacterPos[i].GetComponent<CharaParam>();
                            int afterHP = (int)charaParam.enemyHpSlider.value - Mathf.FloorToInt(GameManager.instance.foucusChara.GetComponent<CharaParam>().Attack * 0.3f);
                            charaParam.enemyHpSlider.DOValue(afterHP, 1f);
                            if (afterHP <= 0)
                            {
                                afterHP = 0;
                                GameManager.instance.enemyCharacterPos[i].SetActive(false);
                                GameManager.instance.enemyCharacterPos.Remove(i);
                            }
                            charaParam.enemySliderText.text = afterHP + "/" + charaParam.HP;
                        }
                    }
                }
            }
        };
        
    }
}
