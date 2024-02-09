using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnablePlayAnimation : MonoBehaviour
{
    public Animator animator;
    public string playAnimationName;

    private void OnEnable()
    {
        animator.Play(playAnimationName, 0, 0);
    }
}
