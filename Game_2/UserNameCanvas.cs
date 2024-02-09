using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using TMPro;

public class UserNameCanvas : MonoBehaviour
{
    public GameManager gameManager;

    public TextMeshProUGUI userName;
    public Button okButton;

    public void NamaChack()
    {
        if (userName.text.Length > 1 && userName.text.Length < 11)
        {
            gameManager.canvasNameInput.SetActive(false);
            gameManager.myName = userName.text;

            PlayerPrefs.SetString(GameManager.KEY_SAVE_NAME, gameManager.myName);
            PlayerPrefs.Save();

            if (!PlayerPrefs.HasKey(GameManager.KEY_SAVE_FIRSTREWARD))
            {
                gameManager.startdashPopUp.SetActive(true);
            }

            gameManager.myNameText.text = userName.text;
        }
    }

    public void NameChanged()
    {
        if (userName.text.Length > 1 && userName.text.Length < 11)
        {
            okButton.interactable = true;
        }
        else
        {
            okButton.interactable = false;
        }
    }
}
