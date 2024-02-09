using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StageManager : MonoBehaviour
{
    //選択されたステージの情報を保管しておくスクリプト（スクリプトの実行順の都合がつかなければGameManagerでもいいかも）
    public static StageManager instance;
    public GameObject stage_Paret;
    public GameObject state_Paret;

    public int selectStageNumber;//今セレクトしているステージの番号
    public int selectStateNumber;//今セレクトしているステートの番号


    [Header("ステージ全体")]
    [Header("ステージアイコン")] public Sprite stageIcon;//背景画像
    public List<StageData> StageObjectData = new List<StageData>();

    [System.Serializable]
    public class StageData
    {
        public GameObject stage;
        [Header("ステージ内情報")]
        [Space(10)]
        [Header("ステージ内に配置するステート数")] public StateInfo[] stateInfo;
    }

    [System.Serializable]
    public class StateInfo
    {
        public GameObject state;//配置するオブジェクト
        public Sprite stateImage;//ステートのアイコン
        public string[] activePaths;//TopとかBottomとか
        public string[] activeState;//TopとかBottomとか

        [Header("背景画像")] public Sprite buttleBackGround;//背景画像
        [Header("消費スタミナ")] public int stamina;
        [Header("階層")] public InnerStageData[] hierarchy;
    }

    [System.Serializable]
    public class InnerStageData
    {
        [Header("出現するキャラ")] public EnnemyRewardData[] enemyCharacters;
    }

    [System.Serializable]
    public class EnnemyRewardData
    {
        public CharacterDefaultParam enemyCharacter;
        [Header("設定したキャラのID")] public int charaID;
        [Header("キャラがドロップする確率")] public int charaDropProbability;
        [Header("ドロップ武器")] public int rewardWeapon;
        [Header("ドロップする確率")] public int dropWeaponProbability;
        [Header("倒した時のお金")] public int dropGold;
    }

    //アクティブにするパス名からステートをアクティブにする
    enum PathAngle
    {
        Top,Right,Bottom,Left
    }

    //アクティブにするボーナスステージ
    enum StateAngle
    {
        Top, Bottom
    }

    private void Awake()
    {
        instance = this;
    }

    public void StageSelect()
    {
        //生成されている子オブジェクトを全て削除する
        foreach (Transform stageObj in stage_Paret.transform)
        {
            Destroy(stageObj.gameObject);
        }

        UIManager.instance.stageUI.SetActive(true);
        for (int i = 0; i < StageObjectData.Count; i++)
        {
            //生成したステージの中のステートを表示させるためのイベントトリガーを設定
            //プレイヤーのステージclear状況からアクティブにする
            if (!GameManager.instance.stageInfo[i].active) return;
            
            int index = i;//iを一度変数に保存してEventTrigger内で使用（そのままiを使用すると最後のindex番号になってしまう。）
            GameObject stageObj = Instantiate(StageObjectData[i].stage, stage_Paret.transform);
            stageObj.GetComponent<StageDataScript>().stageNumber = index;
            EventTrigger stageObjEventTrigger = stageObj.GetComponent<EventTrigger>();
            EventTrigger.Entry stageTriggerEntry = new EventTrigger.Entry();
            stageTriggerEntry.eventID = EventTriggerType.PointerClick;
            stageTriggerEntry.callback.AddListener(_ =>
            {
                //生成されている子オブジェクトを全て削除する
                foreach (Transform stateObj in state_Paret.transform)
                {
                    Destroy(stateObj.gameObject);
                }
                //自分が何番目のオブジェクト（ステージ）かを判断してその番号を保存しておく
                selectStageNumber = stageObj.GetComponent<StageDataScript>().stageNumber;

                UIManager.instance.innerStageUI.SetActive(true);

                for (int stateNum = 0; stateNum < StageObjectData[index].stateInfo.Length; stateNum++)
                {
                    GameObject stateObj = Instantiate(StageObjectData[index].stateInfo[stateNum].state, state_Paret.transform);
                    //アクティブにチェックがついていない（前のステージがクリアされていない場合）
                    if (!GameManager.instance.stageInfo[index].state[stateNum].active)
                    {
                        stateObj.GetComponent<StateData>().middleButton.interactable = false;
                    }
                    stateObj.GetComponent<StateData>().stateNum = stateNum;
                    StateData stateData = stateObj.GetComponent<StateData>();
                    stateData.stateNumber.text = (stateNum + 1).ToString();

                    //すでにコインを獲得していたり、そのステート前までクリアしていなかったら
                    if (GameManager.instance.stageInfo[index].state[stateNum].bonusGet || !stateData.middleButton.interactable)
                    {
                        //HACK　二つのforeachまとめられるかも
                        foreach (Image coinImg in stateData.coinImgs)
                        {
                            coinImg.color = new Color(1, 1, 1, 0.5f);
                        }
                        foreach(Button bonusButton in stateData.bonusButtons)
                        {
                            bonusButton.interactable = false;
                        }
                    }

                    //アクティブにするパス名からステートをアクティブにする
                    for (int path = 0; path < StageObjectData[index].stateInfo[stateNum].activePaths.Length; path++)
                    {
                        switch (StageObjectData[index].stateInfo[stateNum].activePaths[path])
                        {
                            case "Top":
                                stateData.paths[(int)PathAngle.Top].SetActive(true);
                                break;
                            case "Right":
                                stateData.paths[(int)PathAngle.Right].SetActive(true);
                                break;
                            case "Bottom":
                                stateData.paths[(int)PathAngle.Bottom].SetActive(true);
                                break;
                            case "Left":
                                stateData.paths[(int)PathAngle.Left].SetActive(true);
                                break;
                        }
                    }
                    //アクティブにするボーナスステージ
                    for (int bonus = 0; bonus < StageObjectData[index].stateInfo[stateNum].activeState.Length; bonus++)
                    {
                        switch (StageObjectData[index].stateInfo[stateNum].activeState[bonus])
                        {
                            case "Top":
                                stateData.bounusStates[(int)StateAngle.Top].SetActive(true);
                                break;
                            case "Bottom":
                                stateData.bounusStates[(int)StateAngle.Bottom].SetActive(true);
                                break;
                        }
                    }
                    //UIManager.instance.innerStagePartyUI.SetActive(true); 
                        
                }
            });

            //TODOスクロールできないため今後はイベントトリガーを外さないといけない
            stageObjEventTrigger.triggers.Add(stageTriggerEntry);
            
        }
    }
}
