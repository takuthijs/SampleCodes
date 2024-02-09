using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using AssetKits.ParticleImage;
using UnityEngine.Playables;
using Firebase.Analytics;
using Firebase;
using DG.Tweening.Core.Easing;

public class GameManager : MonoBehaviour
{
    public const string KEY_SAVE_PRIVACY = "key_save_privacy";
    public const string KEY_SAVE_NAME = "key_save_name";
    public const string KEY_SAVE_TOKEN = "key_save_token";
    public const string KEY_SAVE_REWARD = "key_save_reward";
    public const string KEY_SAVE_MYBATT = "key_save_mybatt";
    public const string KEY_SAVE_TAP = "key_save_tap";
    public const string KEY_SAVE_COIN  = "key_save_coin";
    public const string KEY_SAVE_SCORE = "key_save_score";
    public const string KEY_SAVE_Power = "key_save_Power";
    public const string KEY_SAVE_Meet = "key_save_Meet";
    public const string KEY_SAVE_Traning = "key_save_Traning";
    public const string KEY_SAVE_EQUIPBATTLIST = "key_save_EquipBattList";
    public const string KEY_SAVE_EQUIPBATT = "key_save_EquipBatt";
    public const string KEY_SAVE_EXECOUNT = "key_save_ExeCount";
    public const string KEY_SAVE_FIRSTREWARD = "key_save_FirstReward";
    

    [HideInInspector] public string myName = "";
    [HideInInspector] public string privacy = "";
    public string myToken = "";

    private int rewardCount = 0;

    public int myCoin = 0;
    public TextMeshProUGUI lb_coin;

    public float hiScore = 0;
    public float currentScore = 0;
    public TextMeshProUGUI lb_hiscore;
    public GameObject highScoreText;//ハイスコアだった場合表示されるテキスト

    public PlayerManager playerManager;
    public CameraManager cameraManager;

    public ModelLevelUpBtn[] powBtns;

    //吹き出し
    public GameObject popUpObj;
    float popupTime = 0;

    //スタートダッシュのポップアップ
    public GameObject startdashPopUp;

    //canvas
    public GameObject canvasHome;
    public GameObject canvasGame;
    public GameObject canvasResult;
    public GameObject canvasNameInput;

    public int tapCount = 50;
    public TextMeshProUGUI tapCountText;

    //コインエフェクト
    public ModelBonusCoin modelBonusCoin;
    public TextMeshProUGUI getCoinBounusText;//入手できるコインの枚数
    public ParticleImage particleImage;
    public GameObject coinDisplay;
    public ParticleImage bounusParticleImage;

    //ネットワークボタン
    public GameObject netWorkConectButton;

    //GAMING
    public GameObject pref_ball;
    public GameObject firePref_ball;
    public GameObject ballWrapper;//ボールとアシストラインを覆っているもの
    public GameObject ball;//ボール単体オブジェクト
    public GameObject ballLine;//ボールのラインオブジェクト
    public bool ballMoving = false; //ボールを動かす
    public BackgroundManager backgroundManager;
    public PlayableDirector playableDirector;

    //パワーが強すぎると一気に山まで到達して跳ね返ってしまうので山のコライダーを1秒間OFFにする処理に使用します
    public List<MountainColider> mountainColiders;

    //ゲーム再生中のしたのボタン
    public GameObject bottomButtons;

    //早送りボタン
    public bool fastforward = false;
    public GameObject fastforwardButton;

    //カーソルの色
    public enum isMeetColor
    {
        red,
        yellow,
        white
    }

    //アシストサークル
    public GameObject assistCircle;

    //弾道ゲージ
    private bool sliderSwitch = false;
    public GameObject ballisticsSliderObj;
    public Slider ballisticsSlider;
    public bool isIncreasing = true;
    public GameObject arrowObj;
    private float minAngle = 80f;
    private float maxAngle = 0f;

    //ピッチャーの情報
    public GameObject pitcherObj;
    public GameObject pitcherBall;
    public Animator pitcherAnimator;

    public AudioManager audioManager;
    public isMeetColor isMeet = isMeetColor.white;
    public TextMeshProUGUI lb_gamecount;
    public TextMeshProUGUI lb_gamescore;
    public int intDiff;//intでの数字を保管
    [HideInInspector]public int swapBackgroundObjCount = 0;//ステージをリピートする際の処理に使用
    int beforeIntDiff;//intでの前フレームの数字を保管

    Rigidbody ballRigidbody;
    public SkyBoxRotation skyBoxRotation;

    AudioSource gameScoreCountUpAudio; //ゲーム中の飛距離の数値が増えるごとに鳴らすSE
    public GameObject ico_meet;
    private bool isSwinged = false;
    public bool isBallStop = false;

    public Transform battParent;
    public int equipBattNumber = 1;//起動時にセットしてその番号に応じて見た目を変更する
    public int selectBattNumber = 1;//現在選択されているバット
    //自分のバットの情報
    public List<BattStatus> myBattStatuses = new List<BattStatus>();
    //実装されているバットのリスト
    public List<BattStatusScriptable> battList = new List<BattStatusScriptable>();
    //バットのプレハブ(所持バットの方)
    public GameObject battSlotEntity;

    //生成したバットのスロット(所持している方)
    public Transform battSlotParent;
    public List<GameObject> battSlots;
    //スロットを生成する際に装備しているバットのウィンドウをスロットにセットする際に使用
    public MyBattWindowStatus myBattWindowStatus;

    //バットSHOP
    public BattShopWindow battShopWindow;
    public Transform battShopParent;
    public GameObject battShopSlot;
    public List<GameObject> battShopSlots;
    [HideInInspector]public List<GameObject> battShopSelectImages;

    //バットの種類
    [System.Serializable]
    public class BattStatus
    {
        public int battNumber;
        public string battName;
        public GameObject battObj;
        public Sprite battImage;
        public int battlevel;
        public int power;
        public int meet;
        public float speed;
        public int value;
    }

    //RESULT
    public TextMeshProUGUI lb_gameresult;

    [Header("ランキングの情報")]
    public TextMeshProUGUI myNameText;
    public TextMeshProUGUI myRankText;
    public TextMeshProUGUI myScoreText;
    public GameObject rankingUI;
    public List<TextMeshProUGUI> rankingNames;
    public List<TextMeshProUGUI> rankingScores;

    //WebAPIのスクリプト
    public WebApiScript webApiScript;

    //AdMobのスクリプト
    public AdMobScript adMobScript;

    //ネットワークの接続状況
    public bool networkConection = false;

    // Start is called before the first frame update
    void Start()
    {
        //ネットワーク状態を確認
        NetWorkConection();

        int exeCount = 1;
        if (PlayerPrefs.HasKey(KEY_SAVE_EXECOUNT))
        {
            exeCount = PlayerPrefs.GetInt(KEY_SAVE_EXECOUNT);
            exeCount++;
        }

        if(exeCount == 3)
        {
            if (networkConection) StartCoroutine(InAppReviewManager.RequestReview());
        }

        PlayerPrefs.SetInt(KEY_SAVE_EXECOUNT, exeCount);
        PlayerPrefs.Save();


        if (PlayerPrefs.HasKey(KEY_SAVE_PRIVACY))
        {
            privacy = PlayerPrefs.GetString(KEY_SAVE_PRIVACY);
        }

        if (PlayerPrefs.HasKey(KEY_SAVE_NAME))
        {
            myName = PlayerPrefs.GetString(KEY_SAVE_NAME);
            myNameText.text = myName;
            if (!PlayerPrefs.HasKey(KEY_SAVE_FIRSTREWARD))
            {
                startdashPopUp.SetActive(true);
            }
        }
        else
        {
            canvasNameInput.SetActive(true);
        }

        if (PlayerPrefs.HasKey(KEY_SAVE_TOKEN))
        {
            myToken = PlayerPrefs.GetString(KEY_SAVE_TOKEN);
            Debug.Log("myToken : " + myToken);
        }
        else
        {
            if (networkConection)
            {
                webApiScript.CoUserAdd();
                Debug.Log("elseMyToken : " + myToken);
            }
        }

        if (PlayerPrefs.HasKey(KEY_SAVE_REWARD))
        {
            rewardCount = PlayerPrefs.GetInt(KEY_SAVE_REWARD);
        }
        else
        {
            rewardCount = 0;
        }

        if (PlayerPrefs.HasKey(KEY_SAVE_TAP))
        {
            tapCount = PlayerPrefs.GetInt(KEY_SAVE_TAP);
            tapCountText.text = tapCount.ToString();
        }

        if (PlayerPrefs.HasKey(KEY_SAVE_COIN)){
            myCoin = PlayerPrefs.GetInt(KEY_SAVE_COIN);
            lb_coin.text = myCoin.ToString("#,0");
        }
        else
        {
            lb_coin.text = 0.ToString("#,0");
        }

        if (PlayerPrefs.HasKey(KEY_SAVE_SCORE))
        {
            hiScore = PlayerPrefs.GetFloat(KEY_SAVE_SCORE);
            lb_hiscore.text = hiScore.ToString("#,0") + "m";
        }
        else
        {
            lb_hiscore.text = 0 + "m";
        }

        //自分が持っているバットを生成する
        if (PlayerPrefs.HasKey(KEY_SAVE_EQUIPBATT))
        {
            equipBattNumber = PlayerPrefs.GetInt(KEY_SAVE_EQUIPBATT);
        }

        //バットをロードする
        if (PlayerPrefs.HasKey(KEY_SAVE_MYBATT))
        {
            //保存されているデータがある場合はデフォルトでセットされているバットデータを削除する
            myBattStatuses.Clear();
            string json = PlayerPrefs.GetString(KEY_SAVE_MYBATT);
            SerializableBattStatuses battStatuses = JsonUtility.FromJson<SerializableBattStatuses>(json);
            myBattStatuses = battStatuses.battStatuses;
            for (int i = 0; i < myBattStatuses.Count; i++)
            {
                battSlots.Add(Instantiate(battSlotEntity, battSlotParent));
                BattSlot battSlot = battSlots[i].GetComponent<BattSlot>();
                battSlot.gameManager = this;
                battSlot.myBattWindowStatus = myBattWindowStatus;
                battSlot.battNumber = myBattStatuses[i].battNumber;
                if (equipBattNumber == battSlot.battNumber)
                {
                    battSlot.selectImage.SetActive(true);
                }
                else
                {
                    battSlot.selectImage.SetActive(false);
                }
                battSlot.battName = myBattStatuses[i].battName;//バットの名前
                battSlot.maxLevel = battList[battSlot.battNumber - 1].maxLevel;//バットのマックスレベル
                battSlot.battLevel = myBattStatuses[i].battlevel;//バットのレベル
                battSlot.battNumber = myBattStatuses[i].battNumber;//バットの番号
                battSlot.battObj = battList[battSlot.battNumber - 1].battObj;//バットのゲームオブジェクト
                battSlot.battImage.sprite = battList[battSlot.battNumber - 1].battImage;//バットの画像
                battSlot.power = myBattStatuses[i].power;//バットのパワー
                battSlot.meet = myBattStatuses[i].meet;//バットのミート
            }

        }
        else
        {
            //所持バット情報がない場合はデフォルトのバット情報を格納する
            for (int i = 0; i < myBattStatuses.Count; i++)
            {
                battSlots.Add(Instantiate(battSlotEntity, battSlotParent));
                BattSlot battSlot = battSlots[i].GetComponent<BattSlot>();
                battSlot.gameManager = this;
                battSlot.myBattWindowStatus = myBattWindowStatus;
                battSlot.battNumber = myBattStatuses[i].battNumber;
                if (equipBattNumber == battSlot.battNumber)
                {
                    battSlot.selectImage.SetActive(true);
                }
                else
                {
                    battSlot.selectImage.SetActive(false);
                }
                battSlot.battName = myBattStatuses[i].battName;//バットの名前
                battSlot.maxLevel = battList[battSlot.battNumber - 1].maxLevel;//バットのマックスレベル
                battSlot.battLevel = myBattStatuses[i].battlevel;//バットのレベル
                battSlot.battNumber = myBattStatuses[i].battNumber;//バットの番号
                battSlot.battObj = battList[myBattStatuses[i].battNumber - 1].battObj;//バットのゲームオブジェクト
                battSlot.battImage.sprite = battList[myBattStatuses[i].battNumber - 1].battImage;//バットの画像
                battSlot.power = myBattStatuses[i].power;//バットのパワー
                battSlot.meet = myBattStatuses[i].meet;//バットのミート
            }
        }


        //最後に装備したバットを装備させる
        //デフォルトで持っているバットを削除する
        Destroy(battParent.GetChild(0).gameObject);
        Instantiate(battList[equipBattNumber - 1].battObj, battParent);

        //ショップに実装済みのバットを生成する
        //Todo今後管理画面から制御したい
        for(int i = 0;i<battList.Count; i++)
        {
            battShopSlots.Add(Instantiate(battShopSlot, battShopParent));
            ShopBattSlot shopBattSlot = battShopSlots[i].GetComponent<ShopBattSlot>();
            shopBattSlot.gameManager = this;
            shopBattSlot.battNum = battList[i].battNumber;
            shopBattSlot.battName = battList[i].battName;
            shopBattSlot.battImage.sprite = battList[i].battImage;
            shopBattSlot.battPower = battList[i].power;
            shopBattSlot.battMeet = battList[i].meet;
            shopBattSlot.powerValue = battList[i].value;//買えるパワーレベル（価格）
            shopBattSlot.battNameText.text = battList[i].battName;
            shopBattSlot.buttonValueText.text = "<sprite=0>"+battList[i].value.ToString("#,0");
            battShopSelectImages.Add(shopBattSlot.selectImage);
        }


        foreach (ModelLevelUpBtn powBtn in powBtns)
        {
            powBtn.GetComponent<ModelLevelUpBtn>().playerManager = playerManager;
            powBtn.InitBtn();
        }

        //ゲーム中の飛距離の数値が増えるごとに鳴らすSE
        gameScoreCountUpAudio = audioManager.SE[1].GetComponent<AudioSource>();
        //バックグラウンドで流れているBGM
        audioManager.PlayBGM(0);
        //ピッチャー側のアイドルアニメーションを再生
        pitcherAnimator.Play("Pitching_Idle", 0, 0);

        if (networkConection)
        {
            if (!string.IsNullOrEmpty(privacy))
            {
                Debug.Log("FireBase");
                //Firebase
                FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
                {
                    FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                });

            }
            //広告をロード
            adMobScript.LoadAd();
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(playerManager.playerDoing == PlayerManager.PlayerDoing.Gaming
            && isSwinged == false && ball != null)
        {
            float distance = Vector3.Distance(ico_meet.transform.position, ball.transform.position);
            assistCircle.transform.localScale = new Vector3(distance+1.2f, distance + 1.2f, distance + 1.2f);
            CursorColor(distance);
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                StartCoroutine(PlayerSwing());
            }
        }

        if (ball!= null && isSwinged && !isBallStop)
        {
            float xDifference = ball.transform.position.x - ico_meet.transform.position.x;
            //Debug.Log(xDifference);
            //1300mでオブジェクトを入れ替える処理を行うため現在の差から1300mを何回超えたかを引いています。
            intDiff = Mathf.CeilToInt(xDifference) - (1300 * swapBackgroundObjCount);
            if (beforeIntDiff != intDiff && !gameScoreCountUpAudio.isPlaying)
            {
                //サウンドを再生する
                audioManager.PlaySE(2);
            }
            //if (ballRigidbody != null)
            //{
            //    skyBoxRotation.rotateSpeed = ballRigidbody.velocity.x;
            //    Debug.Log(ballRigidbody.velocity);
            //}
            beforeIntDiff = intDiff;
            lb_gamescore.text = intDiff + (1300 * swapBackgroundObjCount) + "m";
            backgroundManager.SwapBackgroundObjects();
        }

        if (ballMoving && ball != null)
        {
            ball.transform.position += new Vector3(-0.14f - battList[equipBattNumber - 1].speed, 0, 0);//ボールの移動
            ballLine.transform.position += new Vector3(-0.14f - battList[equipBattNumber - 1].speed, 0, 0);//アシストラインの移動
            ball.transform.rotation *= Quaternion.Euler(0f, 0f, 1f);//ボールを回転させる処理
        }

        
        if (popupTime >= 5 && playerManager.playerDoing != PlayerManager.PlayerDoing.Gaming)
        {
            popUpObj.SetActive(true);
        }
        else
        {
            popupTime += Time.deltaTime;
        }

        if (sliderSwitch)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                sliderSwitch = false;
                StartCoroutine(StopBallistics());
            }

            // Sliderの値を増減させる
            if (isIncreasing)
            {
                ballisticsSlider.value += Time.deltaTime;
                float targetAngle = Mathf.Lerp(minAngle, maxAngle, ballisticsSlider.value / ballisticsSlider.maxValue);
                arrowObj.transform.rotation = Quaternion.Euler(targetAngle, 90, 0f);
                if (ballisticsSlider.value >= ballisticsSlider.maxValue)
                {
                    isIncreasing = false;
                    audioManager.SE[2].Play();
                }
            }
            else
            {
                ballisticsSlider.value -= Time.deltaTime;
                float targetAngle = Mathf.Lerp(minAngle, maxAngle, ballisticsSlider.value / ballisticsSlider.maxValue);
                arrowObj.transform.rotation = Quaternion.Euler(targetAngle, 90, 0f);
                if (ballisticsSlider.value <= 0f)
                {
                    isIncreasing = true;
                }
            }
        }
    }

    //ネットワークに接続されているか
    public void NetWorkConection()
    {
        // ネットワークの状態を出力
        switch (Application.internetReachability)
        {
            case NetworkReachability.NotReachable:
                networkConection = false;
                netWorkConectButton.SetActive(true);
                break;
            case NetworkReachability.ReachableViaCarrierDataNetwork:
                Debug.Log("キャリアデータネットワーク経由で到達可能");
                networkConection = true;
                netWorkConectButton.SetActive(false);
                break;
            case NetworkReachability.ReachableViaLocalAreaNetwork:
                Debug.Log("Wifiまたはケーブル経由で到達可能");
                networkConection = true;
                netWorkConectButton.SetActive(false);
                break;
        }

        if (tapCount == 0)
        {
            adMobScript.LoadAd();
        }
    }

    public void NetWorkConectButton()
    {
        // ネットワークの状態を出力
        switch (Application.internetReachability)
        {
            case NetworkReachability.NotReachable:
                networkConection = false;
                netWorkConectButton.SetActive(true);
                break;
            case NetworkReachability.ReachableViaCarrierDataNetwork:
                Debug.Log("キャリアデータネットワーク経由で到達可能");
                networkConection = true;
                netWorkConectButton.SetActive(false);
                break;
            case NetworkReachability.ReachableViaLocalAreaNetwork:
                Debug.Log("Wifiまたはケーブル経由で到達可能");
                networkConection = true;
                netWorkConectButton.SetActive(false);
                break;
        }

        if (tapCount == 0 && networkConection)
        {
            adMobScript.LoadAd();
        }
    }

    //画面をタップした時にコインをゲットする処理
    public void OnClickTraning()
    {
        //吹き出しが表示されていたら非表示にする
        if (popUpObj.activeInHierarchy)
        {
            popUpObj.SetActive(false);
        }
        popupTime = 0;

        playerManager.AnimJump();
        //コインのアニメーションを再生
        particleImage.Play();
        //コインのSEを再生
        audioManager.SE[5].Play();

        int getCoin = 0;
        getCoin += playerManager.traningLv;
        if (tapCount > 0)
        {
            tapCount--;
        }

        PlayerPrefs.SetInt(KEY_SAVE_TAP, tapCount);
        PlayerPrefs.Save();

        tapCountText.text = tapCount.ToString();
        if (tapCount <= 0)
        {
            adMobScript.ShowInterstitialAd(() =>
            {
                tapCount = 50;
                tapCountText.text = tapCount.ToString();
                PlayerPrefs.SetInt(KEY_SAVE_TAP, tapCount);
                PlayerPrefs.Save();
            });
        }
        //広告をロード
        if (tapCount == 10)
        {
            adMobScript.LoadAd();
        }

        if (tapCount == 0 && networkConection)
        {
            NetWorkConection();
        }

        if (networkConection)
        {
            AddCoin(Mathf.RoundToInt(getCoin));
        }
        else
        {
            //ネットワークに接続されていなかったら
            AddCoin(Mathf.RoundToInt(1));
        }
    }

    public void UpdateHiScore(float val)
    {
        hiScore = val;
        highScoreText.SetActive(true);
        lb_hiscore.text = hiScore.ToString("#,0") + "m";
        //Debug.Log("HighScore"+hiScore);
        PlayerPrefs.SetFloat(KEY_SAVE_SCORE, hiScore);
        PlayerPrefs.Save();

        //Yesボタンだった場合、持っているトークン情報があるか確認してランキング登録
        NetWorkConection();
        if (networkConection)
        {
            if(string.IsNullOrEmpty(myToken))
            {
                webApiScript.CoUserAdd();
            }
            else
            {
                webApiScript.CoRankingAddApi();
            }
        }
    }

    public float GetNextBonusTime()
    {
        return 300;
    }

    public int GetBonusPrice(int level)
    {
        return level * 10;
    }

    public void AddCoin(int val)
    {
        //コインが限界値を超えていたら
        if(myCoin + val < 0)
        {
            myCoin = 2147483647;
        }
        else
        {
            myCoin += val;
        }
        lb_coin.text = myCoin.ToString("#,0");
        PlayerPrefs.SetInt(KEY_SAVE_COIN, myCoin);
        PlayerPrefs.Save();
    }

    public int CoinAmount()
    {
        return myCoin;
    }

    public bool CoinPayment(int val)
    {
        if(myCoin >= val)
        {
            myCoin -= val;
            lb_coin.text = myCoin.ToString("#,0");
            return true;
        }
        PlayerPrefs.SetInt(KEY_SAVE_COIN, myCoin);
        PlayerPrefs.Save();
        return false;
    }

    public int PlayerLevelUp(PlayerManager.PlayerLevelType statusType)
    {
        return playerManager.LevelUp(statusType);
    }


    //GAME START
    public void OnClickPlayBall()
    {
        adMobScript.ShowBanner();
        bottomButtons.SetActive(false);

        //早送りボタンを非アクティブにする
        fastforwardButton.SetActive(false);

        //吹き出しを非表示にする
        popUpObj.SetActive(false);

        canvasHome.SetActive(false);
        canvasGame.SetActive(true);
        //カーソルの色を元に戻す
        ico_meet.SetActive(true);
        ico_meet.GetComponent<Renderer>().material.color = Color.white;
        assistCircle.SetActive(false);

        //ボールを動かすスイッチをON
        ballMoving = true;

        //game position
        playerManager.SetGameMode();
        cameraManager.SetPosition(1);

        isSwinged = false;

        StartCoroutine(GameScene());
    }

    public void OnClickRestart()
    {
        Destroy(ballWrapper);
        bottomButtons.SetActive(false);

        //早送りの状態をリセットする
        Time.timeScale = 1f;
        fastforward = false;

        //バナーを非表示にする
        adMobScript.HideBanner();

        //カーソルの色を元に戻す
        ico_meet.SetActive(true);
        ico_meet.GetComponent<Renderer>().material.color = Color.white;
        assistCircle.SetActive(false);

        cameraManager.isFocusBall = true;
        cameraManager.ball = null;
        cameraManager.SetPosition(0);

        //背景のオブジェクトの位置を元に戻して、カウント数も元に戻す
        for (int i = 0; i < backgroundManager.backgroundObjects.Count; i++)
        {
            backgroundManager.backgroundObjects[i].transform.position = Vector3.zero + (backgroundManager.swapPos * i);
        }
        swapBackgroundObjCount = 0;

        StopAllCoroutines();
        OnClickPlayBall();

        //PV用
        //canvasGame.SetActive(false);
    }

    void TextAnimation(TextMeshProUGUI textMeshPro,float duration)
    {
        lb_gamecount.text = "Play Ball";
        textMeshPro.DOFade(0, 0);
        textMeshPro.characterSpacing = -50;

        //文字間隔を開ける
        DOTween.To(() => textMeshPro.characterSpacing, value => textMeshPro.characterSpacing = value, 3, duration)
            .SetEase(Ease.OutQuart);

        //フェード
        DOTween.Sequence()
            .Append(textMeshPro.DOFade(1, duration / 4))
            .AppendInterval(duration / 2)
            .Append(textMeshPro.DOFade(0, duration / 4));
    }

    IEnumerator GameScene()
    {
        //タイムラインを再生する
        playableDirector.Play();

        //弾道ゲージを0にする
        ballisticsSlider.value = 0;
        isIncreasing = true;
        ballisticsSliderObj.SetActive(false);
        playerManager.playerAnimator.speed = 1;
        arrowObj.SetActive(false);

        ico_meet.SetActive(false);
        isBallStop = false;
        lb_gamescore.text = "0m";

        //BGMをゲーム中のものに切り替える
        audioManager.BGM[0].Stop();
        audioManager.PlayBGM(1);
        
        //ピッチャーが持っているボールをアクティブにする
        pitcherBall.SetActive(true);

        //ピッチャー側のアイドルアニメーションを再生
        pitcherAnimator.Play("Pitcher_Start", 0,0);

        yield return new WaitForSeconds(2f);
        //アニメーションを切り替える
        playerManager.playerAnimator.Play("Swing_Idle");

        yield return new WaitForSeconds(2.5f);

        ico_meet.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        //スタート時のテキストをアニメーションさせます
        TextAnimation(lb_gamecount, 2.5f);

        yield return new WaitForSeconds(2.7f);

        //ピッチャー側の投球アニメーションを再生
        pitcherAnimator.Play("Pitching", 0, 0);

        //リセットとホームボタンを表示
        bottomButtons.SetActive(true);

        yield return new WaitForSeconds(0.8f);
        pitcherBall.SetActive(false);//ピッチャーが持っているボールを非アクティブ
        assistCircle.SetActive(true);
        audioManager.PlaySE(4);//投げる時の音を再生

        //ball
        if(equipBattNumber == 15)
        {
            ballWrapper = Instantiate(firePref_ball);
        }
        else
        {
            ballWrapper = Instantiate(pref_ball);
        }
        ballLine = ballWrapper.transform.GetChild(1).gameObject;//アシストライン
        ball = ballWrapper.transform.GetChild(0).gameObject;//ボール単体
        ball.GetComponent<BallManager>().gameManager = this;
        ball.GetComponent<Rigidbody>().useGravity = false;

        ball.transform.position = new Vector3(-264, 0.5f, -141.268f);

        //var ballRigidbody = ball.GetComponent<Rigidbody>();
        //ballRigidbody.AddForce(Vector3.right * -80f);
        //ballRigidbody.AddForce(Vector3. up * 50);

        //yield return null;
    }

    void CursorColor(float distance)
    {
        //moonswordの場合はカーソルが赤になる
        if (distance < 0.3f && -0.3f < distance || equipBattNumber == 4)
        {
            ico_meet.GetComponent<Renderer>().material.color = Color.red;
            isMeet = isMeetColor.red;
        }
        else if(distance < 0.5f && -0.5f < distance)
        {
            ico_meet.GetComponent<Renderer>().material.color = Color.yellow;
            isMeet = isMeetColor.yellow;
        }
        else
        {
            ico_meet.GetComponent<Renderer>().material.color = Color.white;
            isMeet = isMeetColor.white;
        }
    }

    IEnumerator PlayerSwing()
    {
        float dist = Vector3.Distance(ico_meet.transform.position, ball.transform.position);

        if (dist > 6) yield break ;

        //スイングのアニメーションを最初から再生
        playerManager.playerAnimator.Play("Swing_1", 0,0);
        ball.GetComponent<BallManager>().animationPlayNow = true;

        //ボールを動かすスイッチをOFFにする
        ballMoving = false;
        isSwinged = true;

        yield return new WaitForSeconds(0.8f);
        playerManager.playerAnimator.speed = 0;
        sliderSwitch = true;
        ballisticsSliderObj.SetActive(true);
        arrowObj.SetActive(true);
    }

    IEnumerator StopBallistics()
    {
        //sliderを止めた時のサウンドを再生
        float dist = Vector3.Distance(ico_meet.transform.position, ball.transform.position);
        yield return new WaitForSeconds(0.3f);
        ballisticsSliderObj.SetActive(false);
        arrowObj.SetActive(false);
        playerManager.playerAnimator.speed = 1;
        if (isMeet == isMeetColor.red)
        {
            var sequence = DOTween.Sequence(); //Sequence生成
            Vector3 defPos = Camera.main.transform.position;
            if (SystemInfo.supportsVibration)
            {
                Handheld.Vibrate();
            }
            sequence.Append(Camera.main.transform.DOShakePosition(0.2f, 0.3f, 30, 1, false, true))
                .Join(Camera.main.transform.DOPunchPosition(new Vector3(0, 0, 0.3f), 0.2f, 30, 1f))
                .OnComplete(() =>
                {
                    Camera.main.transform.position = defPos;
                });
            //赤いカーソルの時のサウンドを再生
            audioManager.PlaySE(0);
        }
        else if (isMeet == isMeetColor.yellow)
        {
            //黄色いカーソルの時のサウンドを再生
            audioManager.PlaySE(1);
        }
        else if (isMeet == isMeetColor.white)
        {
            //白いカーソルの時のサウンドを再生する
        }

        yield return new WaitForSeconds(0.3f);

        ballLine.SetActive(false);//ボールのアシストラインを非アクティブにする
        ballRigidbody = ball.GetComponent<Rigidbody>();
        ballRigidbody.useGravity = true;

        float meetBounus = 1f;

        switch (isMeet)
        {
            case isMeetColor.red:
                meetBounus = 7f;
                break;
            case isMeetColor.yellow:
                meetBounus = 3f;
                break;
            default:
                break;
        }

        yield return new WaitForSeconds(0.1f);
        if (playerManager.powerLv < 5) meetBounus += 3;

        //山のコライダーをOFFにします
        foreach (MountainColider mountainColider in mountainColiders)
        {
            mountainColider.coliderSwitchTrigger();
        }
        //バットの分のパワーを加算する
        BattStatus battStatus = myBattStatuses.Find(myBattStatus => myBattStatus.battNumber == equipBattNumber);
        int battPower = battStatus.power;
        int battMeet = battStatus.meet;
        float randomPow = UnityEngine.Random.Range(0.8f, 1.2f);
        float randomMeet = UnityEngine.Random.Range(0.8f, 1.2f);

        //禍々しい剣(13_Ominous Sword)だった場合
        if (equipBattNumber == 13)
        {
            randomPow = UnityEngine.Random.Range(-8, 8f);
            randomMeet = UnityEngine.Random.Range(-8, 8f);
        }

        ballRigidbody.AddForce(Vector3.right * (playerManager.powerLv + battPower) * (meetBounus - dist) * randomPow);
        ballRigidbody.AddForce(Vector3.up * (playerManager.meetLv + battMeet) * ballisticsSlider.value * 0.8f * randomMeet);

        
        StartCoroutine(SwingSecence());
    }

    IEnumerator SwingSecence()
    {
        if (ball != null)
        {
            //バットの分のパワーを加算する
            ball.GetComponent<BallManager>().animationPlayNow = false;

            if (ballisticsSlider.value > 0.5f)
            {
                ball.GetComponent<SphereCollider>().isTrigger = true;
                StartCoroutine(BallColliderOn());
            }
        }
        yield return new WaitForSeconds(1.2f);

        //ゲーム画面の場合カメラを切り替える
        if (playerManager.playerDoing == PlayerManager.PlayerDoing.Gaming)
        {
            cameraManager.foucusBall(ball);
            //早送りボタンをアクティブにする
            fastforwardButton.SetActive(true);
        };
    }

    IEnumerator BallColliderOn()
    {
        yield return new WaitForSeconds(0.2f);
        if(ball != null) ball.GetComponent<SphereCollider>().isTrigger = false;
    }

    public void CoinEffect()
    {
        //コインのSEを再生
        audioManager.SE[6].Play();
    }

    public void CoinEffectFiinsh()
    {
        //コインのSEを再生
        audioManager.SE[5].Play();
    }

    public void OnCllisionBall()
    {
        //if (isSwinged)
        //{
        //    isBallStop = true;
        //    var ballRigidbody = ball.GetComponent<Rigidbody>();
        //    ballRigidbody.constraints = RigidbodyConstraints.FreezePosition;

        //    float dist = Vector3.Distance(ico_meet.transform.position, ball.transform.position);
        //    lb_gameresult.text = Mathf.CeilToInt(dist).ToString() + "m";

        //    Debug.Log("飛んだ距離"+Mathf.CeilToInt(dist));
        //    //ハイスコアだった場合はPlayerPrefに保存する
        //    highScoreText.SetActive(false);
        //    if (hiScore < Mathf.CeilToInt(dist))
        //    {
        //        UpdateHiScore(Mathf.CeilToInt(dist));
        //    }

        //    canvasResult.SetActive(true);
        //    canvasGame.SetActive(false);
        //}
        //else
        //{
        //    isSwinged = false;
        //    Destroy(ball.gameObject);
        //    StartCoroutine(GameSecence());
        //}

    }

    //選択したバットを装備する
    public void SetSelectBat()
    {
        equipBattNumber = selectBattNumber;
        Destroy(battParent.GetChild(0).gameObject);
        Instantiate(battList[equipBattNumber - 1].battObj,battParent);
        PlayerPrefs.SetInt(KEY_SAVE_EQUIPBATT, equipBattNumber);
    }

    
    public void FastForward()
    {
        if (!fastforward)
        {
            Time.timeScale = 5f;
            fastforward = true;
        }
        else
        {
            Time.timeScale = 1f;
            fastforward = false;
        }
    }

    public void BackHome()
    {
        Destroy(ballWrapper);
        ico_meet.SetActive(false);
        playerManager.playerAnimator.speed = 1;

        //弾道ゲージを0にする
        //弾道ゲージと矢印を非アクティブにする
        ballisticsSlider.value = 0;
        ballisticsSliderObj.SetActive(false);
        arrowObj.SetActive(false);

        popupTime = 0;
        Time.timeScale = 1f;
        fastforward = false;

        //背景のオブジェクトの位置を元に戻して、カウント数も元に戻す
        for (int i = 0; i < backgroundManager.backgroundObjects.Count; i++)
        {
            backgroundManager.backgroundObjects[i].transform.position = Vector3.zero + (backgroundManager.swapPos * i);
        }

        StopAllCoroutines();
        swapBackgroundObjCount = 0;

        canvasGame.SetActive(false);
        canvasHome.SetActive(true);
        canvasResult.SetActive(false);

        playerManager.SetHomeMode();
        //待機アニメーションに戻す
        playerManager.playerAnimator.Play("Idle");

        //ピッチャー側のアイドルアニメーションを再生
        pitcherAnimator.Play("Pitching_Idle", 0, 0);

        //BGMをホーム画面のものに切り替える
        audioManager.BGM[1].Stop();
        audioManager.PlayBGM(0);

        cameraManager.isFocusBall = false;
        cameraManager.ball = null;
        cameraManager.SetPosition(0);

        webApiScript.CoRankingUpdate();

        //バナーを非表示にする
        adMobScript.HideBanner();
    }


    public void GameContinue(bool ad = false)
    {
        Destroy(ballWrapper);
        ico_meet.SetActive(false);

        popupTime = 0;
        Time.timeScale = 1f;
        fastforward = false;

        //背景のオブジェクトの位置を元に戻して、カウント数も元に戻す
        for (int i = 0;i < backgroundManager.backgroundObjects.Count;i++)
        {
            backgroundManager.backgroundObjects[i].transform.position = Vector3.zero + (backgroundManager.swapPos * i);
        }
        swapBackgroundObjCount = 0;

        canvasHome.SetActive(true);
        canvasResult.SetActive(false);

        playerManager.SetHomeMode();
        //待機アニメーションに戻す
        playerManager.playerAnimator.Play("Idle");

        //ピッチャー側のアイドルアニメーションを再生
        pitcherAnimator.Play("Pitching_Idle", 0, 0);

        //BGMをホーム画面のものに切り替える
        audioManager.BGM[1].Stop();
        audioManager.PlayBGM(0);

        cameraManager.isFocusBall = false;
        cameraManager.SetPosition(0);

        webApiScript.CoPlayDataLog();
        webApiScript.CoRankingUpdate();

        //バナーを非表示にする
        adMobScript.HideBanner();

        //広告側のボタンの場合は動画を再生する
        if (ad)
        {
            adMobScript.ShowAd(() =>
            {
                //コインをゲットする処理
                if(currentScore <= 0)
                {
                    currentScore = 1;
                }
                myCoin += (int)currentScore * 100;
                rewardCount++;
                PlayerPrefs.SetInt(KEY_SAVE_REWARD, rewardCount);
                lb_coin.text = myCoin.ToString("#,0");
                PlayerPrefs.SetInt(KEY_SAVE_COIN, myCoin);
                PlayerPrefs.Save();

                Firebase.Analytics.FirebaseAnalytics.LogEvent("GameEndAds");

                //広告をロード
                adMobScript.LoadAd();
            });
        }
        else
        {
            //コインをゲットする処理
            if (currentScore <= 0)
            {
                currentScore = 1;
            }
            myCoin += (int)currentScore;
            lb_coin.text = myCoin.ToString("#,0");
            PlayerPrefs.SetInt(KEY_SAVE_COIN, myCoin);
            PlayerPrefs.Save();
        }
    }

    public void ActiveBounusPopup()
    {
        if (modelBonusCoin.time_left < 60) return;
        modelBonusCoin.bounusPopupWindow.SetActive(true);
        getCoinBounusText.text = "<sprite=0>" + (Mathf.FloorToInt(modelBonusCoin.bonusCoin * playerManager.traningLv * modelBonusCoin.time_left * 0.5f)).ToString("#,0");
    }

    public void DebugManyCoin()
    {
        AddCoin(999999);
        lb_coin.text = myCoin.ToString("#,0");
    }

    public void ManyCoin()
    {
        startdashPopUp.SetActive(false);

        if (!networkConection)
        {
            return;
        }
        else
        {
            adMobScript.ShowAd(() =>
            {
                AddCoin(1000000);
                lb_coin.text = myCoin.ToString("#,0");
                PlayerPrefs.SetString(KEY_SAVE_FIRSTREWARD, "get");

                PlayerPrefs.SetInt(KEY_SAVE_COIN, myCoin);
                PlayerPrefs.Save();

                Firebase.Analytics.FirebaseAnalytics.LogEvent("GameEndAds");
                bounusParticleImage.Play();

                //広告をロード
                adMobScript.LoadAd();
            });
        }
    }

    public void DebugLevelUp()
    {
        playerManager.powerLv = 999;
        //ボトムのボタンの表示を更新する
        powBtns[0].level = 999;
        powBtns[0].lb_level.text = "Lv." + playerManager.powerLv.ToString();
        powBtns[0].lb_price.text = (playerManager.powerLv * 10).ToString();
    }
}
