using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.UI;


public class NumberTapController : MonoBehaviour
{
    public static NumberTapController instance;
    public GameObject tapScreen;
    public GameObject flame;

    List<Vector2> rondomPositions = new List<Vector2>();//生成されたランダムなポジションを保存
    List<int> numbers = new List<int>();
    bool positionGenerateLoop = false;
    bool miss = false; //タップをミスったかどうか

    [SerializeField]
    [Tooltip("生成するGameObject")]
    private GameObject numberPrefab;
    [SerializeField]
    [Tooltip("生成する親オブジェクト")]
    private GameObject numberParent;
    [SerializeField]
    [Tooltip("生成する範囲A")]
    private Transform rangeA;
    [SerializeField]
    [Tooltip("生成する範囲B")]
    private Transform rangeB;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        GameManager.instance.conboText.text = 0.ToString() + "combo";
    }

    public void NumberScreenActive(GameObject chara)
    {
        if (chara.GetComponent<CharaParam>().slotObj.GetComponent<Slot>().skillActive)
        {
            //2回目のタップの場合すでに格納されているので一旦クリア
            rondomPositions.Clear();
            numbers.Clear();

            //ミス情報もリセット
            miss = false;

            //生成されている子オブジェクトを全て削除する
            foreach (Transform numberObj in numberParent.transform)
            {
                Destroy(numberObj.gameObject);
            }

            //タップ画面をアクティブにする
            tapScreen.SetActive(true);

            //スキルのコスト分重ならない用にナンバーボタンを生成する
            for (int i = 0; i < chara.GetComponent<CharaParam>().skillCost; i++)
            {
                numbers.Add(i + 1);

                // GameObjectを生成
                GameObject cloneNumberObj = Instantiate(numberPrefab, numberParent.transform);
                cloneNumberObj.GetComponent<NumberData>().num = i + 1;

                cloneNumberObj.GetComponent<NumberData>().numberText.text = (i + 1).ToString();
                positionGenerateLoop = true;
                cloneNumberObj.transform.position = RamdomPosGenerater();

                //数字がタップされた時に実行されるイベントトリガー
                EventTrigger skillTrigger = cloneNumberObj.GetComponent<EventTrigger>();
                EventTrigger.Entry skillTriggerEntry = new EventTrigger.Entry();
                skillTriggerEntry.eventID = EventTriggerType.PointerClick;
                skillTriggerEntry.callback.AddListener(_ =>
                {
                    if(numbers.Min() == cloneNumberObj.GetComponent<NumberData>().num)
                    {
                        cloneNumberObj.GetComponent<NumberData>().completeImage.SetActive(true);
                        numbers.RemoveAt(0);
                        GameManager.instance.conboCount++;//コンボを加算
                        GameManager.instance.conboText.text = GameManager.instance.conboCount.ToString()+"combo!!!";
                        if (numbers.Count == 0)
                        {
                            //タップ画面を非アクティブにする
                            StartCoroutine(NumberTapUIFade());

                            //ミスしてなかったらエフェクト表示
                            if (!miss)
                            {
                                Vector3 originalPosition = GameManager.instance.niceComboObj.transform.position;
                                Sequence sequence = DOTween.Sequence()
                                .OnStart(() =>
                                {
                                    GameManager.instance.niceComboFrame.color = new Color(1,1,1,0);
                                    GameManager.instance.niceComboText.color = new Color(1, 1, 1, 0);
                                    GameManager.instance.niceComboObj.SetActive(true);
                                })
                                .Append(GameManager.instance.niceComboFrame.DOFade(1.0f, 0.5f).SetEase(Ease.OutCubic))
                                .Join(GameManager.instance.niceComboText.DOFade(1.0f, 0.5f).SetEase(Ease.OutCubic))
                                .Join(GameManager.instance.niceComboObj.transform.DOMoveY(originalPosition.y + 0.4f, 0.5f))
                                .OnComplete(() =>
                                {
                                    StartCoroutine(TweenNiceComboFade(originalPosition));
                                }); ;
                                sequence.Play();
                            }

                            //ミニゲームのクリア回数を追加
                            GameManager.instance.miniGamePlayInfo["NumberTap"]++;

                            //スキルの実行
                            Skills.instance.SkillExe(chara.GetComponent<CharaParam>().skillNum);

                            //HPゲージの処理
                            //GameManager.instance.HpGageProcess();
                        }
                    }
                    else
                    {
                        miss = true;
                        if (numbers.Min() > cloneNumberObj.GetComponent<NumberData>().num) return;
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
    }

    public Vector2 RamdomPosGenerater()
    {
        Vector2 position;
        
        if (rondomPositions.Count != 0)
        {
            //新しい要素を追加するためのリスト
            List<Vector2> numberPoints = new List<Vector2>();

            //Distanceを確認するリスト
            List<float> distances = new List<float>();

            foreach (Vector2 rondomPosition in rondomPositions)
            {
                numberPoints.Add(rondomPosition);
            }

            int i = 0;
            while (positionGenerateLoop)
            {
                //300回でダメだったらもうそこで終わり
                i++;
                Debug.Log("実行");
                if(i == 300)
                {
                    positionGenerateLoop = false;
                }
                //rangeAとrangeBのx座標の範囲内でランダムな数値を作成
                float x = Random.Range(rangeA.position.x, rangeB.position.x);
                //rangeAとrangeBのy座標の範囲内でランダムな数値を作成
                float y = Random.Range(rangeA.position.y, rangeB.position.y);

                position = new Vector2(x, y);

                foreach (Vector2 numberPoint in numberPoints)
                {
                    distances.Add(Vector2.Distance(numberPoint, position));
                }

                if (distances.All(i => i > 2))
                {
                    //foreach (float distance in distances)
                    //{
                    //    Debug.Log(distance);
                    //}
                    rondomPositions.Add(position);
                    positionGenerateLoop = false;
                }
                distances.Clear();//distancesは毎回比べられるので都度リセット
            }
            numberPoints.Clear();//格納済のランダムポジションはリターンする前にクリア
            return rondomPositions.Last();
        }
        else
        {
            //rangeAとrangeBのx座標の範囲内でランダムな数値を作成
            float x = Random.Range(rangeA.position.x, rangeB.position.x);
            //rangeAとrangeBのy座標の範囲内でランダムな数値を作成
            float y = Random.Range(rangeA.position.y, rangeB.position.y);
            rondomPositions.Add(new Vector2(x, y));
            return rondomPositions.Last();
        }
    }

    IEnumerator NumberTapUIFade()
    {
        int i = 0;
        CanvasGroup canvasGroup = tapScreen.GetComponent<CanvasGroup>();
        while (canvasGroup.alpha > 0)
        {
            i++;
            canvasGroup.alpha -= 0.1f;
            yield return new WaitForSeconds(0.01f);
            if (i == 100) break;
        }
        tapScreen.SetActive(false);
        canvasGroup.alpha = 1;
    }

    IEnumerator TweenNiceComboFade(Vector3 position)
    {
        yield return new WaitForSeconds(0.5f);
        GameManager.instance.niceComboFrame.DOFade(0f, 0.5f).SetEase(Ease.OutCubic);
        GameManager.instance.niceComboText.DOFade(0f, 0.5f).SetEase(Ease.OutCubic);
        yield return new WaitForSeconds(0.5f);
        GameManager.instance.niceComboObj.transform.position = position;
        GameManager.instance.niceComboObj.SetActive(false);
    }
}