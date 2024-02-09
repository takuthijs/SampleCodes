using UnityEngine;
using UnityEngine.UI;

public class LobbyCharacterRandomMovement : MonoBehaviour
{
    public Image characterImg;
    public SpriteRenderer characterSprite;
    public bool isTouch = false;

    public float minSpeed = 1f; //最小移動速度
    public float maxSpeed = 3f; //最大移動速度
    public float minStopDuration = 1f; //最小停止時間
    public float maxStopDuration = 6f; //最大停止時間

    private bool isMoving = true; //キャラクターが移動中かどうかのフラグ
    private float moveSpeed; //移動速度
    private float stopTimer = 0f; //停止時間を計測するタイマー

    private Vector3 imageScaleA = new Vector3(-30, 30, 1);
    private Vector3 imageScaleB = new Vector3(30, 30, 1);

    //private bool hasReachedBoundary = false; //画面端に到達したかどうかのフラグ

    private void Start()
    {
        //初期の移動速度をランダムに設定する
        moveSpeed = Random.Range(minSpeed, maxSpeed);

        if (Random.Range(0, 101) < 50)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        MoveSwitch();
    }

    private void Update()
    {
        if (!isTouch && IsWithinBoundaryX() && isMoving)
        {
            //キャラクターをx軸方向に移動させる
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
            if (Random.Range(0, 101) < 1)
            {
                MoveSwitch();
                isMoving = false;
            }
        }
        else
        {
            
            isMoving = false;
            
            //停止時間を計測
            stopTimer += Time.deltaTime;
            

            //停止時間が経過したら再び移動させる
            if (stopTimer >= Random.Range(minStopDuration, maxStopDuration))
            {
                isMoving = true;
                stopTimer = 0f;

                //移動速度をランダムに変更する
                moveSpeed = Random.Range(minSpeed, maxSpeed);
                if (moveSpeed < 0) moveSpeed *= -1f;

                //キャラクターのx座標が範囲内にあるか判定する
                float characterX = transform.position.x;
                float leftBoundaryX = GameManager.instance.rangeA.position.x;
                float rightBoundaryX = GameManager.instance.rangeB.position.x;

                //範囲外だったら範囲内にすこ押し戻る
                //左側に当たった時の処理
                if (characterX <= leftBoundaryX)
                {
                    transform.Translate(Vector3.right * moveSpeed * Time.deltaTime * 3);
                }
                //右側に当たった時の処理
                else if(characterX >= rightBoundaryX)
                {
                    moveSpeed *= -1;
                    transform.Translate(Vector3.right * moveSpeed * Time.deltaTime * 3);
                }
                else
                {
                    //移動方向をランダムに変更する
                    if (Random.value < 0.5f) MoveSwitch();
                }

                //画像の向きを変える処理
                if (moveSpeed < 0)
                {
                    characterSprite.transform.localScale = imageScaleA;
                }
                else
                {
                    characterSprite.transform.localScale = imageScaleB;
                }
            }
            
        }
    }
    

    private bool IsWithinBoundaryX()
    {
        //キャラクターのx座標が範囲内にあるか判定する
        float characterX = transform.position.x;
        float leftBoundaryX = GameManager.instance.rangeA.position.x;
        float rightBoundaryX = GameManager.instance.rangeB.position.x;

        return (characterX >= leftBoundaryX && characterX <= rightBoundaryX);
    }

    void MoveSwitch()
    {
        moveSpeed *= -1f;
        if (moveSpeed < 0)
        {
            characterSprite.transform.localScale = imageScaleA;
        }
        else
        {
            characterSprite.transform.localScale = imageScaleB;
        }
    }

    //private bool IsWithinBoundaryY()
    //{
    //    //キャラクターのx座標が範囲内にあるか判定する
    //    float characterY = transform.position.y;
    //    float leftBoundaryY = GameManager.instance.rangeA.position.y;
    //    float rightBoundaryY = GameManager.instance.rangeB.position.y;

    //    return (characterY >= leftBoundaryY && characterY <= rightBoundaryY);
    //}

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    isMoving = false;
    //}
}
