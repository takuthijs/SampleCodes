using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class RewardButton : MonoBehaviour
{
    public Button myButton;
    public TextMeshProUGUI cooldownText;

    private DateTime lastPressTime;
    private TimeSpan cooldownDuration = TimeSpan.FromMinutes(1);



    void Start()
    {
        //myButton.onClick.AddListener(OnButtonClick);
        LoadLastPressTime();
        UpdateButtonState();
    }

    void Update()
    {
        //定期的にUIを更新
        UpdateButtonState();
    }

    public void OnButtonClick()
    {
        //クリックで広告をみてアイテムをゲットしますか、等のメッセージを表示させる
        //はいボタンを押した時にこの処理をトリガーさせる
        GameManager.instance.adMobScript.ShowAd(() =>
        {
            if (!GameManager.instance.networkConection) return;
            if (CanPressButton())
            {
                Debug.Log("Button Pressed!");
                lastPressTime = DateTime.Now;
                SaveLastPressTime();
                UpdateButtonState();

                //ランダムでオブジェクトをゲットできる処理を追加
                int itemId = GameManager.instance.RandomNumber();

                //まだ壊したことのないアイテムの場合はGetItemListに追加
                if (GameManager.instance.getEquipItems.Find(equipItem => equipItem.itemId == itemId) == null)
                {
                    GameManager.EquipItem newEquipItem = new GameManager.EquipItem
                    {
                        itemId = itemId,
                        brokenCount = 1
                    };
                    GameManager.instance.getEquipItems.Add(newEquipItem);

                    //Addした後にソートする
                    GameManager.instance.SortItemList();
                    //アイテムカードを全削除
                    GameManager.instance.RemoveAllChildrenItemCard(GameManager.instance.itemCardParent);
                    //アイテムカードを再生成する
                    GameManager.instance.ItemCardGenerate();
                }
                else
                {
                    //すでに入手済みの場合は壊したカウントをプラス1
                    GameManager.EquipItem newEquipItem = GameManager.instance.getEquipItems.Find(equipItem => equipItem.itemId == itemId);
                    newEquipItem.brokenCount++;
                }

                GameManager.instance.Save();

                GameManager.instance.Fade(() =>
                {
                    //ガチャ結果のウィンドウを表示する
                    GameManager.instance.gachaResultWindow.SetActive(true);

                    //入手したアイテム画面を表示
                    GameManager.instance.gachaResultImage.sprite = GameManager.instance.equipItems[itemId].GetComponent<PowerObj>().equipImage;
                },true);
            }
            else
            {
                Debug.Log("Button on cooldown!");
                GameManager.instance.soundManager.PlaySE(1);
            }

            //Firebase.Analytics.FirebaseAnalytics.LogEvent("GameEndAds");

            //広告をロード
            GameManager.instance.adMobScript.LoadAd();
            
        });

    }

    bool CanPressButton()
    {
        TimeSpan timeSinceLastPress = DateTime.Now - lastPressTime;
        return timeSinceLastPress >= cooldownDuration;
    }

    void UpdateButtonState()
    {
        myButton.interactable = CanPressButton();
        if (myButton.interactable)
        {
            //buttonText.text = "Press Me!";
            cooldownText.text = "Ready!";

        }
        else
        {
            //buttonText.text = "Cooldown...";
            TimeSpan remainingTime = cooldownDuration - (DateTime.Now - lastPressTime);
            cooldownText.text = $"{remainingTime.Minutes:D2}:{remainingTime.Seconds:D2}";
        }
    }

    void SaveLastPressTime()
    {
        PlayerPrefs.SetString("LastPressTime", lastPressTime.ToString());
    }

    void LoadLastPressTime()
    {
        string savedTime = PlayerPrefs.GetString("LastPressTime", string.Empty);
        if (!string.IsNullOrEmpty(savedTime))
        {
            lastPressTime = DateTime.Parse(savedTime);
        }
    }
}
