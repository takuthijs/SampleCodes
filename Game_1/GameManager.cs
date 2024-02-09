using System.Collections;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using System;
using System.IO;
using System.Linq;
using UnityEngine.Pool;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Header("セーブデータ")]
    public SaveManager saveManager;

    [Header("経験値成長率")]
    public int experienceGrowthRate = 100; // 経験値の成長率（1秒あたり）

    [Header("ミニゲームリスト")]
    private List<string> miniGameList = new List<string> { "NumberTap","MaxNumberTap","Roulette","SmashTap"};

    [Header("プレイヤー情報")]
    public string playerName = "Takuto";
    public int playerRank = 1;//プレイヤーのランク
    public int currentPlayerExp;
    public int nextPlayerExp;
    public int jem = 1;//ジェム
    public int maxStamina;//MAXスタミナ
    public int currentStamina;//MAXスタミナ
    public int gold;//お金
    [HideInInspector] public int getCharacterIndex = 10;//今までゲットしたキャラクターのインデックス 11からスタート
    [HideInInspector] public int getEquipIndex = 10;//今までゲットしたキャラクターのインデックス 11からスタート
    [SerializeField]public Dictionary<string, int> miniGamePlayInfo = new Dictionary<string, int>();//ミニゲームのクリア情報
    //ディクショナリーでは保存できないので一旦KeyとValueにして保存する
    [HideInInspector] public List<string> miniGameInfoKey;
    [HideInInspector] public List<int> miniGameInfoValue;

    public Slider playerExpSlider;
    public TextMeshProUGUI expSliderText;
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI playerRankText;
    public TextMeshProUGUI staminaText;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI jemText;

    [Header("パーティー情報")]
    public int selectPartyNumber;//選択中のパーティー番号
    public int selectCardNumber;//選択中のカード番号
    public int selectCharaInfoUniqueID;//現在キャラ詳細に表示しているキャラのunique_id

    public GameObject partyCardEntity;//パーティーにモンスターがセットされていた時に生成するオブジェクト
    public GameObject emptyCardEntity;//パーティーにモンスターがセットされていなかった時に生成するオブジェクト
    public GameObject charaListCardEntity;//キャラ一覧で生成されるオブジェクト
    public GameObject emptyListCardEntity;//キャラ一覧で空き場所に生成されるオブジェクト

    public List<GetCharaParam> getCharaParams = new List<GetCharaParam>();//持っているキャラ情報

    public string selectCharaGenre;
    public List<GetCharaParam> getCharaParamsSort = new List<GetCharaParam>();//キャラをソートしたデータ情報

    public List<GetEquipParam> getEquipParams = new List<GetEquipParam>();//持っている装備情報
    public List<int> lobbyCharaTrainingIds = new List<int>();//ロビーにいたキャラクター
    public GameObject monsterPoop;
    public List<GameObject> monsterPoops = new List<GameObject>();//配置されているモンスターの糞
    public GameObject partyParent;//パーティー3チームが入っている親オブジェクト
    public GameObject charaListParet;//一覧を配置する親オブジェクト
    public List<Party> playerParty = new List<Party>();//プレイヤーのパーティー情報
    public CharacterDefaultParam[] characterDefaultParams;//キャラクターのデフォルトパラメーター
    public EquipDefaultParam[] equipDefaultParams;//装備のデフォルトパラメーター
    // ToDOゲームを辞めた時に選択しているパーティー情報も保存しておき、再開時保存された情報を元にトグルの状態も変える

    [Header("装備に使う情報")]
    public string selectGenre;
    public List<GetEquipParam> getEquipParamsSort = new List<GetEquipParam>();
    public SetEquipment setEquipment;
    public GameObject allParent;//全ての装備のタブ
    public GameObject weaponParent;//武器の装備のタブ
    public GameObject shieldParent;//シールドの装備のタブ
    public GameObject itemParent;//アイテムの装備のタブ

    public int generateNum = 0; //何番目に生成されたオブジェクトか
    public GameObject equipItemSlotObj;//実際に表示させるオブジェクト
    public List<GameObject> generateItems;//生成した入手済みの装備アイテム
    [HideInInspector] public List<int> equipNowUniqueIdList;//キャラクターが装備しているidのリスト
    //[HideInInspector] public List<GetEquipParam> generateItemsScript;//生成したオブジェクトと同じ順序に格納される装備のパラメータ

    [Header("ステージのクリア情報")]
    public List<StagePlayerInfo> stageInfo;

    [Header("Animatorの配列")]
    public List<RuntimeAnimatorController> animatorControllers = new List<RuntimeAnimatorController>();

    [Space(10)]

    public bool isGameRunning;//ゲームが実行中かどうか
    [Space(10)]

    [Header("試合中表示されるテキスト")]
    public GameObject MonsterEntity;//実際のゲームで生成する時に使用するオブジェクト
    public TextMeshProUGUI conboText;//コンボテキスト
    public TextMeshProUGUI tensionText;//テンションテキスト
    [HideInInspector] public int conboCount = 0;
    [Space(10)]

    [HideInInspector] public bool buttonPush = false;//現状使ってないが今後すぐ使うかも
    [Space(10)]

    [Header("ミスしなかった時の表示")]
    public GameObject niceComboObj;
    public Image niceComboFrame;
    public TextMeshProUGUI niceComboText;
    [Space(10)]

    //プレイヤーのキャラ情報を保管する変数
    [Header("ロビーに配置するキャラクター")]
    public GameObject lobbyCharacterParent;
    public GameObject lobbyCharaEntity;
    public RectTransform rangeA;
    public RectTransform rangeB;
    [Space(10)]

    //生成したパーティーキャラを格納しておく
    public List<GameObject> partyCharaCards;
    //生成したキャラリストのオブジェクトを格納しておく
    public List<GameObject> charaListCards;

    //プレイヤーのキャラ情報を保管する変数
    [Header("各々の大将")]
    //public Button playerGeneralButton;
    public Button enemyGeneralButton;

    [Header("ゲーム中のキャラ情報・マス目情報")]
    public GameObject gameSceneScrollView;//ゲームシーンのスクロールできるオブジェクト、ポジションを戻したりするために使用
    [HideInInspector] public int playerSkillSelectGrid;//プレイヤーがスキルで選択したグリッドナンバー
    [HideInInspector] public bool playerGrid;//プレイヤーのグリッドと敵のグリッドどちらか
    private int activeEnemyGrid;//これから行動する敵キャラクター

    [Header("ゲーム中のプレイヤーのキャラ情報・マス目を保管する変数")]
    [HideInInspector] public Dictionary<int,GameObject> playerCharacterPos = new Dictionary<int, GameObject>();//キャラクターがどのマスにいるかの情報
    [HideInInspector] public Dictionary<int, GameObject> playerShieldPos = new Dictionary<int, GameObject>();//味方マスにあるシールドの情報
    public List<GameObject> playerGrids;//マス目
    public GameObject characterParent;

    [Header("ゲーム中の敵のキャラ情報・マス目を保管する変数")]
    [HideInInspector] public Dictionary<int, GameObject> enemyCharacterPos = new Dictionary<int, GameObject>();//キャラクターがどのマスにいるかの情報
    public List<GameObject> enemyAICharacters;
    [HideInInspector] public Dictionary<int, GameObject> enemyShieldPos = new Dictionary<int, GameObject>();//敵マスにあるシールドの情報
    public List<GameObject> enemyGrids;//マス目
    public GameObject enemyParent;

    [HideInInspector] public List<GameObject> commandWindows;//プレイヤーのコマンドウィンドウ
    [HideInInspector] public List<GameObject> selectCloneCharacters = new List<GameObject>();//生成されたキャラクターを保存しておく
    [Space(10)]

    //選択されたステージの敵情報などを保管する変数
    [Header("選択されたステージの敵情報などを保管する変数")]
    //public GameObject enemyCharacterParent;
    //public List<GameObject> enemyCharacters = new List<GameObject>();//選択されたステージの敵キャラクターを保管

    //public List<GameObject> enemyCharactersTest = new List<GameObject>();

    [HideInInspector] public List<GameObject> selectCloneEnemyCharacters = new List<GameObject>();//生成された敵キャラクターを保存しておく
    //public List<GameObject> enemyCharaPositions = new List<GameObject>();//敵キャラクターの生成位置
    [Space(10)]

    //タイマー系変数
    [Header("タイマー系変数")]
    public float totalTime = 60f; // 制限時間の秒数
    public float currentTime = 60f; // 現在の残り時間
    public TextMeshProUGUI timerText; // 制限時間を表示するテキスト
    private bool isRunning = false; // タイマーが動作中かどうかのフラグ
    [Space(10)]

    [HideInInspector] public GameObject foucusChara;//ゲーム中の選択中のキャラ
    [HideInInspector] public CharaParam foucusCharaParam; //選択中のキャラパラメータ

    public Slider enemySlider;
    public int enemyHP = 0;
    public float enemyCurrentHP;
    public TextMeshProUGUI enemyHPText;

    public Slider playerSlider;
    public int playerHP;
    public float playerCurrentHP;
    public TextMeshProUGUI PlayerHPText;

    public GameObject slotParent;
    public GameObject slotPrefab;

    //DoTweenアニメーションで使用している変数
    private DG.Tweening.Sequence _seq;
    public GameObject atackPosition;
    public GameObject playerHpGage;
    public GameObject enemyHpGage;

    [System.Serializable]
    public class GetCharaParam
    {
        public int unique_id;//ユニークID
        public int character_id;//キャラクターのID
        public int rarity;//レアリティ
        public int resetNum;//キャラのリセット回数
        public int maxLevel;
        public int currentLevel;
        public string charaName;
        public Sprite sprite;
        public int animatorControllerNum;
        public int skillNum;
        public int skillGameNum;
        public int skillCost;
        public int skillTime;
        public string description;
        public int HP;
        public int Attack;
        public int Defence;
        public int exp;
        public int nextExp;
        public string date;//例 20230123121820 2023年1月23日12時18分20秒
        public string setPartyTime;
        public List<int> equipItem = new List<int>();//装備6個
        public int feedNum;//使用された餌
    }

    [System.Serializable]
    public class Party
    {
        public List<int> unique_id;
    }

    [System.Serializable]
    public class StagePlayerInfo
    {
        public bool active;
        public bool complete;
        public List<State> state;
    }

    [System.Serializable]
    public class State
    {
        public bool active;
        public bool complete;
        public bool bonusGet;//コインをゲットしたかどうか
        public int stars;
    }

    [System.Serializable]
    public class GetEquipParam
    {
        public int equip_id;//装備のID
        public int unique_id;//ユニークID
        public int rarity;//レアリティ
        public string genre;//装備のジャンル
        public int maxLevel;
        public int currentLevel;
        public string equipName;
        public Sprite sprite;
        public int animatorControllerNum;
        public int equipCost;

        public string description;

        public int increaseHP;
        public int increaseAttack;
        public int increaseDefence;
        public int increaseSkillTime;

        public int exp;
        public int nextExp;

        public string date;//例 20230123121820 2023年1月23日12時18分20秒
        public int genarateNum;//何番目に生成されたか　セーブはしない
    }


    private void Awake()
    {
        instance = this;

        //TODO セーブデータのテストロード
        //セーブデータが存在したら
        if (File.Exists(saveManager.filePath))
        {
            saveManager.Load();
            //SaveManagerでセットしようとしたが、スクリプトの実行順序的にこちらに書きました。
            lobbyCharaTrainingIds = saveManager.save.lobbyCharaTrainingIds;//ロビーにセットされていたキャラクター達
            playerName = saveManager.save.playerName;
            playerRank = saveManager.save.playerRank;//プレイヤーのランク
            currentPlayerExp = saveManager.save.currentPlayerExp;
            nextPlayerExp = saveManager.save.nextPlayerExp;
            jem = saveManager.save.jem;//ジェム
            maxStamina = saveManager.save.maxStamina;//MAXスタミナ
            currentStamina = saveManager.save.currentStamina;//MAXスタミナ
            gold = saveManager.save.gold;//お金
            getCharacterIndex = saveManager.save.getCharacterIndex;//今までゲットしたキャラの数11からスタート
            getEquipIndex = saveManager.save.getEquipIndex;//今までゲットした装備のIndex、11からスタート
            getCharaParams = saveManager.save.getCharaParams;
            selectPartyNumber = saveManager.save.selectPartyNumber;//選択中のパーティー番号
            getEquipParams = saveManager.save.getEquipParams;//持っている装備情報
            playerParty = saveManager.save.playerParty;//プレイヤーのパーティー情報
            stageInfo = saveManager.save.stageInfo;

            miniGamePlayInfo.Clear();

            //ミニゲームのクリア状況
            for (int i = 0; i < saveManager.save.miniGameInfoKey.Count; i++)
            {
                miniGamePlayInfo.Add(saveManager.save.miniGameInfoKey[i], saveManager.save.miniGameInfoValue[i]);//???たまにエラーが発生
            }

            //ミニゲームのクリア情報がなかったらキーを追加する
            foreach(string minigameName in miniGameList)
            {
                if(!miniGamePlayInfo.ContainsKey(minigameName)) miniGamePlayInfo.Add(minigameName, 0);
            }

            //キャラ画像を設定しなおす
            for (int i = 0; i< getCharaParams.Count; i++)
            {
                getCharaParams[i].sprite = characterDefaultParams[getCharaParams[i].character_id - 1].sprite;
            }

            //アイテム画像をセットし直す
            for (int i = 0; i < getEquipParams.Count; i++)
            {
                getEquipParams[i].sprite = equipDefaultParams[getEquipParams[i].equip_id - 1].sprite;
            }
            Debug.Log("ロード成功");
        }

        //そーで使っているジャンル情報を初期化
        selectGenre = "All";
        selectCharaGenre = "All";
    }

    public void ParamUPTest(int x)
    {
        //return UnityEngine.Random.Range(1, getCharaParam.Attack / 5 - (1 / getCharaParam.currentLevel));
        int num = x;
        for (int i = 1; i < 99; i++)
        {
            num = num + num / i;
        }
        Debug.Log(x +":"+num);
    }

    void Start()
    {
        //Todo初期キャラクターをどうするか,デフォルトのキャラクターを一体自動でセットする
        //現状セーブデータがない場合は仮でセットしたキャラクター（scriptableObjectの値のまま）を入れています。
        if (!File.Exists(saveManager.filePath))
        {
            for (int i = 0; i < 10; i++)
            {
                GetCharaParam chara = new GetCharaParam();
                chara.unique_id = getCharacterIndex + 1;
                chara.character_id = characterDefaultParams[i].character_id;//キャラクターのID
                chara.rarity = characterDefaultParams[i].rarity;//レアリティ
                chara.maxLevel = characterDefaultParams[i].maxLevel;
                chara.currentLevel = characterDefaultParams[i].currentLevel;
                chara.charaName = characterDefaultParams[i].charaName;
                chara.sprite = characterDefaultParams[i].sprite;

                chara.animatorControllerNum = characterDefaultParams[i].animatorControllerNum;

                chara.skillGameNum = characterDefaultParams[i].skillGameNum;
                chara.skillNum = characterDefaultParams[i].skillNum;
                chara.skillCost = characterDefaultParams[i].skillCost;

                chara.skillTime = characterDefaultParams[i].skillTime;
                chara.description = characterDefaultParams[i].description;

                chara.HP = characterDefaultParams[i].HP;
                chara.Attack = characterDefaultParams[i].Attack;
                chara.Defence = characterDefaultParams[i].Defence;

                chara.exp = characterDefaultParams[i].exp;
                chara.nextExp = characterDefaultParams[i].nextExp;

                chara.date = characterDefaultParams[i].date;//例 20230123121820 2023年1月23日12時18分20秒

                getCharacterIndex++;
                getCharaParams.Add(chara);


                GetEquipParam equip = new GetEquipParam();
                equip.equip_id = equipDefaultParams[i].equip_id;//装備のID
                equip.unique_id = getEquipIndex + 1;//ユニークID
                equip.rarity = equipDefaultParams[i].rarity;//レアリティ
                equip.genre = equipDefaultParams[i].genre;//装備のジャンル
                equip.maxLevel = equipDefaultParams[i].maxLevel;
                equip.currentLevel = equipDefaultParams[i].currentLevel;
                equip.equipName = equipDefaultParams[i].equipName;
                equip.sprite = equipDefaultParams[i].sprite;
                equip.animatorControllerNum = equipDefaultParams[i].animatorControllerNum;
                equip.equipCost = equipDefaultParams[i].equipCost;

                equip.description = equipDefaultParams[i].description;

                equip.increaseHP = equipDefaultParams[i].increaseHP;
                equip.increaseAttack = equipDefaultParams[i].increaseAttack;
                equip.increaseDefence = equipDefaultParams[i].increaseDefence;

                equip.exp = equipDefaultParams[i].exp;
                equip.nextExp = equipDefaultParams[i].nextExp;

                equip.date = equipDefaultParams[i].date;//例 20230123121820 2023年1月23日12時18分20秒

                getEquipIndex++;
                getEquipParams.Add(equip);
            }

            //ミニゲームのクリア情報がなかったらキーを追加する
            foreach (string minigameName in miniGameList)
            {
                if (!miniGamePlayInfo.ContainsKey(minigameName)) miniGamePlayInfo.Add(minigameName, 0);
            }
        }

        LobbyParamSet();//ロビーのプレイヤー名・ジェム・ゴールド・経験値・スタミナなどをセット
        SetParty();//パーティー情報をセット
        SetCharaList();//キャラクター情報をセット
        SetLobbyCharacter();//ロビーに配置するキャラクターをセット
        SetEquipItems();//装備情報をセット
    }

    void FixedUpdate()
    {
        if (!isGameRunning) return;//ゲームがスタートされていなかったらリターン
        if (isRunning)
        {
            // タイマーが動作中の場合、残り時間を更新して表示する
            currentTime -= Time.deltaTime;
            timerText.text = currentTime.ToString("F0");

            // タイマーが0以下になったら停止する
            if (currentTime <= 0f)
            {
                isRunning = false;
            }
        }
    }

    public void ResetLobbyCharacter()
    {
        foreach(Transform lobbyChara in lobbyCharacterParent.transform)
        {
            if(lobbyChara.tag != "LobbyCharaRange" && lobbyChara.tag != "MonsterPoop")
            Destroy(lobbyChara.gameObject);//メモリリーク大丈夫だろうか...?
        }
    }

    public void SetLobbyCharacter()
    {
        //後で経験値処理をするため一旦今までロビーにいたキャラクターを格納します。
        List<int> unique_ids = new List<int>(lobbyCharaTrainingIds);

        lobbyCharaTrainingIds.Clear();
        List<GameObject> lobbyCharaObj = new List<GameObject>();

        for (int i = 0; i < playerParty.Count; i++)
        {
            for (int j = 0; j < playerParty[i].unique_id.Count; j++)
            {
                //まだ配置していないキャラクターだったら
                if (!lobbyCharaTrainingIds.Exists(x => x == playerParty[i].unique_id[j]))
                {
                    //getCharaParamsの中からunique_idが一致しているキャラクターにどの時の時間を設定
                    GetCharaParam matchedCharaParam = getCharaParams.FirstOrDefault(param => param.unique_id == playerParty[i].unique_id[j]);
                    if (matchedCharaParam != null)
                    {
                        HashSet<int> uniqueIdsSet = new HashSet<int>(unique_ids);
                        //もしunique_idsにidが含まれていたらそれまでセットした時間分の経験値を獲得させる
                        if (uniqueIdsSet.Contains(playerParty[i].unique_id[j]))
                        {
                            DateTime setPartyTime = DateTime.Parse(matchedCharaParam.setPartyTime);
                            TimeSpan timeSinceSetParty = DateTime.Now - setPartyTime;

                            //60秒で一つ糞を生成して、糞の配列が20個以下であれば糞を生成する
                            int poopQuantity = Mathf.FloorToInt((int)timeSinceSetParty.TotalSeconds / 60);

                            if (poopQuantity > 3) poopQuantity = 3;
                            if(monsterPoops.Count < 20)
                            {
                                for (int poopIndex = 0; poopIndex < poopQuantity; poopIndex++)
                                {
                                    if (monsterPoops.Count < 20)
                                    {
                                        GameObject monsterPoop = Instantiate(this.monsterPoop, lobbyCharacterParent.transform);

                                        // rangeAとrangeBのx座標の範囲内でランダムな数値を作成
                                        float x = UnityEngine.Random.Range(rangeA.position.x, rangeB.position.x);
                                        // rangeAとrangeBのy座標の範囲内でランダムな数値を作成
                                        float y = UnityEngine.Random.Range(rangeA.position.y, rangeB.position.y);

                                        monsterPoop.transform.position = new Vector2(x, y);
                                        monsterPoops.Add(monsterPoop);
                                    }
                                }
                            }

                            float elapsedSeconds = (float)timeSinceSetParty.TotalSeconds;
                            int gainedExperience = Mathf.FloorToInt(elapsedSeconds * experienceGrowthRate * GetFeedEfficacy(matchedCharaParam.feedNum));//餌の効果をここで加える
                            if (elapsedSeconds * experienceGrowthRate < 0) gainedExperience = 2147483647;
                            
                            GainExperience(matchedCharaParam,gainedExperience);
                        }
                        //新しい時間にセットしなおす
                        matchedCharaParam.setPartyTime = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss tt");
                    }

                    lobbyCharaTrainingIds.Add(playerParty[i].unique_id[j]);
                }
            }
        }

        //unique_idsにあるキャラクターをランダムな位置に生成する
        for (int i = 0; i < lobbyCharaTrainingIds.Count; i++)
        {
            GameObject lobbyChara = Instantiate(lobbyCharaEntity, lobbyCharacterParent.transform);
            lobbyCharaObj.Add(lobbyChara);

            // rangeAとrangeBのx座標の範囲内でランダムな数値を作成
            float x = UnityEngine.Random.Range(rangeA.position.x, rangeB.position.x);
            // rangeAとrangeBのy座標の範囲内でランダムな数値を作成
            float y = UnityEngine.Random.Range(rangeA.position.y, rangeB.position.y);
            float z = y * 1;

            lobbyCharaObj[i].transform.position = new Vector3(x, y,z);

            LobbyCharacter lobbyCharacter = lobbyCharaObj[i].GetComponent<LobbyCharacter>();
            lobbyCharacter.unique_id = lobbyCharaTrainingIds[i];
            lobbyCharacter.SetLobbyCharacterParam();
        }
    }

    //持っている武器一覧をセットする
    public void SetEquipItems()
    {
        //テストで配置してあるアイテムは一旦削除
        foreach (Transform itemObj in allParent.transform)
        {
            Destroy(itemObj.gameObject);//メモリリーク大丈夫だろうか...?
        }

        //下の処理で非アクティブにする際に使用
        List<int> setCharaEquip = new List<int>();
        for(int i = 0;i < getCharaParams.Count; i++)
        {
            for(int j = 0; j< getCharaParams[i].equipItem.Count; j++)
            {
                setCharaEquip.Add(getCharaParams[i].equipItem[j]);
            }
        }

        for (int i = 0; i < getEquipParams.Count; i++)
        {
            if (i == 50) break;
            //各タブの階層に生成するとメモリがやばそうなので、All階層だけに生成
            GameObject itemObj = Instantiate(equipItemSlotObj, allParent.transform);

            //生成したゲームオブジェクトに画像を設定
            EquipItems equipItems = itemObj.GetComponent<EquipItems>();
            equipItems.itemImage.sprite = getEquipParams[i].sprite;
            equipItems.equipParam = getEquipParams[i];

            //すでにモンスターが装備しているものは非アクティブにする
            HashSet<int> setCharaEquipuniqueIdsSet = new HashSet<int>(setCharaEquip);
            if (setCharaEquipuniqueIdsSet.Contains(equipItems.equipParam.unique_id)) equipNowUniqueIdList.Add(equipItems.equipParam.unique_id);

            //アイテム側の選択された時の画像
            //プール処理で行うのでここでは以下の処理は行わない
            //setEquipment.equipItems.Add(itemObj.GetComponent<EquipItems>());

            //オブジェクトとスクリプトの両方を同じ順番でListに格納する
            //オブジェクトを使い回すためこの方法は難しいかも
            //generateItems.Add(itemObj);
            

            //何番目に生成されたオブジェクトか保存しておく
            //equipItems.genarateNum = generateNum;
            //getEquipParams[i].genarateNum = generateNum;
            //generateNum++;
        }
    }


    //ロビーシーンのデータをセット・更新に使用
    public void LobbyParamSet()
    {
        playerExpSlider.maxValue = nextPlayerExp;
        playerExpSlider.value = currentPlayerExp;
        expSliderText.text = currentPlayerExp + " / " + instance.nextPlayerExp;

        playerNameText.text = playerName;
        playerRankText.text = playerRank.ToString();
        staminaText.text = currentStamina.ToString() + " / " + maxStamina.ToString();
        goldText.text = gold.ToString("#,0");
        jemText.text = jem.ToString();
    }

    //餌の番号によって経験値のたまるスピードをかえる
    public int GetFeedEfficacy(int num)
    {
        switch (num)
        {
            case 0:
                return 1;
            default:
                return 1;
        }
    }

    public void GainExperience(GetCharaParam getCharaParam,int amount)
    {
        if (getCharaParam.maxLevel >= getCharaParam.currentLevel)
        {
            getCharaParam.exp += amount;
            if (getCharaParam.exp < 0) getCharaParam.exp = 2147483647;
            // 経験値の上限チェックなどの処理を追加する場合はここで行う
            if (getCharaParam.exp >= getCharaParam.nextExp)
            {
                LevelUp(getCharaParam);
            }
            // 経験値の変化に応じて見た目を更新する（仮の例）
            //float scale = 1.0f + (experience / 100.0f);
            //transform.localScale = new Vector3(scale, scale, scale);
        }
    }


    //レベルアップの処理
    public void LevelUp(GetCharaParam getCharaParam)
    {
        if (getCharaParam.maxLevel <= getCharaParam.currentLevel) return;
        getCharaParam.currentLevel++;
        getCharaParam.exp -= getCharaParam.nextExp;
        getCharaParam.nextExp = CalculateNextLevelExperience(getCharaParam);
        getCharaParam.HP += CalculateHPIncrease(getCharaParam);
        getCharaParam.Attack += CalculateAttackIncrease(getCharaParam);
        getCharaParam.Defence += CalculateDefenceIncrease(getCharaParam);

        //経験値が次のレベルに達していたら再度LevelUpの処理をする
        if (getCharaParam.exp >= getCharaParam.nextExp)
        {
            LevelUp(getCharaParam);
        } 
    }

    //コインでレベルアップの処理
    public void LevelUpWithCoin(GetCharaParam getCharaParam)
    {
        getCharaParam.currentLevel++;
        //getCharaParam.exp = 0;//これまで貯めた経験値は再利用できるようリセットしない
        getCharaParam.nextExp = CalculateNextLevelExperience(getCharaParam);
        getCharaParam.HP += CalculateHPIncrease(getCharaParam);
        getCharaParam.Attack += CalculateAttackIncrease(getCharaParam);
        getCharaParam.Defence += CalculateDefenceIncrease(getCharaParam);
    }

    private int CalculateNextLevelExperience(GetCharaParam getCharaParam)//引数はとりあえず全部渡す
    {
        // レベルごとの経験値テーブルから必要な経験値を計算する処理
        // レベルアップに必要な経験値の値を返す
        return Mathf.RoundToInt(getCharaParam.nextExp * 1.1f);
    }

    private int CalculateHPIncrease(GetCharaParam getCharaParam)
    {
        // レベルアップ時のHP増加量を計算する処理
        // HP増加量を返す
        int underLine = getCharaParam.resetNum;
        if (underLine > Mathf.RoundToInt(getCharaParam.HP / getCharaParam.currentLevel)) underLine = Mathf.RoundToInt(getCharaParam.HP / getCharaParam.currentLevel);
        int increase = UnityEngine.Random.Range(underLine, getCharaParam.HP / getCharaParam.currentLevel/2);
        return increase;
        //return getCharaParam.HP / getCharaParam.currentLevel;
    }

    private int CalculateAttackIncrease(GetCharaParam getCharaParam)
    {
        // レベルアップ時の攻撃力増加量を計算する処理
        // 攻撃力増加量を返す
        int underLine = getCharaParam.resetNum;
        if (underLine > Mathf.RoundToInt(getCharaParam.Attack / getCharaParam.currentLevel)) underLine = Mathf.RoundToInt(getCharaParam.Attack / getCharaParam.currentLevel);
        int increase = UnityEngine.Random.Range(underLine, getCharaParam.Attack / getCharaParam.currentLevel/2);
        return increase;
        //return getCharaParam.Attack / getCharaParam.currentLevel;
    }

    private int CalculateDefenceIncrease(GetCharaParam getCharaParam)
    {
        // レベルアップ時の防御力増加量を計算する処理
        // 防御力増加量を返す
        int underLine = getCharaParam.resetNum;
        if (underLine > Mathf.RoundToInt(getCharaParam.Defence / getCharaParam.currentLevel)) underLine = Mathf.RoundToInt(getCharaParam.Defence / getCharaParam.currentLevel);
        int increase = UnityEngine.Random.Range(underLine, getCharaParam.Defence / getCharaParam.currentLevel/2);
        //Debug.Log(increase);
        return increase;
        //return getCharaParam.Defence / getCharaParam.currentLevel;
    }

    public void GamePlay()
    {
        StartCoroutine(StartInterval());
    }

    //ゲームに必要なデータをセットする（ボタンから実行しています。）
    public void GameSetting()
    {
        //生成されているオブジェクトを全て削除
        DeleteCharacter();

        //敵の情報と味方の情報を設定
        SetCharacter();
        SetEnemyCharacter();

        //大将攻撃ボタンを非アクティブにする
        enemyGeneralButton.interactable = false;
        //以下の関数は仮で実行しています。
        //本来はステージマネージャーからゲームシーンに配置したスクリプトなどで階層や何か表示させた後に実行
        GamePlay();
    }

    //ゲームスタート時の処理
    //todoあとあと書き足していく予定
    public IEnumerator StartInterval()
    {
        //enemyHP = 10000;
        enemyCurrentHP = enemyHP;
        enemyHPText.text = enemyHP + " / " + enemyHP;//最初の設定なので最大HPを現在HPに入れています。
        enemySlider.maxValue = enemyHP;
        enemySlider.value = enemyHP;
        yield return new WaitForSeconds(3);
        //笛の音を鳴らす
        buttonPush = true;
        isGameRunning = true;
        isRunning = true;
        
        InvokeRepeating("EnemyAI",0,3);
    }


    //敵の行動
    public void EnemyAI()
    {
        //大将攻撃ボタンを非アクティブにする
        enemyGeneralButton.interactable = false;

        //行動する敵キャラを決める
        //enemyAICharactersにキャラがいなかったら再度入れ直す
        if (enemyAICharacters.Count == 0)
        {
            enemyAICharacters.Clear();//Todo バトル終了後　クリアするのを忘れないように
            foreach (var enemyCharas in enemyCharacterPos)
            {
                enemyAICharacters.Add(enemyCharas.Value);
            }
        }

        //enemyListからランダムで一つ選ぶ
        if (enemyCharacterPos.Count == 0) return;
        int randomNum = UnityEngine.Random.Range(0, enemyAICharacters.Count);
        GameObject enemyChara = enemyAICharacters[randomNum];

        //選ばれた要素次回敵の行動時に選ばれないように削除
        enemyAICharacters.Remove(enemyAICharacters[randomNum]);

        //enemyCharacterPosからバリューで検索する
        var result = enemyCharacterPos.FirstOrDefault(x => x.Value.Equals(enemyChara));
        if (result.Equals(default(KeyValuePair<int, GameObject>)))
        {
            // 検索結果が見つからなかった場合の処理
            return;
        }
        else
        {
            // 検索結果が見つかった場合の処理
            activeEnemyGrid = result.Key;//このKeyが敵キャラのマスの番号
        }

        //ここからこのキャラクターを状況に応じて行動を分岐させていく
        //まずはキャラのパラメーターを取得する
        CharaParam enemyCharaParam = enemyChara.GetComponent<CharaParam>();

        //一定確率で攻撃する
        int randomAttack = UnityEngine.Random.Range(0, 11);
        if (randomAttack < 2)
        {
            //優先順位
            //倒せるキャラがいたらそのキャラを攻撃する
            //プレイヤーのポジションのキーNoのグリッドのver_numが0から3に揃っていない場合
            List<int> horiNums = new List<int>();
            foreach(int key in playerCharacterPos.Keys)
            {
                horiNums.Add(playerGrids[key].GetComponent<MoveGrid>().hori_num);
            }
            //シールドの配置も追加する
            foreach (int key in playerShieldPos.Keys)
            {
                horiNums.Add(playerGrids[key].GetComponent<MoveGrid>().hori_num);
            }

            //verNumsのナンバーが全て揃っていたら
            if (horiNums.OrderBy(x => x).SequenceEqual(Enumerable.Range(0, 4)))
            {
                //数字が全て揃っている場合、対象には攻撃できないため、ランダムなプレイヤーキャラにアタックする
                //ランダムなキャラに攻撃する
                RandomEnemyAttack(enemyCharaParam.Attack);
                return;
            }
            else
            {
                //大将が攻撃可能なら大将を攻撃する
                playerHP = playerHP - enemyCharaParam.Attack;
                if (playerHP <= 0) playerHP = 0;
                playerSlider.value = playerHP;
                PlayerHPText.text = playerHP + "/" + playerSlider.maxValue;
            }
        }
        else if(randomAttack > 8)
        {
            RomdomEnemyMove(enemyChara, enemyCharaParam);
            return;
        }

        //HPが1/3だったら移動可能なマスに移動する
        //移動可能なますがない場合は1番HPの低いキャラに攻撃する
        //もしくは大将に攻撃する
        //とりあえず簡単な行動を設定する
        if (enemyCharaParam.HP / 3 >= enemyCharaParam.enemyHpSlider.value)
        {
            RomdomEnemyMove(enemyChara, enemyCharaParam);
        }
        //HPが1/3ではなかったらランダムなプレイヤーに攻撃する
        else
        {
            RandomEnemyAttack(enemyCharaParam.Attack);
        }
    }

    public void RomdomEnemyMove(GameObject enemyChara,CharaParam enemyCharaParam)
    {
        //キー番号から規則に従ったグリッドの選択位置をアクティブにする
        List<int> canMovePositions = new List<int>();

        for (int i = 0; i < GameManager.instance.playerGrids.Count; i++)
        {
            if (i == activeEnemyGrid - 1 || i == activeEnemyGrid + 1 || i == activeEnemyGrid - 3 || i == activeEnemyGrid + 3 || i == activeEnemyGrid - 4 || i == activeEnemyGrid + 4 || i == activeEnemyGrid - 5 || i == activeEnemyGrid + 5)
            {
                canMovePositions.Add(i);
            }

            if ((activeEnemyGrid == 3 || activeEnemyGrid == 7 || activeEnemyGrid == 11 || activeEnemyGrid == 15) && (i == activeEnemyGrid + 1 || i == activeEnemyGrid - 3 || i == activeEnemyGrid + 5))
            {
                canMovePositions.Remove(i);
                continue;
            }

            if ((activeEnemyGrid == 0 || activeEnemyGrid == 4 || activeEnemyGrid == 8 || activeEnemyGrid == 12) && (i == activeEnemyGrid - 1 || i == activeEnemyGrid + 3 || i == activeEnemyGrid - 5))
            {
                canMovePositions.Remove(i);
                continue;
            }

            if (GameManager.instance.enemyCharacterPos.ContainsKey(i) || GameManager.instance.enemyShieldPos.ContainsKey(i))
            {
                canMovePositions.Remove(i);
            }
        }

        //enemyCharaParamに入ったランダムな位置に移動する
        if (canMovePositions.Count > 0)
        {
            int enemyRandomPos = canMovePositions[UnityEngine.Random.Range(0, canMovePositions.Count)];
            enemyChara.transform.position = enemyGrids[enemyRandomPos].transform.position;
            //ディクショナリーの位置も変更する
            enemyCharacterPos.Remove(activeEnemyGrid);
            enemyCharacterPos.Add(enemyRandomPos, enemyChara);
            return;
        }
        //移動できなかったらランダムなプレイヤーに攻撃する
        else
        {
            RandomEnemyAttack(enemyCharaParam.Attack);
        }
    }

    //敵が攻撃してくる時の処理
    public void RandomEnemyAttack(int enemyAttackPower)
    {
        //ランダムなキャラを指定
        List<int> playerCharaKeys = new List<int>();

        foreach (int posKey in playerCharacterPos.Keys)
        {
            playerCharaKeys.Add(posKey);
        }
        int randomNum = UnityEngine.Random.Range(0, playerCharacterPos.Count);
        GameObject playerChara = playerCharacterPos[playerCharaKeys[randomNum]];

        //slotのsliderValueとTextを変更する
        CharaParam charaParam = playerChara.GetComponent<CharaParam>();
        Slot playerCharaSlot = charaParam.slotObj.GetComponent<Slot>();
        int damage = enemyAttackPower - playerChara.GetComponent<CharaParam>().Defence/2;
        if (damage <= 0) damage = UnityEngine.Random.Range(0,2);
        int remainingHP = (int)playerCharaSlot.hpSlider.value - damage;

        //プレイヤーのキャラのHPが0になった時の処理
        if(remainingHP <= 0)
        {
            remainingHP = 0;
            playerCharacterPos[playerCharaKeys[randomNum]].SetActive(false);
            playerCharacterPos.Remove(playerCharaKeys[randomNum]);
            charaParam.slotObj.SetActive(false);
            //マス上のアクティブを全て消す
            playerCharaSlot.GridReset();
        }

        playerCharaSlot.hpSlider.DOValue(remainingHP, 1f);
        playerCharaSlot.hpText.text = remainingHP + "/" + playerCharaSlot.hpSlider.maxValue;
        Debug.Log("敵キャラの攻撃!");
    }

    //敵の大将にアタック
    public void EnemyGeneralAttack()
    {
        Slot slot = Skills.instance.slot.GetComponent<Slot>();
        if (slot.skillActive)
        {
            slot.timeGage = 0;
            slot.fillEffect.SetActive(false);
            slot.skillActive = false;
            enemyGeneralButton.interactable = false;

            CharaParam charaParam = foucusChara.GetComponent<CharaParam>();
            enemyHP = (int)enemySlider.value - charaParam.Attack;
            enemySlider.DOValue(enemyHP, 1f);
            if (enemyHP <= 0)
            {
                enemyHP = 0;
                StartCoroutine(StageClear());
            }
            enemyHPText.text = enemyHP + "/" + enemySlider.maxValue;
        }
    }


    //パーティーの内容を設定・更新
    public void SetParty()
    {
        partyCharaCards.Clear();//一旦生成されているものをクリア
        //パーティー情報からカードエンティティを使用してオブジェクトを生成
        for (int i = 0; i< playerParty.Count; i++)
        {
            int emptyCount = 0;
            int setCount = 0;
            for(int j = 0; j < playerParty[i].unique_id.Count; j++)
            {
                GameObject charaCard = Instantiate(partyCardEntity, partyParent.transform.GetChild(i));
                partyCharaCards.Add(charaCard);
                //持っているモンスターのリストからユニークIDでfindしてそのオブジェクトの値をセット
                GetCharaParam getCharaParam = getCharaParams.Find(x => x.unique_id == playerParty[i].unique_id[j]);
                charaCard.GetComponent<SetCharacterCard>()
                    .SetThisParamator(j, getCharaParam.unique_id, getCharaParam.character_id, getCharaParam.rarity, getCharaParam.maxLevel, getCharaParam.currentLevel, getCharaParam.charaName, getCharaParam.sprite, getCharaParam.exp, getCharaParam.nextExp);
                //イベントトリガーをつけるとスクロールできなくなるためInstantiateしたオブジェクトにscriptをつけています。
                //セットしたキャラ数を保管
                setCount = j;
                //空きキャラ数を保管
                if (j == 0)emptyCount = 4 - playerParty[i].unique_id.Count;
            }

            //空きキャラ分だけからのカードを生成する
            for(int j = 0; j < emptyCount; j++)
            {
                setCount ++;
                GameObject EmptyCard = Instantiate(emptyCardEntity, partyParent.transform.GetChild(i));
                EmptyCard.GetComponent<EmptyCardData>().cardNumber = setCount;
                //空のカードをタップした時にカード一覧画面を表示させる
                //イベントトリガーをつけるとスクロールできなくなるためInstantiateしたオブジェクトにscriptをつけています。
            }
        }
    }


    //パーティー情報をリセット
    public void ResetParty()
    {
        for (int i = 0; i < playerParty.Count; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                Destroy(partyParent.transform.GetChild(i).transform.GetChild(j).gameObject);
            }
        }
    }

    //パーティーの内容を設定・変更する
    //UIプレハブに設定されたユニークIDをパーティーリストに追加
    public void SetCharaInParty()
    {
        int uniqueId = UIManager.instance.cardInfoUI.GetComponent<SetCharacterInfo>().unique_id;

        //設定しているパーティー内にユニークIDが存在しているかどうか判定
        if(!playerParty[selectPartyNumber].unique_id.Exists(x => x.Equals(uniqueId)))
        {
            if (selectCardNumber > playerParty[selectPartyNumber].unique_id.Count - 1)
            {
                playerParty[selectPartyNumber].unique_id.Add(uniqueId);
            }
            else
            {
                playerParty[selectPartyNumber].unique_id[selectCardNumber] = UIManager.instance.cardInfoUI.GetComponent<SetCharacterInfo>().unique_id;
            }
            ResetParty();
            SetParty();
            ResetLobbyCharacter();//ロビーに配置しているキャラを全削除
            SetLobbyCharacter();//ロビーにキャラを再配置
            UIManager.instance.cardInfoUI.SetActive(false);
            UIManager.instance.cardListUI.SetActive(false);
        }
    }

    //セットされているキャラを解除する。Buttonから実行しています。
    public void UnSetCharaFromParty()
    {
        //セレクトしたチームのキャラが1人だった時は外せないようにする
        if (playerParty[selectPartyNumber].unique_id.Count > 1)
        {
            playerParty[selectPartyNumber].unique_id.Remove(playerParty[selectPartyNumber].unique_id[selectCardNumber]);
            ResetParty();
            SetParty();
            ResetLobbyCharacter();//ロビーに配置しているキャラを全削除
            SetLobbyCharacter();//ロビーにキャラを再配置
            UIManager.instance.cardListUI.SetActive(false);
        }
    }

    //キャラリストからボタンで戻った時に数値を更新する処理
    public void UpdateParty()
    {
        foreach(GameObject gameObject in partyCharaCards)
        {
            gameObject.GetComponent<SetCharacterCard>().ParamUpdate();
        }
    }

    public void ResetCharaList()
    {
        foreach (Transform charaList in charaListParet.transform)
        {
            if (charaList.tag != "UnsetButton")
                Destroy(charaList.gameObject);//メモリリーク大丈夫だろうか...?
        }
    }


    public void UpdateCharaList()
    {
        for(int i = 0; i< charaListCards.Count; i++)
        {
            charaListCards[i].GetComponent<SetListCharacterCard>().ParamUpdate();
        }
    }

    //キャラクターリスト画面に所持カードを設定する
    //キャラ入手ごとに更新する（予定）
    public void SetCharaList()
    {
        charaListCards.Clear();//再度追加するので一旦クリアします
        for (int i = 0; i < getCharaParams.Count; i++)
        {
            if (i == 50) break;
            GetCharaParam cp = getCharaParams[i];//cpはgetCharaParamの略。引数が多いので短縮しました。
            GameObject charaListCard = Instantiate(charaListCardEntity, charaListParet.transform);
            charaListCards.Add(charaListCard);
            charaListCard.GetComponent<SetListCharacterCard>().SetListCardParamator(cp.unique_id);
        }
    }

    //ゲームプレイ前に実行される
    void DeleteCharacter()
    {
        //生成されているスロットを全削除
        foreach (Transform n in slotParent.transform)
        {
            Destroy(n.gameObject);
        }

        //生成されているプレイヤーのバトルキャラを全削除
        foreach (Transform n in characterParent.transform)
        {
            Destroy(n.gameObject);
        }

        //生成されているプレイヤーのバトルキャラを全削除
        foreach (Transform n in enemyParent.transform)
        {
            Destroy(n.gameObject);
        }
    }


    //現在buttonから実行
    //ゲームクリアの際にも実行
    public void BattleEnd()
    {
        isGameRunning = false;
        isRunning = false;
        StopAllCoroutines();
        CancelInvoke();
        selectCloneCharacters.Clear();
        selectCloneEnemyCharacters.Clear();
        enemyAICharacters.Clear();
        //プレイヤーの情報をリセット
        playerHP = 0;
        //敵の情報をリセット
        enemyHP = 0;
        //コンボ情報をリセット
        conboCount = 0;
        conboText.text = conboCount.ToString()+"combo";
        //時間の情報をリセット
        currentTime = 60f;
        timerText.text = currentTime.ToString("F0");
    }


    //バトル開始前に実行
    void SetCharacter()
    {
        //格納されているコマンドウィンドウを一旦削除
        //ここに書かないでバトル終了時に消すほうがメモリ的にいいかも
        commandWindows.Clear();
        //キャラクターのポジションの情報もクリアする
        playerCharacterPos.Clear();

        //ゲーム画面のスクロールできるポジションをリセット
        gameSceneScrollView.transform.position = new Vector3(0.00f, 1.00f, 90.00f);

        // ユーザーが選択したキャラをセット
        for (int i = 0; i < playerParty[selectPartyNumber].unique_id.Count; i++)
        {
            GameObject slotClone = Instantiate(slotPrefab, slotParent.transform);
            GameObject cloneCharacter = Instantiate(MonsterEntity, characterParent.transform);
            //cpはgetCharaParamの略。引数が多いので短縮しました。
            GetCharaParam cp = getCharaParams.Find(x => x.unique_id == playerParty[selectPartyNumber].unique_id[i]);

            //キャラクターがどのマスにいるか
            playerCharacterPos.Add(i+4,cloneCharacter);

            //スロットのコマンドウィンドウをリストに格納する
            commandWindows.Add(slotClone.GetComponent<Slot>().commandWindow);

            CharaParam cloneCharaParam = cloneCharacter.GetComponent<CharaParam>();
            cloneCharaParam.SetGameCharaParam(cp.unique_id, cp.character_id,cp.rarity,cp.maxLevel,cp.currentLevel,cp.charaName,cp.sprite,cp.animatorControllerNum, cp.skillGameNum,cp.skillNum,cp.skillCost,cp.skillTime,cp.description, cp.HP,cp.Attack,cp.Defence);

            //装備分パラメータを増加させる
            int increaseHP = 0;
            int increaseAttack = 0;
            int increaseDefence = 0;
            int increaseSkillTime = 0;

            for (int j = 0; j < cp.equipItem.Count; j++)
            {
                //時間あったらハッシュにした方がいいかも
                GameManager.GetEquipParam equipParam = GameManager.instance.getEquipParams.Find(x => x.unique_id == cp.equipItem[j]);
                //武器の上昇分の計算とテキスト設定
                increaseHP += Mathf.FloorToInt(equipParam.increaseHP * 0.01f * cp.HP);
                increaseAttack += Mathf.FloorToInt(equipParam.increaseAttack * 0.01f * cp.Attack);
                increaseDefence += Mathf.FloorToInt(equipParam.increaseDefence * 0.01f * cp.Defence);
                increaseSkillTime += equipParam.increaseSkillTime;
            }

            cloneCharaParam.IncreaseParam(increaseHP, increaseAttack, increaseDefence, increaseSkillTime);

            selectCloneCharacters.Add(cloneCharacter);

            //キャラの重なり順を変更する
            cloneCharacter.GetComponent<CharaParam>().canvas.sortingOrder = i + 2;//フォーカスの画像が1なので+2します。

            //最初のキャラクターがフォーカスキャラになるようにしている
            if (i == 0)
            {
                foucusChara = cloneCharacter;//最初のキャラクターを選択状態にする
                foucusChara.transform.Find("Image_Target").gameObject.SetActive(true);
                SetSlot(slotClone, cloneCharacter); //スロットをキャラクター数生成して、生成したスロットにキャラクターデータを入れる
            }
            else
            {
                SetSlot(slotClone, cloneCharacter, false);
            }

            //プレイヤーのHPを足してキャラクターの位置を設定します。
            playerHP += cloneCharacter.GetComponent<CharaParam>().HP;

            //cloneCharacter.transform.position = playerCharaPositions[i].transform.position;

            //ポジションを変更する
            cloneCharacter.transform.position = playerGrids[i+4].transform.position;
        }

        foucusChara = selectCloneCharacters[0];

        //プレイヤーのHPを設定
        playerCurrentHP = playerHP;
        PlayerHPText.text = playerCurrentHP + " / " + playerHP;
        playerSlider.maxValue = playerHP;
        playerSlider.value = playerHP;
    }


    //バトル開始前に実行
    void SetSlot(GameObject slot, GameObject clone, bool skillsSet = true)
    {
        if (skillsSet) Skills.instance.slot = slot;
        CharaParam charaParam = clone.GetComponent<CharaParam>();
        charaParam.charaObj = clone;
        slot.GetComponent<Slot>().ParamSet(charaParam); 
        clone.GetComponent<CharaParam>().slotObj = slot;

        //後でキャラの属性ごとに色を変えたい
        slot.GetComponent<Slot>().slotBackGround.GetComponent<Image>().color = Color.blue;
    }

    //バトル開始前に実行
    void SetEnemyCharacter()
    {
        //オブジェクトとポジション情報を一旦クリア
        enemyCharacterPos.Clear();

        //ステージに選択されたキャラクターをセット
        StageManager.StateInfo stateInfo = StageManager.instance.StageObjectData[StageManager.instance.selectStageNumber].stateInfo[StageManager.instance.selectStateNumber];
        for (int i = 0; i < stateInfo.hierarchy.Length; i++)
        {
            for(int j = 0;j < stateInfo.hierarchy[i].enemyCharacters.Length; j++)
            {
                GameObject cloneEnemyCharacter = Instantiate(MonsterEntity, enemyParent.transform);
                CharaParam enemyCharaParam = cloneEnemyCharacter.GetComponent<CharaParam>();
                enemyCharaParam.SetEnemyCharaParam(stateInfo.hierarchy[i].enemyCharacters[j].enemyCharacter);
                enemyHP += enemyCharaParam.HP;
                //HPゲージをアクティブにする
                cloneEnemyCharacter.transform.GetChild(2).gameObject.SetActive(true);
                //画像を反転
                cloneEnemyCharacter.transform.GetChild(1).localScale = new Vector2(-1, 1);
                //敵キャラクターがどのマスにいるか
                enemyCharacterPos.Add(j + 4, cloneEnemyCharacter);
                //敵キャラの位置をグリッド上に生成する
                cloneEnemyCharacter.transform.position = enemyGrids[j + 4].transform.position;
                //キャラの重なり順を変更する
                enemyCharaParam.canvas.sortingOrder = i + 2;//フォーカスの画像が1なので+2します。
            }
        }

        //敵のHPを設定
        enemyCurrentHP = enemyHP;
        enemyHPText.text = enemyHP + " / " + enemyHP;//最初の設定なので最大HPを現在HPに入れています。
        enemySlider.maxValue = enemyHP;
        enemySlider.value = enemyHP;
    }


    public void FoucusChara(GameObject chara)
    {
        foucusChara = chara;
        foucusChara.transform.Find("Image_Target").gameObject.SetActive(false);
        //foucusCharaParam = chara.GetComponent<CharaParam>();
        //tensionText.text = foucusCharaParam.currentTension.ToString() + "%";
        chara.transform.Find("Image_Target").gameObject.SetActive(true);
    }

    //通常の攻撃処理
    public void HpGageProcess()
    {
        CharaParam foucusCharaParam = foucusChara.GetComponent<CharaParam>();//GetComponentを複数回しないように一旦変数に格納

        enemySlider.DOValue(enemyCurrentHP - foucusCharaParam.Attack, 1f);
        enemyCurrentHP = enemyCurrentHP - foucusCharaParam.Attack;
        enemyHPText.text = enemyCurrentHP + " / " + instance.enemyHP;
        StartCoroutine(AtackAnimation(foucusChara));//アタックアニメーション（挙動がおかしい）
        ShakeGameobject(enemyHpGage);//HPゲージのシェイクアニメーション
        Skills.instance.SkillExe(foucusCharaParam.skillNum);
        //もし敵が戦闘不能（HPが0）だったら
        if (enemyCurrentHP <= 0)
        {
            enemyCurrentHP = 0;
            enemySlider.value = 0;
            enemyHPText.text = instance.enemyCurrentHP + " / " + instance.enemyHP;
            //TODO 本当は階層分の敵を表示させる,次の階層へ行くかクリアかの判定をする
            StartCoroutine(StageClear());
        }
    }

    IEnumerator StageClear()
    {
        stageInfo[StageManager.instance.selectStageNumber].state[StageManager.instance.selectStateNumber].complete = true;
        if(stageInfo[StageManager.instance.selectStageNumber].state.Count != StageManager.instance.selectStateNumber + 1)
        {
            stageInfo[StageManager.instance.selectStageNumber].state[StageManager.instance.selectStateNumber + 1].active = true;
        }
        else
        {
            //選択されたステージをクリアしたら次のステージを解放する処理
            if(StageManager.instance.selectStageNumber < stageInfo.Count)
            {
                if(StageManager.instance.selectStateNumber + 1 < stageInfo.Count)
                {
                    stageInfo[StageManager.instance.selectStageNumber + 1].state[0].active = true;
                    stageInfo[StageManager.instance.selectStageNumber + 1].active = true;
                }
            }
        }

        yield return new WaitForSeconds(1);

        BattleEnd();
        LoadingManager.instance.LoadNextScene("LobbyScene");
    }


    //何かアクションがあった時のアニメーション。溜まってきたら別スクリプトに移動します。
    public void HitBlink(GameObject obj)
    {
        _seq?.Kill();
        _seq = DOTween.Sequence();
        _seq.AppendCallback(() => obj.GetComponent<Image>().enabled = false);
        _seq.AppendInterval(0.07f);
        _seq.AppendCallback(() => obj.GetComponent<Image>().enabled = true);
        _seq.AppendInterval(0.07f);
        _seq.SetLoops(2);
        _seq.Play();
    }

    IEnumerator AtackAnimation(GameObject chara)
    {
        Vector3 foucusCharaPos = chara.transform.position;
        // 1:移動
        chara.transform.DOMoveX(chara.transform.position.x + 2 ,0.1f);
        yield return new WaitForSeconds(0.1f);
        chara.transform.DOMoveX(foucusCharaPos.x, 0.1f);
        yield return new WaitForSeconds(0.1f);
        chara.transform.DOMoveX(chara.transform.position.x + 2, 0.1f);
        yield return new WaitForSeconds(0.1f);
        chara.transform.DOMoveX(foucusCharaPos.x, 0.2f);
        yield return new WaitForSeconds(0.2f);
        chara.transform.position = foucusCharaPos;
    }

    public void ShakeGameobject(GameObject shakeObj)
    {
        float duration = 0.5f; // 振動の持続時間
        float strength = 15f; // 振動の強さ

        Vector3 originalPosition = shakeObj.transform.position;

        // 振動アニメーション
        shakeObj.transform.DOShakePosition(duration, strength).OnComplete(() =>
        {
            // 振動アニメーションが終了したら元の位置に戻す
            shakeObj.transform.position = originalPosition;
        });

    }

    //パーティーセレクト画面のトグルで使用しています
    public void SelectPartyNumber(int partyNum)
    {
        selectPartyNumber = partyNum;
        selectCardNumber = 0;//切り替えられたら念の為numberを0にします。
    }

    //直接カード一覧を表示する場合
    public void DirectCardsView()
    {
        UIManager.instance.unSetButton.SetActive(false);
        UIManager.instance.cardListUI.SetActive(true);
        UIManager.instance.setButton.SetActive(false);
    }

    //閉じる時にはアンセットボタンを表示させる
    public void ClosecardListUI()
    {
        UIManager.instance.cardListUI.SetActive(false);
        UIManager.instance.unSetButton.SetActive(true);
        UIManager.instance.setButton.SetActive(true);
    }



    //private float timeWhenLostFocus;

    //アプリがバックグラウンドに以降した時の処理
    private void OnApplicationPause(bool pauseStatus)
    {
        Debug.Log("バックグラウンドに以降");
        if (pauseStatus)
        {
            SaveData saveData = new SaveData();
            saveData.playerName = playerName;
            saveData.playerRank = playerRank;//プレイヤーのランク
            saveData.currentPlayerExp = currentPlayerExp;
            saveData.nextPlayerExp = nextPlayerExp;
            saveData.jem = jem;//ジェム
            saveData.maxStamina = maxStamina;//MAXスタミナ
            saveData.currentStamina = currentStamina;//MAXスタミナ
            saveData.gold = gold;//お金
            saveData.getCharacterIndex = getCharacterIndex;//今までゲットしたキャラの数11からスタート
            saveData.getEquipIndex = getEquipIndex;//今までゲットした武器のIndex、11からスタート
            saveData.getCharaParams = getCharaParams;
            saveData.selectPartyNumber = selectPartyNumber;//選択中のパーティー番号
            saveData.getEquipParams = getEquipParams;//持っている装備情報
            saveData.playerParty = playerParty;//プレイヤーのパーティー情報
            saveData.stageInfo = stageInfo;//ステージのクリア状況

            //ミニゲームのクリア状況
            //一度キーバリューで分解
            miniGameInfoValue.Clear();
            foreach (var item in miniGamePlayInfo)
            {
                if (!miniGameInfoKey.Contains(item.Key)) miniGameInfoKey.Add(item.Key);
                miniGameInfoValue.Add(item.Value);
            }
            //分解したものを保存
            saveData.miniGameInfoKey = miniGameInfoKey;
            saveData.miniGameInfoValue = miniGameInfoValue;

            //セーブ
            saveManager.save = saveData;
            saveManager.Save();
            //saveManager.Load();

            //セーブしたら情報を書き換える
            //LobbyParamSet();//ロビーのプレイヤー名・ジェム・ゴールド・経験値・スタミナなどをセット
            //SetParty();//パーティー情報をセット
            //SetCharaList();//キャラクター情報をセット
            //SetLobbyCharacter();//ロビーに配置するキャラクターをセット
        }
        //else
        //{
            //アプリがフォーカスを失ってからの経過時間を計算
            //一定時間経っていたらタイトルに戻るなどの処理を入れる
            //float timeInBackground = Time.realtimeSinceStartup - timeWhenLostFocus;
            //Debug.Log("Background time: " + timeInBackground.ToString("F2") + " seconds");
        //}
    }

    private void OnApplicationQuit()
    {
        //TODO テストセーブ機能・あとはどこで実行するか
        SaveData saveData = new SaveData();
        saveData.lobbyCharaTrainingIds = lobbyCharaTrainingIds;
        saveData.playerName = playerName;
        saveData.playerRank = playerRank;//プレイヤーのランク
        saveData.currentPlayerExp = currentPlayerExp;
        saveData.nextPlayerExp = nextPlayerExp;
        saveData.jem = jem;//ジェム
        saveData.maxStamina = maxStamina;//MAXスタミナ
        saveData.currentStamina = currentStamina;//MAXスタミナ
        saveData.gold = gold;//お金
        saveData.getCharacterIndex = getCharacterIndex;//今までゲットしたキャラの数11からスタート
        saveData.getEquipIndex = getEquipIndex;//今までゲットした武器のIndex、11からスタート
        saveData.getCharaParams = getCharaParams;
        saveData.selectPartyNumber = selectPartyNumber;//選択中のパーティー番号
        saveData.getEquipParams = getEquipParams;//持っている装備情報
        saveData.playerParty = playerParty;//プレイヤーのパーティー情報
        saveData.stageInfo = stageInfo;//ステージのクリア状況

        //ミニゲームのクリア状況
        //一度キーバリューで分解
        miniGameInfoValue.Clear();
        foreach (var item in miniGamePlayInfo)
        {
            if (!miniGameInfoKey.Contains(item.Key)) miniGameInfoKey.Add(item.Key);
            miniGameInfoValue.Add(item.Value);
        }
        //分解したものを保存
        saveData.miniGameInfoKey = miniGameInfoKey;
        saveData.miniGameInfoValue = miniGameInfoValue;

        //セーブ
        saveManager.save = saveData;
        saveManager.Save();
        Debug.Log("ゲーム終了");
    }
}
