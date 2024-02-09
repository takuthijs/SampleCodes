using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using DG.Tweening;
using TMPro;

public class GachaManager : MonoBehaviour
{
    //実装したいこと
    /*
     * ボタンを押したらランダムで設定した確率のキャラが追加される
     * キャラ番号と確率をListで保存しておく
     * ボタンを押したら排出されたキャラ画像を表示させる
     * コインを消費させる
     * ユニークIDを設定する
     * ユニークIDをプラスする
     * 
     */

    //ボタン等
    public Button onceButton;
    public Button tenTimesButton;

    //キャラガチャ一回の消費ゴールド
    int consumptionGold = 50000;
    //装備ガチャ一回の消費ゴールド
    int equipConsumptionGold = 25000;

    //所持金のUI
    public TextMeshProUGUI goldText;

    //宝箱のゲームオブジェクト
    public GameObject treasureChest;
    //宝箱の画像
    public Image treasureChestImage;
    //Tapのオブジェクト
    public GameObject tapTextObj;
    //宝箱の下の『TAP』text
    public TextMeshProUGUI tapText;
    //白フェードイメージ
    public Image whiteImage;
    public Image whiteImageInner;
    //演出後のキャラ オブジェクト
    public GameObject charaImageObj;
    //演出後のキャラ オブジェクト
    public Image charaImage;

    [SerializeField]private LoopScrollRect loopScroll;//キャラのスクロールについているスクリプト
    [SerializeField] private LoopScrollRect equipLoopScroll;//装備のスクロールについているスクリプト

    //排出されるキャラ・装備リスト
    public List<Gacha<CharacterDefaultParam>> characters;
    public List<Gacha<EquipDefaultParam>> equipItems;

    //ガチャのタイプと排出されるキャラ、装備、その他
    [System.Serializable]
    public class Gacha<T>
    {
        public int gachaType;
        public List<GachaProbability<T>> items;
    }

    //排出されるキャラとその確率のセット
    [System.Serializable]
    public class GachaProbability<T>
    {
        public T item;//排出されるもの（キャラか装備またはその他）
        public float probability;//排出される確率
    }

    //1回のみの時のUI
    public GameObject onceUI;
    //10連の時のUI
    public GameObject tenTimesUI;

    //キャラガチャを引いた時のキャラデータ
    List<GameManager.GetCharaParam> charaParams = new List<GameManager.GetCharaParam>();
    //装備ガチャを引いた時のキャラデータ
    List<GameManager.GetEquipParam> equipParams = new List<GameManager.GetEquipParam>();

    LastGacha lastgacha;
    enum LastGacha
    {
        charaGacha,
        equipGacha
    }

    private void OnEnable()
    {
        goldText.text = GameManager.instance.gold.ToString("#,0");
    }


    //キャラガチャを一回引く処理
    public void CharaGachaOnce(int gachaType)
    {
        //最後に引いたガチャをキャラガチャにする
        lastgacha = LastGacha.charaGacha;

        //お金を消費させる
        if (GameManager.instance.gold >= consumptionGold)
        {
            GameManager.instance.gold -= consumptionGold;

            goldText.text = GameManager.instance.gold.ToString("#,0");
            GameManager.instance.goldText.text = GameManager.instance.gold.ToString("#,0");

            //ガチャの両方のボタンを無効状態にする
            onceButton.interactable = false;
            tenTimesButton.interactable = false;

            //1回引いた時のUIをアクティブにする
            onceUI.SetActive(true);

            //キャラクターのリストからランダムでキャラクターを選ぶ
            //まずはガチャのタイプからボタンに設定されたタイプのものを選ぶ
            Gacha<CharacterDefaultParam> gacha = characters.Find(gacha => gacha.gachaType == gachaType);
            CharacterDefaultParam pickUpChara = PickItem(gacha);

            //ピックアップしたキャラをGameManagerに加える
            //unique_idをカウントアップする
            GameManager.instance.getCharacterIndex ++;
            pickUpChara.unique_id = GameManager.instance.getCharacterIndex;

            charaParams.Clear();
            charaParams.Add(GetCharaParamSet(pickUpChara));

            GameManager.instance.getCharaParams.Add(charaParams[0]);

            //スクロールのトータルカウントをプラスする
            loopScroll.totalCount++;
        }
    }

    //装備ガチャを一回引く処理
    public void EquipGachaOnce(int gachaType)
    {
        //最後に引いたガチャを装備ガチャにする
        lastgacha = LastGacha.equipGacha;

        //お金を消費させる
        if (GameManager.instance.gold >= equipConsumptionGold)
        {
            GameManager.instance.gold -= equipConsumptionGold;

            goldText.text = GameManager.instance.gold.ToString("#,0");
            GameManager.instance.goldText.text = GameManager.instance.gold.ToString("#,0");
            //ガチャの両方のボタンを無効状態にする
            onceButton.interactable = false;
            tenTimesButton.interactable = false;

            //1回引いた時のUIをアクティブにする
            onceUI.SetActive(true);

            //キャラクターのリストからランダムでキャラクターを選ぶ
            //まずはガチャのタイプからボタンに設定されたタイプのものを選ぶ
            Gacha<EquipDefaultParam> gacha = equipItems.Find(gacha => gacha.gachaType == gachaType);
            EquipDefaultParam pickUpEquipItem = PickItem(gacha);

            //ピックアップしたキャラをGameManagerに加える
            //unique_idをカウントアップする
            GameManager.instance.getEquipIndex++;
            pickUpEquipItem.unique_id = GameManager.instance.getEquipIndex;

            //EquipParamSetにする
            equipParams.Clear();
            equipParams.Add(GetEquipParamSet(pickUpEquipItem));

            GameManager.instance.getEquipParams.Add(equipParams[0]);//一回の時は0番目を保管して、後でそのデータを使う

            equipLoopScroll.totalCount++;
        }
    }


    public T PickItem<T>(Gacha<T> gacha)
    {
        //アイテムの総確率を計算
        float totalProbability = 0f;
        foreach (GachaProbability<T> item in gacha.items)
        {
            totalProbability += item.probability;
        }

        //ランダムな確率値を生成
        float randomValue = UnityEngine.Random.Range(0f, totalProbability);

        //確率に基づいてアイテムを選択
        float cumulativeProbability = 0f;
        foreach (GachaProbability<T> item in gacha.items)
        {
            cumulativeProbability += item.probability;
            if (randomValue <= cumulativeProbability)
            {
                return item.item;
            }
        }

        //アイテムが選択されなかった場合（通常はここに到達することはありません）
        throw new InvalidOperationException("選択されたアイテムがなかった");
    }


    //キャラのパラメータをセット
    GameManager.GetCharaParam GetCharaParamSet(CharacterDefaultParam defaultParam)
    {
        GameManager.GetCharaParam getCharaParam = new GameManager.GetCharaParam();

        getCharaParam.unique_id = defaultParam.unique_id;//ユニークID
        getCharaParam.character_id = defaultParam.character_id;//キャラクターのID
        getCharaParam.rarity = defaultParam.rarity;//レアリティ
        getCharaParam.maxLevel = defaultParam.maxLevel;
        getCharaParam.currentLevel = defaultParam.currentLevel;
        getCharaParam.charaName = defaultParam.charaName;
        getCharaParam.sprite = defaultParam.sprite;
        getCharaParam.animatorControllerNum = defaultParam.animatorControllerNum;
        getCharaParam.skillNum = defaultParam.skillNum;
        getCharaParam.skillGameNum = defaultParam.skillGameNum;
        getCharaParam.skillCost = defaultParam.skillCost;
        getCharaParam.skillTime = defaultParam.skillTime;
        getCharaParam.description = defaultParam.description;
        getCharaParam.HP = defaultParam.HP;
        getCharaParam.Attack = defaultParam.Attack;
        getCharaParam.Defence = defaultParam.Defence;
        getCharaParam.exp = defaultParam.exp;
        getCharaParam.nextExp = defaultParam.nextExp;
        getCharaParam.date = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss tt");

        return getCharaParam;
    }

    //装備のパラメータをセット
    GameManager.GetEquipParam GetEquipParamSet(EquipDefaultParam defaultParam)
    {
        GameManager.GetEquipParam getEquipParam = new GameManager.GetEquipParam();

        getEquipParam.equip_id = defaultParam.equip_id;
        getEquipParam.unique_id = defaultParam.unique_id;//ユニークID
        getEquipParam.rarity = defaultParam.rarity;//レアリティ
        getEquipParam.genre = defaultParam.genre;

        getEquipParam.maxLevel = defaultParam.maxLevel;
        getEquipParam.currentLevel = defaultParam.currentLevel;
        getEquipParam.equipName = defaultParam.equipName;
        getEquipParam.sprite = defaultParam.sprite;
        getEquipParam.animatorControllerNum = defaultParam.animatorControllerNum;
        getEquipParam.equipCost = defaultParam.equipCost;
        getEquipParam.description = defaultParam.description;
        getEquipParam.increaseHP = defaultParam.increaseHP;
        getEquipParam.increaseAttack = defaultParam.increaseAttack;
        getEquipParam.increaseDefence = defaultParam.increaseDefence;
        getEquipParam.increaseSkillTime = defaultParam.increaseSkillTime;
        getEquipParam.exp = defaultParam.exp;
        getEquipParam.nextExp = defaultParam.nextExp;
        getEquipParam.date = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss tt");

        return getEquipParam;
    }


    public void GachaAnimation()
    {
        //白背景でボタンをブロックする
        whiteImage.raycastTarget = true;
        tapTextObj.SetActive(false);

        var sequence = DOTween.Sequence(); //Sequence生成
        //タップされたら震わせる
        //文字をフェードで消す
        //宝箱を大きくさせながらフェードアウトさせる
        //白い画像をフェードイン＆フェードアウトさせる
        //キャラ画像を表示させる// 震えながらフェードアウト
        sequence.Join(treasureChest.transform.DOShakePosition(2f, 100f, 10))
            .Join(treasureChest.transform.DOScale(1.5f, 2))
            .Join(treasureChestImage.DOFade(0, 2))
            .Join(whiteImage.DOFade(1, 2))
            .OnComplete(() =>
            {
                Color color = new Color(1, 1, 1, 1);
                tapTextObj.SetActive(true);
                treasureChest.transform.localScale = new Vector3(1, 1, 1);
                whiteImage.raycastTarget = false;
                treasureChestImage.color = color;
                whiteImage.color = new Color(1, 1, 1, 0);
                whiteImageInner.color = color;
                whiteImageInner.DOFade(0, 2);

                //ガチャの両方のボタンを有効状態にする
                onceButton.interactable = true;
                tenTimesButton.interactable = true;

                //ゲットした画像に差し替える
                if(lastgacha == LastGacha.charaGacha) charaImage.sprite = charaParams[0].sprite;
                if(lastgacha == LastGacha.equipGacha) charaImage.sprite = equipParams[0].sprite;
                charaImageObj.SetActive(true);
            });
    }

}
