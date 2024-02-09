using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillWindow : MonoBehaviour
{
    public List<GameObject> skillSlotSelectImages;

    //以下はポップアップの情報
    public Image skillImage;
    public TextMeshProUGUI skillName;
    public TextMeshProUGUI skillCost;
    public TextMeshProUGUI skillLevel;
    public List<TextMeshProUGUI> statusTexts;
    public TextMeshProUGUI skillDescription;

    public string selectSkillUniqueId;


    public void SkillWindowBackButton()
    {
        GameManager.instance.isSelectSlot = false;
        GameManager.instance.beforeSelectSkill = null;
        //スキル詳細のポップアップが表示されていたらそれも閉じる
        SelectSlotImagesInActive();
        //GameManager.instance.Save();
    }

    public void SelectSlotImagesInActive()
    {
        //選択しているスロットを非アクティブにする
        foreach (GameObject selectObj in skillSlotSelectImages)
        {
            selectObj.SetActive(false);
        }
    }
}
