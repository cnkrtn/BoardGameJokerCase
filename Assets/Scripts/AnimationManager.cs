using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera animationCamera;
    [SerializeField] private Animator playerAnimator;
    private void OnEnable()
    {
   
        EventManager.OnStoppedOnACell += OnStoppedOnACell;
    }

    
    private void OnDisable()
    {
        
        EventManager.OnStoppedOnACell -= OnStoppedOnACell;
    }

    private void OnStoppedOnACell(int arg1, int arg2, int currentGridIndex)
    {
        var gameObject = GridManager.Instance.finalPathGameObjects[currentGridIndex];
        var animator = gameObject.transform.GetChild(0).GetComponent<Animator>();

        if (animator != null)
        {
            animationCamera.Priority = 11;
            animator.Play("Play");
            StartCoroutine(WaitForAnimationEnd(animator, "Play"));
            playerAnimator.Play("Happy");
        }
        
       
    }

    private IEnumerator WaitForAnimationEnd(Animator animator, string animationName)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // Wait until the animation starts playing
        while (stateInfo.IsName(animationName) == false)
        {
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            yield return null;
        }

        // Wait until the animation is no longer playing
        while (stateInfo.IsName(animationName) && stateInfo.normalizedTime < 1.0f)
        {
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            yield return null;
        }

        // Animation has ended
        OnAnimationEnd();
    }

    private void OnAnimationEnd()
    {
        // Handle what happens after the animation ends
        Debug.Log("Animation has ended.");
        animationCamera.Priority = 9;
        playerAnimator.Play("Idle");
    }

}
