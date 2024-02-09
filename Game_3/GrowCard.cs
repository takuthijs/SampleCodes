using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class GrowCard : MonoBehaviour, IPointerClickHandler
{

    public GameManager.GrowStatus topGrowStatus;//上段の上昇させるステータス
    public GameManager.GrowStatus bottomGrowStatus;//下段の上昇させるステータス

    public TextMeshProUGUI upText;//上段のテキスト
    public TextMeshProUGUI bottomText;//下段のテキスト

    public int upNum;//上段の数値
    public int bottomNum;//下段の数値

    //タップされた時の処理
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        GameManager.instance.homeCanvas.SetActive(true);
        GameManager.instance.tabCanvas.SetActive(true);
        GameManager.instance.traningAfterCanvas.SetActive(false);

        //選択されている昆虫に押されたボタンのステータスを追加する
        BeetleManager.Beetle beetle = GameManager.instance.beetleManager.selectBeetle;

        AddStatus(topGrowStatus,beetle, upNum);
        AddStatus(bottomGrowStatus, beetle, bottomNum);

        //自分が持っているビートルもステータスを加算させる
        BeetleManager.Beetle mybeetle = GameManager.instance.beetleManager.myBeetles.Find(x => x.unique_id == beetle.unique_id);
        mybeetle = beetle;

        //育成回数を減らす
        GameManager.instance.beetleManager.selectBeetle.growCount--;

        //コインを減らす
        GameManager.instance.coin -= 100;

        //ホーム画面のステータスを変更する
        GameManager.instance.uIManager.HomeUIUpdate();

        //セーブ
        GameManager.instance.Save();

        //オブジェクトのスケールを変更する
        BeetleScaler beetleScaler = GameManager.instance.selectBeetleParent.transform.GetChild(0).gameObject.GetComponent<BeetleScaler>();
        if (beetleScaler is not null)
        {
            beetleScaler.power = GameManager.instance.beetleManager.selectBeetle.power;
            beetleScaler.inter = GameManager.instance.beetleManager.selectBeetle.inter;
            beetleScaler.guard = GameManager.instance.beetleManager.selectBeetle.guard;
            beetleScaler.speed = GameManager.instance.beetleManager.selectBeetle.speed;
            beetleScaler.ScaleChange(true);
        }
    }

    void AddStatus(GameManager.GrowStatus growStatus, BeetleManager.Beetle beetle, int addStatus)
    {
        switch (growStatus)
        {
            case GameManager.GrowStatus.power:
                beetle.power += addStatus;
                break;
            case GameManager.GrowStatus.inte:
                beetle.inter += addStatus;
                break;
            case GameManager.GrowStatus.guard:
                beetle.guard += addStatus;
                break;
            case GameManager.GrowStatus.speed:
                beetle.speed += addStatus;
                break;
        }
    }
}
