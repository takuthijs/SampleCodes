using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject titleUI;
    public GameObject LobbyUI;
    public GameObject GameSceneUI;
    public GameObject stageUI;
    public GameObject innerStageUI;
    public GameObject innerStagePartyUI;
    public GameObject partySelect;//パーティー選択画面
    public GameObject cardListUI;//モンスター一覧
    public GameObject cardInfoUI;//カード詳細情報
    public GameObject gachaUI;//ガチャのUI
    public GameObject rouletteUI;//ルーレットのUI
    public GameObject maxNumberTapUI;//1番大きい数字をTapするUI
    public GameObject smashTapUI;//連打ミニゲームUI
    private List<GameObject> All_UI_Objcts = new List<GameObject>();

    [Space(10)]
    //表示しない場合があるボタンたち
    public GameObject unSetButton;
    public GameObject setButton;//キャラクター詳細画面

    private void Awake()
    {
        instance = this;

        instance.All_UI_Objcts.Add(titleUI);
        instance.All_UI_Objcts.Add(LobbyUI);
        instance.All_UI_Objcts.Add(GameSceneUI);
        instance.All_UI_Objcts.Add(stageUI);
        instance.All_UI_Objcts.Add(innerStageUI);
        instance.All_UI_Objcts.Add(innerStagePartyUI);
        instance.All_UI_Objcts.Add(partySelect);
        instance.All_UI_Objcts.Add(cardListUI);
        instance.All_UI_Objcts.Add(cardInfoUI);
        instance.All_UI_Objcts.Add(gachaUI);
        instance.All_UI_Objcts.Add(rouletteUI);
        instance.All_UI_Objcts.Add(maxNumberTapUI);
        instance.All_UI_Objcts.Add(smashTapUI);
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;  //sceneLoadedに関数を追加

        //Canvasのカメラの設定
        if (gameObject.GetComponent<Canvas>().worldCamera == null)
        {
            gameObject.GetComponent<Canvas>().worldCamera = Camera.main;
        }

        //スタート時のシーンに応じてUIを表示する
        if (SceneManager.GetActiveScene().name == "TitleScene") titleUI.SetActive(true);
        if (SceneManager.GetActiveScene().name == "LobbyScene") LobbyUI.SetActive(true);
        if (SceneManager.GetActiveScene().name == "GameScene") GameSceneUI.SetActive(true);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("シーン読み込まれました");
        //シーンが変わる時は一度全てのUIを非アクティブにする
        foreach(GameObject ui in instance.All_UI_Objcts)
        {
            ui.SetActive(false);
        }

        if (instance.GetComponent<Canvas>().worldCamera == null)
        {
            instance.GetComponent<Canvas>().worldCamera = Camera.main;
        }

        if(scene.name == "TitleScene")
        {
            instance.titleUI.SetActive(true);
        }

        if(scene.name == "LobbyScene")
        {
            instance.LobbyUI.SetActive(true);
        }

        if (scene.name == "GameScene")
        {
            instance.GameSceneUI.SetActive(true);
        }
    }
}
