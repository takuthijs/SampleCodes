using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Analytics;
using Firebase;

public class PrivacyScript : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject privacyCanvas;
    public Button agreeButton;

    public enum Language
    {
        english,
        japanese
    }

    public Language language = Language.english;

    public TextMeshProUGUI privacyTitleText;
    public TextMeshProUGUI privacyText;
    public TextMeshProUGUI privacyButtonText;
    public TextMeshProUGUI AgreeButtonText;

    private string englishPrivacyTitle = "Privacy Policy";
    private string japanesePrivacyTitle = "プライバシーポリシー";

    private string englishPrivacy = "";
    private string japanesePrivacy = "";

    private string ppEnglishButtonText = "Privacy Policy";
    private string ppJapaneseButtonText = "プライバシーポリシー";

    private string englishAgreeButton = "Agree";
    private string japaneseAgreeButton = "同意する";

    private void Start()
    {
        if (string.IsNullOrEmpty(gameManager.privacy))
        {
            privacyCanvas.SetActive(true);
        }
        else
        {
            privacyCanvas.SetActive(false);
        }
        agreeButton.interactable = false;

        //起動時端末設定が日本語だった場合
        if (Application.systemLanguage == SystemLanguage.Japanese)
        {
            language = Language.japanese;
            privacyTitleText.text = japanesePrivacyTitle;
            privacyText.text = japanesePrivacy;
            privacyButtonText.text = ppJapaneseButtonText;
            AgreeButtonText.text = japaneseAgreeButton;
        }
        else
        {
            language = Language.english;
            privacyTitleText.text = englishPrivacyTitle;
            privacyText.text = englishPrivacy;
            privacyButtonText.text = ppEnglishButtonText;
            AgreeButtonText.text = englishAgreeButton;
        }
    }

    public void onClickPrivacy()
    {
        agreeButton.interactable = true;
    }

    public void AgreeButton()
    {
        privacyCanvas.SetActive(false);
        PlayerPrefs.SetString(GameManager.KEY_SAVE_PRIVACY, "Agree");
        PlayerPrefs.Save();
        //Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
        });
    }

    public void ChangeLanguage()
    {
        if(language == Language.english)
        {
            language = Language.japanese;
            privacyTitleText.text = japanesePrivacyTitle;
            privacyText.text = japanesePrivacy;
            privacyButtonText.text = ppJapaneseButtonText;
            AgreeButtonText.text = japaneseAgreeButton;
        }
        else
        {
            language = Language.english;
            privacyTitleText.text = englishPrivacyTitle;
            privacyText.text = englishPrivacy;
            privacyButtonText.text = ppEnglishButtonText;
            AgreeButtonText.text = englishAgreeButton;
        }
    }
}
