using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//実装されているスキルなど管理

//デフォルトの方
[CreateAssetMenu]
public class SkillInfoSO : ScriptableObject
{
    public string unique_id;
    public int skillNum;
    public string skillName;
    public int cost;
    public int costUpperLimit;
    public int costLowerLimit;
    public int level;
    public int levelUpperLimit;
    public int levelLowerLimit;
    public GameManager.GrowStatus skillType;
    public List<StatusCondition> statusConditions;//セットするための条件
    public Sprite skillImage;
    public string skillDescription;
    public string dateTime;
}

//スキルをセットするために必要な条件
[System.Serializable]
public class StatusCondition
{
    public GameManager.GrowStatus termStatus;
    public int statusLevel;
}



