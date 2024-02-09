using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchPlayAnimation : MonoBehaviour, IPointerClickHandler
{
    //タップされた時の処理
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        Animator animator = GameManager.instance.selectBeetleParent.transform.GetChild(0)?.gameObject.GetComponent<Animator>();
        if (animator != null)
        {
            //Debug.Log("null判定");
            foreach (var clip in animator.GetCurrentAnimatorClipInfo(0))
            {
                if (clip.clip.name.Contains("Wait"))
                {
                    animator?.Play("WingOpen", 0, 0);
                    break;
                }
            }
        }
        
    }
}
