using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using System;
using System.Linq;

public class SkillCard : MonoBehaviour, IPointerClickHandler,IPointerDownHandler //IPointerUpHandler
{
    public bool isSkillSlot;//自身がスキルスロットかどうか
    public bool isGachaSlot;//ガチャを引いた後に使っているスロットかどうか
    public int slotNumber;//この変数は生成されたカードの方は関係ないです。slotの方のみ使用。
    public GameObject effect;
    public GameObject equipText;
    public GameObject cannotEquipText;

    public string skillUniqueId;
    public string setCharaUniqueId;

    public int skillNum;//ScriptableObjectから説明を持ってくる時に使用
    public int skillCost;//昆虫のコストを計算するときに使用
    public TextMeshProUGUI costText;//スキルカードのコスト部分のテキスト
    public TextMeshProUGUI levelText;//スキルカードのレベル部分のテキスト
    public Image skillImage;

    public Button button;

    void Start()
    {
        button.OnPointerDownAsObservable().Select(_ => true)
        // OnPointerUpをfalseに変換して合成
        // この後には押した時にtrueが、離した時にfalseが流れるようになる
        .Merge(button.OnPointerUpAsObservable().Select(_ => false))
        // 0.3秒間新しい値が来なかったら最後に来た値を流す
        .Throttle(TimeSpan.FromSeconds(0.3f))
        // trueだけを流す
        .Where(b => b)
        // このboolはどうでもいい値なのでUnitにする
        .AsUnitObservable()
        // 長押しを購読！！！
        .Subscribe(OnLongClick);

        // スキルが変わったことを受け取った場合は表示を切り替える
        GameManager.instance.slotChanged.Subscribe(value => EffectActivate());
    }

    //自身がアクティブになった時にエフェクトをアクティブにする
    private void OnEnable()
    {
        EffectActivate();
    }

    public void EffectActivate()
    {
        try
        {
            //スキルスロットではなかったら
            if (!isSkillSlot && !isGachaSlot && !GameManager.instance.gachaWindow.activeInHierarchy)
            {
                bool skillEquip = GameManager.instance.beetleManager.selectBeetle.selectSkill.Contains(skillUniqueId);
                bool skillValidation = SkillValidation(false);
                effect.SetActive(skillValidation);//セット可能だったらアクティブ
                cannotEquipText.SetActive(!skillValidation);//装備不可の場合はテキスト表示
                equipText.SetActive(skillEquip);//装備してたら『装備中』の表示
                if (skillEquip) cannotEquipText.SetActive(false);//装備中が表示している場合は装備不可の表示を非アクティブ
            }
        }
        catch (UnassignedReferenceException ex)
        {
            // 未割り当てのオブジェクトが使用された場合の処理
            Debug.LogError("UnassignedReferenceException: " + ex.Message);
            Debug.Log(gameObject.name);
        }
        catch (Exception ex)
        {
            // その他の例外が発生した場合の処理
            Debug.LogError("Exception: " + ex.Message);
        }
    }

    public void SetSkillCard(string unique_id)
    {
        GameManager.SkillInfo skill = GameManager.instance.mySkills.Find(skill => skill.unique_id == unique_id);
        if(skill != null)
        {
            skillUniqueId = skill.unique_id;
            setCharaUniqueId = skill.setCharaUniqueId;
            skillNum = skill.skillNum;
            skillCost = skill.cost;
            costText.text = skill.cost.ToString();
            levelText.text = "Lv." + skill.level;
            skillImage.sprite = GameManager.instance.skillInfo[skill.skillNum - 1].skillImage;
        }
        else
        {
            skillUniqueId = "none";
            skillNum = 0;
            skillCost = 0;
            costText.text = "";
            levelText.text = "";
            skillImage.sprite = null;
        }
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        GameManager.instance.skillWindow.selectSkillUniqueId = skillUniqueId;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        //ボタンのアニメーション処理を入れる
        transform.DOScale(Vector3.one * 0.9f, 0.1f)
            .OnComplete(() =>
            {
                // 0.2秒かけて元の倍率(1倍)に戻す
                transform.DOScale(1, 0.1f);
            });

        //選択されているスロット番号にスキルのunique_idを入れる
        //スキルスロットが選択されているかどうか判定
        if (isSkillSlot)
        {
            if(GameManager.instance.beforeSelectSkill != null)
            {
                string temp_unique_id = GameManager.instance.beforeSelectSkill.skillUniqueId;
                //もし2回同じスロットをタップしたら外れるようにする
                if (skillUniqueId == temp_unique_id)
                {
                    SetSkillCard("none");
                    GameManager.instance.beetleManager.selectBeetle.selectSkill[GameManager.instance.selectSlotNumber] = skillUniqueId;
                    //スロット選択状態を解除してUIを更新する
                    GameManager.instance.isSelectSlot = false;
                    GameManager.instance.beforeSelectSkill = null;
                    GameManager.instance.uIManager.HomeSkillUIUpdate();
                    //選択中の画像を非アクティブにする
                    GameManager.instance.skillWindow.SelectSlotImagesInActive();

                    //ゲームマネージャーの変数を変更して生成されているオブジェクトたちが変更を感知しオブジェクトのアクティブを切り替える
                    GameManager.instance.slotChanged.Value++;
                    return;
                }

                //後に選択されたスロットの場合
                //最初に選択された方に自身のunique_idを入れる
                GameManager.instance.beetleManager.selectBeetle.selectSkill[GameManager.instance.selectSlotNumber] = skillUniqueId;
                //後に選択された方に最初に選択されたunique_idを入れる
                GameManager.instance.beetleManager.selectBeetle.selectSkill[slotNumber] = temp_unique_id;

                //スロット選択状態を解除してUIを更新する
                GameManager.instance.isSelectSlot = false;

                //自身と交換先のパラメータをセットする
                GameManager.instance.beforeSelectSkill.SetSkillCard(skillUniqueId);
                SetSkillCard(temp_unique_id);

                GameManager.instance.uIManager.HomeSkillUIUpdate();
                //選択中の画像を非アクティブにする
                GameManager.instance.skillWindow.SelectSlotImagesInActive();

                //最初に選択されたskillCardを空にする
                GameManager.instance.beforeSelectSkill = null;
            }
            else
            {
                GameManager.instance.isSelectSlot = true;
                GameManager.instance.beforeSelectSkill = this;
                GameManager.instance.selectSlotNumber = slotNumber;
                //選択している部分にエフェクトを入れる
                GameManager.instance.skillWindow.skillSlotSelectImages[slotNumber].SetActive(true);
                //アニメーションも再生する
                GameManager.instance.skillWindow.skillSlotSelectImages[slotNumber].GetComponent<Animator>().Play("SkillSlotSelectAnime");
            }
        }
        else
        {
            if (GameManager.instance.beforeSelectSkill == null) return;
            ChangeSkillSlot();
        }

        //ゲームマネージャーの変数を変更して生成されているオブジェクトたちが変更を感知しオブジェクトのアクティブを切り替える
        GameManager.instance.slotChanged.Value++;
    }

    //長押ししたらスキル情報を表示させる
    void SkillPopupSkillParamSetAndActivate(GameManager.SkillInfo skillInfo)
    {
        //スキルが未選択の場合は処理を終了する
        if(skillInfo == null) return;

        //スキルポップアップをアクティブにする
        GameManager.instance.skillWindowPopup.SetActive(true);

        int skillNum = skillInfo.skillNum - 1;

        GameManager.instance.skillWindow.skillImage.sprite = GameManager.instance.skillInfo[skillNum].skillImage;
        GameManager.instance.skillWindow.skillName.text = GameManager.instance.skillInfo[skillNum].skillName;
        GameManager.instance.skillWindow.skillCost.text = "スキルコスト:" + skillInfo.cost;
        GameManager.instance.skillWindow.skillLevel.text = "スキルレベル:" + skillInfo.level;
        for(int i = 0; i < 4; i++)
        {
            switch (i)
            {
                case 0:
                    GameManager.instance.skillWindow.statusTexts[i].text = "<sprite=0 color=#ff0000>" + GameManager.instance.skillInfo[skillNum].statusConditions[i].statusLevel;
                    break;

                case 1:
                    GameManager.instance.skillWindow.statusTexts[i].text = "<sprite=1 color=#D96B00>" + GameManager.instance.skillInfo[skillNum].statusConditions[i].statusLevel;
                    break;

                case 2:
                    GameManager.instance.skillWindow.statusTexts[i].text = "<sprite=2 color=#008000>" + GameManager.instance.skillInfo[skillNum].statusConditions[i].statusLevel;
                    break;

                case 3:
                    GameManager.instance.skillWindow.statusTexts[i].text = "<sprite=3 color=#0050B0>" + GameManager.instance.skillInfo[skillNum].statusConditions[i].statusLevel;
                    break;

                default:
                    break;
            }
        }
        GameManager.instance.skillWindow.skillDescription.text = GameManager.instance.skillInfo[skillNum].skillDescription;
    }

    private void OnLongClick(Unit _)
    {
        GameManager.SkillInfo skillInfo = GameManager.instance.mySkills.Find(skillInfo => skillInfo.unique_id == GameManager.instance.skillWindow.selectSkillUniqueId);
        SkillPopupSkillParamSetAndActivate(skillInfo);
    }


    //スキルがセットできるか確認する処理
    //引っかかった箇所に応じてポップアップ内のテキストを変更する
    bool SkillValidation(bool textChanged = true)
    {
        //スキルスロットに設定されているスキルのunique_idと自身が違うかどうかを判定
        if(GameManager.instance.beforeSelectSkill?.skillUniqueId == skillUniqueId)
        {
            //GameManager.instance.popup.SetActive(true);
            if(textChanged) GameManager.instance.popupText.text = "そのスキルは既に選択されています";
            return false;
        }

        //すでに今選択している昆虫でセットされていないかを確認
        foreach (string unique_id in GameManager.instance.beetleManager.selectBeetle.selectSkill)
        {
            if (unique_id == skillUniqueId)
            {
                //GameManager.instance.popup.SetActive(true);
                if (textChanged) GameManager.instance.popupText.text = "そのスキルは既に選択されています";
                return false;
            }
        }

        int skillCost = 0;

        //選択されているスロットのコストを計算する
        foreach(string skill_unique_id in GameManager.instance.beetleManager.selectBeetle.selectSkill)
        {
            if(skill_unique_id.Length >= 16)
            {
                GameManager.SkillInfo skillInfo = GameManager.instance.mySkills.Find(skillInfo => skillInfo.unique_id == skill_unique_id);
                skillCost += skillInfo.cost;
            }
        }
        //選択されているスロットのUnique_idが16文字の場合、その分のコストを引く
        if (GameManager.instance.beetleManager.selectBeetle.selectSkill[GameManager.instance.selectSlotNumber].Length >= 16)
        {
            GameManager.SkillInfo skillInfo = GameManager.instance.mySkills.Find(skillInfo => skillInfo.unique_id == GameManager.instance.beetleManager.selectBeetle.selectSkill[GameManager.instance.selectSlotNumber]);
            skillCost -= skillInfo.cost;
        }

        //選択している昆虫が装備可能かどうか
        if (!GameManager.instance.beetleManager.selectBeetle.learnableSkills.Contains(skillNum))
        {
            if (textChanged) GameManager.instance.popupText.text = "このスキルを選択することができません。";
            return false;
        }

        //選択している昆虫のスキルコストは問題ないかどうか
        if (GameManager.instance.beetleManager.selectBeetle.skillCostLimit < skillCost + this.skillCost)
        {
            if (textChanged) GameManager.instance.popupText.text = "コストがオーバーしています。";
            return false;
        }


        GameManager.SkillInfo skillInfo_2 = GameManager.instance.mySkills.Find(skillInfo => skillInfo.unique_id == skillUniqueId);
        if (skillInfo_2 is null) return false;

        //スキルを習得できるレベルに達しているか
        for(int i = 0; i < 4; i++)
        {
            switch (i)
            {
                case 0:
                    if (skillInfo_2.statusConditions[i].statusLevel > GameManager.instance.beetleManager.selectBeetle.power)
                    {
                        if (textChanged) GameManager.instance.popupText.text = "このスキルをセットするために必要なレベルに達していません。\nトレーニングをしてレベルをアップしましょう。";
                        return false;
                    }
                    break;
                case 1:
                    if (skillInfo_2.statusConditions[i].statusLevel > GameManager.instance.beetleManager.selectBeetle.inter)
                    {
                        if (textChanged) GameManager.instance.popupText.text = "このスキルをセットするために必要なレベルに達していません。\nトレーニングをしてレベルをアップしましょう。";
                        return false;
                    }
                    break;
                case 2:
                    if (skillInfo_2.statusConditions[i].statusLevel > GameManager.instance.beetleManager.selectBeetle.guard)
                    {
                        if (textChanged) GameManager.instance.popupText.text = "このスキルをセットするために必要なレベルに達していません。\nトレーニングをしてレベルをアップしましょう。";
                        return false;
                    }
                    break;
                case 3:
                    if (skillInfo_2.statusConditions[i].statusLevel > GameManager.instance.beetleManager.selectBeetle.speed)
                    {
                        if (textChanged) GameManager.instance.popupText.text = "このスキルをセットするために必要なレベルに達していません。\nトレーニングをしてレベルをアップしましょう。";
                        return false;
                    }
                    break;
            }
        }

        return true;
    }


    public void ChangeSkillSlot()
    {
        //セットできない場合はポップアップを表示
        if (!SkillValidation()) GameManager.instance.popup.SetActive(true);

        //選択されているスロットのunique_idが違うものでかつ自分が装備していなかったら選択しているスロットにunique_idを入れてUIを更新する
        if (SkillValidation())
        {
            GameManager.instance.beetleManager.selectBeetle.selectSkill[GameManager.instance.selectSlotNumber] = skillUniqueId;
            GameManager.instance.beforeSelectSkill.SetSkillCard(skillUniqueId);

            GameManager.instance.uIManager.HomeSkillUIUpdate();

            GameManager.instance.Save();
        }

        //選択中の画像を非アクティブにする
        GameManager.instance.skillWindow.SelectSlotImagesInActive();
        GameManager.instance.beforeSelectSkill = null;
        GameManager.instance.isSelectSlot = false;
    }

}
