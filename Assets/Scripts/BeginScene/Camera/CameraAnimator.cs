using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraAnimator : MonoBehaviour
{
    private Animator animator;

    private UnityAction onAnimationComplete;
    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
    }

    public void TurnLeft(UnityAction onComplete)
    {
        animator.SetTrigger("Left");
        onAnimationComplete = onComplete;
    }

    public void TurnRight(UnityAction onComplete)
    {
        animator.SetTrigger("Right");
        onAnimationComplete = onComplete;
    }

    public void OnAnimationComplete()
    {
        onAnimationComplete?.Invoke();
        onAnimationComplete = null;
    }

}
