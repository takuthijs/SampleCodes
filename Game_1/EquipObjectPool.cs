using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Pool;
using UnityEngine.UI;

[RequireComponent(typeof(LoopScrollRect))]
[DisallowMultipleComponent]
public sealed class EquipObjectPool : MonoBehaviour, LoopScrollPrefabSource, LoopScrollDataSource
{
    private ObjectPool<GameObject> _pool;
    public GameObject item;

    // Implement your own Cache Pool here. The following is just for example.
    Stack<Transform> pool = new Stack<Transform>();
    public GameObject GetObject(int index)
    {
        if (pool.Count == 0)
        {
            return Instantiate(item,GameManager.instance.allParent.transform);
        }
        Transform candidate = pool.Pop();
        candidate.gameObject.SetActive(true);
        return candidate.gameObject;
    }

    public void ReturnObject(Transform trans)
    {
        // Use `DestroyImmediate` here if you don't need Pool
        //和訳：プールが必要ない場合はここでDestroyImmediateを使用してください
        trans.SendMessage("ScrollCellReturn", SendMessageOptions.DontRequireReceiver);
        trans.gameObject.SetActive(false);
        trans.SetParent(transform, false);

        //ゲームオブジェクトが消える時はGameManager.instance.generateItemsから消す
        //GameManager.instance.generateItems.Remove(trans.gameObject);
        //GameManager.instance.setEquipment.equipItems.Remove(trans.gameObject.GetComponent<EquipItems>());
        //生成番号もマイナスする
        //GameManager.instance.generateNum--;

        pool.Push(trans);
    }

    public void ProvideData(Transform transform, int idx)
    {
        //すでにモンスターが装備しているものとジャンルが違うものは非アクティブにする
        HashSet<int> setCharaEquipuniqueIdsSet = new HashSet<int>(GameManager.instance.equipNowUniqueIdList);
        if (setCharaEquipuniqueIdsSet.Contains(GameManager.instance.getEquipParamsSort[idx].unique_id)) {
            //transform.gameObject.SetActive(false);
            ReturnObject(transform);
        }
        else
        {
            transform.gameObject.GetComponent<EquipItems>().SetEquipParam(GameManager.instance.getEquipParamsSort[idx].unique_id);
            //生成されたものの選択された時の画像をリストに追加する、もしかしたら消えたオブジェクトを上の関数でリムーブした方がいいかも
            GameManager.instance.setEquipment.equipItems.Add(transform.gameObject.GetComponent<EquipItems>());
            GameManager.instance.generateItems.Add(transform.gameObject);
        }
    }

    void OnEnable()
    {
        GameManager.instance.generateItems.Clear();
        GameManager.instance.setEquipment.equipItems.Clear();
        GameManager.instance.generateNum = 0;

        var ls = GetComponent<LoopScrollRect>();
        ls.prefabSource = this;
        ls.dataSource = this;
        ls.totalCount = GameManager.instance.getEquipParamsSort.Count;
        ls.RefillCells();
    }

    public void UpdateEquipItems()
    {
        GameManager.instance.generateItems.Clear();
        GameManager.instance.setEquipment.equipItems.Clear();
        GameManager.instance.generateNum = 0;

        var ls = GetComponent<LoopScrollRect>();
        ls.prefabSource = this;
        ls.dataSource = this;
        ls.totalCount = GameManager.instance.getEquipParamsSort.Count;
        ls.RefillCells();
    }
    
}
