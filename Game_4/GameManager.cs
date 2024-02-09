using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System;
using UnityEngine.Tilemaps;
//using Firebase.Analytics;
//using Firebase;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //playfabのマネージャー
    public PlayfabManager playfabManager;
    public SoundManager soundManager;

    //プライバシーポリシーの画面
    public GameObject privacyPolicyWindow;
    private bool privacy = false;

    //プレイヤーネーム
    [HideInInspector]public string playerName = "";
    public GameObject playerNameWindow;
    public TMP_InputField nameInputValue;
    public Button nameChangeButton;

    //プレイヤーのランキング
    public TextMeshProUGUI playerRankText;
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI playerScoreText;

    //ランキングに使用する情報
    public Transform rankingParent;
    public GameObject rankingWindow;
    public GameObject rankingPlate;

    //テキスト系
    public GameObject scoreObj;
    public TextMeshProUGUI scoreText;
    public GameObject untilTextObj;
    public TextMeshProUGUI untilText;

    //スコア
    [HideInInspector]public int score = 0;

    //ポップアップ系
    public GameObject restartPopup;

    //ガチャ結果に使用するimageとキャンバス
    public Image gachaResultImage;
    public GameObject gachaResultWindow;

    //Button系
    public GameObject startButton;
    public GameObject restartButton;
    public GameObject speedUpButton;

    private bool isMoving = true; // カメラが動いているかどうかのフラグ
    private int cameraPositionNumber = 0;//現在のカメラ位置インデックス
    public List<Vector3> cameraStartPositions;//カメラのスタート位置を保存する配列
    private float cameraMoveValue = -1.35f;//カメラの動く量

    [HideInInspector] public float currentCameraSpeed = 0.1f;//現在のカメラスピード
    [HideInInspector] public float cameraSpeed = 0.1f;//カメラスピード
    [HideInInspector] public float speedUp = 2;//カメラスピード

    public Transform targetObjParent;//ここに撃ち落とす対象のオブジェクトを生成して必要がなくなったら消す
    public int targetItemId;//撃ち落とす対象のオブジェクト

    public GameObject titleObjects;//タイトルに表示されていオブジェクト
    public Transform objectsParent;//オブジェクトの親要素
    public List<GameObject> objects;//並ぶゲームオブジェクト等
    public List<Vector3> objectPositions = new List<Vector3>();//ゲームオブジェクトが配置されるポジション

    private bool isTap = false; //タップできるかどうか
    public bool isSpeedUp = false; //スピードアップボタンが押されているかどうか

    public GameObject equipimageButton;
    public GameObject equipWindow;
    public Image equipImage;
    public int equipItemNum = 0;
    public List<EquipItem> getEquipItems = new List<EquipItem>();//今までゲットしたことのあるitem
    public List<GameObject> equipItems = new List<GameObject>();

    public Transform itemCardParent;
    public GameObject itemCardPrefab;

    public GameObject powerObj;//タップした時に発射されるオブジェクト
    public Transform effectObjParent;//後で消すために同じ要素内に入れる
    public GameObject hitEffect;//ぶつかった時に生成されるエフェクト

    public float powerObjSpeed = 1f;

    public Image fadeImage;
    public CanvasGroup fadeImageCanvas;//カメラ移動の際などに使用

    //床に衝突したオブジェクト等
    public List<GameObject> dropItems;

    //ゲームの判定起動フラグ
    [HideInInspector] private int fase = 0;//何周目か
    [HideInInspector] private bool resultFlag = false;
    private Vector3 resultCameraPosition = new Vector3(1.47f,1.08f,0.19f);

    //AdMobのスクリプト
    public AdMobScript adMobScript;

    //ゲームカウント
    public int gameCount = 2;

    //ネットワークに接続しているかどうか
    public bool networkConection;

    [Serializable]
    public class EquipItem
    {
        public int itemId;
        public int brokenCount;
    }

    [Serializable]
    public class SaveEquipItem
    {
        public List<EquipItem> equipItems = new List<EquipItem>();
    }

    [HideInInspector]public SaveEquipItem saveEquipItem;


    private void Awake()
    {
        Load();
        //装備アイテムを変える
        powerObj = equipItems[equipItemNum];
        //装備中の画像も変える
        equipImage.sprite = powerObj.GetComponent<PowerObj>().equipImage;

        //アイテムカードを生成する
        ItemCardGenerate();

        //もしセーブデータがなかった場合はプレイヤー名を入力するwindowを表示させる
        if(playerName == "")
        {
            playerNameWindow.SetActive(true);
        }
        else
        {
            playerNameText.text = playerName;
            nameInputValue.text = playerName;
        }
    }

    private void Start()
    {
        instance = this;
        fadeImage.raycastTarget = false;
        GameButtonSwicher(false);
        scoreObj.SetActive(false);

        //ネットワーク状態を確認
        NetWorkConection();
        if (networkConection && privacy) adMobScript.ShowBanner();

        soundManager.PlayBGM(0);//BGMスタート
    }

    //ネットワークに接続されているか
    public void NetWorkConection()
    {
        // ネットワークの状態を出力
        switch (Application.internetReachability)
        {
            case NetworkReachability.NotReachable:
                networkConection = false;
                break;
            case NetworkReachability.ReachableViaCarrierDataNetwork:
                Debug.Log("キャリアデータネットワーク経由で到達可能");
                networkConection = true;
                break;
            case NetworkReachability.ReachableViaLocalAreaNetwork:
                Debug.Log("Wifiまたはケーブル経由で到達可能");
                networkConection = true;
                break;
        }

        if (networkConection)
        {
            adMobScript.LoadAd();
        }
    }

    public void SortItemList()
    {
        getEquipItems = getEquipItems.OrderBy(equipItem => equipItem.itemId).ToList();
    }

    public void ItemCardGenerate()
    {
        foreach (EquipItem equipItem in getEquipItems)
        { 
            GameObject itemCard = Instantiate(itemCardPrefab, itemCardParent);
            ItemCard itemCardScript = itemCard.GetComponent<ItemCard>();
            itemCardScript.itemImage.sprite = equipItems[equipItem.itemId].GetComponent<PowerObj>().equipImage;
            itemCardScript.itemId = equipItem.itemId;
            itemCardScript.brokenCount.text = equipItem.brokenCount.ToString();
            if (itemCardScript.itemId == 0) itemCardScript.brokenCount.text = "∞";
        }
    }

    public void RemoveAllChildrenItemCard(Transform parent)
    {
        //objectsParentがnullでないことを確認
        if (parent != null)
        {
            //親の子要素を全て削除
            foreach (Transform child in parent)
            {
                Destroy(child.gameObject);
            }
        }
        else
        {
            Debug.LogWarning("itemCardParent is not assigned.");
        }
    }

    //プレイヤー名の変更
    public void ChangePlayerName()
    {
        playerNameWindow.SetActive(false);
        if (playerName == nameInputValue.text) return;//同じ名前なら処理しない
        playerName = nameInputValue.text;
        playerNameText.text = playerName;
        Save();
        playfabManager.UpdatePlayerName();
    }

    //プレイヤー名の名前を変えた時のバリデーション
    public void PlayerNameValidation()
    {
        bool textLength = (nameInputValue.text.Length > 2) && (nameInputValue.text.Length <= 10);//テキストの長さ（3文字以上10文字以下）
        bool empty = string.IsNullOrEmpty(nameInputValue.text);
        if (textLength && !empty)
        {
            nameChangeButton.interactable = true;
        }
        else
        {
            nameChangeButton.interactable = false;
        }
    }

    public void StartButton()
    {
        if (gameCount < 1)
        {
            if (networkConection)
            {
                adMobScript.ShowInterstitialAd(() =>
                {
                    gameCount = 1;//1回ごとに広告
                    PlayerPrefs.SetInt("GAME_COUNT", gameCount);
                    PlayerPrefs.Save();

                    //フェードのキャンバスでブロックされますが、スタートボタンを押せないようにする処理
                    startButton.GetComponent<Button>().interactable = false;
                    startButton.SetActive(false);

                    soundManager.PlaySE(3);//ゲームスタート音

                    //1秒後にカメラの移動を開始
                    StartCoroutine(GameStart());
                });
            }
            else
            {
                return;
            }
        }
        else
        {
            gameCount--;
            PlayerPrefs.SetInt("GAME_COUNT", gameCount);
            PlayerPrefs.Save();

            //フェードのキャンバスでブロックされますが、スタートボタンを押せないようにする処理
            startButton.GetComponent<Button>().interactable = false;
            startButton.SetActive(false);

            soundManager.PlaySE(3);//ゲームスタート音

            //1秒後にカメラの移動を開始
            StartCoroutine(GameStart());
        }
    }


    //オブジェクトを生成する処理
    public void ObjectsGenerate()
    {
        List<int> nums = new List<int>();
        List<int> posNums = new List<int>();
        //オブジェクトの数分の数字を配列に入れていく
        for (int i = 0; i < objects.Count; i++)
        {
            nums.Add(i);
            posNums.Add(i);
        }

        //numsリストから数字を一つピックアップしてリストから削除
        while(nums.Count > 0)
        {
            // ランダムなインデックスを選択
            int randomIndex = UnityEngine.Random.Range(0, nums.Count);
            int posRandomindex = UnityEngine.Random.Range(0, posNums.Count);

            // 選択された数字を取得
            int pickedNumber = nums[randomIndex];
            int pickedPosNumber = posNums[posRandomindex];

            // リストから選択された数字を削除
            nums.Remove(pickedNumber);
            posNums.Remove(pickedPosNumber);

            GameObject obj = Instantiate(objects[pickedNumber], objectsParent);
            obj.transform.position = objectPositions[pickedPosNumber];
        }
    }


    //オブジェクトを全て削除する処理
    void RemoveAllChildrenObjects()
    {
        // objectsParentがnullでないことを確認
        if (objectsParent != null)
        {
            // 親の子要素を全て削除
            foreach (Transform child in objectsParent)
            {
                Destroy(child.gameObject);
            }
        }
        else
        {
            Debug.LogWarning("objectsParent is not assigned.");
        }
    }

    void GameButtonSwicher(bool b)
    {
        restartButton.SetActive(b);
        speedUpButton.SetActive(b);
    }


    void Update()
    {
        // タップが発生した場合
        if (isTap && !isSpeedUp && !restartPopup.activeInHierarchy && Input.GetMouseButtonDown(0))
        {
            //タップした位置からRayを発射して、3Dオブジェクトを検出
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                //Debug.Log(hit.transform.gameObject.name);
                ItemObject itemObj = hit.transform.gameObject.GetComponent<ItemObject>();
                if (itemObj?.hp > 0)
                {
                    itemObj.hp--;
                }
                ShootToPosition(hit.point);
                soundManager.PlaySE(8);//投げる音
            }

        }

        //カメラを動かす場合は以下のスクリプトを実行
        if (!isMoving) MoveCamera();
    }

    IEnumerator GameStart()
    {
        //装備したアイテムを減らす処理を行う
        //もしカウントが0な場合はリストから削除してセーブする
        EquipItem equipItem = getEquipItems.Find(equipItem => equipItem.itemId == equipItemNum);
        if (equipItem.itemId != 0)
        {
            equipItem.brokenCount--;
            if (equipItem.brokenCount == 0) getEquipItems.RemoveAll(equipItem => equipItem.itemId == equipItemNum);
            Save();
        }

        yield return new WaitForSeconds(1);

        fase = 0;//フェーズを0にする

        isMoving = true;//動カメラがかないようにする
        targetItemId = RandomNumber();//飛ばす対象のidを保存

        Fade(() =>
        {
            if (targetObjParent != null && targetObjParent.childCount > 0) Destroy(targetObjParent.GetChild(0).gameObject);

            //リザルトの関数を有効にする
            resultFlag = false;

            //リスタート時もこの関数を併用するため成功時のテキストを非表示にする
            untilTextObj.SetActive(false);

            GameButtonSwicher(false);

            titleObjects.SetActive(false);//タイトルのオブジェクトを非表示にする
            equipimageButton.SetActive(false);//装備中のイメージを非表示にする
            rankingWindow.SetActive(false);
            RemoveAllChildrenObjects();//生成されたオブジェクトを破棄する処理
            
            ObjectsGenerate();//オブジェクトを生成する処理

            //背景が白いところに移動させる
            Camera.main.transform.position = new Vector3(-1.47f, 1.29f, 2.25f);//座標は適当です
            ResetCameraRotation();

            //オブジェクトをカメラ前に生成してプレイヤーに伝える
            //prefabの順序が変わるとよくないのでFindしたid-1のオブジェクトを生成します
            // itemIdToFindで指定したIDを持つItemObjectを探す
            ItemObject foundItem = objects
                .Select(obj => obj.GetComponent<ItemObject>())
                .FirstOrDefault(item => item != null && item.itemId == targetItemId);

            GameObject itemObj;
            if (foundItem != null)
            {
                itemObj = Instantiate(objects[targetItemId - 1], targetObjParent);//pickUpObjectsにはidが返ってきているので-1します。
            }
            else
            {
                //基本ここに来ることはないですが、念の為の処理
                targetItemId = 1;
                itemObj = Instantiate(objects[0], targetObjParent);
            }

            //カメラの座標から少し引いた位置に移動させます
            Camera mainCamera = Camera.main;
            Vector3 spawnPosition = mainCamera.transform.position + mainCamera.transform.forward * 1.5f;
            spawnPosition.y -= 0.1f;
            itemObj.transform.position = spawnPosition;//new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y - 0.11f, Camera.main.transform.position.z - 1);
            itemObj.GetComponent<Rigidbody>().useGravity = false;//重力で落ちないように
        },true);//サウンドをならしたいので第二引数はtrue

        yield return new WaitForSeconds(6);

        Fade(() =>
        {
            isTap = true;
            //撃ち落とす対象のオブジェクトを削除する
            Destroy(targetObjParent.GetChild(0).gameObject);

            //カメラの位置をセットする
            SetCameraPosition(0);

            //ゲーム中に表示させるボタンやスコアをアクティブにする
            scoreText.text = "Score:"+score;
            scoreObj.SetActive(true);
            GameButtonSwicher(true);
        });

        yield return new WaitForSeconds(2.5f);
        isMoving = false;
    }

    public void Fade(Action action,bool sound = false)
    {
        // 初期化
        fadeImageCanvas.alpha = 0;
        fadeImage.raycastTarget = true;

        //フェードのサウンドも鳴らす
        if(sound) soundManager.PlaySE(6);

        // 1秒かけてalphaを1にする
        fadeImageCanvas.DOFade(1, 1.0f)
            .OnComplete(() =>
            {
                action();
                
                // 0.5秒待機後にalphaを0にする
                DOVirtual.DelayedCall(0.5f, () =>
                {
                    fadeImage.raycastTarget = false;
                    fadeImageCanvas.DOFade(0, 1.0f);
                });
            });
    }

    void MoveCamera()
    {
        float step = cameraSpeed * Time.deltaTime;
        Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, new Vector3(cameraMoveValue, Camera.main.transform.position.y, Camera.main.transform.position.z), step);

        //カメラが指定された距離だけ移動したら次の位置に移動
        if (Vector3.Distance(Camera.main.transform.position, new Vector3(cameraMoveValue, Camera.main.transform.position.y, Camera.main.transform.position.z)) < 0.01f)
        {
            isMoving = true;
            Invoke("NextCameraPosition", 1.0f);
        }
    }

    void NextCameraPosition()
    {
        //次の位置にカメラを移動
        cameraPositionNumber++;
        if (cameraPositionNumber < cameraStartPositions.Count)
        {
            SetCameraPosition(cameraPositionNumber);
        }
        else
        {
            cameraPositionNumber = 0;
            SetCameraPosition(cameraPositionNumber);//上からスタート
            fase++;
            //cameraPositionNumberが3の場合は停止
            //Debug.Log("カメラの動きを停止します。");
        }
        isMoving = false;
    }

    //カメラの回転情報をリセット
    void ResetCameraRotation()
    {
        Camera.main.transform.rotation = Quaternion.Euler(0, 180, 0);
    }

    //カメラを格段のスタート位置にセットする
    void SetCameraPosition(int index)
    {
        // 指定された位置にカメラを移動
        Camera.main.transform.position = cameraStartPositions[index];
    }

    //見えないオブジェクトを発射して指で突いたような表現にする
    void ShootToPosition(Vector3 targetPosition)
    {
        // ゲームオブジェクトを生成
        GameObject powerObj = Instantiate(this.powerObj, transform.position, Quaternion.identity);
        powerObj.transform.position = Camera.main.transform.position;

        // ターゲットの方向を計算
        Vector3 shootDirection = (targetPosition - powerObj.transform.position).normalized;

        // ゲームオブジェクトに力を加えて発射
        powerObj.GetComponent<Rigidbody>().AddForce(shootDirection * powerObjSpeed, ForceMode.VelocityChange);
    }

    //突き落とすオブジェクトのIDを返す
    public int RandomNumber()
    {
        //配置されるオブジェクトの中からランダムなIDを返す
        //objectsに格納されてる数までの数字でランダムな数字を一つピックアップ
        int randomNumber = UnityEngine.Random.Range(0, objects.Count);
        return objects[randomNumber].GetComponent<ItemObject>().itemId;
    }

    //ゲームリスタート デバッグ用
    public void Restart()
    {
        resultFlag = true;//リザルト判定をオンにしてリスタート後にリザルト画面にならないようにフラグを切り替える
        //2秒かけてalphaを1にする
        //同時にBGMもフェードアウトさせる
        fadeImageCanvas.DOFade(1, 2.0f);
        soundManager.BGM[0].DOFade(0.0f, 2)
                .OnComplete(() =>
                {
                    //フェードアウト完了後の処理
                    soundManager.BGM[0].Stop();
                    //現在のアクティブなシーンを再読み込み
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    //Save();
                });
    }

    public void ResultCheck()
    {
        if (resultFlag) return;
        StopAllCoroutines();
        CancelInvoke();

        //ポップアップを非表示に
        restartPopup.SetActive(false);

        //何度も処理されないようにフラグを切り替える
        resultFlag = true;

        //オブジェクトをタップできないようにする
        isTap = false;

        //カメラの動きをストップさせる
        isMoving = true;

        //カメラを遠ざけながらフェードさせる
        //StartCoroutine(FinishMoveCamera());

        Fade(() =>
        {
            //カメラの位置を切り替えてコルーチンを終了させる
            StopAllCoroutines();
            CancelInvoke();

            //背景が白いところに移動させる
            Camera.main.transform.position = new Vector3(-1.47f, 1.29f, 2.25f);//座標は適当です
            ResetCameraRotation();

            //最初に落ちたオブジェクトだけ表示
            GameObject itemObj = Instantiate(objects[dropItems[0].GetComponent<ItemObject>().itemId -1], targetObjParent);
            itemObj.GetComponent<Rigidbody>().useGravity = false;

            //カメラの座標から少し引いた位置に移動させます
            Camera mainCamera = Camera.main;
            Vector3 spawnPosition = mainCamera.transform.position + mainCamera.transform.forward * 1.5f;
            spawnPosition.y -= 0.1f;
            itemObj.transform.position = spawnPosition;

            untilTextObj.SetActive(true);
            TextSpaceAnimation("Finish", untilText);

            StartCoroutine(DropItemCamera());
        });

    }

    void TextSpaceAnimation(string text, TextMeshProUGUI textMeshPro)
    {
        //TextMeshProの初期設定
        textMeshPro.text = text;
        textMeshPro.characterSpacing = 25;
        textMeshPro.color = new Color(textMeshPro.color.r, textMeshPro.color.g, textMeshPro.color.b, 0); // 透明度を0に設定

        //DoTweenでアニメーションを作成
        Sequence sequence = DOTween.Sequence();
        sequence.Append(textMeshPro.DOColor(new Color(textMeshPro.color.r, textMeshPro.color.g, textMeshPro.color.b, 1), 1.0f)); // 透明度を1に変化
        sequence.Join(textMeshPro.DOText(text, 1.0f, scrambleMode: ScrambleMode.None)); // テキストを変化

        //スペースの変化
        DOTween.To(() => textMeshPro.characterSpacing, value => textMeshPro.characterSpacing = value, 10, 1)
            .SetEase(Ease.OutQuart).OnComplete(()=>{
            sequence.Kill();
        });
    }

    //落ちたアイテムを確認するカメラワーク
    IEnumerator DropItemCamera()
    {
        restartButton.SetActive(false);
        speedUpButton.SetActive(false);

        yield return new WaitForSeconds(2);
        untilTextObj.SetActive(false);

        yield return new WaitForSeconds(3);

        //クリアしたかどうかを判定する
        DropItemCheck();
    }

    void DropItemCheck()
    {
        //落ちたアイテムとターゲットのアイテムが一緒であればスピードをアップして再配置
        bool isTargetItemExists = dropItems.Any(item => item.GetComponent<ItemObject>().itemId == targetItemId);
        if (isTargetItemExists)
        {
            untilTextObj.SetActive(true);
            //クリアの表示を出す
            TextSpaceAnimation("Success", untilText);
            soundManager.PlaySE(4);//成功サウンド

            //カメラスピードを上げる
            currentCameraSpeed += 0.1f;
            speedUp += 0.1f;
            cameraSpeed = currentCameraSpeed;

            //オブジェクトを一旦全て消して再生成
            dropItems.Clear();

            //セットカメラポジションを0にする
            cameraPositionNumber = 0;

            //スコアにステージクリアの得点を追加する
            score += 100 * ((int)(cameraSpeed * 100));
            if(fase == 0) score += 1000;
            scoreText.text = "Score:" + score;

            StartCoroutine(GameStart());
        }
        else
        {
            untilTextObj.SetActive(true);
            //失敗の表示を出す
            TextSpaceAnimation("Failure", untilText);
            soundManager.PlaySE(5);//失敗サウンド

            //タイトル画面へ戻る処理
            //初期化
            fadeImageCanvas.alpha = 0;
            fadeImage.raycastTarget = true;

            // 2秒かけてalphaを1にする
            //同時にBGMもフェードアウトさせる
            fadeImageCanvas.DOFade(1, 2.0f);
            soundManager.BGM[0].DOFade(0.0f, 2)
                    .OnComplete(() =>
                    {
                        // フェードアウト完了後の処理
                        soundManager.BGM[0].Stop();
                        playfabManager.SubmitScore();
                    });

        }
    }

    public void Save()
    {
        //getEquipItemsをJSONに変換して保存
        saveEquipItem = new SaveEquipItem
        {
            equipItems = getEquipItems
        };
        string equipItemsJson = JsonUtility.ToJson(saveEquipItem);
        Debug.Log(equipItemsJson);
        PlayerPrefs.SetString("EQUIP_GET_ITEMS", equipItemsJson);

        // equipitemNumを保存
        PlayerPrefs.SetInt("EQUIP_ITEM", equipItemNum);

        // equipitemNumを保存
        PlayerPrefs.SetString("PLAYER_NAME", playerName);

        // PlayerPrefsの変更を保存
        PlayerPrefs.Save();
    }

    void Load()
    {
        //プラバシーポリシーの同意が得られていない場合
        if (!PlayerPrefs.HasKey("PRIVACY"))
        {
            privacyPolicyWindow.SetActive(true);
            privacy = false;
        }
        else
        {
            privacy = true;
        }

        // EQUIP_GET_ITEMSの保存データを取得し、getEquipItemsに格納
        if (PlayerPrefs.HasKey("EQUIP_GET_ITEMS"))
        {
            string equipItemsJson = PlayerPrefs.GetString("EQUIP_GET_ITEMS");
            saveEquipItem = JsonUtility.FromJson<SaveEquipItem>(equipItemsJson);
            getEquipItems = saveEquipItem.equipItems;
        }
        else
        {
            getEquipItems.Add(new EquipItem
            {
                itemId= 0,
                brokenCount = 0
            });
        }

        // EQUIP_ITEMの保存データを取得し、equipitemNumに格納
        if (PlayerPrefs.HasKey("EQUIP_ITEM"))
        {
            equipItemNum = PlayerPrefs.GetInt("EQUIP_ITEM");
            //ここでリストに装備中のIDが存在しない場合は0にセットし直す
            bool isItemIdExists = getEquipItems.Any(equipItem => equipItem.itemId == equipItemNum);
            if (!isItemIdExists) equipItemNum = 0;
        }
        else
        {
            equipItemNum = 0;
        }

        //広告表示までのゲームカウント数をgameCountに格納
        if (PlayerPrefs.HasKey("GAME_COUNT"))
        {
            gameCount = PlayerPrefs.GetInt("GAME_COUNT");
        }

        //プレイヤーネーム
        if (PlayerPrefs.HasKey("PLAYER_NAME"))
        {
            playerName = PlayerPrefs.GetString("PLAYER_NAME");
        }
        else
        {
            playerName = "";
        }
    }

    // スクリプトが破棄されたときにデータを保存
    void OnDestroy()
    {
        Save();
    }

    //リワード広告を表示させるためのスクリプト
    void ShowReward()
    {
        adMobScript.ShowAd(() =>
        {
            //AddCoin(1000000);

            //PlayerPrefs.SetInt(KEY_SAVE_COIN, myCoin);
            //PlayerPrefs.Save();

            //Firebase.Analytics.FirebaseAnalytics.LogEvent("GameEndAds");

            //広告をロード
            adMobScript.LoadAd();
        });
    }

    //FireBaseにログを送ることができるようになる処理
    void FireBaseInit()
    {
        //FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        //{
        //    FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
        //});
    }

    //FireBaseにログを送る処理
    void FireBaseLog(string logName)
    {
        //Firebase.Analytics.FirebaseAnalytics.LogEvent(logName);

    }
}
