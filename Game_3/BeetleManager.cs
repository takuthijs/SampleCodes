using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BeetleManager : MonoBehaviour
{
    //実装している昆虫のscriptableObject
    public List<BeetleSO> beetleSos;

    //所持している昆虫
    public List<Beetle> myBeetles;

    //選択されている虫のステータス
    public Beetle selectBeetle;

    //相手の昆虫データ
    public Beetle enemyBeetle;

    [System.Serializable]
    public class Beetle
    {
        public string beetleToken;//ランキングの取得に使用
        public int rank;
        public int rate;
        public int reincarnationCount;
        public string unique_id;
        public int beetle_id;
        public string beetleName;
        public Sprite beetleCardImage;
        public GameManager.GrowStatus beetleType;
        public int growCount;
        public int power;
        public int inter;
        public int guard;
        public int speed;
        public List<string> selectSkill = new List<string>();//セットしているスキル 順番はグー,チョキ,パーの順番
        public List<int> learnableSkills = new List<int>();//習得可能なスキル
        public int skillCostLimit;
        public string dateTime;
    }

    private void Start()
    {
        //Debug.Log("生成された");
    }
}
