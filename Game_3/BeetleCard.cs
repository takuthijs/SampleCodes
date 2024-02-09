using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;

public class BeetleCard : MonoBehaviour, IPointerClickHandler
{
    public string beetleToken;//ランキングの取得に使用
    //public int rank;
    public int rate;
    public string unique_id;
    public int beetle_id;
    public string beetleName;
    public GameManager.GrowStatus beetleType;
    public int growCount;
    public int power;
    public int inter;
    public int guard;
    public int speed;
    public List<string> selectSkill = new List<string>();//セットしているスキル 順番はグー,チョキ,パーの順番
    //public List<int> learnableSkills = new List<int>();//習得可能なスキル
    //public int skillCostLimit;
    public string dateTime;

    public Image beetleImage;
    public TextMeshProUGUI typeIcon;
    public TextMeshProUGUI growCountText;

    public void SetBeetleCard(string unique_id)
    {
        BeetleManager.Beetle beetle = GameManager.instance.beetleManager.myBeetles.Find(beetleElement => beetleElement.unique_id == unique_id);
        this.unique_id = beetle.unique_id;
        beetleToken = beetle.beetleToken;
        rate = beetle.rate;
        beetle_id = beetle.beetle_id;
        beetleName = beetle.beetleName;
        beetleImage.sprite = GameManager.instance.beetleManager.beetleSos[beetle_id - 1].beetleCardImage;
        //Debug.Log("set");
        beetleType = beetle.beetleType;
        growCount = beetle.growCount;
        power = beetle.power;
        inter = beetle.inter;
        guard = beetle.guard;
        speed = beetle.speed;

        //何回もAddされないように一度クリアする
        selectSkill.Clear();
        //一旦selectSkillにnoneを入れておく
        for(int i = 0; i < 3; i++)
        {
            selectSkill.Add("none");
        }
        //選択しているスキルのunique_idを入れる
        //Debug.Log(beetle.selectSkill.Count);
        for (int i = 0; i < beetle.selectSkill.Count; i++)
        {
            if (i > 2) break;//3つ以上のスキルが選択されている場合はfor文を終わる
            selectSkill[i] = beetle.selectSkill[i];
        }

        dateTime = beetle.dateTime;

        beetleImage.sprite = beetle.beetleCardImage;
        typeIcon.text = GameManager.instance.BeetleTypeIcon(beetleType);
        growCountText.text = beetle.growCount + "/100";
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        //ボタンのアニメーション処理を入れる
        //ダウンとアップでアニメーションをいじった方がいいかも？
        transform.DOScale(Vector3.one * 0.9f, 0.1f)
            .OnComplete(() =>
            {
                // 0.2秒かけて元の倍率(1倍)に戻す
                transform.DOScale(1, 0.1f);
            });
        //ゲームマネージャーに選択中の昆虫のunique_idを設定する
        GameManager.instance.selectBeetle_uniqueId = unique_id;

        BeetleSO beetleSO = GameManager.instance.beetleManager.beetleSos.Find(beetle => beetle.beetle_id == beetle_id);
        BeetleManager.Beetle beetle = GameManager.instance.beetleManager.myBeetles.Find(beetle => beetle.unique_id == GameManager.instance.selectBeetle_uniqueId);

        //生成されている育成カードを削除する
        foreach (Transform n in GameManager.instance.beetlePopupObjParent.transform)
        {
            if(n.gameObject.tag == "BeetleInfoPopupObj")
            {
                GameObject.Destroy(n.gameObject);
            }
        }

        //自身のユニークIDでfindしたゲームオブジェクトを生成
        GameObject beetleObj = Instantiate(beetleSO.model,GameManager.instance.beetlePopupObjParent.transform);

        //育てているスケール分サイズを変更する
        BeetleScaler beetleScaler = beetleObj.GetComponent<BeetleScaler>();
        if (beetleScaler is not null)
        {
            beetleScaler.power = beetle.power;
            beetleScaler.inter = beetle.inter;
            beetleScaler.guard = beetle.guard;
            beetleScaler.speed = beetle.speed;
            beetleScaler.ScaleChange();
        }

        beetleObj.tag = "BeetleInfoPopupObj";
        //レイヤーも変える
        GameManager.instance.SetLayerRecursively(beetleObj, 6);

        GameManager.instance.mushikagoWIndow.beetleName.text = GameManager.instance.BeetleTypeIcon(beetle.beetleType)+beetleName;
        GameManager.instance.mushikagoWIndow.growCountText.text = "育成回数:" + growCount + "/100";
        for (int i = 0; i < 4; i++)
        {
            switch (i)
            {
                case 0:
                    GameManager.instance.mushikagoWIndow.statusTexts[i].text = "<sprite=0>" + "<color=#ffffff>" + power.ToString();
                    if(beetleType == GameManager.GrowStatus.power)
                        GameManager.instance.mushikagoWIndow.statusTexts[i].text = "<sprite=0 color=#ff0000>" + "<color=#FF9800>" + power;
                    break;

                case 1:
                    GameManager.instance.mushikagoWIndow.statusTexts[i].text = "<sprite=1>" + "<color=#ffffff>" + inter.ToString();
                    if (beetleType == GameManager.GrowStatus.inte)
                        GameManager.instance.mushikagoWIndow.statusTexts[i].text = "<sprite=1 color=#D96B00>" + "<color=#FF9800>" + inter;
                    break;

                case 2:
                    GameManager.instance.mushikagoWIndow.statusTexts[i].text = "<sprite=2>" + "<color=#ffffff>" + guard.ToString();
                    if (beetleType == GameManager.GrowStatus.guard)
                        GameManager.instance.mushikagoWIndow.statusTexts[i].text = "<sprite=2 color=#008000>" + "<color=#FF9800>" + guard;
                    break;

                case 3:
                    GameManager.instance.mushikagoWIndow.statusTexts[i].text = "<sprite=3>" + "<color=#ffffff>" + speed.ToString();
                    if (beetleType == GameManager.GrowStatus.speed)
                        GameManager.instance.mushikagoWIndow.statusTexts[i].text = "<sprite=3 color=#0050B0>" + "<color=#FF9800>" + speed;
                    break;

                default:
                    break;
            }
        }

        //選択中のスキルの画像も入れる
        int skillCost = 0;

        for (int i = 0; i < 3; i++)
        {
            GameManager.SkillInfo skill = GameManager.instance.mySkills.Find(skillElement => skillElement.unique_id == selectSkill[i]);
            if (skill == null)
            {
                GameManager.instance.mushikagoWIndow.selectSkillImage[i].sprite = null;
                GameManager.instance.mushikagoWIndow.skillCostTexts[i].text = "";
            }
            else
            {
                GameManager.instance.mushikagoWIndow.selectSkillImage[i].sprite = skill.skillImage;
                GameManager.instance.mushikagoWIndow.skillCostTexts[i].text = skill.cost.ToString();
                skillCost += skill.cost;
            }
        }

        //スキルコストを入れる
        GameManager.instance.mushikagoWIndow.skillCostText.text = skillCost + "/" + beetleSO.skillCostLimit;

        //昆虫の情報を入れたら昆虫の情報をアクティブにする
        GameManager.instance.beetleInfoPopup.SetActive(true);
    }
}
