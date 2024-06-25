using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Animator playerAnimator;
    private int _currentGridIndex, _targetIndex;
    private static readonly int IsWalking = Animator.StringToHash("isWalking");

    private void Start()
    {
        _currentGridIndex = 0;
    }

    private void OnEnable()
    {
        EventManager.OnMapCreationCompleted += OnMapCreationCompleted;
        EventManager.OnAnimationFinished += OnAnimationFinished;
    }

    private void OnDisable()
    {
        EventManager.OnMapCreationCompleted -= OnMapCreationCompleted;
        EventManager.OnAnimationFinished -= OnAnimationFinished;
    }

    private void OnAnimationFinished(int sum)
    {
        Debug.Log($"OnAnimationFinished triggered with sum: {sum}");
        FindWalkLength(sum);
    }

    private void FindWalkLength(int sum)
    {
        int finalPathCount = GridManager.Instance.finalPathGameObjects.Count;

        // Calculate new position
        _targetIndex = (_currentGridIndex + sum) % finalPathCount;
        Debug.Log($"Current Grid Position: {_currentGridIndex}, Target Grid Position: {_targetIndex}");

        // Move player to the new position
        MovePlayerToNewPosition(_targetIndex);
    }

    private IEnumerator AdjustPlayerTurns(Vector3 targetDirection)
    {
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        while (Quaternion.Angle(player.transform.rotation, targetRotation) > 0.1f)
        {
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, Time.deltaTime * 8f); // Adjust the speed factor as needed
            yield return null;
        }

        player.transform.rotation = targetRotation;
    }

    private void MovePlayerToNewPosition(int targetIndex)
    {
        Debug.Log($"MovePlayerToNewPosition called with targetIndex: {targetIndex}");
        if (_currentGridIndex != targetIndex)
        {
            StartCoroutine(MoveThroughPath(targetIndex));
        }
    }

    private IEnumerator MoveThroughPath(int targetIndex)
    {
        while (_currentGridIndex != targetIndex)
        {
            Debug.Log("Moving through path...");
            var currentCellTransform = GridManager.Instance.finalPathGameObjects[_currentGridIndex].transform;
            var nextCellTransform = GridManager.Instance
                .finalPathGameObjects[(_currentGridIndex + 1) % GridManager.Instance.finalPathGameObjects.Count]
                .transform;

            // Start moving to the next cell
            playerAnimator.SetBool(IsWalking, true);

            // Calculate the target direction for rotation
            Vector3 targetDirection = (nextCellTransform.position - currentCellTransform.position).normalized;
            yield return StartCoroutine(AdjustPlayerTurns(targetDirection));
        
            yield return StartCoroutine(LerpHelper.LerpPosition(player.transform, currentCellTransform.position, nextCellTransform.position, 2f, LerpHelper.GetEasingFunction(EasingFunctionType.EaseInOutQuad)));
            playerAnimator.SetBool(IsWalking, false);

            // Increment the current grid index
            _currentGridIndex = (_currentGridIndex + 1) % GridManager.Instance.finalPathGameObjects.Count;
        }
    }



    private void OnMapCreationCompleted()
    {
        Debug.Log("Map Creation Completed Event Triggered");
        player.SetActive(true);
        var startTransform = GridManager.Instance.finalPathGameObjects[0].transform;
         player.transform.position = startTransform.position;
        // var r0 = new Vector3(0, -90, 0);
        // player.transform.rotation = Quaternion.Euler(r0);
    }
}
