using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;
using System;

public class SetCharacterInfo : MonoBehaviour
{
    public int unique_id;
    //public int character_id;//キャラクターのID
    public TextMeshProUGUI rarityText;//レアリティ
    private int maxLevel;
    private int currentLevel;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI currentLevelText;
    public TextMeshProUGUI maxLevelText;
    public TextMeshProUGUI charaName;
    public TextMeshProUGUI resetNum;//リセット回数
    public TextMeshProUGUI minigameName;
    public TextMeshProUGUI description;

    //キャラ本体のステータス
    public TextMeshProUGUI hp;
    public TextMeshProUGUI attack;
    public TextMeshProUGUI defence;
    public TextMeshProUGUI skillSpeed;

    //上昇分のステータス
    public TextMeshProUGUI increaseHP;
    public TextMeshProUGUI increaseAttack;
    public TextMeshProUGUI increaseDefence;
    public TextMeshProUGUI increaseSkillSpeed;

    private Sprite sprite;
    public Slider expSlider;
    public TextMeshProUGUI currentExpText;

    public Button coinLevelUpButton;
    public TextMeshProUGUI coinLevelUpText;
    public Button levelResetButton;

    public Image characterMainImage;
    public Image characterBackGroundImage;

    public List<int> equipIds;
    public List<Image> equipImages;

    private void Start()
    {
        //Todo Sliderの値が変わったら都度入れる。今後は他のステータスも反映させるようにする。
        //Todo キャラクターリストのバリューも変更できたら最高
        expSlider.UpdateAsObservable()
            .Select(_ => expSlider.value)
            .DistinctUntilChanged()
            .Subscribe(_=> SliderUpdate()); 
    }

    public void SliderUpdate()
    {
        var getCharaParam = GameManager.instance.getCharaParams.Find(x => x.unique_id == unique_id);
        expSlider.value = getCharaParam.exp;
    }

    public void SetCharacterinfo(int unique_id = 0)
    {
        if(unique_id != 0) this.unique_id = unique_id;

        //unique_idからプレイヤーの保存しているキャラクターの情報を取得して値をセットする
        var getCharaParam = GameManager.instance.getCharaParams.Find(x => x.unique_id == this.unique_id);
        sprite = getCharaParam.sprite;
        characterMainImage.sprite = sprite;
        characterBackGroundImage.sprite = sprite;

        switch (getCharaParam.rarity)
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
        goldText.text = GameManager.instance.gold.ToString("#,0");
        currentLevelText.text = getCharaParam.currentLevel.ToString();
        maxLevelText.text = "MaxLv."+getCharaParam.maxLevel;
        charaName.text = getCharaParam.charaName;
        resetNum.text = getCharaParam.resetNum.ToString();

        //ミニゲーム名をセット
        switch (getCharaParam.skillGameNum)
        {
            case 0:
                minigameName.text = "MiniGame : NumberTap";
                break;

            case 1:
                minigameName.text = "MiniGame : Roulette";
                break;
            case 2:
                minigameName.text = "MiniGame : MaxNumberTap";
                break;
            case 3:
                minigameName.text = "MiniGame : Smash";
                break;

            default:
                minigameName.text = "";
                break;
        }

        description.text = getCharaParam.description;
        hp.text = getCharaParam.HP.ToString();
        attack.text = getCharaParam.Attack.ToString();
        defence.text = getCharaParam.Defence.ToString();
        skillSpeed.text = getCharaParam.skillTime + " seconds";
        expSlider.value = getCharaParam.exp;
        expSlider.maxValue = getCharaParam.nextExp;
        currentExpText.text = getCharaParam.exp.ToString() + " / " + getCharaParam.nextExp.ToString();
        coinLevelUpText.text = (getCharaParam.currentLevel * 100).ToString("#,0");

        //一旦全て非アクティブにする
        //装備の画像
        foreach (Image image in equipImages)
        {
            image.enabled = false;
        }

        int increaseHP = 0;
        int increaseAttack = 0;
        int increaseDefence = 0;
        int increaseSkillTime = 0;

        for (int i = 0; i < getCharaParam.equipItem.Count; i++)
        {
            equipImages[i].enabled = true;
            GameManager.GetEquipParam equipParam = GameManager.instance.getEquipParams.Find(x => x.unique_id == getCharaParam.equipItem[i]);
            equipImages[i].sprite = equipParam.sprite;

            //武器の上昇分の計算とテキスト設定
            increaseHP += Mathf.FloorToInt(equipParam.increaseHP * 0.01f * getCharaParam.HP);
            increaseAttack += Mathf.FloorToInt(equipParam.increaseAttack * 0.01f * getCharaParam.Attack);
            increaseDefence += Mathf.FloorToInt(equipParam.increaseDefence * 0.01f * getCharaParam.Defence);
            increaseSkillTime += equipParam.increaseSkillTime;
        }

        //上昇分のステータス　(if文です。else条件があり長くなるので1行にしています。)
        this.increaseHP.text = (increaseHP != 0) ? "+" + increaseHP : "";
        this.increaseAttack.text = (increaseAttack != 0) ? "+" + increaseAttack : "";
        this.increaseDefence.text = (increaseDefence != 0) ? "+" + increaseDefence : "";
        this.increaseSkillSpeed.text = (increaseSkillTime != 0) ? "+" + increaseSkillTime : "";

}

    //コインでレベルアップさせる処理
    public void LevelUpWithCoins()
    {
        if (!coinLevelUpButton.interactable) return;

        var getCharaParam = GameManager.instance.getCharaParams.Find(x => x.unique_id == unique_id);
        int nowgold = GameManager.instance.gold;
        if(GameManager.instance.gold >= getCharaParam.currentLevel * 100 && (getCharaParam.maxLevel > getCharaParam.currentLevel && 99 > getCharaParam.currentLevel))
        {
            GameManager.instance.gold -= getCharaParam.currentLevel * 100;
            // 指定したupdateNumberまでカウントアップ・カウントダウンする
            DOTween.To(() => nowgold, (n) => nowgold = n, nowgold - getCharaParam.currentLevel * 100, 1)
                .OnUpdate(() => goldText.text = nowgold.ToString("#,0"))
                .OnComplete(() => {
                    GameManager.instance.LevelUpWithCoin(getCharaParam);
                    SetCharacterinfo(unique_id);
                    });
            coinLevelUpButton.interactable = false;
            GameManager.instance.goldText.text = GameManager.instance.gold.ToString("#,0");
            //Todo時間もリセットする
            Invoke("LevelUpButtonActivate",1.5f);//レベルアップボタンを連打できないようにする

            //キャラがマックスレベルに到達したらリセットボタンをアクティブにする
            if (getCharaParam.currentLevel == getCharaParam.maxLevel) levelResetButton.interactable = true;
        }
    }

    void LevelUpButtonActivate()
    {
        coinLevelUpButton.interactable = true;
    }

    //レベルとステータスを初期化する処理
    public void ResetStatus()
    {
        var getCharaParam = GameManager.instance.getCharaParams.Find(x => x.unique_id == unique_id);
        //キャラクターのリセット回数をプラスする
        getCharaParam.resetNum += 1;
        //キャラのマックスレベルもプラスする
        if(getCharaParam.maxLevel < 99) getCharaParam.maxLevel += 1;

        //デフォルトのパタメーターに設定する
        getCharaParam.currentLevel = 1;
        getCharaParam.HP = GameManager.instance.characterDefaultParams[getCharaParam.character_id-1].HP;
        getCharaParam.Attack = GameManager.instance.characterDefaultParams[getCharaParam.character_id-1].Attack;
        getCharaParam.Defence = GameManager.instance.characterDefaultParams[getCharaParam.character_id-1].Defence;
        getCharaParam.nextExp = GameManager.instance.characterDefaultParams[getCharaParam.character_id-1].nextExp;

        GameManager.instance.LevelUp(getCharaParam);
        SetCharacterinfo(unique_id);

        //経験値が足りなかったらボタンを無効状態にする
        if(getCharaParam.currentLevel < getCharaParam.maxLevel)
        {
            levelResetButton.interactable = false;
        }

    }

    private void OnEnable()
    {
        SetCharacterinfo(unique_id);

        var getCharaParam = GameManager.instance.getCharaParams.Find(x => x.unique_id == unique_id);

            GameManager.instance.selectCharaInfoUniqueID = unique_id;
        //Maxレベルだったらリセットボタンを有効にする
        if (getCharaParam.maxLevel <= getCharaParam.currentLevel)
        {
            levelResetButton.interactable = true;
        }
        else
        {
            levelResetButton.interactable = false;
        }

        //すでにパーティーにセット済みの場合、ボタンを無効にする
        //入っていなかった場合は有効にする
        if(GameManager.instance.playerParty[GameManager.instance.selectPartyNumber].unique_id.Exists(x => x == unique_id))
        {
            UIManager.instance.setButton.GetComponent<Button>().interactable = false;
        }
        else
        {
            UIManager.instance.setButton.GetComponent<Button>().interactable = true;
        }

    }
}
