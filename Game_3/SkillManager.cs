using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;

    // スキル一覧
    public List<Action> skills = new List<Action>();

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        skills.Add(Skill_None());//0
        skills.Add(Skill_A());//1
        skills.Add(SkillBuffInter());//2
        skills.Add(SkillBuffGuard());//3
        skills.Add(SkillBuffSpeed());//4
        skills.Add(Skill_C());//5
        skills.Add(Skill_D());//6
    }

    public void SkillExe(int skillNum)
    {
        skills[skillNum]();
    }

    Action Skill_None()
    {
        return () =>
        {
            Debug.Log("技セットなし");
        };
    }

    //アタック＋1
    Action Skill_A()
    {
        return () =>
        {
            switch (GameManager.instance.battleManager.turnResult)
            {
                case BattleManager.TurnResult.win:
                    Debug.Log("先手撃ち発動(プレイヤー)");
                    GameManager.instance.battleManager.skillDamageToEnemy += 1;
                    break;
                case BattleManager.TurnResult.lose:
                    Debug.Log("先手撃ち発動(相手)");
                    GameManager.instance.battleManager.skillDamageToPlayer += 1;
                    break;
                default:
                    break;
            }
            
        };
    }

    //次のターンインテリ1.5倍
    Action SkillBuffInter()
    {
        return () =>
        {
            Buff(2,GameManager.GrowStatus.inte,1.5f,1);
        };
    }

    //次のターンガード1.5倍
    Action SkillBuffGuard()
    {
        return () =>
        {
            Buff(3, GameManager.GrowStatus.guard, 1.5f, 1);
        };
    }

    //次のターンスピード1.5倍
    Action SkillBuffSpeed()
    {
        return () =>
        {
            Buff(4, GameManager.GrowStatus.speed, 1.5f, 1);
        };
    }


    //回復+3
    Action Skill_C()
    {
        return () =>
        {
            //スキルの内容をかく
        };
    }

    //被ダメージ-1
    Action Skill_D()
    {
        return () =>
        {
            //スキルの内容をかく
        };
    }

    //前回ターンが負けの時発動：HPを入れ替える
    Action Skill_E()
    {
        return () =>
        {
            //スキルの内容をかく
        };
    }






    //バフデバフ系スキルの時に使用
    //デバフの場合は最後の引数をfalseにする
    void Buff(int skillNum, GameManager.GrowStatus statusBuff, float magnification, int turn, bool buff = true)
    {
        switch (GameManager.instance.battleManager.turnResult)
        {
            case BattleManager.TurnResult.win:
                Debug.Log(GameManager.instance.skillInfo[skillNum-1].skillName+"発動");
                //同じスキルの効果がかけられているか見つける
                BattleManager.BuffInfo playerBuff = GameManager.instance.battleManager.playerBuffInfos?.Find(buff => buff.skillNum == skillNum);
                //同じバフがかけられていた場合は効果を追加する
                if (playerBuff is null)
                {
                    BattleManager.BuffInfo buffInfo = new BattleManager.BuffInfo
                    {
                        skillNum = 2,
                        statusBuff = statusBuff,
                        count = 1,//何回分の効果がかかっているか
                        magnification = magnification,//倍率
                        turn = turn//どのターンまで持続するか
                    };
                    GameManager.instance.battleManager.playerBuffInfos.Add(buffInfo);
                }
                else
                {
                    playerBuff.count += 1;
                    playerBuff.turn += turn;
                }

                break;
            case BattleManager.TurnResult.lose:
                Debug.Log(GameManager.instance.skillInfo[skillNum - 1].skillName + "発動");
                //同じスキルの効果がかけられているか見つける
                BattleManager.BuffInfo enemyBuff = GameManager.instance.battleManager.enemyBuffInfos?.Find(buff => buff.skillNum == skillNum);
                //同じバフがかけられていた場合は効果を追加する
                if (enemyBuff is null)
                {
                    BattleManager.BuffInfo buffInfo = new BattleManager.BuffInfo
                    {
                        skillNum = skillNum,
                        statusBuff = statusBuff,
                        count = 1,//何階分の効果がかかっているか
                        magnification = magnification,//倍率
                        turn = turn//どのターンまで持続するか
                    };
                    GameManager.instance.battleManager.enemyBuffInfos.Add(buffInfo);
                }
                else
                {
                    enemyBuff.count += 1;
                    enemyBuff.turn += turn;
                }
                break;
            default:
                break;
        }
    }
}
