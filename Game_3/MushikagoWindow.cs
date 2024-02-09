using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MushikagoWindow : MonoBehaviour
{
    //以下はポップアップの情報
    //public Image skillImage;
    public TextMeshProUGUI beetleName;
    public TextMeshProUGUI growCountText;
    public List<TextMeshProUGUI> statusTexts;
    public List<Image> selectSkillImage;
    public List<TextMeshProUGUI> skillCostTexts;
    public TextMeshProUGUI skillCostText;


    public string selectBeetleUniqueId;//カブトムシのユニークID

    public void ChangeSelectBeetle()
    {
        //現在のセレクトビートルの情報を一旦セーブ
        GameManager.instance.Save();

        //セレクトビートルを入れ替える入れ替える
        BeetleManager.Beetle foundBeetle = GameManager.instance.beetleManager.myBeetles.Find(beetleElement => beetleElement.unique_id == GameManager.instance.selectBeetle_uniqueId);
        //BeetleSO beetleSO = GameManager.instance.BeetleManager.beetleSos.Find(beetleElement => beetleElement.beetle_id == foundBeetle.beetle_id);
        BeetleManager.Beetle newSelectBeetle = GameManager.instance.NewBeetle(foundBeetle);

        //新しく生成した昆虫をselectBeetleに入れ直す
        GameManager.instance.beetleManager.selectBeetle = newSelectBeetle;

        //Addする前にクリア
        GameManager.instance.selectBeetleSkills.selectSkills.Clear();
        for (int i = 0; i < 3; i++)
        {
            GameManager.instance.beetleManager.selectBeetle.selectSkill[i] = newSelectBeetle.selectSkill[i];
            GameManager.instance.selectBeetleSkills.selectSkills.Add(GameManager.instance.mySkills.Find(myskill => myskill.unique_id == newSelectBeetle.selectSkill[i]));
        }
        GameManager.instance.beetleManager.selectBeetle.dateTime = newSelectBeetle.dateTime;

        //スキルコストと覚えることが可能なスキルをSOを見て入れ直す
        GameManager.instance.beetleManager.selectBeetle.skillCostLimit = newSelectBeetle.skillCostLimit;
        GameManager.instance.beetleManager.selectBeetle.learnableSkills = newSelectBeetle.learnableSkills;

        //ゲームマネージャーのslotSkillCardsを入れ直す
        //スキルウィンドウのslotにセットされているSkillCardにもunique_idをセットする
        for (int i = 0; i < 3; i++)
        {
            GameManager.instance.slotSkillCards[i].SetSkillCard(GameManager.instance.beetleManager.selectBeetle.selectSkill[i]);
        }

        //中央のオブジェクトを切り替える
        GameManager.instance.SelectBeetleGenerate();

        //UIをアップデートする
        GameManager.instance.uIManager.HomeUIUpdate();
        GameManager.instance.uIManager.HomeSkillUIUpdate();

        //入れ替えた後再度セーブ
        GameManager.instance.Save();
    }
}
