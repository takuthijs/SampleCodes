using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core.Easing;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.IO;
using UniRx;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Header("アプリバージョン")]
    public const string appVersion = "1.0.0";

    [Header("セーブデータ")]
    public SaveManager saveManager;

    [Header("プレイヤーの情報")]
    [HideInInspector] public int coin = 10000;
    //string breederName = "";
    public string breederName { get; set; }

    [Header("起動時にオブジェクトを生成するタイミングで使用する変数")]
    public GameObject beetleContentParent;//虫かごウィンドウのcontent
    public GameObject skillContentParent;//スキルウィンドウのcontent
    public GameObject beetleCardPrefab;//生成する昆虫カード
    public GameObject skillCardPrefab;//生成するスキルカード
    public GameObject selectBeetleParent;//中央の昆虫
    public SpriteRenderer typeSprite;//土台の中にある昆虫のタイプのスプライト
    public List<Sprite> typeIcons;

    [Header("メインシーンでアプリケーションをポーズした際に使用")]
    public ToggleGroup tabToggleGroup;//違うタブで表示されてしまうのでどのタブかを取得します。

    [Header("ムシカゴウィンドウで使う変数")]
    public MushikagoWindow mushikagoWIndow;
    public GameObject beetlePopupObjParent;
    public GameObject beetleInfoPopup;
    public string selectBeetle_uniqueId;//現在選択中の昆虫のunique_id

    [Header("スキルスロットの情報")]
    public ReactiveProperty<int> slotChanged = new ReactiveProperty<int>();

    public TextMeshProUGUI homeSkillCostText;//ホームで表示しているスキルコスト
    public TextMeshProUGUI skillWindowSkillCostText;//スキルウィンドウで表示しているスキルコスト

    public List<SkillCard> slotSkillCards = new List<SkillCard>();
    public bool isSelectSlot;//スキルスロットが設定されているかどうか
    public int selectSlotNumber;//どの番号のslotNumberが選択されたか
    public SkillCard beforeSelectSkill;

    public List<TextMeshProUGUI> skillWindowSkillCostTexts;
    public List<Image> skillWindowSkillImages;//グーチョキパーの順番でセットする
    public List<TextMeshProUGUI> homeSkillCostTexts;
    public List<Image> homeSkillImages;

    public SkillWindow skillWindow;//スキルウィンドウ
    public GameObject skillWindowPopup;//スキルウィンドウのポップアップ

    [Header("ガチャとショップで使う変数")]
    public GameObject shopWindow;
    public GameObject shopPopup;//確認画面
    public GameObject skillGachaPopup;//スキルガチャ確認画面
    public GameObject gachaWindow;//ホワイトバックと「タッチ」というテキストのみ
    public GameObject gachaBeetleObj;//ガチャを引いた後に表示するオブジェクト
    public GameObject gachaBeetleParent;
    public GameObject gachaTreeParent;
    public GameObject beetleObj;//ガチャの時に消したりします。
    public GameObject gachaTreeObj;//ガチャの時に表示するツリーオブジェクト

    [Header("警告で表示するときの汎用ポップアップ")]
    public GameObject popup;
    public TextMeshProUGUI popupText;

    [Header("ブリーダ名の変更や登録に使用する変数")]
    public GameObject breederNameWindow;
    public TMP_InputField breederNameInputValue;
    public Button breederNameChangeButton;

    [Header("トレーニングボタンで使用する変数たち")]
    public TraningPair selectTraning;//選択されたトレーニングボタン
    public GrowStatus growStatus_1,growStatus_2;//成長させる項目2つ
    public int growValue_1, growValue_2;//成長量2つ

    private int traningNum;//広告をみるボタンでボーナスを加える時に使用

    public GameObject growCardParent;
    public GameObject growCard;

    public GameObject homeCanvas;//ホーム画面上部
    public GameObject tabCanvas;//タブ
    public GameObject traningCanvas;//トレーニングメニュー画面
    public GameObject traningNowCanvas;//トレーニング中の画面
    public GameObject traningAfterCanvas;//トレーニング後の画面

    [Header("Playfabのコントローラー")]
    public PlayFabController playFabController;

    [Header("マッチング画面情報")]
    public TextMeshProUGUI matchingWindow_title;
    public Button cancelMatchingButton;

    public GameObject matchingWindow_PlayerBeetleObjParent;
    public GameObject matchingWindow_EnemyBeetleObjParent;

    public GameObject matchingWindow;
    public TextMeshProUGUI matchingWindow_rate;
    public TextMeshProUGUI matchingWindow_beetleName;
    public TextMeshProUGUI matchingWindow_breederName;

    public TextMeshProUGUI matchingWindow_skillName_guu;
    public TextMeshProUGUI matchingWindow_skillName_choki;
    public TextMeshProUGUI matchingWindow_skillName_par;

    [Header("リザルト画面")]
    public GameObject resultWindow;
    public GameObject resultBeetleParent;//昆虫オブジェクトの親
    public TextMeshProUGUI battleResultTitle;
    public GameObject titleEffect;
    public TextMeshProUGUI resultRankText;
    public TextMeshProUGUI resultRateText;
    private int getCoin;//広告をみて獲得するコインを増やす処理に使用します
    public TextMeshProUGUI resultCoinText;
    public GameObject resultAdsButtonObj;

    [Header("バトルマネージャー")]
    public BattleManager battleManager;
    [Header("UIマネージャー")]
    public UIManager uIManager;

    [Header("持っているスキル情報")]
    public List<SkillInfo> mySkills;
    public SkillInfoBattle selectBeetleSkills;

    [Header("敵の情報")]
    public string enemyBreederName;
    public SkillInfoBattle enemySkills;

    //保存する方
    [System.Serializable]
    public class SkillInfo
    {
        public string unique_id;
        public string setCharaUniqueId;
        public int skillNum;
        public string skillName;
        public int cost;
        public int level;
        public List<StatusCondition> statusConditions;//セットするための条件
        public Sprite skillImage;
        public string skillDescription;
        public string dateTime;
    }

    //バトルに使用する方
    [System.Serializable]
    public class SkillInfoBattle
    {
        public List<SkillInfo> selectSkills = new List<SkillInfo>();
    }

    //ステータスボーナスの送信に使用する方
    [System.Serializable]
    public class StatusBounus
    {
        public List<bool> enemyStatusBounus = new List<bool>();
    }

    //成長させるカードの組み合わせ
    public enum TraningPair
    {
        powerInte,
        inteGuard,
        guardSpeed,
        all
    }
    //成長する項目
    public enum GrowStatus
    {
        power,
        inte,
        guard,
        speed
    }

    [Space(10)]

    [Header("昆虫の情報とスキル情報")]
    //実装されている昆虫や所持している昆虫を管理
    public BeetleManager beetleManager;
    //スキル情報
    public List<SkillInfoSO> skillInfo;

    //トークンを生成するための変数
    private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    private void Awake()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("GameManager");
        //GameManagerが存在したら生成しない
        if (objects.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        instance = this;

        
        //セーブデータがなかったら最初の一匹目をmyBeetleに加える
        if (!File.Exists(saveManager.filePath))
        {
            BeetleManager.Beetle beetle = new BeetleManager.Beetle();
            beetle.beetleToken = GenerateRandomString(16);
            beetle.rate = 1000;
            beetle.rank = 0;
            beetle.unique_id = GenerateRandomString(16);
            beetle.beetle_id = beetleManager.beetleSos[0].beetle_id;
            beetle.reincarnationCount = 0;
            beetle.beetleName = beetleManager.beetleSos[0].beetleName;
            beetle.beetleCardImage = beetleManager.beetleSos[0].beetleCardImage;
            beetle.beetleType = beetleManager.beetleSos[0].beetleType;
            beetle.growCount = 100;
            beetle.power = beetleManager.beetleSos[0].power;
            beetle.inter = beetleManager.beetleSos[0].inter;
            beetle.guard = beetleManager.beetleSos[0].guard;
            beetle.speed = beetleManager.beetleSos[0].speed;
            beetle.skillCostLimit = beetleManager.beetleSos[0].skillCostLimit;
            beetle.dateTime = System.DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss tt");

            //選択中のスキル
            for (int i = 0; i < 3; i++)
            {
                beetle.selectSkill.Add("none");
            }

            foreach (int num in beetleManager.beetleSos[0].learnableSkills)
            {
                beetle.learnableSkills.Add(num);
            }

            //マイビートルとセレクトビートルに入れる
            beetleManager.selectBeetle = beetle;

            //初回コインを追加する
            coin = 10000;

            //名前のポップアップを表示する
            breederNameWindow.SetActive(true);

            Save();
        }
        else
        {
            //ロード処理
            Load();
        }

        //シーン遷移でInspectorでセットしたものが消えてしまうものがあるので
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    //シーンをロードするとinspectorで配置したステージのオブジェクトが消えてしまうので再設定
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "MainScene") return;
        gachaTreeParent = GameObject.FindWithTag("GachaTree");
        gachaTreeObj = gachaTreeParent.transform.Find("GachaTree").gameObject;
    }

    public string GenerateRandomString(int length)
    {
        System.Text.StringBuilder builder = new System.Text.StringBuilder(length);

        for (int i = 0; i < length; i++)
        {
            int index = Random.Range(0, chars.Length);
            builder.Append(chars[index]);
        }

        return builder.ToString();
    }

    // Start is called before the first frame update
    void Start()
    {
        //UIを更新する
        uIManager.HomeUIUpdate();
        uIManager.HomeSkillUIUpdate();
    }

    // Update is called once per frame
    void Update()
    {
        //Hack Updateで行うのはあんま良くないと思うので後で修正が必要かも
        if(gachaTreeObj == null)
        {
            //ガチャで使うオブジェクトをfindしてセットする
            gachaTreeObj = GameObject.FindWithTag("GachaTree");
        }
    }

    //バトルシーンに遷移する
    public IEnumerator LoadBattleScene()
    {
        //UIManager.instance.matchingText.text += "バトルシーンへ移動します。" + "\n";
        yield return new WaitForSeconds(2f);
        //メインシーンの昆虫を非アクティブにする
        beetleObj.SetActive(false);
        //シーンをロードする
        LoadingManager.instance.LoadNextScene("GameScene");
    }

    //ゲーム終了後
    public void LoadMainScene()
    {
        resultWindow.SetActive(false);
        //メインシーンの昆虫をアクティブにする
        beetleObj.SetActive(true);

        //シーンをロードする
        LoadingManager.instance.LoadNextScene("MainScene");
        homeCanvas.SetActive(true);
        tabCanvas.SetActive(true);
    }

    //育成するボタンを押した時の処理
    public void TraningButton(int traningNum)
    {
        //その昆虫に育成するカウントが残っていたら処理を実行
        if (beetleManager.selectBeetle.growCount > 0)
        {
            //広告表示の方の処理にも使用するためtraningNumを保管
            this.traningNum = traningNum;

            //ボタンからの処理だと引数が限られるためここで数字をenumに変換
            TraningPair traningPair = (TraningPair)traningNum;

            homeCanvas.SetActive(false);//ホーム画面のボタンを非アクティブにする
            tabCanvas.SetActive(false);//タブを非アクティブにする
            traningNowCanvas.SetActive(true);//トレーニング中の画面をアクティブにする

            StartCoroutine(TraningCardGenerate(traningPair, 0));
        }
        else
        {
            //Todo育成できない旨のポップアップを表示させる
            Debug.Log("成長限界");
        }
    }


    //広告をみるボタンを押した時の処理
    public void TraningButtonBounus()
    {
        //コルーチンをキャンセルする
        StopAllCoroutines();

        //ボタンからの処理だと引数が限られるためここで数字をenumに変換
        StartCoroutine(TraningCardGenerate((TraningPair)traningNum, 3));
    }


    IEnumerator TraningCardGenerate(TraningPair traningPair , int bounus)
    {
        //3秒間トレーニング中の画面を表示
        yield return new WaitForSeconds(3 - bounus);

        traningNowCanvas.SetActive(false);//トレーニング中の画面を非アクティブにする

        //生成されている育成カードを削除する
        foreach (Transform n in growCardParent.transform)
        {
            GameObject.Destroy(n.gameObject);
        }

        //育成カードを3種類生成する
        for (int i = 0; i < 3; i++)
        {
            switch (traningPair)
            {
                case TraningPair.powerInte://パワーとインテリ
                    selectTraning = TraningPair.powerInte;
                    (growStatus_1, growStatus_2) = (GrowStatus.power, GrowStatus.inte);
                    break;
                case TraningPair.inteGuard://インテリとガード
                    (growStatus_1, growStatus_2) = (GrowStatus.inte, GrowStatus.guard);
                    break;
                case TraningPair.guardSpeed://ガードとスピード
                    (growStatus_1, growStatus_2) = (GrowStatus.guard, GrowStatus.speed);
                    break;
                case TraningPair.all://全部の中から2つセレクト(同じ場合もある)
                    growStatus_1 = (GrowStatus)Random.Range(0, System.Enum.GetValues(typeof(GrowStatus)).Length);
                    growStatus_2 = (GrowStatus)Random.Range(0, System.Enum.GetValues(typeof(GrowStatus)).Length);
                    break;
            }

            //以下の10の部分をもし自分のタイプと一致していた場合は+でボータスステータスを足す
            //セレクトビートルのタイプと(growStatus_1, growStatus_2)のどちらかが一緒なら
            int typeBounus_1 = 0;
            int typeBounus_2 = 0;
            if (growStatus_1 == beetleManager.selectBeetle.beetleType)
            {
                typeBounus_1 = Random.Range(1, 10);
            }

            if(growStatus_2 == beetleManager.selectBeetle.beetleType)
            {
                typeBounus_2 = Random.Range(1, 10);
            }

            //合計が広告もみずタイプも一致しない場合は10になるように最初の値のみランダムな数値にする
            growValue_1 = Random.Range(1, 10 + typeBounus_1 + bounus);
            growValue_2 = 10 + typeBounus_2 + bounus - growValue_1;
            if (growValue_2 < 0) growValue_2 = 0;

            //Todo苦手属性の場合は成長数値から少し引く

            //成長カードを生成する処理
            GrowCard card = Instantiate(growCard, growCardParent.transform).GetComponent<GrowCard>();
            (card.upText.text, card.bottomText.text) = (GrowStatusIcon(growStatus_1) + "+" + growValue_1, GrowStatusIcon(growStatus_2) + "+" + growValue_2);
            (card.topGrowStatus,card.bottomGrowStatus) = (growStatus_1, growStatus_2);
            (card.upNum, card.bottomNum) = (growValue_1, growValue_2);
        }

        traningCanvas.SetActive(false);
        traningAfterCanvas.SetActive(true);
    }

    public string GrowStatusIcon(GrowStatus growStatus)
    {
        switch (growStatus)
        {
            case GrowStatus.power:
                return "<sprite=0 color=#ff0000>パワー\n";
            case GrowStatus.inte:
                return "<sprite=1 color=#D96B00>インテリ\n";
            case GrowStatus.guard:
                return "<sprite=2 color=#008000>ガード\n";
            case GrowStatus.speed:
                return "<sprite=3 color=#0050B0>スピード\n";
            default:
                return "";
        }
    }

    public string BeetleTypeIcon(GrowStatus growStatus)
    {
        switch (growStatus)
        {
            case GrowStatus.power:
                return "<sprite=0 color=#ff0000>";
            case GrowStatus.inte:
                return "<sprite=1 color=#D96B00>";
            case GrowStatus.guard:
                return "<sprite=2 color=#008000>";
            case GrowStatus.speed:
                return "<sprite=3 color=#0050B0>";
            default:
                return "";
        }
    }

    public void MatchingCanvasActivate()
    {
        matchingWindow.SetActive(true);
        tabCanvas.SetActive(false);
        homeCanvas.SetActive(false);
        //デフォルトでセットされている昆虫のオブジェクト(自分)を削除する
        foreach (Transform n in matchingWindow_PlayerBeetleObjParent.transform)
        {
            GameObject.Destroy(n.gameObject);
        }

        //デフォルトでセットされている昆虫のオブジェクト(敵)を削除する
        foreach (Transform n in matchingWindow_EnemyBeetleObjParent.transform)
        {
            GameObject.Destroy(n.gameObject);
        }
        GameObject beetleObj = Instantiate(beetleManager.beetleSos[beetleManager.selectBeetle.beetle_id - 1].model, matchingWindow_PlayerBeetleObjParent.transform);

        //育てているステータス分サイズを変更する
        BeetleScaler beetleScaler = beetleObj.GetComponent<BeetleScaler>();

        if (beetleScaler is not null)
        {
            beetleScaler.power = beetleManager.selectBeetle.power;
            beetleScaler.inter = beetleManager.selectBeetle.inter;
            beetleScaler.guard = beetleManager.selectBeetle.guard;
            beetleScaler.speed = beetleManager.selectBeetle.speed;
            beetleScaler.ScaleChange();
        }

        SetLayerRecursively(beetleObj, 6); // "UI 3D Object"

        matchingWindow_rate.text = "レート：---";
        matchingWindow_beetleName.text = "名前：---";
        matchingWindow_breederName.text = "ブリーダー名：---";

        //Todo相手の技番号に合わせて技名をFindしてセットする
        matchingWindow_skillName_guu.text = "---";
        matchingWindow_skillName_choki.text = "---";
        matchingWindow_skillName_par.text = "---";
    }

    //マッチングした時に実行するもの
    public void MatchingCanvasUpdate()
    {
        matchingWindow_title.text = "マッチングしました!";
        
        GameObject beetleObj = Instantiate(beetleManager.beetleSos[beetleManager.enemyBeetle.beetle_id - 1].model, matchingWindow_EnemyBeetleObjParent.transform);

        //育てているステータス分サイズを変更する
        BeetleScaler beetleScaler = beetleObj.GetComponent<BeetleScaler>();

        if (beetleScaler is not null)
        {
            beetleScaler.power = beetleManager.enemyBeetle.power;
            beetleScaler.inter = beetleManager.enemyBeetle.inter;
            beetleScaler.guard = beetleManager.enemyBeetle.guard;
            beetleScaler.speed = beetleManager.enemyBeetle.speed;
            beetleScaler.ScaleChange();
        }

        SetLayerRecursively(beetleObj,6); // "UI 3D Object"

        matchingWindow_rate.text = "レート : "+beetleManager.enemyBeetle.rate.ToString();
        matchingWindow_beetleName.text = "名前 : "+ beetleManager.enemyBeetle.beetleName;
        matchingWindow_breederName.text = "ブリーダー名 : "+enemyBreederName;

        //相手の技情報に合わせて技名セットする
        matchingWindow_skillName_guu.text = enemySkills.selectSkills[0].skillName;
        matchingWindow_skillName_choki.text = enemySkills.selectSkills[1].skillName;
        matchingWindow_skillName_par.text = enemySkills.selectSkills[2].skillName;
    }

    //NPCとマッチさせる処理
    public void NpcMaching()
    {
        matchingWindow_title.text = "マッチングしました!";
        Debug.Log("NpcMaching処理");

        //デフォルトでセットされている昆虫のオブジェクトを削除する
        foreach (Transform n in matchingWindow_EnemyBeetleObjParent.transform)
        {
            GameObject.Destroy(n.gameObject);
        }

        //実装済みの昆虫からランダムで選択
        int randomNum = Random.Range(0, beetleManager.beetleSos.Count);

        BeetleManager.Beetle beetle = new BeetleManager.Beetle();
        beetle.beetleToken = GenerateRandomString(16);
        beetle.rate = 1000;//ランダムにしたい
        //beetle.rank = 0;
        //beetle.unique_id = GenerateRandomString(16);
        beetle.beetle_id = beetleManager.beetleSos[randomNum].beetle_id;
        beetle.beetleName = beetleManager.beetleSos[randomNum].beetleName;
        beetle.beetleType = beetleManager.beetleSos[randomNum].beetleType;
        beetle.growCount = 100;
        beetle.power = beetleManager.beetleSos[randomNum].power;
        beetle.inter = beetleManager.beetleSos[randomNum].inter;
        beetle.guard = beetleManager.beetleSos[randomNum].guard;
        beetle.speed = beetleManager.beetleSos[randomNum].speed;

        beetleManager.enemyBeetle = beetle;

        GameObject beetleObj = Instantiate(beetleManager.beetleSos[randomNum].model, matchingWindow_EnemyBeetleObjParent.transform);

        //育てているステータス分サイズを変更する
        BeetleScaler beetleScaler = beetleObj.GetComponent<BeetleScaler>();

        if (beetleScaler is not null)
        {
            beetleScaler.power = beetleManager.enemyBeetle.power;
            beetleScaler.inter = beetleManager.enemyBeetle.inter;
            beetleScaler.guard = beetleManager.enemyBeetle.guard;
            beetleScaler.speed = beetleManager.enemyBeetle.speed;
            beetleScaler.ScaleChange();
        }

        SetLayerRecursively(beetleObj, 6); // "UI 3D Object"

        matchingWindow_rate.text = "レート : " + beetleManager.enemyBeetle.rate.ToString();
        matchingWindow_beetleName.text = "名前 : " + beetleManager.enemyBeetle.beetleName;
        matchingWindow_breederName.text = "ブリーダー名 : NPC_" +randomNum;

        //Todo相手の技をランダムに選び技名をセットする
        //ガチャ機能もしくは用意したスキルをセット
        matchingWindow_skillName_guu.text = "技セットなし";
        matchingWindow_skillName_choki.text = "技セットなし";
        matchingWindow_skillName_par.text = "技セットなし";

        StartCoroutine(LoadBattleScene());
    }

    //引数1個目のオブジェクトとその子オブジェクトのレイヤーを全て変更する関数
    //カメラの処理でUI上に3Dオブジェクトを表示するために必要
    public void SetLayerRecursively(GameObject gameObject, int layer)
    {
        gameObject.layer = layer;

        foreach (Transform n in gameObject.transform)
        {
            SetLayerRecursively(n.gameObject, layer);
        }
    }

    //マッチングをキャンセルするボタン
    public void CancelMatchingButton()
    {
        playFabController.cancelButton = true;

        //ソケットが存在していたら切断する
        if(playFabController.socket != null) playFabController.socket.Disconnect();
        if (playFabController.cancelRequest.TicketId != "") playFabController.CancelTicket();

        matchingWindow.SetActive(false);
        playFabController.isMatching = false;

        tabCanvas.SetActive(true);
        homeCanvas.SetActive(true);
    }

    public void AddCoin(int coin)
    {
        this.coin += coin;
        //コインが1,000,000以上にならないようにする
        if (this.coin > 999999)
        {
            this.coin = 999999;
        }
        Save();
    }

    //ブリーダー名の変更
    public void ChangeBreederName()
    {
        breederName = breederNameInputValue.text;
        uIManager.breederName.text = "ブリーダー名：" + breederName;
        breederNameWindow.SetActive(false);
        Save();
    }

    //ブリーダー名の名前を変えた時のバリデーション
    public void BreederNameValidation()
    {
        bool textLength = (breederNameInputValue.text.Length > 0) && (breederNameInputValue.text.Length <= 10);//テキストの長さ（0文字以上10文字以下）
        bool empty = string.IsNullOrEmpty(breederNameInputValue.text);
        if (textLength && !empty)
        {
            breederNameChangeButton.interactable = true;
        }
        else
        {
            breederNameChangeButton.interactable = false;
        }
    }

    //昆虫転生の処理
    public void BeetleReincarnation()
    {
        //昆虫転生のウィンドウを表示させる
        //成長限界になった昆虫を表示させる
        //転生カウントをインクリメントする
        //前世のステータスの10%をローカル変数に保存
        //1番伸びているタイプを保存
        //1番伸びたタイプに変化
        //1番伸びているステータスの部位が少し大きくなる（Beetleに倍率を保存する）
        //ステータスのscriptableObjectの値に前世のステータスの10%をローカル数値分追加
    }


    //セーブ処理
    public void Save()
    {
        saveManager.save.breederName = breederName;
        saveManager.save.coin = coin;
        saveManager.save.selectBeetle = beetleManager.selectBeetle;
        saveManager.save.myBeetle = beetleManager.myBeetles;
        saveManager.save.mySkills = mySkills;

        //セレクトビートルの情報をmybeetleに上書きする
        //selectBeetleのunique_idと一致する要素を検索
        BeetleManager.Beetle foundBeetle = beetleManager.myBeetles.Find(beetle => beetle.unique_id == beetleManager.selectBeetle.unique_id);

        //新しいBeetleオブジェクトを作成して情報をコピー
        BeetleManager.Beetle beetle = NewBeetle(beetleManager.selectBeetle);

        // 条件に一致する要素が見つかった場合
        if (foundBeetle != null)
        {
            foundBeetle.unique_id = beetle.unique_id;
            foundBeetle.beetleToken = beetle.beetleToken;
            foundBeetle.rank = beetle.rank;
            foundBeetle.rate = beetle.rate;
            foundBeetle.beetle_id = beetle.beetle_id;
            foundBeetle.beetleName = beetle.beetleName;
            foundBeetle.beetleCardImage = beetle.beetleCardImage;
            foundBeetle.beetleType = beetle.beetleType;
            foundBeetle.growCount = beetle.growCount;
            foundBeetle.power = beetle.power;
            foundBeetle.inter = beetle.inter;
            foundBeetle.guard = beetle.guard;
            foundBeetle.speed = beetle.speed;
            foundBeetle.skillCostLimit = beetle.skillCostLimit;
            foundBeetle.dateTime = beetle.dateTime;

            foundBeetle.selectSkill.Clear();
            foundBeetle.selectSkill.AddRange(beetle.selectSkill);

            foundBeetle.learnableSkills.Clear();
            foundBeetle.learnableSkills.AddRange(beetle.learnableSkills);
        }
        else
        {
            Debug.Log("基本初回セーブ");
            beetleManager.myBeetles.Add(beetle);
        }

        saveManager.Save();
    }

    //ロード処理
    private void Load()
    {
        saveManager.Load();
        coin = saveManager.save.coin;
        breederName = saveManager.save.breederName;
        beetleManager.selectBeetle = saveManager.save.selectBeetle;

        //selectBeetleの画像を入れ直す
        beetleManager.selectBeetle.beetleCardImage = beetleManager.beetleSos[beetleManager.selectBeetle.beetle_id -1].beetleCardImage;
        //selectBeetleの名前を入れ直す
        beetleManager.selectBeetle.beetleName = beetleManager.beetleSos[beetleManager.selectBeetle.beetle_id - 1].beetleName;

        beetleManager.myBeetles = saveManager.save.myBeetle;
        mySkills = saveManager.save.mySkills;

        //スキルをセットできる条件をInfoから入れ直す
        for(int i = 0;i< mySkills.Count; i++)
        {
            mySkills[i].statusConditions = GameManager.instance.skillInfo[mySkills[i].skillNum - 1].statusConditions;
        }

        //スキルコスト上限をBeetleSOから参照して入れる
        beetleManager.selectBeetle.skillCostLimit = beetleManager.beetleSos[beetleManager.selectBeetle.beetle_id - 1].skillCostLimit;

        //セット可能なスキルをBeetleSOから参照して入れる
        beetleManager.selectBeetle.learnableSkills = beetleManager.beetleSos[beetleManager.selectBeetle.beetle_id - 1].learnableSkills;

        //持っている昆虫を生成する
        for (int i = 0; i < 40; i++)
        {
            if (beetleManager.myBeetles.Count <= i) continue;
            GameObject beetleCardObj = Instantiate(beetleCardPrefab, beetleContentParent.transform);
            BeetleCard beetleCard = beetleCardObj.GetComponent<BeetleCard>();

            //スプライトは再設定してあげないといけないので先に設定します。
            int num = beetleManager.myBeetles[i].beetle_id - 1;
            beetleManager.myBeetles[i].beetleName = beetleManager.beetleSos[num].beetleName;
            beetleManager.myBeetles[i].beetleCardImage = beetleManager.beetleSos[num].beetleCardImage;

            beetleCard.SetBeetleCard(beetleManager.myBeetles[i].unique_id);
        }

        //持ってるスキルを生成する
        for (int i=0; i< 40; i++)
        {
            if (mySkills.Count <= i) continue;
            GameObject skillCardObj = Instantiate(skillCardPrefab, skillContentParent.transform);
            SkillCard skillCard = skillCardObj.GetComponent<SkillCard>();

            //スプライトは再設定してあげないといけないので先に設定します。
            int num = mySkills[i].skillNum - 1;
            mySkills[i].skillImage = skillInfo[num].skillImage;
            //スキル名、スキル説明が変わっている場合があるので、設定しなおします。
            mySkills[i].skillName = skillInfo[num].skillName;
            mySkills[i].skillDescription = skillInfo[num].skillDescription;

            skillCard.SetSkillCard(mySkills[i].unique_id);
        }

        //Addする前にクリア
        selectBeetleSkills.selectSkills.Clear();
        //スキルウィンドウのslotにセットされているSkillCardにもunique_idをセットする
        for (int i = 0; i < 3; i++)
        {
            slotSkillCards[i].SetSkillCard(beetleManager.selectBeetle.selectSkill[i]);
            selectBeetleSkills.selectSkills.Add(GameManager.instance.mySkills.Find(myskill => myskill.unique_id == beetleManager.selectBeetle.selectSkill[i]));
        }

        //中央に選択中の昆虫を生成する
        SelectBeetleGenerate();

        //ホーム画面とスキル画面のセットスキルを反映させる
        uIManager.HomeSkillUIUpdate();
    }

    public BeetleManager.Beetle NewBeetle(BeetleManager.Beetle beetle)
    {
        BeetleManager.Beetle newBeetle = new BeetleManager.Beetle
        {
            unique_id = beetle.unique_id,
            beetleToken = beetle.beetleToken,
            rank = beetle.rank,
            rate = beetle.rate,
            reincarnationCount = beetle.reincarnationCount,
            beetle_id = beetle.beetle_id,
            beetleName = beetle.beetleName,
            beetleCardImage = beetle.beetleCardImage,
            beetleType = beetle.beetleType,
            growCount = beetle.growCount,
            power = beetle.power,
            inter = beetle.inter,
            guard = beetle.guard,
            speed = beetle.speed,
            skillCostLimit = beetle.skillCostLimit,
            dateTime = beetle.dateTime
        };

        newBeetle.selectSkill.AddRange(beetle.selectSkill);
        newBeetle.learnableSkills.AddRange(beetle.learnableSkills);

        return newBeetle;
    }

    public void SelectBeetleGenerate()
    {
        //デフォルトでセットされている昆虫のオブジェクトを削除する
        foreach (Transform n in selectBeetleParent.transform)
        {
            GameObject.Destroy(n.gameObject);
        }

        //中央に選択中の昆虫を生成する
        BeetleScaler beetleScaler = Instantiate(beetleManager.beetleSos[beetleManager.selectBeetle.beetle_id - 1].model, selectBeetleParent.transform).GetComponent<BeetleScaler>(); ;

        //スケールを変更する
        //育てているステータス分サイズを変更する
        if (beetleScaler is not null)
        {
            beetleScaler.power = GameManager.instance.beetleManager.selectBeetle.power;
            beetleScaler.inter = GameManager.instance.beetleManager.selectBeetle.inter;
            beetleScaler.guard = GameManager.instance.beetleManager.selectBeetle.guard;
            beetleScaler.speed = GameManager.instance.beetleManager.selectBeetle.speed;
            beetleScaler.ScaleChange();
        }
    }

    //アプリケーションを一時停止したらソケット通信を切断する
    private void OnApplicationPause(bool pause)
    {
        if (playFabController.cancelRequest.TicketId != "")
        {
            CancelMatchingButton();
            //CPUモードに切り替える
            if (battleManager != null)
            {
                battleManager.battleMode = BattleManager.BattleMode.cpu;
                //入力待ち状態だった場合かつバトルマネージャーが存在していた場合はコマンドチェックに移動する
                bool checkCommand = (battleManager.battleStatus == BattleManager.BattleStatus.wait) && (battleManager != null);
                if (checkCommand) battleManager.CheckCommands();
            }
        }
        //playfabコントローラーのポーリング処理をキャンセルする
        playFabController.StopAllCoroutines();
        Debug.Log("OnApplicationPause : " + pause);

        //pauseがfalseだったかつHome画面ではない場合はhome画面のUIがアクティブにならないようにする
        //現在のシーンがMainSceneではなかったらUIを非表示にする
        if(SceneManager.GetActiveScene().name != "MainScene")
        {
            tabCanvas.SetActive(false);
            homeCanvas.SetActive(false);
        }else if(SceneManager.GetActiveScene().name == "MainScene")
        {
            //トグルがホームだった場合はHomeのUIを表示させる
            // Toggle Group内のすべてのToggleを取得
            Toggle[] toggles = tabToggleGroup.GetComponentsInChildren<Toggle>();

            foreach (Toggle toggle in toggles)
            {
                if (toggle.isOn)
                {
                    Debug.Log(toggle.name);
                    if (toggle.name == "Home") homeCanvas.SetActive(true);
                }
            }
        }

    }

    //アプリケーションが終了したらソケット通信を切断する
    private void OnApplicationQuit()
    {
        CancelMatchingButton();
        //playfabコントローラーのポーリング処理をキャンセルする
        playFabController.StopAllCoroutines();
        Debug.Log("OnApplicationQuit");
    }
}
