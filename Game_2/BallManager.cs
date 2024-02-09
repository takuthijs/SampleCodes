using System.Collections;
using System.Collections.Generic;
using DG.Tweening.Core.Easing;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    [HideInInspector]public GameManager gameManager;
    bool ballTouch = false;
    public bool animationPlayNow = false;

    //ボールが高く上がりすぎて真下に落ちている時終了するために使用
    private bool moveXcheck = false;
    private Vector3 previousPosition;
    private float timer = 0f;
    private float timeThreshold = 3f;

    void Update()
    {
        bool isGround = gameObject.transform.position.y < -5;//地面を貫通した時
        bool isSleeping = (gameObject.GetComponent<Rigidbody>().IsSleeping() && !ballTouch && !animationPlayNow);//ボールの動きが止まった時
        bool isMoveXFreze = moveXcheck && transform.position.y > 1000;//ボールが垂直落下していてかつy軸が1000以上の時

        //y座標が0以下かボールの動きが止まっていたら
        if (isGround || isSleeping || isMoveXFreze)
        {
            ballTouch = true;
            moveXcheck = false;

            //早送り状態を元に戻す
            Time.timeScale = 1f;
            gameManager.fastforward = false;

            gameManager.isBallStop = true;
            var ballRigidbody = gameObject.GetComponent<Rigidbody>();
            ballRigidbody.constraints = RigidbodyConstraints.FreezePosition;

            //float dist = Vector3.Distance(gameManager.ico_meet.transform.position, gameObject.transform.position);
            float dist = gameManager.intDiff + (1300 * gameManager.swapBackgroundObjCount);
            gameManager.lb_gameresult.text = Mathf.CeilToInt(dist).ToString() + "m";

            //ハイスコアだった場合はPlayerPrefに保存する
            gameManager.highScoreText.SetActive(false);
            
            if (gameManager.hiScore < Mathf.CeilToInt(dist))
            {
                gameManager.UpdateHiScore(Mathf.CeilToInt(dist));
            }

            gameManager.currentScore = Mathf.CeilToInt(dist);

            gameManager.canvasResult.SetActive(true);
            gameManager.canvasGame.SetActive(false);
            gameManager.audioManager.PlaySE(3);//歓声SEを再生
        }

        if(gameObject.transform.position.x <= -282.68f)
        {
            gameManager.highScoreText.SetActive(false);
            //失敗時のcanvasを入れた方が良いかも
            gameManager.currentScore = 0;
            gameManager.lb_gameresult.text = 0 + "m";
            gameManager.ballMoving = false;
            gameManager.canvasResult.SetActive(true);
            gameManager.canvasGame.SetActive(false);
            Destroy(gameManager.ball);
            gameManager.ball = null;
        }

        if (!moveXcheck && !ballTouch && !animationPlayNow)
        {
            //現在の位置を取得
            Vector3 currentPosition = transform.position;

            //前のフレームとの位置が同じかチェック
            if (currentPosition.x == previousPosition.x)
            {
                //タイマーを加算
                timer += Time.deltaTime;

                //タイマーがしきい値を超えたら処理を実行
                if (timer >= timeThreshold)
                {
                    //3秒間x軸が変化しなかった場合の処理
                    moveXcheck = true;
                }
            }
            else
            {
                //位置が変わった場合、タイマーをリセット
                timer = 0f;
            }

            //現在の位置を保存
            previousPosition = currentPosition;
        }
    }

    bool trigger = true;
    private void OnTriggerEnter(Collider other)
    {
        if (trigger)
        {
            Rigidbody ballRigidbody = gameObject.GetComponent<Rigidbody>();
            ballRigidbody.drag = 0.25f;
            ballRigidbody.angularDrag = 0.25f;
            trigger = false;

            //山に当たったらisTriggerをOnにする
            if (other.tag == "Mountain")
            {
                other.GetComponent<MeshCollider>().isTrigger = true;
            }
        }

    }
}
