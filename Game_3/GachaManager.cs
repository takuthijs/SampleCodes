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
    */

    //昆虫ガチャ一回の消費ゴールド
    int consumptionCoin = 200;
    //スキルガチャ一回の消費ゴールド
    int skillConsumptionCoin = 200;

    //所持金のUI
    public TextMeshProUGUI coinText;

    //Tapのオブジェクト
    public GameObject tapTextObj;
    //宝箱の下の『TAP』text
    public TextMeshProUGUI tapText;
    //白フェードイメージ
    public Image whiteImage;
    public GameObject gachaEndButtonObj;
    public Image whiteImageInner;
    //演出後のスキル画像 オブジェクト
    public GameObject skillImageObj;
    //演出後のスキル オブジェクト
    public Image skillImage;
    public TextMeshProUGUI skillCostText;
    public TextMeshProUGUI skillLevelText;

    [SerializeField] private LoopScrollRect loopScroll;//昆虫のスクロールについているスクリプト
    [SerializeField] private LoopScrollRect skillLoopScroll;//スキルのスクロールについているスクリプト

    //排出される昆虫とスキル
    public List<Gacha<BeetleSO>> beetles;
    public List<Gacha<SkillInfoSO>> skillItems;

    //ガチャのタイプと排出される昆虫、スキル、その他
    [System.Serializable]
    public class Gacha<T>
    {
        public string gachaType;
        public List<GachaProbability<T>> items;
    }

    //排出されるキャラとその確率のセット
    [System.Serializable]
    public class GachaProbability<T>
    {
        public T item;//排出されるもの（キャラか装備またはその他）
        public float probability;//排出される確率
    }

    //キャラガチャを引いた時のキャラデータ
    List<BeetleManager.Beetle> beetleParams = new List<BeetleManager.Beetle>();
    //スキルガチャを引いた時のキャラデータ
    List<GameManager.SkillInfo> skillParams = new List<GameManager.SkillInfo>();

    LastGacha lastgacha;
    enum LastGacha
    {
        beetleGacha,
        skillGacha,
        feedGacha
    }

    //キャラガチャを一回引く処理
    public void BeetleGachaOnce(string gachaType)
    {
        //最後に引いたガチャを昆虫ガチャにする
        lastgacha = LastGacha.beetleGacha;

        //お金を消費させる
        if (GameManager.instance.coin >= consumptionCoin)
        {
            GameManager.instance.coin -= consumptionCoin;

            coinText.text = GameManager.instance.coin.ToString("#,0");

            //昆虫のリストからランダムでキャラクターを選ぶ
            //まずはガチャのタイプからボタンに設定されたタイプのものを選ぶ
            Gacha<BeetleSO> gacha = beetles.Find(gacha => gacha.gachaType == gachaType);
            //BeetleManager.Beetle pickUpBeetle = PickItem(gacha);
            BeetleSO pickUpBeetle = PickItem(gacha);

            //後で演出に使用するので一旦クリアしてから追加する
            beetleParams.Clear();
            beetleParams.Add(GetBeetleParamSet(pickUpBeetle));

            //持っている昆虫リストにも追加
            GameManager.instance.beetleManager.myBeetles.Add(beetleParams[0]);

            //ホーム画面のUIを更新
            GameManager.instance.uIManager.HomeUIUpdate();

            //セーブ
            GameManager.instance.Save();

            //スクロールのトータルカウントをプラスする
            loopScroll.totalCount++;
        }
    }

    //スキルガチャを一回引く処理
    public void SkillGachaOnce(string gachaType)
    {
        //最後に引いたガチャを装備ガチャにする
        lastgacha = LastGacha.skillGacha;

        //お金を消費させる
        if (GameManager.instance.coin >= skillConsumptionCoin)
        {
            GameManager.instance.coin -= skillConsumptionCoin;

            coinText.text = GameManager.instance.coin.ToString("#,0");

            //スキルのリストからランダムでスキルを選ぶ
            //まずはガチャのタイプからボタンに設定されたタイプのものを選ぶ
            Gacha<SkillInfoSO> gacha = skillItems.Find(gacha => gacha.gachaType == gachaType);
            SkillInfoSO pickUpSkillItem = PickItem(gacha);

            //ピックアップしたスキルをGameManagerに加える
            pickUpSkillItem.unique_id = GameManager.instance.GenerateRandomString(16);

            //後で演出に使用するので一旦クリアしてから追加する
            skillParams.Clear();
            skillParams.Add(GetSkillParamSet(pickUpSkillItem));

            //持っているスキルリストにも追加
            GameManager.instance.mySkills.Add(skillParams[0]);

            //ホーム画面のUIを更新
            GameManager.instance.uIManager.HomeUIUpdate();

            //セーブ
            GameManager.instance.Save();

            skillLoopScroll.totalCount++;
        }
    }


    public T PickItem<T>(Gacha<T> gacha)
    {
        // アイテムの総確率を計算
        float totalProbability = 0f;
        foreach (GachaProbability<T> item in gacha.items)
        {
            totalProbability += item.probability;
        }

        // ランダムな確率値を生成
        float randomValue = UnityEngine.Random.Range(0f, totalProbability);

        // 確率に基づいてアイテムを選択
        float cumulativeProbability = 0f;
        foreach (GachaProbability<T> item in gacha.items)
        {
            cumulativeProbability += item.probability;
            if (randomValue <= cumulativeProbability)
            {
                return item.item;
            }
        }

        // アイテムが選択されなかった場合（通常はここに到達することはありません）
        throw new InvalidOperationException("選択されたアイテムがなかった");
    }


    //昆虫のパラメータをセット
    BeetleManager.Beetle GetBeetleParamSet(BeetleSO beetleSO)
    {
        BeetleManager.Beetle newBeetle = new BeetleManager.Beetle();

        newBeetle.beetleToken = GameManager.instance.GenerateRandomString(16);
        newBeetle.unique_id = GameManager.instance.GenerateRandomString(16);
        //newBeetle.unique_id = beetleSO.unique_id;//ユニークID
        //newBeetle.beetleToken = beetleSO.beetleToken;//ランキングの取得に使用
        newBeetle.rank = 0;
        newBeetle.rate = 1000;
        newBeetle.beetle_id = beetleSO.beetle_id;
        newBeetle.beetleName = beetleSO.beetleName;
        newBeetle.beetleCardImage = beetleSO.beetleCardImage;
        newBeetle.beetleType = beetleSO.beetleType;
        newBeetle.growCount = 100;
        newBeetle.power = beetleSO.power;
        newBeetle.inter = beetleSO.inter;
        newBeetle.guard = beetleSO.guard;
        newBeetle.speed = beetleSO.speed;
        newBeetle.skillCostLimit = beetleSO.skillCostLimit;
        for (int i = 0; i < 3; i++)
        {
            newBeetle.selectSkill.Add("none");
        }
        newBeetle.learnableSkills = beetleSO.learnableSkills;//習得可能なスキル
        newBeetle.dateTime = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss tt");
        return newBeetle;
    }

    //装備のパラメータをセット
    GameManager.SkillInfo GetSkillParamSet(SkillInfoSO skillSO)
    {
        GameManager.SkillInfo skill = new GameManager.SkillInfo();
        skill.unique_id = skillSO.unique_id;
        skill.skillNum = skillSO.skillNum;
        skill.skillName = skillSO.skillName;
        skill.cost = UnityEngine.Random.Range(skillSO.costLowerLimit, skillSO.costUpperLimit + 1);
        skill.level = UnityEngine.Random.Range(skillSO.levelLowerLimit, skillSO.levelUpperLimit + 1);
        skill.statusConditions = skillSO.statusConditions;//セットするための条件
        skill.skillImage = skillSO.skillImage;
        skill.skillDescription = skillSO.skillDescription;
        skill.dateTime = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss tt");

        return skill;
    }

    //ガチャボタン→確認ボタンを押した時に実行
    public void GachaStartButton()
    {
        tapTextObj.SetActive(true);//タッチというテキスト
        GameManager.instance.shopWindow.SetActive(false);//ショップタブ
        GameManager.instance.beetleObj.SetActive(false);//セレクトしている昆虫
        gachaEndButtonObj.SetActive(false);//ガチャを終えた時に押すボタンを非アクティブにする
        whiteImage.raycastTarget = false;//ボタンを連打できないようにブロックしていたImageのブロックを解除
        GameManager.instance.tabCanvas.SetActive(false);//タブ
        skillImageObj.SetActive(false);

        GameManager.instance.gachaWindow.SetActive(true);//見た目は何もないボタンやらがついているキャンバス
        GameManager.instance.gachaTreeObj.SetActive(true);//背景の大きい木
    }

    //ガチャ終了時に実行
    public void GachaEndButton()
    {
        GameManager.instance.shopWindow.SetActive(true);//ショップのタブ
        GameManager.instance.beetleObj.SetActive(true);//セレクトしている昆虫
        GameManager.instance.tabCanvas.SetActive(true);//タブ

        GameManager.instance.gachaBeetleObj.SetActive(false);//ガチャを引いた後の昆虫とその台座
        GameManager.instance.gachaTreeObj.SetActive(false);//背景の大きい木
        GameManager.instance.gachaWindow.SetActive(false);//ボタンやらが付いている見た目は何もないキャンバス
        GameManager.instance.shopPopup.SetActive(false);//ビートルガチャ確認画面
        GameManager.instance.skillGachaPopup.SetActive(false);//スキルガチャ確認画面
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
        sequence.Join(tapTextObj.transform.DOShakePosition(2f, 100f, 10))
            .Join(tapTextObj.transform.DOScale(1.5f, 2))
            .Join(whiteImage.DOFade(1, 2))
            .OnComplete(() =>
            {
                Color color = new Color(1, 1, 1, 1);
                GameManager.instance.gachaTreeObj.SetActive(false);//ツリーのオブジェクトを非アクティブにする
                
                tapTextObj.transform.localScale = new Vector3(1, 1, 1);
                gachaEndButtonObj.SetActive(true);
                
                whiteImage.color = new Color(1, 1, 1, 0);
                whiteImageInner.color = color;
                whiteImageInner.DOFade(0, 2);

                //オブジェクトを変える
                //昆虫の場合は3Dモデルを表示、スキルの場合は画像を表示
                if (lastgacha == LastGacha.beetleGacha)
                {
                    GameManager.instance.gachaBeetleObj.SetActive(true);//引いたこ昆虫が中央に表示されるようにする

                    //scriptableObjectのidを変えないで沼にハマったので気をつけてください。
                    //idを間違えるとガチャ後のモデルに違うものが映ってしまいます。
                    GameObject beetleObj = GameManager.instance.beetleManager.beetleSos[beetleParams[0].beetle_id - 1].model;
                    Debug.Log(beetleObj);
                    //生成されているものを消す
                    foreach (Transform n in GameManager.instance.gachaBeetleParent.transform)
                    {
                        GameObject.Destroy(n.gameObject);
                    }
                    Instantiate(beetleObj, GameManager.instance.gachaBeetleParent.transform);
                }

                if (lastgacha == LastGacha.skillGacha)
                {
                    skillImageObj.SetActive(true);
                    skillImage.sprite = skillParams[0].skillImage;
                    skillCostText.text = skillParams[0].cost.ToString();
                    skillLevelText.text = "Lv."+skillParams[0].level;
                }
            });
    }

}
