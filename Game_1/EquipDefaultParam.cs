using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu]
public class EquipDefaultParam : ScriptableObject
{
    public int equip_id;//装備のID
    public int unique_id;//ユニークID
    public int rarity;//レアリティ
    public string genre;//装備のジャンル
    public int maxLevel;
    public int currentLevel;
    public string equipName;
    public Sprite sprite;
    public int animatorControllerNum;
    public int equipCost;

    public string description;

    public int increaseHP;
    public int increaseAttack;
    public int increaseDefence;
    public int increaseSkillTime;

    public int exp = 0;
    public int nextExp = 20;

    public string date;//例 20230123121820 2023年1月23日12時18分20秒
}
