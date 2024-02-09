using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RouletteController : MonoBehaviour
{
    public static RouletteController instance;

    public RectTransform spinImage;
    float rotateZ = 0;
    public int cost ;//コスト(スピード)
    public bool buttonPush = false;
    [SerializeField]List<string> judge;

    private void Start()
    {
        instance = this;
    }

    void FixedUpdate()
    {
        if (buttonPush)
        {
            rotateZ -= cost * 1.2f;
            spinImage.rotation = Quaternion.Euler(0, 0, rotateZ);
        }
    }

    public void RouletteStop()
    {
        
        buttonPush = false;
            
        int RoulettePos = Mathf.FloorToInt((float)(spinImage.rotation.eulerAngles.z - 22.5f) / (360 / judge.Count)) + 1;//22.5は初期画像のズレ

        if (RoulettePos == judge.Count) RoulettePos = 0;//タイミンングが良いと配列の上限を一つ超えてしまうので最初の値にセット
        if(judge[RoulettePos] == "Success")
        {
            StartCoroutine(CoRouletteEnd(GameManager.instance.foucusChara.GetComponent<CharaParam>().skillNum));

            //ミニゲームのクリア回数を追加
            GameManager.instance.miniGamePlayInfo["Roulette"]++;
        }
        else
        {
            StartCoroutine(CoRouletteEnd(0));
        }
    }

    void RouletteReset()
    {
        rotateZ = 0;
        spinImage.rotation = Quaternion.Euler(0, 0, 0);
    }

    IEnumerator CoRouletteEnd(int num)
    {
        yield return new WaitForSeconds(1);
        
        int i = 0;
        CanvasGroup canvasGroup = UIManager.instance.rouletteUI.GetComponent<CanvasGroup>();
        while (canvasGroup.alpha > 0)
        {
            i++;
            canvasGroup.alpha -= 0.1f;
            yield return new WaitForSeconds(0.01f);
            if (i == 100) break;
        }
        UIManager.instance.rouletteUI.SetActive(false);
        canvasGroup.alpha = 1;

        Skills.instance.SkillExe(num);
        //if(num != 0) GameManager.instance.HpGageProcess();
        RouletteReset();//ルーレットの位置を初期値に戻す
        //StopAllCoroutines();
    }
}
