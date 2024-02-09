using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayfabManager : MonoBehaviour
{
    private const string TitleId = "Brow_Hit_Away";

    void Start()
    {
        Login();
    }

    void Login()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            PlayFabSettings.staticSettings.TitleId = TitleId;
        }

        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };

        //アカウント作成処理
        PlayFabClientAPI.LoginWithCustomID(request,
            (result) =>
            {
                GetRanking();//ランキングを取得
                GetLeaderboardAroundPlayer();//プレイヤーのランキング
                // 既に作成済みだった場合
                if (!result.NewlyCreated)
                {
                    //Debug.LogWarning("already account");
                    return;
                }

                // アカウント作成完了
                Debug.Log("Create Account Success!!");
            },
            (error) =>
            {
                Debug.LogError("Create Account Failed...");
                Debug.LogError(error.GenerateErrorReport());
            });

        //PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    private void OnLoginSuccess(LoginResult result)
    {

        Debug.Log("Login Success: " + result.PlayFabId);
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError("Login Failed.");
        Debug.LogError(error.GenerateErrorReport());
    }

    public void UpdatePlayerName()
    {
        // プレイヤー名の設定
        PlayFabClientAPI.UpdateUserTitleDisplayName(
            new UpdateUserTitleDisplayNameRequest { DisplayName = GameManager.instance.playerName },
            (result) =>
            {
                Debug.Log("Save Display Name Success!!");
                //プレイヤーのランキングプレートを削除して再生成
                GameManager.instance.RemoveAllChildrenItemCard(GameManager.instance.rankingParent);
                GetRanking();//1~10位のランキング取得
                GetLeaderboardAroundPlayer();//プレイヤーのランキング取得
            },
            (error) =>
            {
                Debug.LogError("Save Display Name Failed...");
                Debug.LogError(error.GenerateErrorReport());
            });
    }

    public void SubmitScore()
    {
        //スコア送信実行
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
                    {
                        new StatisticUpdate
                        {
                            StatisticName = "weekly-score",
                            Value = GameManager.instance.score // スコア
                        }
                    }
        },
            (result) =>
            {
                //スコア送信完了
                Debug.Log("Send Ranking Score Success!!");
                GameManager.instance.Restart();
            },
            (error) =>
            {
                Debug.LogError("Send Ranking Score Failed...");
                Debug.LogError(error.GenerateErrorReport());
            });
    }

    public void GetRanking()
    {
        //ランキング情報の取得
        PlayFabClientAPI.GetLeaderboard(new GetLeaderboardRequest
        {
            StatisticName = "weekly-score",
            StartPosition = 0,    //取得する開始位置
            MaxResultsCount = 10 //最大取得数
        },
            (result) =>
            {
                //取得したランキング情報
                if (result?.Leaderboard != null)
                {
                    for (var i = 0; i < result.Leaderboard.Count; i++)
                    {
                        var entry = result.Leaderboard[i];
                        GameObject rankkingPlateObj = Instantiate(GameManager.instance.rankingPlate, GameManager.instance.rankingParent);
                        RankingPlate rankingPlate = rankkingPlateObj.GetComponent<RankingPlate>();

                        int rank = entry.Position + 1;
                        rankingPlate.rank.text = rank.ToString();
                        rankingPlate.playerName.text = entry.DisplayName;
                        rankingPlate.score.text = entry.StatValue.ToString("N0");
                    }
                }
                Debug.Log("Get Leader Board Success!!");
            },
            (error) =>
            {
                Debug.LogError("Get Leader Board Failed...");
                Debug.LogError(error.GenerateErrorReport());
            });
    }


    //自分の周辺のランキングを取得する
    public void GetLeaderboardAroundPlayer()
    {
        //GetLeaderboardAroundPlayerRequestのインスタンスを生成
        var request = new GetLeaderboardAroundPlayerRequest
        {
            StatisticName = "weekly-score", //ランキング名(統計情報名)
            MaxResultsCount = 1                  //自分を含め前後何件取得するか
        };

        //自分の順位周辺のランキング(リーダーボード)を取得
        Debug.Log($"自分の順位周辺のランキング(リーダーボード)の取得開始");
        PlayFabClientAPI.GetLeaderboardAroundPlayer(request, OnGetLeaderboardAroundPlayerSuccess, OnGetLeaderboardAroundPlayerFailure);
    }

    //自分の順位周辺のランキング(リーダーボード)の取得成功
    private void OnGetLeaderboardAroundPlayerSuccess(GetLeaderboardAroundPlayerResult result)
    {
        Debug.Log($"自分の順位周辺のランキング(リーダーボード)の取得に成功しました");

        foreach (var entry in result.Leaderboard)
        {
            int rank = entry.Position + 1;
            GameManager.instance.playerRankText.text = rank.ToString();
            GameManager.instance.playerNameText.text = entry.DisplayName;
            GameManager.instance.playerScoreText.text = entry.StatValue.ToString("N0");
        }
    }

    //自分の順位周辺のランキング(リーダーボード)の取得失敗
    private void OnGetLeaderboardAroundPlayerFailure(PlayFabError error)
    {
        GameManager.instance.playerRankText.text = "---";
        GameManager.instance.playerNameText.text = GameManager.instance.playerName;
        GameManager.instance.playerScoreText.text = "---";
        Debug.LogError($"自分の順位周辺のランキング(リーダーボード)の取得に失敗しました\n{error.GenerateErrorReport()}");
    }
}
