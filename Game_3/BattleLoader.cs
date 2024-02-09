using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleLoader : MonoBehaviour
{
    public BattleManager battleManager;

    //昆虫を生成する親
    public GameObject playerBeetleParent;
    public GameObject enemyBeetleParent;

    //テストで入れている文字
    public TextMeshProUGUI testText;

    public GameObject skillExeCanvas;

    void Start()
    {
        if (GameManager.instance.playFabController.isMatching) battleManager.battleMode = BattleManager.BattleMode.player;
        GameManager.instance.battleManager = battleManager;

        //selectBeetleが直接変わらないようにbattleManagerのbattleBeetleに入れる
        battleManager.battleBeetle = GameManager.instance.NewBeetle(GameManager.instance.beetleManager.selectBeetle);

        //通信できた相手の昆虫情報をセットする
        //デフォルトでセットされている昆虫のオブジェクトを削除する
        //エフェクトを配置するオブジェクト以外削除
        foreach (Transform n in playerBeetleParent.transform)
        {
            if(n.tag != "EffectPos" && n.tag != "TypeBounus") GameObject.Destroy(n.gameObject);
        }
        foreach (Transform n in enemyBeetleParent.transform)
        {
            if (n.tag != "EffectPos" && n.tag != "TypeBounus") GameObject.Destroy(n.gameObject);
        }

        //プレイヤーの昆虫と相手の昆虫を生成する
        battleManager.playerBeetleObj = Instantiate(GameManager.instance.beetleManager.beetleSos[GameManager.instance.beetleManager.selectBeetle.beetle_id - 1].model, playerBeetleParent.transform);
        //オブジェクトのステータスに応じた大きさに変更する
        BeetleScaler beetleScaler = battleManager.playerBeetleObj.GetComponent<BeetleScaler>();
        if (beetleScaler is not null)
        {
            beetleScaler.power = battleManager.battleBeetle.power;
            beetleScaler.inter = battleManager.battleBeetle.inter;
            beetleScaler.guard = battleManager.battleBeetle.guard;
            beetleScaler.speed = battleManager.battleBeetle.speed;
            beetleScaler.ScaleChange();
        }

        battleManager.enemyBeetleObj = Instantiate(GameManager.instance.beetleManager.beetleSos[GameManager.instance.beetleManager.enemyBeetle.beetle_id - 1].model, enemyBeetleParent.transform);

        battleManager.enemyBeetleName.text = GameManager.instance.BeetleTypeIcon(GameManager.instance.beetleManager.enemyBeetle.beetleType) + GameManager.instance.beetleManager.enemyBeetle.beetleName;
        //敵のオブジェクトのステータスに応じた大きさに変更する
        beetleScaler = battleManager.enemyBeetleObj.GetComponent<BeetleScaler>();
        if (beetleScaler is not null)
        {
            beetleScaler.power = GameManager.instance.beetleManager.enemyBeetle.power;
            beetleScaler.inter = GameManager.instance.beetleManager.enemyBeetle.inter;
            beetleScaler.guard = GameManager.instance.beetleManager.enemyBeetle.guard;
            beetleScaler.speed = GameManager.instance.beetleManager.enemyBeetle.speed;
            beetleScaler.ScaleChange();
        }

        battleManager.playerBeetleAnimator = battleManager.playerBeetleObj.GetComponent<Animator>();
        battleManager.enemyBeetleAnimator = battleManager.enemyBeetleObj.GetComponent<Animator>();

        //各ボタンに自身の昆虫のスキル番号を付与
        for(int i = 0; i < 3; i++)
        {
            //ユニークIDからFindしてスキルの番号を入れる
            if (GameManager.instance.beetleManager.selectBeetle.selectSkill[i].Length >= 16)
            {
                GameManager.SkillInfo skill = GameManager.instance.mySkills.Find(skill => skill.unique_id == GameManager.instance.beetleManager.selectBeetle.selectSkill[i]);
                battleManager.skillButtons[i].GetComponent<SkillButton>().skillNum = skill.skillNum;
                battleManager.skillButtons[i].GetComponent<SkillButton>().unique_id = skill.unique_id;
            }
            else
            {
                battleManager.skillButtons[i].GetComponent<SkillButton>().skillNum = 0;
            }
        }

        //スキル発動画面をbattleManagerに渡す
        battleManager.skillExeCanvas = this.skillExeCanvas;
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
