using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu]
public class CharacterDefaultParam : ScriptableObject
{
    public int character_id;//キャラクターのID
    public int unique_id;//ユニークID
    public int rarity;//レアリティ
    public int maxLevel;
    public int currentLevel;
    public string charaName;
    public Sprite sprite;
    public int animatorControllerNum;
    public int skillGameNum;
    public int skillNum;
    public int skillCost;

    public int skillTime;
    public string description;

    public int HP;
    public int Attack;
    public int Defence;

    public int exp = 0;
    public int nextExp = 20;

    public string date;//例 20230123121820 2023年1月23日12時18分20秒

    public List<int> equipItem;//装備6個
    public int feedNum;//使用された餌（デフォルトの0の場合は通常のスピード）
}
