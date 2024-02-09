using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    //スキルの番号、バトル画面ロード時にここに保存されます
    public int skillNum;//いらないかもしれない
    public string unique_id;//skillinfoからfindしてその能力を反映させるために使用

    public BattleManager.Janken janken;

    public TextMeshProUGUI buttonText;

    public Button skillButton;
    public GameObject buttonEffect_1;
    public GameObject buttonEffect_2;

    //Todoスキル詳細を表示するスクリプトを書く予定

    //ボタンがアクティブになった時にボタンが有効ではない場合はエフェクトを非アクティブにする
    private void OnEnable()
    {
        if (!skillButton.interactable)
        {
            buttonEffect_1.SetActive(false);
            buttonEffect_2.SetActive(false);
        }
        else
        {
            buttonEffect_1.SetActive(true);
            buttonEffect_2.SetActive(true);
        }

        //チャージボタンではなくスキルがセットされていない場合もエフェクトを非表示にする
        if(skillNum == 0 && janken != BattleManager.Janken.charge)
        {
            buttonEffect_1.SetActive(false);
            buttonEffect_2.SetActive(false);
        }
    }
}
