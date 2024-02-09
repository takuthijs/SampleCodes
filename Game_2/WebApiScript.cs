using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;

public class WebApiScript : MonoBehaviour
{
    public GameManager gameManager;

    [System.Serializable]
    public class Rankings
    {
        public Ranking[] cityHomerunRanks;
    }

    [System.Serializable]
    public class Ranking
    {
        public string user_name;
        public int score;
    }

    private void Start()
    {
        StartCoroutine(RankingRequest("***"));
        StartCoroutine(MyRankPostRequest("***"));
    }

    public void CoRankingUpdate()
    {
        StartCoroutine(RankingRequest("***"));
        StartCoroutine(MyRankPostRequest("***"));
    }

    //ランキングの表示
    private IEnumerator RankingRequest(string url)
    {
        //ウェブリクエストを生成
        var request = UnityEngine.Networking.UnityWebRequest.Get(url);
        //通信待ち
        yield return request.SendWebRequest();
        switch (request.result)
        {
            case UnityWebRequest.Result.InProgress:
                Debug.Log("リクエスト中");
                break;

            case UnityWebRequest.Result.Success:
                Debug.Log("リクエスト成功");

                var arr = JsonUtility.FromJson<Rankings>(request.downloadHandler.text);
                int index = 0;
                foreach (var user in arr.cityHomerunRanks)
                {
                    gameManager.rankingNames[index].text = user.user_name;
                    gameManager.rankingScores[index].text = user.score.ToString("#,0") + "m"; ;
                    index++;
                }
                for (int i = 0; i < 10 - index;)
                {
                    gameManager.rankingNames[index].text = "None";
                    gameManager.rankingScores[index].text = "0m";
                    index++;
                }
                break;

            case UnityWebRequest.Result.ConnectionError:
                Debug.Log
                (
                    @"サーバとの通信に失敗。
                    リクエストが接続できなかった、
                    セキュリティで保護されたチャネルを確立できなかったなど。"
                );
                break;

            case UnityWebRequest.Result.ProtocolError:
                Debug.Log
                (
                    @"サーバがエラー応答を返した。
                    サーバとの通信には成功したが、
                    接続プロトコルで定義されているエラーを受け取った。"
                );
                break;

            case UnityWebRequest.Result.DataProcessingError:
                Debug.Log
                (
                    @"データの処理中にエラーが発生。
                    リクエストはサーバとの通信に成功したが、
                    受信したデータの処理中にエラーが発生。
                    データが破損しているか、正しい形式ではないなど。"
                );
                break;

            default: throw new ArgumentOutOfRangeException();
        }

    }


    //自分のランクを確認する処理
    [System.Serializable]
    public class AddCityHomerunRank
    {
        public CityHomerunRank cityHomerunRank;
    }

    [System.Serializable]
    public class CityHomerunRank
    {
        public int id;
        public string token;
        public string user_name;
        public int score;
        public int rank;
    }

    IEnumerator MyRankPostRequest(string url)
    {
        gameManager.myRankText.text = "---";
        gameManager.myNameText.text = gameManager.myName;
        gameManager.myScoreText.text = "---";
        if (String.IsNullOrEmpty(gameManager.myToken)) yield break;

        WWWForm form = new WWWForm();
        form.AddField("token", gameManager.myToken);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
                    {
                try
                {
                    Debug.Log(www.downloadHandler.text);

                    var myRank = JsonUtility.FromJson<AddCityHomerunRank>(www.downloadHandler.text);

                    gameManager.myScoreText.text = myRank.cityHomerunRank.score.ToString("#,0") + "m";
                    gameManager.myRankText.text = myRank.cityHomerunRank.rank.ToString();
                    gameManager.myNameText.text = gameManager.myName;
                }
                catch (System.Exception ex)
                {
                    Debug.Log(ex.ToString());
                }
            }
        }
    }




    //ユーザーを追加する処理
    [System.Serializable]
    public class NewUser
    {
        public User user;
    }

    [System.Serializable]
    public class User
    {
        public string token;
        public int id;
    }


    public void CoUserAdd()
    {
        StartCoroutine(UserAdd("***"));
    }

    IEnumerator UserAdd(string url)
    {
        WWWForm form = new WWWForm();
        form.AddField("servicekey", "***");

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                try
                {
                    Debug.Log("UserAdd : "+www.downloadHandler.text);

                    var newUser = JsonUtility.FromJson<NewUser>(www.downloadHandler.text);

                    Debug.Log(newUser.user);
                    Debug.Log(newUser.user.token);

                    gameManager.myToken = newUser.user.token;
                    PlayerPrefs.SetString(GameManager.KEY_SAVE_TOKEN, newUser.user.token);
                    PlayerPrefs.Save();
                    CoRankingAddApi();
                }
                catch (System.Exception ex)
                {
                    Debug.Log(ex.ToString());
                }
            }
        }
    }


    //ランキングの登録
    [System.Serializable]
    public class Player
    {
        public PlayerData player;
    }


    [System.Serializable]
    public class PlayerData
    {
        public string token;
        public string user_name;
        public int score;
        public int lv_power;
        public int lv_meet;
        public int lv_training;
    }

    public void CoRankingAddApi()
    {
        StartCoroutine(RankingAddApi("***"));
    }

    IEnumerator RankingAddApi(string url)
    {
        WWWForm form = new WWWForm();
        form.AddField("token", gameManager.myToken);
        Debug.Log(gameManager.myToken);
        form.AddField("user_name", gameManager.myName);
        form.AddField("score", (int)gameManager.hiScore);
        form.AddField("lv_power", gameManager.playerManager.powerLv);
        form.AddField("lv_meet", gameManager.playerManager.meetLv);
        form.AddField("lv_training", gameManager.playerManager.traningLv);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                try
                {
                    Debug.Log(www.downloadHandler.text);
                }
                catch (System.Exception ex)
                {
                    Debug.Log(ex.ToString());
                }
            }
        }
    }

    public void CoPlayDataLog()
    {
        StartCoroutine(PlayDataLog("***"));
    }

    IEnumerator PlayDataLog(string url)
    {
        WWWForm form = new WWWForm();
        form.AddField("token", gameManager.myToken);
        form.AddField("user_name", gameManager.myName);
        form.AddField("score", (int)gameManager.currentScore);
        form.AddField("lv_power", gameManager.playerManager.powerLv);
        form.AddField("lv_meet", gameManager.playerManager.meetLv);
        form.AddField("lv_training", gameManager.playerManager.traningLv);
        //form.AddField("battName", gameManager.battList[gameManager.equipBattNumber-1].battName);
        //form.AddField("battLevel", gameManager.battList[gameManager.equipBattNumber - 1].battlevel);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("プレイログ送信");
                try
                {
                    Debug.Log(www.downloadHandler.text);
                }
                catch (System.Exception ex)
                {
                    Debug.Log(ex.ToString());
                }
            }
        }
    }



}
