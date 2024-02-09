using System.Collections;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateData : MonoBehaviour
{
    public int stateNum;
    public TextMeshProUGUI stateNumber;
    public Button middleButton;
    public List<Button> bonusButtons;
    public List<Image> coinImgs;

    public List<GameObject> paths;
    public List<GameObject> stars;

    public List<GameObject> bounusStates;
    public List<Image> stateImages;

    //コインステートについているボタンから呼んでいます
    public void GetGoldState(int gold)
    {
        //Debug.Log("ゴールド獲得アニメーション再生、テキストDoTween");
        int i = 0;
        foreach(Button bonusButton in bonusButtons)
        {
            if (bonusButton.gameObject.activeInHierarchy && middleButton.interactable && bonusButton.interactable)
            {
                coinImgs[i].color = new Color(1, 1, 1, 0.5f);
                GameManager.instance.gold += gold;
                GameManager.instance.LobbyParamSet();
                GameManager.instance.stageInfo[StageManager.instance.selectStageNumber].state[StageManager.instance.selectStateNumber].bonusGet = true;
                bonusButton.interactable = false;
            }
            i++;
        }
    }
}
