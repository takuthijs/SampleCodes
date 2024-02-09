using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class CanvasGroupAnimation : MonoBehaviour, IPointerUpHandler
{
    public CanvasGroup canvasGroup; // CanvasGroupの参照
    private Sequence sequence;

    void Start()
    {
        // 関数の呼び出し例
        sequence = DOTween.Sequence();
        StartAlphaAnimation();
    }

    void StartAlphaAnimation()
    {
        // CanvasGroupがnullでないことを確認
        if (canvasGroup != null)
        {
            // DoTweenでアニメーションを作成
            sequence.Append(canvasGroup.DOFade(1, 2.0f)); // alphaを1に変化
            sequence.Append(canvasGroup.DOFade(0, 2.0f)); // alphaを0に変化

            // ループ設定（無限ループ）
            sequence.SetLoops(-1, LoopType.Restart);
        }
        else
        {
            Debug.LogWarning("CanvasGroup is not assigned.");
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        sequence.Kill();
    }
}
