using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.EventSystems;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class MaxNumberTapController : MonoBehaviour
{
    public static MaxNumberTapController instance;

    public GameObject maxNumberUI;
    public GameObject maxNumberParent;
    public GameObject maxNumEntity;

    public GameObject flame;

    public List<int> numbers = new List<int>();

    public int skillCost;

    public int successCount;

    void Start()
    {
        instance = this;
    }

    public void NumbersGenerate()
    {
        //numbers配列を削除
        numbers.Clear();
        //生成されている子オブジェクトを全て削除する
        foreach (Transform numberObj in maxNumberParent.transform)
        {
            Destroy(numberObj.gameObject);
        }

        for (int i = 0; i < 3; i++)
        {
            RandomNumber();
        }

        for (int i = 0; i < 3; i++)
        {
            //GameObjectを生成
            GameObject cloneNumberObj = Instantiate(maxNumEntity, maxNumberParent.transform);
            cloneNumberObj.GetComponent<NumberData>().num = numbers[i];
            cloneNumberObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = numbers[i].ToString();

            //数字がタップされた時に実行されるイベントトリガー
            EventTrigger skillTrigger = cloneNumberObj.GetComponent<EventTrigger>();
            EventTrigger.Entry skillTriggerEntry = new EventTrigger.Entry();
            skillTriggerEntry.eventID = EventTriggerType.PointerClick;
            skillTriggerEntry.callback.AddListener(_ =>
            {
                if (cloneNumberObj.GetComponent<NumberData>().num == numbers.Max())
                {
                    Debug.Log("最大値");
                    successCount++;
                    GameManager.instance.conboCount++;
                    GameManager.instance.conboText.text = GameManager.instance.conboCount.ToString() + "combo!!!";
                    if(successCount == skillCost)
                    {
                       StartCoroutine(NumberTapUIFade());

                        //HPゲージの処理
                        //GameManager.instance.HpGageProcess();
                        //スキルの実行
                        Skills.instance.SkillExe(GameManager.instance.foucusChara.GetComponent<CharaParam>().skillNum);

                        //ミニゲームのクリア回数を追加
                        GameManager.instance.miniGamePlayInfo["MaxNumberTap"]++;
                    }
                    else
                    {
                        NumbersGenerate();
                    }
                }
                else
                {
                    GameManager.instance.conboCount = 0;
                    GameManager.instance.conboText.text = GameManager.instance.conboCount.ToString() + "combo";
                    //HPゲージの処理
                    //ミスなのでプレイヤーのHPを減らす
                    GameManager.instance.playerSlider.DOValue(GameManager.instance.playerCurrentHP - GameManager.instance.playerHP * 0.1f, 1f);
                    GameManager.instance.playerCurrentHP = GameManager.instance.playerCurrentHP - GameManager.instance.playerHP * 0.1f;
                    GameManager.instance.PlayerHPText.text = GameManager.instance.playerCurrentHP + " / " + GameManager.instance.playerHP;
                    GameManager.instance.ShakeGameobject(GameManager.instance.playerHpGage);//HPゲージのシェイクアニメーション
                    GameManager.instance.HitBlink(flame);//ミスした時のアニメーション

                    //もし自分が戦闘不能（HPが0）だったら
                    if (GameManager.instance.playerCurrentHP < 0)
                    {
                        GameManager.instance.playerCurrentHP = 0;
                        GameManager.instance.playerSlider.value = 0;
                        GameManager.instance.PlayerHPText.text = GameManager.instance.playerCurrentHP + " / " + GameManager.instance.playerHP;
                    }
                }
            });
            skillTrigger.triggers.Add(skillTriggerEntry);
        }
    }

    void RandomNumber()
    {
        int i = Random.Range(0, 100);
        if(numbers.Exists(x => x.Equals(i)))
        {
            RandomNumber();
        }
        else
        {
            numbers.Add(i);
        }
    }

    IEnumerator NumberTapUIFade()
    {
        int i = 0;
        CanvasGroup canvasGroup = maxNumberUI.GetComponent<CanvasGroup>();
        while (canvasGroup.alpha > 0)
        {
            i++;
            canvasGroup.alpha -= 0.1f;
            yield return new WaitForSeconds(0.01f);
            if (i == 100) break;
        }
        maxNumberUI.SetActive(false);
        canvasGroup.alpha = 1;
    }
}
