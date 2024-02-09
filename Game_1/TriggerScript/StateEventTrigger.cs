using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StateEventTrigger : MonoBehaviour,IPointerClickHandler
{
    [SerializeField] private Button stateButton;
    [SerializeField] private StateData stateData;
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (stateButton.interactable)
        {
            UIManager.instance.innerStagePartyUI.SetActive(true);
            //自分が何番目のオブジェクト（ステージ）か保存しておく
            StageManager.instance.selectStateNumber = stateData.stateNum;
        }
    }
}