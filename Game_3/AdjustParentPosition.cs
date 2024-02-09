using UnityEngine;

public class AdjustParentPosition : MonoBehaviour
{
    public GameObject footObj;
    public GameObject parentObj;

    private Vector3 initialFootPosition; //初期の足の設置面のワールド座標
    private Vector3 initialParentPosition; //初期の親オブジェクトのワールド座標

    public void SetInitialPosition(bool training = false)
    {
        //初期の足の設置面のワールド座標を保持
        initialFootPosition = footObj.transform.position;
         if(training) initialFootPosition = new Vector3(-0.83f, -0.93f, -1.33f);

        // 初期の親オブジェクトのワールド座標を保持
        initialParentPosition = parentObj.transform.position;
    }

    //Todo飛んでいるアニメーション中に変更を行なってしまうと位置がおかしくなってしまうため、待機モーションに変更してから一の変更を行う
    public void ChangePosition()
    {
        //足の設置面の現在のワールド座標を取得
        Vector3 currentFootPosition = footObj.transform.position;

        //初期位置との高さの差分を計算
        float heightDifference = currentFootPosition.y - initialFootPosition.y;
        //Debug.Log(heightDifference);

        //親オブジェクトを高さの差分だけ上げる
        parentObj.transform.position = new Vector3(
            initialParentPosition.x,
            initialParentPosition.y - heightDifference,
            initialParentPosition.z
        );
    }
}
