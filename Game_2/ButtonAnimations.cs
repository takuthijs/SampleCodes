using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ButtonAnimations : MonoBehaviour
{
    public enum animationName{
        scale,
        hitBlink,
        popup
    }

    public animationName setAnimation;

    private DG.Tweening.Sequence _seq;

    // Start is called before the first frame update
    void Start()
    {
        switch (setAnimation)
        {
            case animationName.scale:
                ScaleAnime();
                break;
            case animationName.popup:
                PopUPAnime();
                break;
            default:
                break;
        }
    }

    public void ScaleAnime()
    {
        var sequence = DOTween.Sequence(); //Sequence生成

        sequence.Append(transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.5f))
            .Append(transform.DOScale(new Vector3(1, 1, 1), 0.5f))
            .SetLoops(-1, LoopType.Yoyo)
        ;
    }

    public void PopUPAnime()
    {
        var sequence = DOTween.Sequence(); //Sequence生成

        sequence.Append(transform.DOScale(new Vector3(0.12f, 0.12f, 0.12f), 0.2f))
            .Append(transform.DOScale(new Vector3(0.1f, 0.1f, 0.1f), 0.5f))
            .SetLoops(-1, LoopType.Yoyo)
        ;
    }
}
