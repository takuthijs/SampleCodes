using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;
using UniRx;
using UniRx.Triggers;
using System.Linq;
using UnityEngine.SocialPlatforms;

public class LobbyCharacter : MonoBehaviour,IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public int unique_id;
    public Image characterImg;
    public SpriteRenderer characterSprite;
    public TextMeshProUGUI levelText;
    public Slider expSlider;
    public GameObject expSliderObj;
    public TextMeshProUGUI sliderText;
    public TextMeshProUGUI hp;
    public TextMeshProUGUI attack;
    public TextMeshProUGUI defence;
    public TextMeshProUGUI skillTime;
    public GameObject status;
    public GameManager.GetCharaParam getCharaParam;
    public Button button;
    public LobbyCharacterRandomMovement lobbyCharacterRandomMovement;

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
    }

    private void Update()
    {
        //ゲームが重ければ再検討、レイヤーの順序を変更する処理
        if (!status.activeInHierarchy) gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
    }

    private void UpdateUI()
    {
        GameManager.GetCharaParam charaParam = GameManager.instance.getCharaParams.Find(x => x.unique_id == unique_id);
        //DateTime setPartyTime = DateTime.ParseExact(charaParam.setPartyTime, "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);//こっちだとちゃんとパースされない
        DateTime setPartyTime = DateTime.Parse(charaParam.setPartyTime);
        TimeSpan timeSinceSetParty = DateTime.Now - setPartyTime;
        float elapsedSeconds = (float)timeSinceSetParty.TotalSeconds;
        int gainedExperience = Mathf.FloorToInt(elapsedSeconds * GameManager.instance.experienceGrowthRate * GameManager.instance.GetFeedEfficacy(charaParam.feedNum));//餌の効果をここで加える
        int poopQuantity = Mathf.FloorToInt((int)timeSinceSetParty.TotalSeconds / 60);//60秒で一つ糞を生成して、糞の配列が20個以下であれば糞を生成する

        // ステータスの表示と経験値のスライダーを最前面に表示するための処理
        Canvas statusCanvas = status.GetComponent<Canvas>();
        Canvas sliderCanvas = expSliderObj.GetComponent<Canvas>();
        // レイヤー名を指定してレイヤーを選択
        (statusCanvas.sortingLayerName, sliderCanvas.sortingLayerName) = ("Front_UI", "Front_UI");

        //タップした時糞を生成(いらないかもなぁ...)
        if (poopQuantity > 3) poopQuantity = 3;
        if (GameManager.instance.monsterPoops.Count < 20)
        {
            for (int poopIndex = 0; poopIndex < poopQuantity; poopIndex++)
            {
                if (GameManager.instance.monsterPoops.Count < 20)
                {
                    GameObject monsterPoop = Instantiate(GameManager.instance.monsterPoop, GameManager.instance.lobbyCharacterParent.transform);

                    monsterPoop.transform.position = new Vector2(gameObject.transform.position.x - 1, gameObject.transform.position.y);
                    GameManager.instance.monsterPoops.Add(monsterPoop);
                }
            }
        }

        if (gainedExperience < 0) gainedExperience = 2147483647;
        GameManager.instance.GainExperience(charaParam, gainedExperience);
        //新しい時間にセットしなおす
        charaParam.setPartyTime = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss tt");

        levelText.text = getCharaParam.currentLevel.ToString();
        expSlider.maxValue = charaParam.nextExp;
        expSlider.value = charaParam.exp;
        sliderText.text = charaParam.exp+" / " +charaParam.nextExp;

        hp.text = charaParam.HP.ToString();
        attack.text = charaParam.Attack.ToString();
        defence.text = charaParam.Defence.ToString();
    }

    public void SetLobbyCharacterParam()
    {
        getCharaParam = GameManager.instance.getCharaParams.Find(x => x.unique_id == unique_id);
        levelText.text = getCharaParam.currentLevel.ToString();
        expSlider.maxValue = getCharaParam.nextExp;
        expSlider.value = getCharaParam.exp;

        characterImg.sprite = getCharaParam.sprite;
        characterSprite.sprite = getCharaParam.sprite;

        sliderText.text = getCharaParam.exp + " / " + getCharaParam.nextExp;
        hp.text = getCharaParam.HP.ToString();
        attack.text = getCharaParam.Attack.ToString();
        defence.text = getCharaParam.Defence.ToString();
        skillTime.text = getCharaParam.skillTime.ToString();
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
       
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        
    }

    private void OnLongClick(Unit _)
    {
        Debug.Log("ステータス表示");
        UpdateUI();

        gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z + 1);
        
        status.SetActive(true);
        expSliderObj.SetActive(true);
        lobbyCharacterRandomMovement.isTouch = true;
    }

    //指を離した時
    public void OnPointerUp(PointerEventData pointerEventData)
    {
        status.SetActive(false);
        expSliderObj.SetActive(false);
        lobbyCharacterRandomMovement.isTouch = false;
    }
}
