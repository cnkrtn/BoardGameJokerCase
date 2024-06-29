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
        AudioManager.Instance.PlaySound("WalkEnd");
        var gameObject = GridManager.Instance.finalPathGameObjects[currentGridIndex];
        var animator = gameObject.transform.GetChild(0).GetComponent<Animator>();

        if (animator == null) return;
        animationCamera.Priority = 11;
        animator.Play("Play");
        StartCoroutine(WaitForAnimationEnd(animator, "Play"));
       

        var index = GridManager.Instance.finalPathGameObjects[currentGridIndex].GetComponent<GridObject>().tileTypeIndex;
        
        
        switch (index)
        {
            case 1:
                AudioManager.Instance.PlaySound("AppleWin");
                playerAnimator.Play("Win");
                break;
            case 2:
                AudioManager.Instance.PlaySound("StrawberryWin");
                playerAnimator.Play("Win");
                break;
            case 3:
                AudioManager.Instance.PlaySound("PearWin");
                playerAnimator.Play("Win");
                break;
            case 4:
                AudioManager.Instance.PlaySound("AppleLose");
                playerAnimator.Play("Lose");
                break;
            case 5:
                AudioManager.Instance.PlaySound("StrawberryLose");
                playerAnimator.Play("Lose");
                
                break;
            case 6:
                AudioManager.Instance.PlaySound("PearLose");
                playerAnimator.Play("Lose");
                break;
            case 7:
                AudioManager.Instance.PlaySound("Market");
                playerAnimator.Play("Win");
                break;
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
