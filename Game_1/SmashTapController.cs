using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using static UnityEngine.GraphicsBuffer;

public class SmashTapController : MonoBehaviour
{
    public static SmashTapController instance;
    public int count;
    public TextMeshProUGUI countText;
    //public GameObject tapObject;
    [HideInInspector]public GameObject textObj;
    public int skillCost;

    public GameObject smashTapPrefab;
    public GameObject smashTapParent;

    private GameObject smashTapObj;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    public void SetCount()
    {
        smashTapObj = Instantiate(smashTapPrefab, smashTapParent.transform);
        textObj = smashTapObj.transform.GetChild(0).gameObject;
        countText = textObj.GetComponent<TextMeshProUGUI>();
        count = skillCost * 10 / 2;
        countText.text = count.ToString();
    }

    public void SmashTap()
    {
        if(count > 0)
        {
            count--;
            countText.text = count.ToString();
            if (count == 0)
            {
                //ミニゲームのクリア回数を追加
                GameManager.instance.miniGamePlayInfo["SmashTap"]++;
                StartCoroutine(SmashTapUIFade());
            }
            //タップオブジェクトをゆらす
            if (smashTapObj != null)
            {
                GameManager.instance.ShakeGameobject(smashTapObj);
                GameManager.instance.ShakeGameobject(textObj);
            }

        }
    }


    IEnumerator SmashTapUIFade()
    {
        int i = 0;
        CanvasGroup canvasGroup = UIManager.instance.smashTapUI.GetComponent<CanvasGroup>();
        while (canvasGroup.alpha > 0)
        {
            i++;
            canvasGroup.alpha -= 0.1f;
            yield return new WaitForSeconds(0.01f);
            if (i == 100) break;
        }
        UIManager.instance.smashTapUI.SetActive(false);
        canvasGroup.alpha = 1;
        Skills.instance.SkillExe(GameManager.instance.foucusChara.GetComponent<CharaParam>().skillNum);

        //if (GameManager.instance.foucusChara.GetComponent<CharaParam>().skillNum != 0) GameManager.instance.HpGageProcess();

        yield return new WaitForSeconds(0.5f);
        Destroy(smashTapObj);
        smashTapObj = null;
    }

}
