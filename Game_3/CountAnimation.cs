using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class CountAnimation : MonoBehaviour
{
    public GameObject textObj;
    public TextMeshProUGUI text;
    public Vector3 defaultScale = new Vector3(1, 1, 1);
    private Color defaultColor = new Color(0.038f, 1, 1, 1);
    private Color redColor = new Color(1, 0, 0, 1);
    private string previousText; // 前回のテキストを保存

    void Start()
    {
        // 最初のテキストを取得して保存
        previousText = text.text;
    }

    void Update()
    {
        if(GameManager.instance.battleManager.isRunning)
        {
            textObj.transform.localScale -= new Vector3(0.002f, 0.002f, 0.002f);
            text.color -= new Color(0, 0, 0, 0.01f);
        }

        // テキストが前回の値と異なるかを確認
        if (text.text != previousText)
        {
            // テキストが変更されたらスケールをリセット
            textObj.transform.localScale = defaultScale;
            text.color = defaultColor;
            if (new[] { "0", "1", "2", "3" }.Contains(text.text))
            {
                text.color = redColor;
            }

            // テキストが変更されたら前回のテキストを更新
            previousText = text.text;
            //Todo音を鳴らす処理も入れたい！！！
        }
    }
}
