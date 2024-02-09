using UnityEngine;
using TMPro;
using DG.Tweening;

public class TextFlowAnimation : MonoBehaviour
{
    //HACK このスクリプトが原因で警告が出ている
    public TextMeshProUGUI textMeshPro;
    public RectTransform rectTransform;
    private float duration = 0;
    private float textWidth = 0;
    private float screenWidth = 0;

    private Tween animationTween;

    void Start()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("GameManager");
        //GameManagerが他に存在したら処理しない
        if (objects.Length > 1) return;

        duration = textMeshPro.text.Length * 0.5f;
        screenWidth = rectTransform.sizeDelta.x;
        textWidth = textMeshPro.GetPreferredValues().x;

        if (screenWidth >= textWidth)
        {
            textWidth += screenWidth - textWidth;
        }

        textMeshPro.rectTransform.anchoredPosition = new Vector2(screenWidth, 0f);

        // Tweenを保持
        animationTween = textMeshPro.rectTransform.DOAnchorPosX(-textWidth, duration)
            .SetEase(Ease.Linear)
            .OnComplete(() => ResetTextPosition());
    }

    void ResetTextPosition()
    {
        textMeshPro.rectTransform.anchoredPosition = new Vector2(screenWidth, 0f);

        Start();
    }
}
