using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static GameManager;

public class SetCharacterCard : MonoBehaviour
{
    public int cardNumber;

    public int unique_id;
    public int character_id;//キャラクターのID
    public int rarity;//レアリティ
    public int maxLevel;
    public int currentLevel;
    public string charaName;
    public Sprite sprite;
    //public int skillNum;
    //public int skillCost;
    //public int skillTime;
    //public string discription;
    //public int HP;
    //public int Atack;
    //public int Defence;
    public int exp;
    public int nextExp;

    public Image characterImage;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI rarityText;
    public TextMeshProUGUI expText;
    public Slider expSlider;
    public List<GameObject> stars;


    //生成されたら行う処理
    void SetCardParam()
    {
        characterImage.sprite = sprite;
        levelText.text = currentLevel.ToString();
        nameText.text = charaName;

        switch (rarity)
        {
            case 1:
                rarityText.text = "Nomal";
                break;

            case 2:
                rarityText.text = "Rare";
                break;

            default:
                rarityText.text = "SuperRare";
                break;
        }

        StarActive(rarity);

        expText.text = exp + " / " + nextExp;
        expSlider.maxValue = nextExp;
        expSlider.value = exp;
    }

    void StarActive(int num)
    {
        for (int i = 0; i < num; i++)
        {
            stars[i].SetActive(true);
        }
    }

    public void SetThisParamator(int cardNumber,int unique_id,int character_id,int rarity,int maxLevel,int currentLevel,string charaName,Sprite sprite,int exp,int nextExp)
    {
        this.cardNumber = cardNumber;
        this.unique_id = unique_id;
        this.character_id = character_id;
        this.rarity = rarity;
        this.maxLevel = maxLevel;
        this.currentLevel = currentLevel;
        this.charaName = charaName;
        this.sprite = sprite;
        this.exp = exp;
        this.nextExp = nextExp;
        SetCardParam();
    }

    public void ParamUpdate()
    {
        GetCharaParam getCharaParam = GameManager.instance.getCharaParams.Find(x => x.unique_id == this.unique_id);
        levelText.text = getCharaParam.currentLevel.ToString();
        expText.text = getCharaParam.exp + " / " + getCharaParam.nextExp;
        expSlider.maxValue = getCharaParam.nextExp;
        expSlider.value = getCharaParam.exp;
    }
}
