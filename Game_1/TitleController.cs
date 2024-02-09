using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using TMPro;

public class TitleController : MonoBehaviour
{
    public GameObject nameInputPopUp;
    public TextMeshProUGUI userName;
    public Button okButton;

    public void SaveDataChack()
    {
        if (!File.Exists(GameManager.instance.saveManager.filePath))
        {
            nameInputPopUp.SetActive(true);
        }
        else
        {
            LoadingManager.instance.LoadNextScene("LobbyScene");
        }
    }

    public void NamaChack()
    {
        if (userName.text.Length > 1)
        {
            UIManager.instance.titleUI.SetActive(false);
            nameInputPopUp.SetActive(false);
            GameManager.instance.playerName = userName.text;
            GameManager.instance.playerNameText.text = userName.text;
            LoadingManager.instance.LoadNextScene("LobbyScene");
        }
    }
}
