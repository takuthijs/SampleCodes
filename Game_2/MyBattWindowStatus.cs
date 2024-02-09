using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MyBattWindowStatus : MonoBehaviour
{
    public GameManager gameManager;

    public Image battImage;
    public TextMeshProUGUI battName;
    public TextMeshProUGUI battPowerText;
    public TextMeshProUGUI battMeetText;

    private void OnEnable()
    {
        battImage.sprite = gameManager.battList[gameManager.equipBattNumber - 1].battImage;
        battName.text = gameManager.battList[gameManager.equipBattNumber - 1].battName;
        gameManager.selectBattNumber = gameManager.equipBattNumber;

        GameManager.BattStatus battStatus = gameManager.myBattStatuses.Find(myBattStatus => myBattStatus.battNumber == gameManager.selectBattNumber);

        //ステータス画面に反映させる
        battName.text = gameManager.battList[battStatus.battNumber-1].battName;
        battImage.sprite = gameManager.battList[battStatus.battNumber - 1].battImage;

        //battPowerText.text = "+"+battStatus.power;
        if (battStatus.power > 0)
        {
            battPowerText.color = new Color(0.02f, 1, 0, 1);
            battPowerText.text = "+" + battStatus.power;
        }
        else
        {
            battPowerText.color = Color.red;
            battPowerText.text = battStatus.power.ToString();
        }

        if (battStatus.meet > 0)
        {
            battMeetText.color = new Color(0.02f, 1, 0, 1);
            battMeetText.text = "+" + battStatus.meet;
        }
        else
        {
            battMeetText.color = Color.red;
            battMeetText.text = battStatus.meet.ToString();
        }
    }
}
