using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ModelBonusCoin : MonoBehaviour
{
    public float time_left = 0;
    public GameObject bounusPopupWindow;
    public TextMeshProUGUI lb_timeleft;//インスペクター上で設定しています

    public int bonusCoin = 10;

    public GameManager gameManager;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (time_left  < 60　&& lb_timeleft!=null) {
            time_left += Time.deltaTime;
            lb_timeleft.text =  Mathf.FloorToInt(time_left).ToString();
        }else{
            time_left = 60;
        }
    }

    //広告なしボタン
    public void OnClickNoAd()
    {
        if (time_left < 60) return;
        gameManager.AddCoin(Mathf.FloorToInt(bonusCoin * gameManager.playerManager.traningLv * time_left * 0.5f));
        time_left = 0;
        gameManager.bounusParticleImage.Play();
        bounusPopupWindow.SetActive(false);
    }

    //広告ありボタン
    public void OnClick()
    {
        if (time_left < 60) return;
        gameManager.NetWorkConection();
        if (gameManager.networkConection)
        {
            gameManager.adMobScript.ShowAd(() =>
            {
                gameManager.AddCoin(Mathf.FloorToInt(bonusCoin * gameManager.playerManager.traningLv * time_left * 1f));
                time_left = 0;
                gameManager.bounusParticleImage.Play();
                bounusPopupWindow.SetActive(false);

                Firebase.Analytics.FirebaseAnalytics.LogEvent("coinAdd");

                //広告をロード
                gameManager.adMobScript.LoadAd();
            });
        }
        else
        {
            OnClickNoAd();
        }
    }

}
