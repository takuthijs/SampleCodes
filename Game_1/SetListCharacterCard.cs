using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using static GameManager;

public class SetListCharacterCard : MonoBehaviour
{
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
    //public TextMeshProUGUI rarityText;
    public TextMeshProUGUI expText;
    public Slider expSlider;
    public List<GameObject> stars;

    public GameManager.GetCharaParam getCharaParam;

    //生成されたら行う処理
    void SetCardParam()
    {
        characterImage.sprite = sprite;
        levelText.text = currentLevel.ToString();
        nameText.text = charaName;

        StarActive(rarity);

        expText.text = exp + " / " + nextExp;
        expSlider.maxValue = nextExp;
        expSlider.value = exp;
    }

    public void ParamUpdate()
    {
        levelText.text = getCharaParam.currentLevel.ToString();
        expText.text = getCharaParam.exp + " / " + getCharaParam.nextExp;
        expSlider.maxValue = getCharaParam.nextExp;
        expSlider.value = getCharaParam.exp;
    }

    void StarActive(int num)
    {
        for (int i = 0; i < num; i++)
        {
            stars[i].SetActive(true);
        }
    }

    public void SetListCardParamator(int unique_id)
    {
        this.unique_id = unique_id;
        //unique_idからプレイヤーの保存しているキャラクターの情報を取得して値をセットする
        getCharaParam = GameManager.instance.getCharaParams.Find(x => x.unique_id == unique_id);
        if (getCharaParam == null) return;
        characterImage.sprite = getCharaParam.sprite;

        character_id = getCharaParam.character_id;
        rarity = getCharaParam.rarity;
        maxLevel = getCharaParam.maxLevel;
        currentLevel = getCharaParam.currentLevel;
        charaName = getCharaParam.charaName;
        sprite = getCharaParam.sprite;
        exp = getCharaParam.exp;
        nextExp = getCharaParam.nextExp;

        SetCardParam();
    }

    private void OnEnable()
    {
        if (getCharaParam == null) return;
        currentLevel = getCharaParam.currentLevel;
        levelText.text = currentLevel.ToString();

        exp = getCharaParam.exp;
        nextExp = getCharaParam.nextExp;
        expText.text = exp + " / " + nextExp;
        expSlider.maxValue = nextExp;
        expSlider.value = exp;
    }
}
