using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PowerObj : MonoBehaviour
{
    //ヒットエフェクト
    GameObject hitEffect;

    //投げるオブジェクトのイメージ
    public Sprite equipImage;
    public int equipId;

    //一度だけエフェクトが表示されるようにするフラグ
    bool isEffect = true;

    private void Start()
    {
        //0.2秒後にDestroyメソッドを呼び出して自身を消滅させる
        Invoke("DestroySelf", 0.5f);
    }

    //自身が他の何かにぶつかった0.2秒後に消滅する
    void OnCollisionEnter(Collision collision)
    {
        ItemObject itemObj = collision.gameObject.GetComponent<ItemObject>();
        if (itemObj?.hp == 0 && isEffect)
        {
            //まだ壊したことのないアイテムの場合はGetItemListに追加
            if(GameManager.instance.getEquipItems.Find(equipItem => equipItem.itemId == itemObj.itemId) == null)
            {
                GameManager.EquipItem newEquipItem = new GameManager.EquipItem{
                    itemId = itemObj.itemId,
                    brokenCount = 1
                };
                GameManager.instance.getEquipItems.Add(newEquipItem);

                //Addした後にソートする
                GameManager.instance.SortItemList();
                //アイテムカードを全削除
                GameManager.instance.RemoveAllChildrenItemCard(GameManager.instance.itemCardParent);
                //アイテムカードを再生成する
                GameManager.instance.ItemCardGenerate();
            }
            else
            {
                //すでに入手済みの場合は壊したカウントをプラス1
                GameManager.EquipItem newEquipItem = GameManager.instance.getEquipItems.Find(equipItem => equipItem.itemId == itemObj.itemId);
                newEquipItem.brokenCount++;
            }

            //すでに何かのオブジェクトにぶつかり重力が働いている場合は処理しない

            //何かにぶつかったら1度だけエフェクトを表示させる
            hitEffect = Instantiate(GameManager.instance.hitEffect,GameManager.instance.effectObjParent);
            GameManager.instance.soundManager.PlaySE(7);//壊れる音
            hitEffect.transform.position = gameObject.transform.position;
            GameManager.instance.dropItems.Add(collision.gameObject);
            collision.gameObject.SetActive(false);//対象のオブジェクトを消す
            GameManager.instance.ResultCheck();//何かを破壊したらリザルト画面へ

            isEffect = false;
            gameObject.GetComponent<Rigidbody>().useGravity = true;//そのままつき進むのを防ぐために重力を使用して力を抑える

            //BoxCollider bc = gameObject.GetComponent<BoxCollider>();
            //SphereCollider sc = gameObject.GetComponent<SphereCollider>();
            //if (bc == null && sc == null) return;
            //if(bc != null) if (gameObject.GetComponent<BoxCollider>().isTrigger) return;//当たり判定がない場合は処理しない
            //if(sc != null) if (gameObject.GetComponent<SphereCollider>().isTrigger) return;//当たり判定がない場合は処理しない
        }
        else
        {
            //ゲームオブジェクトではなかった場合は当たり判定をなくす
            //地面のオブジェウトではない場合は当たり判定をなくす
            if(collision.gameObject.tag != "Plane" && collision.gameObject.tag != "Room")
            {
                if (!gameObject.GetComponent<BoxCollider>()) return;
                gameObject.GetComponent<BoxCollider>().isTrigger = true;
            }
            else
            {
                gameObject.GetComponent<Rigidbody>().useGravity = true;//そのままつき進むのを防ぐために重力を使用して力を抑える
            }
        }

    }

    void DestroySelf()
    {
        //エフェクトが発生していた場合エフェクトを削除
        if(!isEffect) Destroy(GameManager.instance.effectObjParent.GetChild(0)?.gameObject);
        //自身を消滅させる
        Destroy(gameObject);
    }
}
