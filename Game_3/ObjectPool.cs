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
    public ContentType contentType;
    public enum ContentType
    {
        beetles,
        skills
    }

    // Implement your own Cache Pool here. The following is just for example.
    Stack<Transform> pool = new Stack<Transform>();
    public GameObject GetObject(int index)
    {
        if (pool.Count == 0)
        {
            return Instantiate(item);
        }
        Transform candidate = pool.Pop();
        if (candidate != null)
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

        //以下の部分に生成するたびに実行する処理を入れる。
        switch (contentType)
        {
            case ContentType.beetles:
                if (idx >= GameManager.instance.beetleManager.myBeetles.Count) return;
                transform.gameObject.GetComponent<BeetleCard>().SetBeetleCard(GameManager.instance.beetleManager.myBeetles[idx].unique_id);
                break;
            case ContentType.skills:
                if (idx >= GameManager.instance.mySkills.Count) return;
                transform.gameObject.GetComponent<SkillCard>().SetSkillCard(GameManager.instance.mySkills[idx].unique_id);
                transform.gameObject.GetComponent<SkillCard>().EffectActivate();
                break;
            default:
                break;
        }
    }

    void OnEnable()
    {
        var ls = GetComponent<LoopScrollRect>();
        ls.prefabSource = this;
        ls.dataSource = this;
        switch(contentType)
        {
            case ContentType.beetles:
                ls.totalCount = GameManager.instance.beetleManager.myBeetles.Count;
                break;
            case ContentType.skills:
                ls.totalCount = GameManager.instance.mySkills.Count;
                break;
            default:
                break;
        }
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
        switch (contentType)
        {
            case ContentType.beetles:
                ls.totalCount = GameManager.instance.beetleManager.myBeetles.Count;
                break;
            case ContentType.skills:
                ls.totalCount = GameManager.instance.mySkills.Count;
                break;
            default:
                break;
        }
        ls.RefillCells();
    }
}