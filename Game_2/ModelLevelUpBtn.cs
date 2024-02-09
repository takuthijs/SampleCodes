using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModelLevelUpBtn : MonoBehaviour
{

    public int level;

    public GameManager gameManager;

    [SerializeField]
    private  PlayerManager.PlayerLevelType levelBtnType;
    public PlayerManager playerManager;

    public TextMeshProUGUI lb_title;
    public TextMeshProUGUI lb_level;
    public TextMeshProUGUI lb_price;

    [SerializeField] private ParticleSystem particle;

    private string prefKey;


    // Start is called before the first frame update
    void Start()
    {
    }

    public void InitBtn()
    {
        level = 1;
        switch (levelBtnType)
        {
            case PlayerManager.PlayerLevelType.Power:
                prefKey = GameManager.KEY_SAVE_Power;
                break;
            case PlayerManager.PlayerLevelType.Meet:
                prefKey = GameManager.KEY_SAVE_Meet;
                break;
            case PlayerManager.PlayerLevelType.Traning:
                prefKey = GameManager.KEY_SAVE_Traning;
                break;
            default:
                throw new System.Exception("Save key not found");
        }

        if (PlayerPrefs.HasKey(prefKey))
        {
            level = PlayerPrefs.GetInt(prefKey);
        }

        switch (levelBtnType)
        {
            case PlayerManager.PlayerLevelType.Power:
                playerManager.powerLv = level;
                break;
            case PlayerManager.PlayerLevelType.Meet:
                playerManager.meetLv = level;
                break;
            case PlayerManager.PlayerLevelType.Traning:
                playerManager.traningLv = level;
                break;
            default:
                throw new System.Exception("Save key not found");
        }

        int price = gameManager.GetBonusPrice(level);
        UpdateLabel(level, price);
    }

    public void OnBtnClick()
    {
        if (level < 9999)
        {
            var price = gameManager.GetBonusPrice(level);
            if (!gameManager.CoinPayment(price)) return;
            //StartCoroutine(TapEffectActive());
            particle.Stop();
            particle.Play();
            //level++;

            level = gameManager.PlayerLevelUp(levelBtnType);
            var nextPrice = gameManager.GetBonusPrice(level);
            PlayerPrefs.SetInt(prefKey, level); 
            PlayerPrefs.Save();

            UpdateLabel(level, nextPrice);
        }
    }

    private void UpdateLabel(int level, int price)
    {
        //lb_title.text = levelBtnType.ToString() + "\r\nLv:" + level.ToString();
        Debug.Log(level);
        lb_level.text = "Lv." + level;
        lb_price.text = "<sprite=0>" + price.ToString();
    }
}
