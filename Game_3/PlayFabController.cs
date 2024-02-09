using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

using PlayFab;
using PlayFab.ClientModels;
using PlayFab.MultiplayerModels;

using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
//using Debug = System.Diagnostics.Debug;
using Debug = UnityEngine.Debug;
using System.Threading;

public class PlayFabController : MonoBehaviour
{
    public SocketIOUnity socket;
    public string room_id;

    public CancelMatchmakingTicketRequest cancelRequest;
    public PostCommandClass postCommand = new PostCommandClass();//このインスタンスに色々入れてポストしたりレシーブしたりする

    public bool cancelButton = false;
    public bool isMatching = false;//マッチングしているかどうか
    bool beetleInfo = false;

    // Start is called before the first frame update
    void Start()
    {
        Login();
        //SocketSetting();
    }

    private void Update()
    {
        if (beetleInfo)
        {
            beetleInfo = false;
            StartCoroutine(GameManager.instance.LoadBattleScene());
        }
    }

    void Login()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(request,OnSuccess,OnError);
    }

    void OnSuccess(LoginResult result)
    {
        Debug.Log("ログイン成功");
    }

    void OnError(PlayFabError error)
    {
        Debug.Log("ログイン失敗");
    }

    /*
     * ＜マッチメイキングの処理＞
     * Create ticketをplayfabにリクエスト => チケットの作成が成功した場合、サービスは、TicketId を返します。
     * リクエストに成功したらGetMatchMakingTicketをplayfabリクエスト
     * TickketStatusに応じてGetMatchの処理に移る（6秒ごとに更新する）
     * GetMatchの処理を実行する
     */

    public void Matchmaking()
    {
        //キャンセルボタンを押してしばらくすると、NPCモードでバトルシーンに遷移してしまうため下記のフラグを使用しています。
        cancelButton = false;
        //キャンセルボタンを有効にします。
        GameManager.instance.cancelMatchingButton.interactable = true;

        GameManager.instance.matchingWindow_title.text = "マッチング中...";

        //UIManager.instance.matchingText.text += "マッチメイキングチケットをキューに積みます...\n";
        
        // プレイヤーの情報を作ります。
        var matchmakingPlayer = new MatchmakingPlayer
        {
            // Entityは下記のコードで決め打ちで大丈夫です。
            Entity = new PlayFab.MultiplayerModels.EntityKey
            {
                Id = PlayFabSettings.staticPlayer.EntityId,
                Type = PlayFabSettings.staticPlayer.EntityType
            },

            //カブトムシのレートが100以上離れている場合はマッチしない
            //100という数字はPlayfabのキュー上で設定
            Attributes = new MatchmakingPlayerAttributes
            {
                DataObject = new {
                    Rate = GameManager.instance.beetleManager.selectBeetle.rate,
                    //AppVersion = GameManager.appVersion
                }
            }
        };

        CreateMatchmakingTicketRequest request = new CreateMatchmakingTicketRequest
        {
            // 先程作っておいたプレイヤー情報です。
            Creator = matchmakingPlayer,
            // マッチングできるまで待機する秒数を指定します。最大600秒です。
            GiveUpAfterSeconds = 5,//後で30秒に修正
            // GameManagerで作ったキューの名前を指定します。
            QueueName = "1vs1Battle"
        };

        Debug.Log("CreateMatchmakingTicket");
        PlayFabMultiplayerAPI.CreateMatchmakingTicket(request, OnCreateMatchmakingTicketSuccess, OnFailure);

        void OnCreateMatchmakingTicketSuccess(CreateMatchmakingTicketResult result)
        {
            //UIManager.instance.matchingText.text += "マッチメイキングチケットをキューに積みました！\n\n";

            // キューに積んだチケットの状態をマッチングするかタイムアウトするまでポーリングします。
            var getMatchmakingTicketRequest = new GetMatchmakingTicketRequest
            {
                TicketId = result.TicketId,
                QueueName = request.QueueName
            };

            cancelRequest = new CancelMatchmakingTicketRequest
            {
                QueueName = request.QueueName,
                TicketId = result.TicketId
            };

            StartCoroutine(Polling(getMatchmakingTicketRequest));
        }
    }

    public void RamdomMatchmaking()
    {
        //キャンセルボタンを押してしばらくすると、NPCモードでバトルシーンに遷移してしまうため下記のフラグを使用しています。
        cancelButton = false;
        //キャンセルボタンを有効にします。
        GameManager.instance.cancelMatchingButton.interactable = true;

        GameManager.instance.matchingWindow_title.text = "マッチング中...";

        //UIManager.instance.matchingText.text += "マッチメイキングチケットをキューに積みます...\n";

        // プレイヤーの情報を作ります。
        var matchmakingPlayer = new MatchmakingPlayer
        {
            // Entityは下記のコードで決め打ちで大丈夫です。
            Entity = new PlayFab.MultiplayerModels.EntityKey
            {
                Id = PlayFabSettings.staticPlayer.EntityId,
                Type = PlayFabSettings.staticPlayer.EntityType
            },

            //ランダムマッチなのでレートは無視
            Attributes = new MatchmakingPlayerAttributes
            {
                DataObject = new { AppVersion = GameManager.appVersion }
            }
        };

        CreateMatchmakingTicketRequest request = new CreateMatchmakingTicketRequest
        {
            // 先程作っておいたプレイヤー情報です。
            Creator = matchmakingPlayer,
            // マッチングできるまで待機する秒数を指定します。最大600秒です。
            GiveUpAfterSeconds = 10,//後で30秒に修正
            // GameManagerで作ったキューの名前を指定します。
            QueueName = "Ramdom_Match"
        };

        Debug.Log("CreateRamdomMatchmakingTicket");
        PlayFabMultiplayerAPI.CreateMatchmakingTicket(request, OnCreateMatchmakingTicketSuccess, OnFailure);

        void OnCreateMatchmakingTicketSuccess(CreateMatchmakingTicketResult result)
        {
            //UIManager.instance.matchingText.text += "マッチメイキングチケットをキューに積みました！\n\n";

            // キューに積んだチケットの状態をマッチングするかタイムアウトするまでポーリングします。
            var getMatchmakingTicketRequest = new GetMatchmakingTicketRequest
            {
                TicketId = result.TicketId,
                QueueName = request.QueueName
            };

            cancelRequest = new CancelMatchmakingTicketRequest
            {
                QueueName = request.QueueName,
                TicketId = result.TicketId
            };

            StartCoroutine(Polling(getMatchmakingTicketRequest));
        }
    }

    IEnumerator Polling(GetMatchmakingTicketRequest request)
    {
        //ポーリングは1分間に10回まで許可されているので6秒間隔で実行。
        var seconds = 6f;
        var MatchedOrCanceled = false;

        while (true)
        {
            if (MatchedOrCanceled)
            {
                yield break;
            }

            PlayFabMultiplayerAPI.GetMatchmakingTicket(request, OnGetMatchmakingTicketSuccess, OnFailure);
            yield return new WaitForSeconds(seconds);
        }

        void OnGetMatchmakingTicketSuccess(GetMatchmakingTicketResult result)
        {
            switch (result.Status)
            {
                case "Matched":
                    isMatching = true;
                    MatchedOrCanceled = true;
                    //UIManager.instance.matchingText.text += $"対戦相手が見つかりました！\n\nMatchIDは {result.MatchId} です！";

                    room_id = result.MatchId;
                    //マッチングキャンセルボタンを無効にする
                    GameManager.instance.cancelMatchingButton.interactable = false;
                    SocketSetting();

                    //joinボタンをアクティブにしてルームに入る
                    //以下二つの処理は保険処理
                    StopAllCoroutines();
                    //StartCoroutine(JoinRoomCo());

                    return;

                case "Canceled":
                    MatchedOrCanceled = true;
                    if (cancelButton)
                    {
                        //ポーリング処理を終了させる
                        StopAllCoroutines();
                        return;
                    }
                    //マッチングキャンセルボタンを無効にする
                    GameManager.instance.cancelMatchingButton.interactable = false;
                    GameManager.instance.NpcMaching();
                    //GameManager.instance.matchingWindow_title.text = "対戦相手が見つかりませんでした。";
                    //UIManager.instance.matchingText.text += "対戦相手が見つからないのでキャンセルしました...";
                    return;

                default:
                    //UIManager.instance.matchingText.text += "対戦相手が見つかるまで待機します...\n";
                    return;
            }
        }
    }

    public void CancelTicket()
    {
        Debug.Log("CancelTicket");
        StopAllCoroutines();
        PlayFabMultiplayerAPI.CancelMatchmakingTicket(cancelRequest, (result) => { Debug.Log(result); }, OnFailure);
        PlayFabMultiplayerAPI.CancelMatchmakingTicket(
        cancelRequest,
        OnTicketCanceled,
        OnFailure
        );
    }

    void OnTicketCanceled(CancelMatchmakingTicketResult result)
    {
        Debug.Log($"{result}");
    }

    void OnFailure(PlayFabError error)
    {
        Debug.Log($"{error.ErrorMessage}");

    }

    //IEnumerator JoinRoomCo()
    //{
    //    yield return new WaitForSeconds(2);
    //}


    //ソケット通信の処理
    void SocketSetting()
    {
        var uri = new Uri("***");
        var context = SynchronizationContext.Current;
        socket = new SocketIOUnity(uri, new SocketIOOptions
        {
            EIO = 4
            ,
            Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
        });
        socket.JsonSerializer = new NewtonsoftJsonSerializer();

        ///// reserved socketio events
        socket.OnConnected += (sender, e) =>
        {
            Debug.Log("socket.OnConnected");
            // スレッドプール上にて処理を実行する
            ThreadPool.QueueUserWorkItem(_ =>
            {
                Debug.Log("スレッドプールのスレッドID:" + Thread.CurrentThread.ManagedThreadId);

                // なにか処理
                //Thread.Sleep(100);

                // 確保したSynchronizationContextを使ってメインスレッドに処理を戻す
                context.Post(__ =>
                {
                    JoinRoom();
                }, null);
            });
        };
        socket.OnDisconnected += (sender, e) =>
        {
            Debug.Log("disconnect: " + e);
        };

        UnityEngine.Debug.Log("Connecting...");
        socket.Connect();

        socket.OnUnityThread("joinedRoom", (data) => {
            UnityEngine.Debug.Log("ルームID:" + room_id + "に接続成功");
            UnityEngine.Debug.Log("対戦相手を待っています...");
        });

        socket.OnUnityThread("matching", (data) => {
            PostBeetleInfo();
            CancelInvoke();
            UnityEngine.Debug.Log("対戦相手が入場しました");
            UnityEngine.Debug.Log("通信スタート");
        });

        socket.OnUnityThread("receive", (command) => {
            UnityEngine.Debug.Log("相手のコマンド：" + command);
            string receiveCommand = command.ToString();
            //受け取るコマンドに"[]"がついているため切り取ります。
            if (receiveCommand.StartsWith("[") && receiveCommand.EndsWith("]"))
            {
                receiveCommand = receiveCommand.Substring(1, receiveCommand.Length - 2);
            }

            PostCommandClass postCommandClass = JsonUtility.FromJson<PostCommandClass>(receiveCommand);

            //受け取ったメッセージによって処理を分岐させる
            ReceiveAction(postCommandClass);
        });

        socket.OnUnityThread("reave", (data) => {
            //CPUモードに切り替える
            if(GameManager.instance.battleManager != null) GameManager.instance.battleManager.battleMode = BattleManager.BattleMode.cpu;
            //入力待ち状態だった場合かつバトルマネージャーが存在していた場合はコマンドチェックに移動する
            bool checkCommand = (GameManager.instance.battleManager.battleStatus == BattleManager.BattleStatus.wait) && (GameManager.instance.battleManager != null);
            if (checkCommand) GameManager.instance.battleManager.CheckCommands();
            isMatching = false;

            //不具合出ないかチェック
            GameManager.instance.battleManager.statusBounusReceive = true;

            //ターンの開始まちの時に相手の回線が切断された場合、次のターンを開始する
            if (GameManager.instance.battleManager?.battleStatus == BattleManager.BattleStatus.nextTurn)
            {
                GameManager.instance.battleManager.NextTurn();
            }

            UnityEngine.Debug.Log("回線が切断されました");
            //UIManager.instance.matchingText.text += "回線が切断されました";
        });

    }

    public void Emit(string json,string eventName)
    {
        if (!IsJSON(json))
        {
            socket.Emit(eventName, json);
        }
        else
        {
            socket.EmitStringAsJSON(eventName, json);
        }
    }

    public static bool IsJSON(string str)
    {
        if (string.IsNullOrWhiteSpace(str)) { return false; }
        str = str.Trim();
        if ((str.StartsWith("{") && str.EndsWith("}")) || //For object
            (str.StartsWith("[") && str.EndsWith("]"))) //For array
        {
            try
            {
                var obj = JToken.Parse(str);
                return true;
            }
            catch (Exception ex) //some other exception
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    //チケット取得後にキャンセルする場合
    public void CancelRoom()
    {
        //joinRoomButton.interactable = false;
        postCommand.room = room_id;
        string json = JsonUtility.ToJson(postCommand);

        postCommand.message = "roomCancel";
        Emit(json, "post");
    }

    
    public void JoinRoom()
    {
        //joinRoomButton.interactable = false;
        postCommand.room = room_id;
        postCommand.message = "joinRoom";//なくても大丈夫ですが一応入れています。
        postCommand.breederName = GameManager.instance.breederName;
        postCommand.selectBeetle = GameManager.instance.beetleManager.selectBeetle;
        string json = JsonUtility.ToJson(postCommand);

        Debug.Log(json);
        //socket.EmitStringAsJSON("joinRoom", json);
        Emit(json, "joinRoom");
    }

    public void PostCommand(string command)
    {
        postCommand.room = room_id;
        postCommand.message = command;

        string json = JsonUtility.ToJson(postCommand);
        Debug.Log(json);
        //socket.EmitStringAsJSON("post", json);
        Emit(json, "post");
    }

    public void PostBeetleInfo()
    {
        if (beetleInfo)
        {
            CancelInvoke();
            return;
        }
        postCommand.room = room_id;
        postCommand.message = "beetleInfo";
        postCommand.selectBeetle = GameManager.instance.beetleManager.selectBeetle;

        //選択している昆虫のスキルを変数に保存しておく
        GameManager.SkillInfoBattle skillInfoBattle = new GameManager.SkillInfoBattle();
        foreach (string unique_id in GameManager.instance.beetleManager.selectBeetle.selectSkill)
        {
            GameManager.SkillInfo skillInfo = GameManager.instance.mySkills.Find(skillInfo => skillInfo.unique_id == unique_id);
            Debug.Log("skillInfo" + skillInfo);
            skillInfoBattle.selectSkills.Add(skillInfo);
        }
        GameManager.instance.selectBeetleSkills = skillInfoBattle;
        postCommand.skillInfo = skillInfoBattle;

        string json = JsonUtility.ToJson(postCommand);
        //UnityEngine.Debug.Log(json);
        Emit(json, "post");
    }

    public void ReceiveAction(PostCommandClass postCommandClass)
    {
        //受けとったメッセージが昆虫の情報だったらBeetleManagerに保管します。
        if (postCommandClass.message == "beetleInfo")
        {
            BeetleManager.Beetle enemyBeetle = postCommandClass.selectBeetle;
            GameManager.instance.enemyBreederName = postCommandClass.breederName;
            GameManager.instance.beetleManager.enemyBeetle = enemyBeetle;
            GameManager.instance.enemySkills = postCommandClass.skillInfo;
            //UIManager.instance.matchingText.text += "相手の昆虫情報を取得しました。" + "\n";
            //マッチング画面にオブジェクトを生成する
            GameManager.instance.MatchingCanvasUpdate();
            CompleteBeetleInfo();
        }

        //相手が昆虫の情報を受け取れたという情報だったら
        if (postCommandClass.message == "completeBeetleInfo")
        {
            //相手が自分の昆虫の情報を受け取ったかどうか
            beetleInfo = true;
        }

        //ターンが終了したという通知
        if(postCommandClass.message == "turnEnd")
        {
            GameManager.instance.battleManager.enemyTurn = true;
            //すでに自分が待機状態だった場合は次のターンへ行く
            if(GameManager.instance.battleManager.battleStatus == BattleManager.BattleStatus.nextTurn)
            {
                GameManager.instance.battleManager.NextTurn();
            }
        }

        //受け取ったメッセージがキャンセルコマンドだったら再度チケットをとり直します。
        if (postCommandClass.message == "roomCancel")
        {
            Matchmaking();
            //UIManager.instance.matchingText.text += "相手が退出しました。再度チケットを取得します。" + "\n";
            //ここで3Dオブジェクトなどのアクティブを切り替える
        }

        //コマンドが送信されていたらコマンドの処理に以降
        if (postCommandClass.message == "command")
        {
            //相手からコマンド情報が来ていたらバトルマネージャーに入れる
            GameManager.instance.battleManager.enemyCommand = true;
            GameManager.instance.battleManager.pushEnemySkillNum = postCommandClass.skillNum;
            GameManager.instance.battleManager.pushEnemySkillUniqueId = postCommandClass.skillUniqueId;
            GameManager.instance.battleManager.pushEnemyJanken = postCommandClass.jankenCommand;

            //自身が待機状態でコマンドを受け取った際はコマンドの確認を実行
            if(GameManager.instance.battleManager.battleStatus == BattleManager.BattleStatus.wait)
            {
                GameManager.instance.battleManager.CheckCommands();
            }
        }

        if(postCommandClass.message == "statusBounus")
        {
            //Debug.Log("配列数" + postCommandClass.statusBounus.enemyStatusBounus);
            GameManager.instance.battleManager.enemyStatusResults = postCommandClass.statusBounus.enemyStatusBounus;
            GameManager.instance.battleManager.statusBounusReceive = true;
        }
        
    }

    //昆虫の情報を受け取れたというリアクションを送る
    void CompleteBeetleInfo()
    {
        postCommand.room = room_id;
        postCommand.message = "completeBeetleInfo";
        postCommand.selectBeetle = null;
        
        string json = JsonUtility.ToJson(postCommand);
        Emit(json, "post");
    }

    //しばらく経っても昆虫情報がない時は相手に再送信をお願いする
    //void RequestBeetleInfo()
    //{
    //    postCommand.room = room_id;
    //    postCommand.message = "incompleteBeetleInfo";
    //    string json = JsonUtility.ToJson(postCommand);
    //    Emit(json, "post");
    //}

    [System.Serializable]
    public class PostCommandClass
    {
        public string room;
        public string message;
        public string breederName;
        public BattleManager.Janken jankenCommand;
        public int skillNum;
        public string skillUniqueId;
        public BeetleManager.Beetle selectBeetle;
        public GameManager.SkillInfoBattle skillInfo;
        public GameManager.StatusBounus statusBounus;
        public int turn;
    }

}

