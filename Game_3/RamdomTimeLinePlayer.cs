using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class RandomTimelinePlayer : MonoBehaviour
{
    public List<PlayableDirector> inputAnimations;
    int randomIndex = 0;
    private bool isPlaying = false;
    private float nextPlayTime = 0f;
    private float playInterval = 2f; // 再生間隔（2~3秒）

    void Start()
    {
        // 最初の再生を設定
        SetNextPlayTime();
    }

    void Update()
    {
        if (!isPlaying && (GameManager.instance.battleManager.battleStatus == BattleManager.BattleStatus.wait || GameManager.instance.battleManager.battleStatus == BattleManager.BattleStatus.waitInput))
        {
            // 次の再生時間に達したらランダムなTimelineを再生
            if (Time.time >= nextPlayTime)
            {
                PlayRandomTimeline();
                SetNextPlayTime();
            }
        }
    }

    void PlayRandomTimeline()
    {
        if (inputAnimations.Count > 0)
        {
            int RamdomNum = Random.Range(0, inputAnimations.Count);
            // ランダムにPlayableDirectorを選択
            while (randomIndex == RamdomNum)
            {
                RamdomNum = Random.Range(0, inputAnimations.Count);
                if (randomIndex != RamdomNum) break;
            }

            randomIndex = RamdomNum;
            PlayableDirector selectedDirector = inputAnimations[randomIndex];
            GameManager.instance.battleManager.inputAnimations = selectedDirector;

            // Timeline再生
            selectedDirector.Play();
            isPlaying = true;

            // Timelineが終了したら再生状態をリセット
            StartCoroutine(WaitForTimelineCompletion(selectedDirector));
        }
    }

    void SetNextPlayTime()
    {
        // 再生間隔を2~3秒の範囲でランダムに設定
        playInterval = Random.Range(2f, 2.3f);
        nextPlayTime = Time.time + playInterval;
    }

    IEnumerator WaitForTimelineCompletion(PlayableDirector director)
    {
        yield return new WaitForSeconds((float)director.duration);

        // 再生が完了したら再生状態をリセット
        isPlaying = false;
    }
}
