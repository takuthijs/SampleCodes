using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class CharaParam : MonoBehaviour
{
    public int unique_id;//ユニークID
    public int character_id;//キャラクターのID
    public int rarity;//レアリティ
    public int maxLevel;
    public int currentLevel;
    public string charaName;
    public Sprite sprite;
    public GameObject charaObj;
    public int skillGameNum;
    public int skillNum;
    public int skillCost;
    public int animatorControllerNum;
    public Image monsterImage;
    public Sprite ShieldSprite;

    public Slider enemyHpSlider;
    public TextMeshProUGUI enemySliderText;

    public Canvas canvas;//重なり順に使用します

    //public int tapScreenActiveTime;

    public int skillTime;
    public string discription;

    public int HP;
    public int Attack;
    public int Defence;

    public int exp = 0;
    public int nextExp = 20;

    public string date;//例 20230123121820 2023年1月23日12時18分20秒

    public int[] equipItem;//装備6個

    [HideInInspector]public GameObject slotObj;

    //private void Start()
    //{
    //    gameObject.GetComponent<Image>().sprite = sprite;
    //}

    public void SetGameCharaParam(
        int unique_id,int character_id, int rarity, int maxLevel, int currentLevel,
        string charaName, Sprite sprite, int animatorControllerNum,int skillGameNum,int skillNum, int skillCost,
        int skillTime, string discription, int HP, int Atack,int defence
    )
    {
        this.unique_id = unique_id;
        this.character_id = character_id;
        this.rarity = rarity;
        this.maxLevel = maxLevel;
        this.currentLevel = currentLevel;
        this.charaName = charaName;
        this.sprite = sprite;
        monsterImage.sprite = sprite;
        this.animatorControllerNum = animatorControllerNum;
        if (character_id == 5) charaObj.GetComponent<Animator>().runtimeAnimatorController = GameManager.instance.animatorControllers[animatorControllerNum];
        this.skillGameNum = skillGameNum;
        this.skillNum = skillNum;
        this.skillCost = skillCost;
        this.skillTime = skillTime;
        this.discription = discription;

        this.HP = HP;
        this.Attack = Atack;
        this.Defence = defence;
    }

    public void IncreaseParam(int i_hp,int i_attack, int i_defence,int i_skillTime)
    {
        this.HP += i_hp;
        this.Attack += i_attack;
        this.Defence += i_defence;
        this.skillTime += i_skillTime;
    }

    public void SetEnemyCharaParam(CharacterDefaultParam enemyParam)
    {
        monsterImage.sprite = enemyParam.sprite;
        this.skillGameNum = enemyParam.skillGameNum;
        this.skillNum = enemyParam.skillNum;
        this.skillCost = enemyParam.skillCost;
        this.skillTime = enemyParam.skillTime;
        this.HP = enemyParam.HP;

        enemySliderText.text = enemyParam.HP + "/" + enemyParam.HP;
        enemyHpSlider.maxValue = enemyParam.HP;
        enemyHpSlider.value = enemyParam.HP;

        this.Attack = enemyParam.Attack;
        this.Defence  = enemyParam.Defence;
    }

    public void SetShieldParam(int defence)
    {
        this.HP = 1;
        this.Attack = 0;
        this.Defence = Mathf.FloorToInt(defence / 2);
        monsterImage.sprite = ShieldSprite;
    }
}

