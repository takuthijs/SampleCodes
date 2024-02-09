using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public int hp;
    public int itemId;
    private bool hasLogged = false;
    public GameObject bottomPoint;
    private Renderer objectRenderer;

    public enum enumMaterial
    {
        stone,
        wood,
        metal
    }
    public enumMaterial material;

    private void Start()
    {
        gameObject.GetComponent<Rigidbody>().mass = hp * 5;
        objectRenderer = GetComponent<Renderer>();
    }

    //アイテムが落ちた時の処理
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PowerObj")
        {
            if (objectRenderer != null)
            {
                // DoTweenを使用して点滅するアニメーション
                float duration = 0.1f; // アニメーションの時間
                int blinkCount = 3; // 点滅回数（適宜調整）

                // DOTween.Sequence()で順次アニメーションを追加
                Sequence sequence = DOTween.Sequence();

                for (int i = 0; i < blinkCount; i++)
                {
                    sequence.Append(objectRenderer.material.DOColor(Color.white, duration / (blinkCount * 2)))
                            .Append(objectRenderer.material.DOColor(Color.clear, duration / (blinkCount * 2)))
                            .Append(objectRenderer.material.DOColor(Color.white, duration / (blinkCount * 2)));
                }

                // アニメーションの再生
                sequence.Play();
            }
        }

        //材質に応じたサウンドを再生する
        switch (material)
        {
            case enumMaterial.stone:
                GameManager.instance.soundManager.PlayMaterialSound(0);
                break;
            case enumMaterial.wood:
                GameManager.instance.soundManager.PlayMaterialSound(1);
                break;
            case enumMaterial.metal:
                GameManager.instance.soundManager.PlayMaterialSound(2);
                break;
        }

        //得点を加算
        GameManager.instance.score += 10;
        GameManager.instance.scoreText.text = "Score:" + GameManager.instance.score;

        //Debug.Log(other.gameObject.tag);
        //Planeタグが付いたオブジェクトに触れたかどうかをチェック
        if (other.gameObject.tag == "Plane" && !hasLogged)
        {
            //床に落ちたオブジェクトを保管するリストに入れる
            GameManager.instance.dropItems.Add(gameObject);

            //フラグを立てて、以降のログ出力を防ぐ
            hasLogged = true;

            //成功したかどうかの判定を起動
            GameManager.instance.ResultCheck();
        }
    }

    //生成される位置を自動で入れるために作成した関数
    //void test()
    //{
    //    Transform bottomPoint = gameObject.transform.Find("BottomPoint");

    //    // BottomPointが存在する場合、ワールド座標をログで表示
    //    if (bottomPoint != null)
    //    {
    //        Vector3 bottomPointPosition = bottomPoint.position;
    //        GameManager.instance.objectPositions.Add(bottomPointPosition);
    //    }
    //}
}
