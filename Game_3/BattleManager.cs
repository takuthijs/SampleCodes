using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Playables;
using DG.Tweening;
using MoreMountains.Feedbacks;
//using DG.Tweening.TMPro;

public class BattleManager : MonoBehaviour
{
    //UIアニメーター
    public Animator UIAnimator;

    //敵と味方の昆虫オブジェクト
    public GameObject playerBeetleObj;
    public GameObject enemyBeetleObj;

    [Header("スキル関係")]
    //スキルボタン
    public List<Button> skillButtons;

    //スキル発動時のキャンバス
    public GameObject skillExeCanvas;

    //ステータスボーナスが成功した時に表示する画像
    public Image statusBounusSuccessImage;
    public GameObject statusBounusSuccessCanvas;//ここにキャンバスグループがついている

    //現在実行したスキル情報(SKillManagerで使用)
    public GameManager.SkillInfo executeSkillInfo;

    //スキル処理で与えるダメージや回復するダメージ
    public int skillDamageToEnemy = 0;
    public int skillDamageToPlayer = 0;
    public int skillHealToPlayer = 0;
    public int skillHealToEnemy = 0;

    //ターンスタートのコルーチンは常に一個であってほしいためstaticな変数に入れて実行
    public static IEnumerator turnStart;

    //ターン数のテキスト
    public GameObject turnCountObj;
    public TextMeshProUGUI turnText;

    //入力待ちのテキスト
    public GameObject nomalTextObj;
    public TextMeshProUGUI nomalText;

    //ターン表示などのフェーズ（なんの表示をしているか）の背景
    public Image faseImage;
    public List<Sprite> faseImages;

    //自分と敵の昆虫アニメーション
    public Animator playerBeetleAnimator;
    public Animator enemyBeetleAnimator;

    [Header("Feelフィードバックアニメーション")]
    //Feelのフィードバックアニメーション
    public MMFeedbacks playerChargeFeedBacks;
    public MMFeedbacks enemyChargeFeedBacks;

    public MMFeedbacks playerAvoidFeedBacks;
    public MMFeedbacks enemyAvoidFeedBacks;

    //押されたボタンのじゃんけん情報とスキルナンバーを保存
    public int pushSkillNum;
    public string pushSkillUniqueId;
    public Janken pushPlayerJanken;

    //相手の押されたじゃんけんの情報とスキルナンバーを保存
    public int pushEnemySkillNum;
    public string pushEnemySkillUniqueId;
    public Janken pushEnemyJanken;

    //カウントダウンの情報
    public GameObject countDownTextObj;
    public TextMeshProUGUI countDownText;
    public bool isRunning = false;
    float currentTime;

    //勝負あり！のテキスト
    public GameObject battleEndTextObj;

    //HPゲージのオブジェクトとImageの情報
    public TextMeshProUGUI enemyBeetleName;//敵の昆虫の名前
    public GameObject playerHpBackground;//プレイヤー
    public GameObject enemyHpBackground;//敵
    public Image playerHpImage;
    public Image enemyHpImage;

    public int playerHP = 5;
    public int enemyHP = 1;

    public TextMeshProUGUI playerHpValue;
    public TextMeshProUGUI enemyHpValue;

    public int currentTurn = 1;//現在のターン

    //アニメーションのタイムラインの変数
    [HideInInspector] public PlayableDirector inputAnimations;

    public PlayableDirector startCameraWork;//スタートアニメーション
    public PlayableDirector playerFoucusAnimation;//プレイヤーにフォーカスを当てたTimeLine
    public PlayableDirector enemyFoucusAnimation;//敵にフォーカスを当てたTimeLine
    public PlayableDirector commandInputCameraAnimation;//敵にフォーカスを当てたTimeLine
    public PlayableDirector drawCameraWork;//ドローアニメーション
    public PlayableDirector resultCameraWork;//結果アニメーション

    //相手からコマンド情報が来たかどうか
    public bool enemyCommand = false;
    //相手がターンを終了したかどうか
    public bool enemyTurn = false;

    //ボタンのスプライトを保存
    [SerializeField] GameObject commandResultWindow;//コマンド全体のオブジェクト
    [SerializeField] Image playerCommandImage;//自分が出したボタン
    [SerializeField] TextMeshProUGUI playerCommandTopText;//勝ち負けのテキスト
    [SerializeField] TextMeshProUGUI playerCommandText;//画像内のテキスト
    [SerializeField] Image enemyCommandImage;//相手が出したボタン
    [SerializeField] TextMeshProUGUI enemyCommandTopText;//勝ち負けのテキスト
    [SerializeField] TextMeshProUGUI enemyCommandText;//画像内のテキスト
    [SerializeField] List<Sprite> commandImages;
    public GameObject playerCommandObj;
    public GameObject enemyCommandObj;

    //初期状態はゲームスタート
    public BattleStatus battleStatus = BattleStatus.gameStart;

    //バトルの状態
    public enum BattleStatus
    {
        gameStart,//ゲームスタート
        waitInput,//自身の入力待ち
        wait,//相手の入力待ち
        checkCommands,//お互いのコマンドを確認
        battle,//勝敗に応じたアニメーションを実行
        winLoseCheck,//勝敗ついたかチェック
        nextTurn//次のターンを始める前の処理
    }

    //現在誰と対戦しているか
    public BattleMode battleMode = BattleMode.cpu;
    public enum BattleMode
    {
        player,
        cpu
    }

    //じゃんけんの手
    public enum Janken
    {
        guu,
        choki,
        par,
        charge,
        none
    }

    public TurnResult turnResult = TurnResult.none;
    public enum TurnResult
    {
        none,
        win,
        lose
    }

    //最後のバトル結果
    public enum BattleResult
    {
        win,
        lose,
        draw
    }

    [Header("ステータスボーナスキャンバス")]
    //通信中に処理を待つ時に使用
    [HideInInspector] public bool statusBounusReceive = false;

    //自分と相手の計算結果のList
    List<bool> statusResults = new List<bool>();
    [HideInInspector] public List<bool> enemyStatusResults = new List<bool>();

    public List<float> statusProbability = new List<float>();

    //それそれステータスの順番通りにしまう
    public GameObject StatusBounusCanvasObj;//ステータスボーナス全体の親
    public List<TextMeshProUGUI> statusBounusTitleTexts;//ステータスボーナスのタイトル
    public List<GameObject> typematchObjects;//タイプ一致の表示
    public List<TextMeshProUGUI> statusBounusProbablityTexts;//成功確率パーセンテージ
    public List<TextMeshProUGUI> statusBounusResultTexts;//成功したか失敗したかのテキスト
    public List<Image> statusBounusBackgrounds;//ステータスボーナスの背景画像


    [Header("バフアイコン")]

    //昆虫のステータス情報
    //バフレベルを保存
    public List<Image> playerBuffIcons;
    public List<Image> enemyBuffIcons;

    public BeetleManager.Beetle battleBeetle;//現在バトルで使用している自分の昆虫
    public List<BuffInfo> playerBuffInfos = new List<BuffInfo>();
    public List<BuffInfo> enemyBuffInfos = new List<BuffInfo>();

    //チャージテキストアニメーション
    public GameObject chargeAnimationObj;//全体
    public GameObject chargeAnimationObj2;//後ろで動いてる方

    //あいこアニメーション
    public GameObject hitEffect;

    //Todo スキル実行時countの制限回数を入れる
    public class BuffInfo
    {
        public int skillNum;//同じスキルの場合まとめるため()
        public GameManager.GrowStatus statusBuff;
        public int count;//何回分(何ターン分)の効果がかかっているか
        public float magnification;//倍率
        public int turn;//どのターンまで持続するか
    }

    //public enum StatusBuff
    //{
    //    powerBuff,
    //    interBuff,
    //    guardBuff,
    //    speedBuff,
    //}

    public GameObject playerEffectParent;
    public GameObject enemyEffectParent;
    public List<GameObject> effectObject = new List<GameObject>();
    //配列の番号を指定する時に使用
    enum Effects
    {
        buff,
        debuff,
        heal,
        drawHit,
    }

    public void Start()
    {
        //アニメーション確認ようコード
        //enemyAvoidFeedBacks?.PlayFeedbacks();

        GameManager.instance.matchingWindow.SetActive(false);
        countDownTextObj.SetActive(false);

        playerHpBackground.SetActive(false);
        enemyHpBackground.SetActive(false);

        //ターンスタートのコルーチンを保管する
        turnStart = TurnStart();

        //最初のアニメーションを実行
        StartCoroutine(StartAnimation());
    }

    private void FixedUpdate()
    {
        CountDown();
    }

    //初回に実行される
    IEnumerator StartAnimation()
    {
        //カメラワークなどを実行させる処理
        startCameraWork.Play();

        yield return new WaitForSeconds(7f);

        StartCoroutine(turnStart);
    }

    void SkillDamageReset()
    {
        skillDamageToEnemy = 0;
        skillDamageToPlayer = 0;
        skillHealToEnemy = 0;
        skillHealToPlayer = 0;
    }

    public IEnumerator TurnStart()
    {
        ////最初のアニメーションを実行
        //StartAnimation();
        enemyCommand = false;
        enemyTurn = false;
        pushEnemyJanken = Janken.none;
        turnResult = TurnResult.none;

        //通信中のテキストを非アクティブ
        nomalTextObj.SetActive(false);

        //ステータスの抽選結果をクリア
        enemyStatusResults.Clear();
        statusResults.Clear();

        //スキルで与えるダメージなどをリセット
        SkillDamageReset();

        //入っているスキルの情報もリセット
        executeSkillInfo = null;

        isRunning = false;
        currentTime = 10;//カウントを10からスタートさせたいため
        countDownText.text = currentTime.ToString();

        yield return StartCoroutine(FaseAnimation(1,currentTurn+"ターン目"));

        //カウントダウンのUIをアクティブにする
        countDownTextObj.SetActive(true);
        //それぞれのUIをアクティブにする
        playerHpBackground.SetActive(true);
        enemyHpBackground.SetActive(true);

        //バトルステータスを入力待ち状態にする
        battleStatus = BattleStatus.waitInput;

        yield return new WaitForSeconds(1f);
        //タイマーを稼働させる
        isRunning = true;
    }


    //ボタンが押された際にスキル番号を保存
    public void PushSkill(GameObject skillButton)
    {
        //タイマーが稼働していない場合は処理を返す
        if (!isRunning) return;

        //TurnStartの処理が動くことがあるためここでコルーチンをストップ
        StopCoroutine(turnStart);

        //全てのボタンを有効状態にする
        foreach (Button button in skillButtons)
        {
            button.interactable = true;
        }
        //押したボタンは無効状態にする
        skillButton.GetComponent<Button>().interactable = false;

        //じゃんけんの番号とスキル番号を保存
        pushPlayerJanken = skillButton.GetComponent<SkillButton>().janken;
        pushSkillNum = skillButton.GetComponent<SkillButton>().skillNum;
        pushSkillUniqueId = skillButton.GetComponent<SkillButton>().unique_id;

        battleStatus = BattleStatus.wait;//バトルステータスを相手の入力待ち状態に変更する

        //カウントダウンのテキストを非表示にしてカウントを10秒に戻す
        countDownTextObj.SetActive(false);

        //HPゲージなども非表示にする
        playerHpBackground.SetActive(false);
        enemyHpBackground.SetActive(false);

        //コマンド情報をセットしたりポストする
        CommandSetting();
    }


    void CommandSetting()
    {
        //ゲームモードがcpu戦ではない場合ゲームサーバーからの指示を待ちます
        //バトルステータスをコマンド確認状態にする
        if (battleMode == BattleMode.player)
        {
            GameManager.instance.playFabController.postCommand.message = "command";
            GameManager.instance.playFabController.postCommand.jankenCommand = pushPlayerJanken;
            GameManager.instance.playFabController.postCommand.skillNum = this.pushSkillNum;
            GameManager.instance.playFabController.postCommand.skillUniqueId = this.pushSkillUniqueId;
            GameManager.instance.playFabController.postCommand.turn = currentTurn;
            string json = JsonUtility.ToJson(GameManager.instance.playFabController.postCommand);
            GameManager.instance.playFabController.Emit(json, "post");

            //すでに相手からコマンド情報が来ていたらコマンドチェックに移動
            if (enemyCommand) CheckCommands();
        }
        else
        {
            CheckCommands();
        }
    }

    //チャージボタン
    public void ChargeButton(GameObject chargeButton)
    {
        //全てのボタンを有効状態にする
        foreach (Button button in skillButtons)
        {
            button.interactable = true;
        }
        //押したボタンは無効状態にする
        chargeButton.GetComponent<Button>().interactable = false;

        //playerHpValue.text = playerHP.ToString();

        //じゃんけんの番号とスキル番号を保存
        pushPlayerJanken = Janken.charge;
        pushSkillNum = 0;

        battleStatus = BattleStatus.wait;//バトルステータスを相手の入力待ち状態に変更する

        //カウントダウンのテキストを非表示にしてカウントを10秒に戻す
        countDownTextObj.SetActive(false);

        //HPゲージなども非表示にする
        playerHpBackground.SetActive(false);
        enemyHpBackground.SetActive(false);

        //コマンド情報をセットしたりポストする
        CommandSetting();
    }


    //お互いの手を確認する
    public void CheckCommands()
    {
        //バトルステータスをコマンド確認状態にする
        battleStatus = BattleStatus.checkCommands;

        //タイムラインアニメーションをデフォルトの位置に戻す
        inputAnimations.Stop();
        commandInputCameraAnimation.Play();

        //開発中はチョキしか出しません。
        //if (battleMode == BattleMode.cpu) EnemyRandomAIJanken();
        if (battleMode == BattleMode.cpu) pushEnemyJanken = Janken.choki;

        //勝敗チェック
        bool janken = (pushPlayerJanken == Janken.guu && pushEnemyJanken == Janken.choki)
            || (pushPlayerJanken == Janken.choki && pushEnemyJanken == Janken.par)
            || (pushPlayerJanken == Janken.par && pushEnemyJanken == Janken.guu)
            || (pushPlayerJanken != Janken.none && pushEnemyJanken == Janken.none)
            || (pushPlayerJanken != Janken.charge && pushPlayerJanken != Janken.none && pushEnemyJanken == Janken.charge);

        bool drow = (pushPlayerJanken == pushEnemyJanken);

        //勝敗に応じた処理を実行
        StartCoroutine(Battle(janken, drow));
    }

    Sprite CommandImage(Janken janken, TextMeshProUGUI text,Image image)
    {
        switch (janken)
        {
            case Janken.guu:
                text.text = "<size=80><sprite=0 color=#ff0000>";
                image.color = new Color(1,0,0,1);
                return commandImages[0];
            case Janken.choki:
                text.text = "<size=80><sprite=1 color=#FFD300>";
                image.color = new Color(1,0.83f,0,1);
                return commandImages[0];
            case Janken.par:
                text.text = "<size=80><sprite=2 color=#00BCFF>";
                image.color = new Color(0,0.74f,1,1);
                return commandImages[0];
            case Janken.charge:
                text.text = "<size=80><sprite=3 color=#5BC81B>";
                image.color = new Color(0.35f,0.78f,0.1f,1);
                return commandImages[0];
            case Janken.none:
                text.text = "<color=#333333><size=62>時間切れ";
                image.color = new Color(0.3f, 0.3f, 0.3f, 1);
                return commandImages[1];
            default:
                return commandImages[1];
        }
    }

    void SkillDamageCalcation()
    {
        enemyHP -= skillDamageToEnemy;
        playerHP -= skillDamageToPlayer;
        enemyHP += skillHealToEnemy;
        playerHP -= skillHealToPlayer;
    }

    IEnumerator FaseAnimation(int backgroundNum ,string text)
    {
        //ターンのテキストを入れる
        turnText.text = "<size=70>"+text;

        //背景画像を入れ替える
        faseImage.sprite = faseImages[backgroundNum];

        //ターン数の表示
        Sequence sequence = DOTween.Sequence();
        Vector2 temPos = turnCountObj.transform.position;
        CanvasGroup canvasGroup = turnCountObj.GetComponent<CanvasGroup>();
        sequence.Append(turnCountObj.transform.DOMoveY(turnCountObj.transform.position.y + 20, 0.5f))
                .Join(canvasGroup.DOFade(1f, 0.25f))
                .AppendInterval(0.5f)
                .Append(canvasGroup.DOFade(0f, 0.25f))
                .OnComplete(() =>
                {
                    turnCountObj.transform.position = temPos;
                });
        yield return new WaitForSeconds(1.5f);
    }

    //プレイヤーか敵の手に応じたアニメーションを実行
    //Todo GameManagerに技の配列を作成して、そこのアニメーションを実行させる
    IEnumerator Battle(bool janken, bool draw)
    {
        //コマンド同士がぶつかり合うようなアニメーションを実行する
        commandResultWindow.SetActive(true);

        //コマンドの画像とテキストを変える
        playerCommandImage.sprite = CommandImage(pushPlayerJanken, playerCommandText, playerCommandImage);
        enemyCommandImage.sprite = CommandImage(pushEnemyJanken, enemyCommandText, enemyCommandImage);

        //勝ち負けのテキストを追加する
        if (janken)
        {
            playerCommandTopText.text = "<color=#FFA400>勝ち";
            enemyCommandTopText.text = "<color=#47BCFF>負け";
        }
        else if (draw)
        {
            playerCommandTopText.text = "<color=#ffffff>あいこ";
            enemyCommandTopText.text = "<color=#ffffff>あいこ";
        }
        else
        {
            playerCommandTopText.text = "<color=#47BCFF>負け";
            enemyCommandTopText.text = "<color=#FFA400>勝ち";
        }

        CanvasGroup canvasGroup = commandResultWindow.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;

        yield return new WaitForSeconds(2f);

        int i = 0;
        while (canvasGroup.alpha > 0)
        {
            i++;
            canvasGroup.alpha -= 0.1f;
            yield return new WaitForSeconds(0.01f);
            if (i == 100) break;
        }
        commandResultWindow.SetActive(false);

        //自分のステータスボーナスを表示する
        statusProbability.Clear();

        StatusBounusUIReset();
        StatusBounusCanvasObj.SetActive(true);
        yield return StartCoroutine(StatusBounusActivateAnimation("自分のステータスボーナス", battleBeetle.beetleType, battleBeetle));
        StatusBounusCanvasObj.SetActive(false);

        Time.timeScale = 1f;
        statusProbability.Clear();

        yield return new WaitForSeconds(0.2f);

        //相手のステータスボーナスを表示
        StatusBounusUIReset();
        StatusBounusCanvasObj.SetActive(true);

        //結果をポストして今度は相手のステータスボーナスを表示させる
        if (GameManager.instance.playFabController.isMatching)
        {
            GameManager.StatusBounus statusBounus = new GameManager.StatusBounus
            {
                enemyStatusBounus = statusResults
            };
            //上で作成したコマンドをPostCommandに入れてEmit
            GameManager.instance.playFabController.postCommand.statusBounus = statusBounus;
            GameManager.instance.playFabController.postCommand.message = "statusBounus";
            string json = JsonUtility.ToJson(GameManager.instance.playFabController.postCommand);
            GameManager.instance.playFabController.Emit(json, "post");

            //statusBounusReceiveがtrueになるまで処理をまつ
            yield return new WaitUntil(() => statusBounusReceive);

            //待ってる間に相手が切断した場合のために条件を入れています
            if (GameManager.instance.playFabController.isMatching) yield return StartCoroutine(StatusBounusActivateAnimation("相手のステータスボーナス", GameManager.instance.beetleManager.enemyBeetle.beetleType, GameManager.instance.beetleManager.enemyBeetle, false, true));
            if (!GameManager.instance.playFabController.isMatching) yield return StartCoroutine(StatusBounusActivateAnimation("相手のステータスボーナス", GameManager.instance.beetleManager.enemyBeetle.beetleType, GameManager.instance.beetleManager.enemyBeetle, false));
        }
        else
        {
            yield return StartCoroutine(StatusBounusActivateAnimation("相手のステータスボーナス", GameManager.instance.beetleManager.enemyBeetle.beetleType, GameManager.instance.beetleManager.enemyBeetle, false));
        }

        Time.timeScale = 1f;

        statusBounusReceive = false;
        StatusBounusCanvasObj.SetActive(false);


        //どちらかがチャージボタンを押していたら
        if (pushEnemyJanken == Janken.charge || pushPlayerJanken == Janken.charge) yield return StartCoroutine(FaseAnimation(2,"チャージフェーズ"));

        //チャージボタンだった場合はHPを2回復する
        //それぞれの処理の後にチャージアニメーションを入れる
        if (pushEnemyJanken == Janken.charge)
        {
            enemyHP += 2;
            if (enemyHP > 10) enemyHP = 10;

            //カメラを敵の昆虫に向ける
            enemyFoucusAnimation.Play();
            enemyChargeFeedBacks?.PlayFeedbacks();
            yield return new WaitForSeconds(2f);
            ChargeAnimation();
            yield return new WaitForSeconds(2f);
            enemyFoucusAnimation.Stop();
        }

        if (pushPlayerJanken == Janken.charge)
        {
            playerHP += 2;
            if (playerHP > 10) playerHP = 10;

            playerFoucusAnimation.Play();
            playerChargeFeedBacks?.PlayFeedbacks();
            yield return new WaitForSeconds(2f);
            ChargeAnimation();
            yield return new WaitForSeconds(2f);
            playerFoucusAnimation.Stop();
        }

        //Todo ダメージ処理を関数にして成功するか失敗するかや回避した防御成功など様々な処理を行う
        /*
         * 実装したいこと
         * タイプに応じた処理を行う
         * 与ダメージをパワーの数値分×0.1の確率で1増やす
	     * インテリ数値分×0.1の確率でスキルが成功
	     * 100-ガードの数値分×0.1の確率で被ダメージが1増える
	     * あいこ時にスピード数値の高い方が先にスキルを発動できる
        */

        yield return StartCoroutine(FaseAnimation(4, "バトルフェーズ"));
        //勝った時
        if (janken)
        {
            //Debug.Log("勝った");
            turnResult = TurnResult.win;

            //自分にフォーカスを当てたタイムラインの再生
            playerFoucusAnimation.Play();

            playerBeetleAnimator.Play("Attack", 0, 0);

            yield return new WaitForSeconds(1f);

            //スキルの効果キャンバスを表示
            yield return StartCoroutine(PlayerOrEnemySkillExecute());

            SkillDamageCalcation();
            playerFoucusAnimation.Stop();
            commandInputCameraAnimation.Play();

            //ダメージ処理
            //ここで敵のスピード抽選が成功していた場合は回避アニメーション＆通常攻撃無効化
            if (enemyStatusResults[(int)GameManager.GrowStatus.speed] && pushEnemyJanken != Janken.charge && pushEnemyJanken != Janken.none)
            {
                //回避成功のアイコンを表示する
                yield return StartCoroutine(StatusBounusAnimation(GameManager.GrowStatus.speed));

                //プレイヤーの攻撃アニメーションを再生する
                playerBeetleObj.transform.DOPunchPosition(new Vector3(0, 0, 0.5f), 0.5f, 2, 1f);
                yield return new WaitForSeconds(0.2f);

                //回避のfeedbackアニメーションを実行
                enemyAvoidFeedBacks?.PlayFeedbacks();
                yield return new WaitForSeconds(1f);
            }
            else
            {
                //Todoアニメーションを再生する
                playerBeetleObj.transform.DOPunchPosition(new Vector3(0, 0, 0.5f), 0.5f, 2, 1f);
                yield return new WaitForSeconds(0.2f);
                hitEffect.SetActive(true);
                enemyBeetleObj.transform.DOPunchPosition(new Vector3(0.5f, 0, 0), 0.5f, 5, 1f);
                enemyHP -= 1;
                enemyHpImage.fillAmount = enemyHP * 0.1f;
                enemyHpValue.text = enemyHP.ToString();
            }
        }
        //あいこ時
        else if (draw)
        {
            //素早さ判定
            if (battleBeetle.speed > GameManager.instance.beetleManager.enemyBeetle.speed)
            {
                //素早さが勝っている場合”自分のスキル”を発動
                turnResult = TurnResult.win;

                //スキルの効果キャンバスを表示
                yield return StartCoroutine(PlayerOrEnemySkillExecute());
            }
            else if (battleBeetle.speed < GameManager.instance.beetleManager.enemyBeetle.speed)
            {
                //素早さが負けた場合”相手のスキル”を発動
                turnResult = TurnResult.lose;
                //スキルの効果キャンバスを表示
                yield return StartCoroutine(PlayerOrEnemySkillExecute());
            }
            else
            {
                //同じ場合
                turnResult = TurnResult.win;
                //味方
                //スキルの効果キャンバスを表示
                yield return StartCoroutine(PlayerOrEnemySkillExecute());

                turnResult = TurnResult.lose;
                //敵
                //スキルの効果キャンバスを表示
                yield return StartCoroutine(PlayerOrEnemySkillExecute());

            }

            //スキル分のダメージを反映させる
            SkillDamageCalcation();

            //先攻スキル発動後勝敗チェックを行う
            if (enemyHP <= 0 || playerHP <= 0)
            {
                //Debug.Log("判定通過");
                yield return StartCoroutine(BattleCheck());
                yield break;
            }

            SkillDamageReset();

            //素早さが負けている方のダメージ処理
            if (battleBeetle.speed > GameManager.instance.beetleManager.enemyBeetle.speed)
            {
                //素早さが勝っている”場合相手のスキル”発動
                turnResult = TurnResult.lose;
                //スキルの効果キャンバスを表示
                yield return StartCoroutine(PlayerOrEnemySkillExecute());
            }
            else if (battleBeetle.speed < GameManager.instance.beetleManager.enemyBeetle.speed)
            {
                //素早さが負けた場合”自分のスキル”発動
                turnResult = TurnResult.win;
                //スキルの効果キャンバスを表示
                yield return StartCoroutine(PlayerOrEnemySkillExecute());
            }

            //スキル分のダメージを反映させる
            SkillDamageCalcation();

            //後攻スキル発動後勝敗チェックを行う
            if (enemyHP <= 0 || playerHP <= 0)
            {
                yield return StartCoroutine(BattleCheck());
                yield break;
            }

            //ドローアニメーション前にカメラを元の位置に戻す
            commandInputCameraAnimation.Play();
            //Todoスキル発動後あいこのアニメーションを再生する
            playerBeetleObj.transform.DOPunchPosition(new Vector3(1, 0, 0), 0.5f, 10, 1f);
            enemyBeetleObj.transform.DOPunchPosition(new Vector3(0.5f, 0, 0), 0.5f, 5, 1f);
            hitEffect.SetActive(true);

            //ダメージ処理
            playerHP -= 1;
            playerHpImage.fillAmount = playerHP * 0.1f;
            playerHpValue.text = playerHP.ToString();

            //エネミーダメージ処理
            enemyHP -= 1;
            enemyHpImage.fillAmount = enemyHP * 0.1f;
            enemyHpValue.text = enemyHP.ToString();
        }
        //負けた時
        else
        {
            turnResult = TurnResult.lose;

            //敵にフォーカスを当てたタイムラインの再生
            enemyFoucusAnimation.Play();
            //enemyBeetleAnimator.Play("Cast Spell", 0, 0);
            yield return new WaitForSeconds(1f);

            //スキルの効果キャンバスを表示
            yield return StartCoroutine(PlayerOrEnemySkillExecute());

            SkillDamageCalcation();
            enemyFoucusAnimation.Stop();
            commandInputCameraAnimation.Play();

            //アニメーションを再生する
            //自分の素早さボーナスが成功していてチャージコマンドでも時間ぎれでもなかった場合通常攻撃を無効化
            if (statusResults[(int)GameManager.GrowStatus.speed] && pushPlayerJanken != Janken.charge && pushPlayerJanken != Janken.none)
            {
                //回避成功のアイコンを表示する
                yield return StartCoroutine(StatusBounusAnimation(GameManager.GrowStatus.speed));

                enemyBeetleObj.transform.DOPunchPosition(new Vector3(0, 0, -0.5f), 0.5f, 2, 1f);

                //回避のfeedbackアニメーションを実行
                playerAvoidFeedBacks?.PlayFeedbacks();
            }
            else
            {
                //Todoアニメーションを再生する
                enemyBeetleObj.transform.DOPunchPosition(new Vector3(0, 0, -0.5f), 0.5f, 2, 1f);
                yield return new WaitForSeconds(0.2f);
                hitEffect.SetActive(true);
                playerBeetleObj.transform.DOPunchPosition(new Vector3(0.5f, 0, 0), 0.5f, 5, 1f);
                playerHP -= 1;
                playerHpImage.fillAmount = playerHP * 0.1f;
                playerHpValue.text = playerHP.ToString();
            }
        }

        yield return new WaitForSeconds(0.5f);
        hitEffect.SetActive(false);

        //Todo育成ボーナスダメージフェーズ
        yield return StartCoroutine(BattleCheck());
    }


    IEnumerator StatusBounusFase(bool isPlayer)
    {
        //乗っているスキルのバフ効果を変数に保存
        float powerBuff = 0;
        float interBuff = 0;
        float guardBuff = 0;
        float speedBuff = 0;
        List<BuffInfo> buffInfos = new List<BuffInfo>();

        //引数のプレイヤーか敵かでバフ情報を切り替える
        if (isPlayer) buffInfos = playerBuffInfos;
        if (!isPlayer) buffInfos = enemyBuffInfos;

        //かかっているバフ効果ぶんループする
        foreach (BuffInfo buffInfo in buffInfos)
        {
            //同じスキルのカウント変数分ループさせる
            for (int i = 0; i < buffInfo.count; i++)
            {
                switch (buffInfo.statusBuff)
                {
                    case GameManager.GrowStatus.power:
                        powerBuff += buffInfo.magnification;
                        break;
                    case GameManager.GrowStatus.inte:
                        interBuff += buffInfo.magnification;
                        break;
                    case GameManager.GrowStatus.guard:
                        guardBuff += buffInfo.magnification;
                        break;
                    case GameManager.GrowStatus.speed:
                        speedBuff += buffInfo.magnification;
                        break;
                    default:
                        break;
                }
            }
        }

        yield return null;

        //各ステータスで計算する
        //Battle関数内で必要になるタイミングでそれぞれの計算結果を元に処理を分岐させる
        StatusBounus(GameManager.GrowStatus.power, battleBeetle, powerBuff);//trueの場合追加ダメージ1
        StatusBounus(GameManager.GrowStatus.inte, battleBeetle, interBuff);//trueの場合スキル成功
        StatusBounus(GameManager.GrowStatus.guard, battleBeetle, guardBuff);//trueの場合被ダメージが1追加...(じゃんけん勝利の場合はなし)
        StatusBounus(GameManager.GrowStatus.speed, battleBeetle, speedBuff);//trueの場合で自身がスピードタイプの場合は攻撃をかわす（ダメージ無効化・スキル効果は反映）
    }

    void StatusBounusUIReset()
    {
        //テキストをリセットする
        for (int i = 0; i < 4; i++)
        {
            //タイプ一致のオブジェクトを非表示にする
            statusBounusBackgrounds[i].fillAmount = 0;
            typematchObjects[i].SetActive(false);
            statusBounusProbablityTexts[i].text = "";
            statusBounusResultTexts[i].text = "";
        }
    }

    IEnumerator StatusBounusAnime(int i, GameManager.GrowStatus type, BeetleManager.Beetle beetle, bool isPlayer, bool matching)
    {
        //ステータスボーナスの背景
        while (statusBounusBackgrounds[i].fillAmount < 1)
        {
            statusBounusBackgrounds[i].fillAmount += 0.2f; //fillAmountを徐々に増やす
            yield return null; //1フレーム待つ
        }

        yield return new WaitForSeconds(0.2f);

        //タイトルテキストの表示
        StatusBounusTitleTextAnimation(statusBounusTitleTexts[i]);
        yield return new WaitForSeconds(0.1f);

        //Todoバフ分の確率を入れる処理をかく
        //ここの0に入れる関数を作成してステータスの順番でbuffInfoからfindして合計のバフ数値を返す
        float buff = 0;

        //タイプ一致だった時はタイプ一致のオブジェクトを表示させる
        switch (i)
        {
            case 0:
                if (isPlayer) statusResults.Add(StatusBounus(GameManager.GrowStatus.power, beetle, buff));//自分だったら
                if (!isPlayer && !matching) enemyStatusResults.Add(StatusBounus(GameManager.GrowStatus.power, beetle, buff));//cpu戦だったら
                if (!isPlayer && matching) StatusBounus(GameManager.GrowStatus.power, beetle, buff);//マッチング中の相手だったら
                if (type == GameManager.GrowStatus.power) typematchObjects[i].SetActive(true);
                break;
            case 1:
                if (isPlayer) statusResults.Add(StatusBounus(GameManager.GrowStatus.inte, beetle, buff));//自分だったら
                if (!isPlayer && !matching) enemyStatusResults.Add(StatusBounus(GameManager.GrowStatus.inte, beetle, buff));//cpu戦だったら
                if (!isPlayer && matching) StatusBounus(GameManager.GrowStatus.inte, beetle, buff);//マッチング中の相手だった
                if (type == GameManager.GrowStatus.inte) typematchObjects[i].SetActive(true);
                break;
            case 2:
                if (isPlayer) statusResults.Add(StatusBounus(GameManager.GrowStatus.guard, beetle, buff));//自分だったら
                if (!isPlayer && !matching) enemyStatusResults.Add(StatusBounus(GameManager.GrowStatus.guard, beetle, buff));//cpu戦だったら
                if (!isPlayer && matching) StatusBounus(GameManager.GrowStatus.guard, beetle, buff);//マッチング中の相手だった
                if (type == GameManager.GrowStatus.guard) typematchObjects[i].SetActive(true);
                break;
            case 3:
                if (isPlayer) statusResults.Add(StatusBounus(GameManager.GrowStatus.speed, beetle, buff));//自分だったら
                if (!isPlayer && !matching) enemyStatusResults.Add(StatusBounus(GameManager.GrowStatus.speed, beetle, buff));//cpu戦だったら
                if (!isPlayer && matching) StatusBounus(GameManager.GrowStatus.speed, beetle, buff);//マッチング中の相手だった
                if (type == GameManager.GrowStatus.speed) typematchObjects[i].SetActive(true);
                break;
            default:
                break;
        }
        yield return new WaitForSeconds(0.5f);

        //パーセンテージのアニメーションを表示
        if (statusProbability[i] > 100) statusProbability[i] = 100;//100以上なら100に直す
        string probability = "";
        if (statusProbability[i] != 100)
        {
            probability = statusProbability[i].ToString("F1");
        }
        else
        {
            probability = "100";
        }

        statusBounusProbablityTexts[i].DOText(probability + "%", 0.5f, false, ScrambleMode.Numerals).SetEase(Ease.Linear);

        yield return new WaitForSeconds(1f);

        //成功か失敗かの表示
        if (isPlayer)
        {
            if (statusResults[i])
            {
                statusBounusResultTexts[i].text = "<color=#FFA600>成功";
                StatusBounusTitleTextAnimation(statusBounusResultTexts[i]);
            }
            else
            {
                statusBounusResultTexts[i].text = "<color=#FFFFFF>失敗";
                StatusBounusTitleTextAnimation(statusBounusResultTexts[i]);
            }
        }
        else
        {
            if (enemyStatusResults[i])
            {
                statusBounusResultTexts[i].text = "<color=#FFA600>成功";
                StatusBounusTitleTextAnimation(statusBounusResultTexts[i]);
            }
            else
            {
                statusBounusResultTexts[i].text = "<color=#FFFFFF>失敗";
                StatusBounusTitleTextAnimation(statusBounusResultTexts[i]);
            }
        }
    }

    IEnumerator StatusBounusActivateAnimation(string text, GameManager.GrowStatus type, BeetleManager.Beetle beetle, bool isPlayer = true, bool matching = false)
    {
        yield return StartCoroutine(FaseAnimation(3,text));

        for (int i = 0; i < 4; i++)
        {
            StartCoroutine(StatusBounusAnime(i, type, beetle, isPlayer, matching));
            yield return new WaitForSeconds(0.2f); ;
        }
        yield return new WaitForSeconds(4f);
    }


    void StatusBounusTitleTextAnimation(TextMeshProUGUI textMeshPro)
    {
        textMeshPro.DOFade(0, 0);

        DOTweenTMPAnimator tmproAnimator = new DOTweenTMPAnimator(textMeshPro);

        for (int i = 0; i < tmproAnimator.textInfo.characterCount; ++i)
        {
            //有効じゃない文字はnullになるのでスルー
            if (!tmproAnimator.textInfo.characterInfo[i].isVisible)
            {
                continue;
            }
            tmproAnimator.DOScaleChar(i, 1f, 0);
            Vector3 currCharOffset = tmproAnimator.GetCharOffset(i);
            DOTween.Sequence()
                .Append(tmproAnimator.DOOffsetChar(i, currCharOffset + new Vector3(0, 30, 0), 0.4f).SetEase(Ease.OutFlash, 1))
                .Join(tmproAnimator.DOFadeChar(i, 1, 0.4f))
                .Join(tmproAnimator.DOScaleChar(i, 1, 0.4f).SetEase(Ease.OutBack))
                .SetDelay(0.05f * i);
        }
    }

    IEnumerator BattleCheck()
    {
        //Debug.Log("勝敗判定");
        //バトル続行かリザルト画面を表示するかの処理
        if (enemyHP <= 0 || playerHP <= 0)
        {
            //決着のアニメーションを再生して、リザルト画面を表示する
            commandInputCameraAnimation.Play();
            //勝負あり！！！的なオブジェクトを表示させる
            battleEndTextObj.SetActive(true);
            // 初期のアルファ値を0に設定
            battleEndTextObj.GetComponent<CanvasGroup>().alpha = 0;
            // フェードと拡大アニメーションを同時に実行
            Sequence sequence = DOTween.Sequence();
            sequence.Append(battleEndTextObj.GetComponent<CanvasGroup>().DOFade(1, 0.5f)); // 0.5秒かけてアルファ値を1に
            sequence.Join(battleEndTextObj.transform.DOScale(Vector3.one * 1.5f, 0.5f)); // 0.5秒かけて1.5倍の大きさに

            yield return new WaitForSeconds(2f);
            battleEndTextObj.SetActive(false);//勝負あり！のテキストを非表示にする

            //引きで2匹の昆虫が映ったカメラワークにする
            drawCameraWork.Play();
            yield return new WaitForSeconds(1f);
            drawCameraWork.Stop();

            //最初は負けた方にフォーカスして、アニメーションが再生された後、勝った方にフォーカスしてアニメーションを再生する
            switch (BattleResults())
            {
                case BattleResult.win:
                    playerFoucusAnimation.Play();
                    yield return new WaitForSeconds(1f);
                    playerFoucusAnimation.Stop();
                    enemyFoucusAnimation.Play();
                    yield return new WaitForSeconds(1f);
                    enemyBeetleAnimator.Play("Die", 0, 0);
                    break;
                case BattleResult.lose:
                    enemyFoucusAnimation.Play();
                    yield return new WaitForSeconds(1f);
                    enemyFoucusAnimation.Stop();
                    playerFoucusAnimation.Play();
                    yield return new WaitForSeconds(1f);
                    playerBeetleAnimator.Play("Die", 0, 0);
                    break;
                case BattleResult.draw:

                    enemyBeetleAnimator.Play("Die", 0, 0);
                    playerBeetleAnimator.Play("Die", 0, 0);
                    break;
            }

            //バトルを終了する際に必要なことを実行
            if (GameManager.instance.playFabController.socket != null)
            {
                GameManager.instance.playFabController.socket.Disconnect();
            }
            yield return new WaitForSeconds(2f);

            GameManager.instance.resultWindow.SetActive(true);//リザルト画面
            //UIManager.instance.HomeUIUpdate();
            GameManager.instance.uIManager.HomeUIUpdate();

            yield break;
        }
        else
        {
            currentTurn++;
            //エフェクトを更新する
            EffectSwitcher();

            turnStart = null;
            battleStatus = BattleStatus.nextTurn;

            //サーバーへターン終了情報を送って同時に次のターンを開始
            if (battleMode == BattleMode.player)
            {
                //対プレイヤー戦の時に実行
                GameManager.instance.playFabController.postCommand.turn = currentTurn;
                GameManager.instance.playFabController.postCommand.message = "turnEnd";
                string json = JsonUtility.ToJson(GameManager.instance.playFabController.postCommand);
                GameManager.instance.playFabController.Emit(json, "post");

                //入力待ちのテキストをアクティブにする処理をかく、ターン開始に非表示にする処理もかく
                nomalTextObj.SetActive(true);

                //敵のターン終了メッセージを受け取っていたら
                if (enemyTurn)
                {
                    NextTurn();
                }
            }
            else
            {
                NextTurn();
            }
        }
    }

    //playfabのmessageを受けとった際に実行
    public void NextTurn()
    {
        turnStart = TurnStart();
        StartCoroutine(turnStart);
    }


    //PlayerかEnemyがスキルを発動する時の処理
    IEnumerator PlayerOrEnemySkillExecute()
    {
        switch (turnResult)
        {
            case TurnResult.win:
                //スキルをfindする
                executeSkillInfo = GameManager.instance.selectBeetleSkills.selectSkills.Find(selectSkill => selectSkill.unique_id == pushSkillUniqueId);

                //味方インテリの抽選が成功していてスキルセット中なら
                if (statusResults[(int)GameManager.GrowStatus.inte] && executeSkillInfo is not null)
                {
                    //スキル発動成功のエフェクト
                    yield return StartCoroutine(StatusBounusAnimation(GameManager.GrowStatus.inte));
                }
                else
                {
                    //発動失敗なのでリターン
                    yield break;
                }

                SkillExe();

                //Todoスキル情報があればアニメーションを再生する
                if (IsPlayerSkillExecute())
                {
                    if (executeSkillInfo.skillNum != 0)
                    {
                        yield return StartCoroutine(ExeSkillCanvasActivate(3, executeSkillInfo));//3は背景画像のどれを使うか
                    }
                }
                break;

            case TurnResult.lose:
                //スキルをfindする
                executeSkillInfo = GameManager.instance.enemySkills.selectSkills.Find(selectSkill => selectSkill.unique_id == pushEnemySkillUniqueId);

                //敵インテリの抽選が成功していてスキルセット中なら
                if (enemyStatusResults[(int)GameManager.GrowStatus.inte] && executeSkillInfo is not null)
                {
                    //スキル発動成功のエフェクト
                    yield return StartCoroutine(StatusBounusAnimation(GameManager.GrowStatus.inte));
                }
                else
                {
                    //発動失敗なのでリターン
                    yield break;
                }
                EnemySkillExe();

                //Todoスキル情報があればアニメーションを再生する
                if (IsEnemySkillExecute())
                {
                    if (executeSkillInfo.skillNum != 0)
                    {
                        yield return StartCoroutine(ExeSkillCanvasActivate(3, executeSkillInfo));//3は背景画像のどれを使うか
                    }
                }
                break;

            default:
                break;
        }
    }

    //自分のスキルを実行するかどうか
    bool IsPlayerSkillExecute()
    {
        return executeSkillInfo is not null && pushPlayerJanken != Janken.charge && pushPlayerJanken != Janken.none;
    }

    //敵スキルを実行するかどうか
    bool IsEnemySkillExecute()
    {
        return executeSkillInfo is not null && pushEnemyJanken != Janken.charge && pushEnemyJanken != Janken.none;
    }

    //リザルト画面の更新処理もここで行う
    //Todo 順位がハードコーディングになっています。
    BattleResult BattleResults()
    {
        GameManager gameManager = GameManager.instance;
        if (enemyHP <= 0 && playerHP <= 0)
        {
            gameManager.titleEffect.SetActive(false);
            string rateText = gameManager.beetleManager.selectBeetle.rate + "(変化なし)";
            ResultCanvasUpdate("<color=#ffffff>引き分け", "10位", rateText, 50);
            BeetleStatusUpdate(10, gameManager.beetleManager.selectBeetle.rate);
            GameManager.instance.AddCoin(50);
            return BattleManager.BattleResult.draw;
        }
        else if (enemyHP <= 0)
        {
            gameManager.titleEffect.SetActive(true);
            int rate = gameManager.beetleManager.selectBeetle.rate + RateCalculationResult(gameManager.beetleManager.selectBeetle.rate, gameManager.beetleManager.enemyBeetle.rate);
            string rateText = rate + "<color=#ff9f00>(" + RateCalculationResult(gameManager.beetleManager.selectBeetle.rate, gameManager.beetleManager.enemyBeetle.rate) + "UP)";
            ResultCanvasUpdate("<color=#FFAA00>勝利!!!", "1位", rateText, 100);
            BeetleStatusUpdate(1, rate);
            GameManager.instance.AddCoin(100);
            return BattleManager.BattleResult.win;
        }
        else
        {
            gameManager.titleEffect.SetActive(false);
            int rate = gameManager.beetleManager.selectBeetle.rate - RateCalculationResult(gameManager.beetleManager.selectBeetle.rate, gameManager.beetleManager.enemyBeetle.rate);
            string rateText = rate + "<color=#0092FF>(" + RateCalculationResult(gameManager.beetleManager.selectBeetle.rate, gameManager.beetleManager.enemyBeetle.rate) + "DOWN)";
            ResultCanvasUpdate("<color=#0092FF>敗北...", "100位", rateText, 10);
            BeetleStatusUpdate(100, rate);
            GameManager.instance.AddCoin(10);
            return BattleManager.BattleResult.lose;
        }
    }

    //自分がじゃんけんに勝った時スキルを実行
    public void SkillExe()
    {
        skillExeCanvas.GetComponent<SkillExeCanvas>().skillExeTitle.text = "自分のスキル発動!!!";
        //セットされているスキル番号を実行
        //現在実行中のスキルを変数に入れる、skillManagerの方でその値を参照して攻撃する
        SkillManager.instance.skills[pushSkillNum]();
    }

    //敵がじゃんけんに勝った時スキルを実行
    public void EnemySkillExe()
    {
        skillExeCanvas.GetComponent<SkillExeCanvas>().skillExeTitle.text = "相手のスキル発動!!!";
        //セットされているスキル番号を実行
        //現在実行中のスキルを変数に入れる、skillManagerの方でその値を参照して攻撃する
        SkillManager.instance.skills[pushEnemySkillNum]();
    }

    IEnumerator StatusBounusAnimation(GameManager.GrowStatus status)
    {
        //引数のステータスでアイコンを切り替える
        //numにはGameManagerのenumをキャストしたものを入れる
        switch (status)
        {
            case GameManager.GrowStatus.power:
                statusBounusSuccessImage.sprite = GameManager.instance.typeIcons[(int)status];
                statusBounusSuccessImage.color = new Color(1, 0, 0, 0.5f);
                break;
            case GameManager.GrowStatus.inte:
                statusBounusSuccessImage.sprite = GameManager.instance.typeIcons[(int)status];
                statusBounusSuccessImage.color = new Color(1, 0.5f, 0, 0.5f);
                break;
            case GameManager.GrowStatus.guard:
                statusBounusSuccessImage.sprite = GameManager.instance.typeIcons[(int)status];
                statusBounusSuccessImage.color = new Color(0, 0.56f, 0, 0.5f);
                break;
            case GameManager.GrowStatus.speed:
                statusBounusSuccessImage.sprite = GameManager.instance.typeIcons[(int)status];
                statusBounusSuccessImage.color = new Color(0, 0.5f, 1, 0.5f);
                break;
        }



        //ツイーンでAnimationを再生
        //Tweenの設定
        Vector3 tempScale = new Vector3();
        DOTween.Sequence()
            .OnStart(() =>
            {
                tempScale = statusBounusSuccessCanvas.transform.localScale;
                statusBounusSuccessCanvas.SetActive(true);
            })
            .Append(statusBounusSuccessImage.DOFade(0.8f, 0.5f))
            .Join(statusBounusSuccessCanvas.transform.DOScale(Vector3.one * 1.5f, 0.5f))
            .AppendInterval(0.5f)
            .Append(statusBounusSuccessCanvas.transform.DOScale(Vector3.one * 2.0f, 0.5f))
            .Join(statusBounusSuccessImage.DOFade(0, 0.5f))
            .OnComplete(() =>
            {
                statusBounusSuccessCanvas.SetActive(false);
                statusBounusSuccessCanvas.transform.localScale = tempScale;
            }).Play();
        yield return new WaitForSeconds(1.5f);
    }

    //発動したスキルを表示する処理
    IEnumerator ExeSkillCanvasActivate(float waitTime, GameManager.SkillInfo skillInfo)
    {
        //Todoスキルアニメーションを再生する
        //スキル情報をキャンバスに入れる
        SkillExeCanvas skillExeCanvas = this.skillExeCanvas.GetComponent<SkillExeCanvas>();
        skillExeCanvas.skillImage.sprite = GameManager.instance.skillInfo[skillInfo.skillNum - 1].skillImage;
        skillExeCanvas.skillName.text = skillInfo.skillName;
        skillExeCanvas.skillCost.text = "スキルコスト：" + skillInfo.cost;
        skillExeCanvas.skillLevel.text = "スキルレベル：" + skillInfo.level;
        skillExeCanvas.requiredPower.text = "<sprite=0 color=#ff0000>" + skillInfo.statusConditions[0].statusLevel;
        skillExeCanvas.requiredInter.text = "<sprite=1 color=#ffff00>" + skillInfo.statusConditions[1].statusLevel;
        skillExeCanvas.requiredGuard.text = "<sprite=2 color=#00ff00>" + skillInfo.statusConditions[2].statusLevel;
        skillExeCanvas.requiredSpeed.text = "<sprite=3 color=#0080ff>" + skillInfo.statusConditions[3].statusLevel;
        skillExeCanvas.skillDescription.text = skillInfo.skillDescription;

        this.skillExeCanvas.SetActive(true);
        yield return new WaitForSeconds(waitTime);
        this.skillExeCanvas.SetActive(false);
    }

    //カウントダウンの処理
    void CountDown()
    {
        //カウントダウン処理
        //タイマーが稼働状態で入力待ち状態だった場合
        if (isRunning && battleStatus == BattleStatus.waitInput)
        {
            // タイマーが動作中の場合、残り時間を更新して表示する
            currentTime -= Time.deltaTime;
            countDownText.text = currentTime.ToString("F0");

            // タイマーが0以下になったら停止する
            if (currentTime <= 0f)
            {
                countDownTextObj.SetActive(false);
                isRunning = false;

                //相手が出した手に負けるボタンをアクティブにする
                pushPlayerJanken = Janken.none;
                //ボタンを全て選択可能にする
                //全てのボタンを有効状態にする
                foreach (Button button in skillButtons)
                {
                    button.interactable = true;
                }
                //HPゲージなども非表示にする
                playerHpBackground.SetActive(false);
                enemyHpBackground.SetActive(false);
                battleStatus = BattleStatus.wait;
                CommandSetting();
            }
        }
    }

    //リザルト画面のアップデート処理を行う
    void ResultCanvasUpdate(string battleResult, string rankText, string rateText, int getCoin)
    {
        //Todo 昆虫を生成して大きさも変えてレイヤーも変える
        //リザルト画面の昆虫を削除する
        foreach (Transform n in GameManager.instance.resultBeetleParent.transform)
        {
            GameObject.Destroy(n.gameObject);
        }

        //バトルの昆虫を生成する
        GameObject resultBeetleObj = Instantiate(playerBeetleObj, GameManager.instance.resultBeetleParent.transform);
        //レイヤーを変更する
        GameManager.instance.SetLayerRecursively(resultBeetleObj,6);

        GameManager.instance.battleResultTitle.text = battleResult;
        GameManager.instance.resultRankText.text = rankText;
        GameManager.instance.resultRateText.text = rateText;
        GameManager.instance.resultCoinText.text = "<sprite=0>" + getCoin + "コイン";
    }

    //昆虫のレートやランクなどのアップデート処理を行う
    void BeetleStatusUpdate(int rank, int rate)
    {
        GameManager.instance.beetleManager.selectBeetle.rank = rank;
        GameManager.instance.beetleManager.selectBeetle.rate = rate;
        //更新したタイミングで自分の持っている昆虫リストも更新する
        //現在選択中の昆虫のunique_idでmyBeetlesからFindしたものを更新する
        //selectBeetleのunique_idと一致する要素を検索
        BeetleManager.Beetle foundBeetle = GameManager.instance.beetleManager.myBeetles.Find(beetle => beetle.unique_id == GameManager.instance.beetleManager.selectBeetle.unique_id);

        //新しいBeetleオブジェクトを作成して情報をコピー
        BeetleManager.Beetle beetle = GameManager.instance.NewBeetle(GameManager.instance.beetleManager.selectBeetle);

        // 条件に一致する要素が見つかった場合(見つからないことは基本的にない)
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
    }

    //ランダムでじゃんけんの手を選択する処理
    private void EnemyRandomAIJanken()
    {
        // ランダムにJankenの要素を選んでpushEnemyJankenに代入
        //pushEnemyJanken = (Janken)Random.Range(0, System.Enum.GetValues(typeof(Janken)).Length);
        int randomIndex = Random.Range(0, System.Enum.GetValues(typeof(Janken)).Length - 2);
        pushEnemyJanken = (Janken)randomIndex;
    }

    //レートの計算結果を返す処理 *0.5の場合は偶数を返します。 
    private int RateCalculationResult(int playerRate, int enemyRate)
    {
        return Mathf.RoundToInt(16 + (enemyRate - playerRate) * 0.04f);
    }

    //それぞれのパラメーターの特性が発生したかどうかを判定
    bool StatusBounus(GameManager.GrowStatus growStatus, BeetleManager.Beetle beetle, float addProbability)
    {
        //引数のステータスの計算をする
        float randomNum = Random.Range(0, 101);
        float statusProbability = 0;

        //引数の昆虫のパラメータを取得
        switch (growStatus)
        {
            case GameManager.GrowStatus.power:
                statusProbability = 100 - beetle.power * 0.1f;
                break;
            case GameManager.GrowStatus.inte:
                statusProbability = 100 - beetle.inter * 0.1f;
                break;
            case GameManager.GrowStatus.guard:
                statusProbability = 100 - beetle.guard * 0.1f;
                break;
            case GameManager.GrowStatus.speed:
                statusProbability = 100 - beetle.speed * 0.1f;
                break;
        }

        //スキルレベル分成功確率を上げる
        statusProbability += addProbability * statusProbability;
        statusProbability = 100 - statusProbability;

        //確率を表示させるためにstatusProbabilityをグローバル変数に入れます
        this.statusProbability.Add(statusProbability);

        //ステータスの数値の方がランダムの数字より大きい場合は成功
        if (statusProbability >= randomNum)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    void ChargeAnimation()
    {
        chargeAnimationObj.SetActive(true);
        //ターン数の表示
        Sequence sequence = DOTween.Sequence();
        CanvasGroup canvasGroup = chargeAnimationObj.GetComponent<CanvasGroup>();
        sequence.Append(canvasGroup.DOFade(1, 0.5f))
                .Join(chargeAnimationObj2.transform.DOScale(2f, 0.5f))
                .Join(chargeAnimationObj2.GetComponent<Image>().DOFade(0, 0.5f))
                .AppendInterval(0.5f)
                .Append(canvasGroup.DOFade(0f, 0.5f))
                .OnComplete(() =>
                {
                    chargeAnimationObj2.transform.localScale = new Vector3(1, 1, 1);
                    chargeAnimationObj.SetActive(false);
                    chargeAnimationObj2.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                });
    }


    //昆虫にかけられているスキル効果に応じてエフェクトを切り替える
    void EffectSwitcher()
    {
        //自分と敵のエフェクトをとりあえず削除する
        foreach (Transform n in playerEffectParent.transform)
        {
            GameObject.Destroy(n.gameObject);
        }
        foreach (Transform n in enemyEffectParent.transform)
        {
            GameObject.Destroy(n.gameObject);
        }

        //Todoここに現在のバフ効果をみて必要なエフェクトを生成する
    }
}
