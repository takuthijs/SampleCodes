using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

[RequireComponent(typeof(LoopScrollRect))]
[DisallowMultipleComponent]
public sealed class ObjectPool : MonoBehaviour, LoopScrollPrefabSource, LoopScrollDataSource
{
    private ObjectPool<GameObject> _pool;
    public GameObject item;

    // Implement your own Cache Pool here. The following is just for example.
    Stack<Transform> pool = new Stack<Transform>();
    public GameObject GetObject(int index)
    {
        if (pool.Count == 0)
        {
            return Instantiate(item);
        }
        Transform candidate = pool.Pop();
        if(candidate != null)
        {
            candidate.gameObject.SetActive(true);
            return candidate.gameObject;
        }

        return null;
    }

    public void ReturnObject(Transform trans)
    {
        // Use `DestroyImmediate` here if you don't need Pool
        trans.SendMessage("ScrollCellReturn", SendMessageOptions.DontRequireReceiver);
        trans.gameObject.SetActive(false);
        trans.SetParent(transform, false);
        pool.Push(trans);
    }

    public void ProvideData(Transform transform, int idx)
    {
        //transform.SendMessage("ScrollCellIndex", idx);
        //transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = idx.ToString();
        transform.gameObject.GetComponent<SetListCharacterCard>().SetListCardParamator(GameManager.instance.getCharaParamsSort[idx].unique_id);
    }

    void OnEnable()
    {
        var ls = GetComponent<LoopScrollRect>();
        ls.prefabSource = this;
        ls.dataSource = this;
        ls.totalCount = GameManager.instance.getCharaParamsSort.Count;
        ls.RefillCells();
    }

    public void UpdateEquipItems()
    {
        //GameManager.instance.generateItems.Clear();
        //GameManager.instance.setEquipment.equipItems.Clear();
        //GameManager.instance.generateNum = 0;

        var ls = GetComponent<LoopScrollRect>();
        ls.prefabSource = this;
        ls.dataSource = this;
        ls.totalCount = GameManager.instance.getCharaParamsSort.Count;
        ls.RefillCells();
    }
}