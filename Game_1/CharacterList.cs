using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterList : MonoBehaviour
{
    public ObjectPool charaListObjectPool;

    // Start is called before the first frame update
    void OnEnable()
    {
        GameManager.instance.getCharaParamsSort.Clear();
        foreach (GameManager.GetCharaParam getcharaParam in GameManager.instance.getCharaParams)
        {
            GameManager.instance.getCharaParamsSort.Add(getcharaParam);
        }
        //際表示された際指定のソート順にしなおす
        CharacterSort(GameManager.instance.selectCharaGenre);
    }

    public void CharacterSort(string genre)
    {
        //選択されているジャンル
        GameManager.instance.selectCharaGenre = genre;
        GameManager.instance.getCharaParamsSort.Clear();

        //クリアしたあと再度変数に入手順で入れ直し、それぞれ設定された順でソートしなおす
        foreach (GameManager.GetCharaParam getcharaParam in GameManager.instance.getCharaParams)
        {
            GameManager.instance.getCharaParamsSort.Add(getcharaParam);
        }

        if (genre == "Monster")//リストをキャラのID順でソートする
            GameManager.instance.getCharaParamsSort.Sort((a, b) => a.character_id.CompareTo(b.character_id));
        if (genre == "Attack")//リストをキャラの攻撃力順でソートする
            GameManager.instance.getCharaParamsSort.Sort((a, b) => b.Attack.CompareTo(a.Attack));
        if (genre == "Defence")//リストをキャラの守備力順でソートする
            GameManager.instance.getCharaParamsSort.Sort((a, b) => b.Defence.CompareTo(a.Defence));
        //プール処理をしている箇所で条件を更新して再度表示させる
        charaListObjectPool.UpdateEquipItems();
    }
}
