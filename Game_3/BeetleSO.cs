using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BeetleSO : ScriptableObject
{
    public string beetleToken;//ランキングの取得に使用
    public int rate;
    public string unique_id;
    public int beetle_id;
    public string beetleName;
    public Sprite beetleCardImage;
    public GameManager.GrowStatus beetleType;
    public int growCount;
    public int power;
    public int inter;
    public int guard;
    public int speed;
    public GameObject model;
    public int skillCostLimit;
    public List<int> learnableSkills;//習得可能なスキル
}
