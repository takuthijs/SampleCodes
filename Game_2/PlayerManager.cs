using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int powerLv = 1;
    public int meetLv = 1;
    public int traningLv = 1;

    public GameManager gameManager;

    public float autoInterval = 100;//sec
    private float coolTime = 100;

    public Animator playerAnimator;
    public PlayerDoing playerDoing = PlayerDoing.HomeTraning;



    public enum PlayerDoing
    {
        HomeTraning,
        Gaming,
    }

    public enum PlayerLevelType
    {
        Power,
        Meet,
        Traning
    }

    [SerializeField]
    public Vector3[] positions;
    public Vector3[] rotations;

    // Start is called before the first frame update
    void Start()
    {
        coolTime = autoInterval;
        playerAnimator = GetComponent<Animator>();

        SetHomeMode();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerDoing == PlayerDoing.HomeTraning)
        {
            coolTime -= Time.deltaTime;
            if (coolTime < 0)
            {
                //gameManager.OnClickTraning();
                coolTime += autoInterval;
            }
        }
    }

    public void AnimJump()
    {
        //playerAnimator.speed = speed;
        //playerAnimator.SetTrigger("jump");
        playerAnimator.Play("SwingTraning",0,0);
    }

    public int GetStatus(PlayerLevelType playerStatusType)
    {
        switch (playerStatusType)
        {
            case PlayerLevelType.Power:
                return powerLv;
            case PlayerLevelType.Meet:
                return meetLv;
            case PlayerLevelType.Traning:
                return traningLv;
        }
        return 0;
    }

    public int LevelUp(PlayerLevelType playerStatusType)
    {
        switch (playerStatusType)
        {
            case PlayerLevelType.Power :
                if(powerLv < 9999) powerLv++;
                break;
            case PlayerLevelType.Meet:
                if (meetLv < 9999) meetLv++;
                break;
            case PlayerLevelType.Traning:
                if (traningLv < 9999) traningLv++;
                break;
        }

        return GetStatus(playerStatusType);
    }

    public void SetPosition(int index)
    {
        transform.position = positions[index];
        transform.rotation = Quaternion.Euler(rotations[index].x, rotations[index].y, rotations[index].z);
    }

    public void SetHomeMode()
    {
        SetPosition(0);
        playerDoing = PlayerDoing.HomeTraning;
    }

    public void SetGameMode()
    {
        SetPosition(1);
        playerDoing = PlayerDoing.Gaming;
    }
}

