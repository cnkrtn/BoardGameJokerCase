using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject player,playerParent;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private float lerpDuration,slerpDuration;
    private int _currentGridIndex, _targetIndex;
    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    

    private void Start()
    {
        _currentGridIndex = 0;
    }

    private void OnEnable()
    {
        EventManager.OnMapCreationCompleted += OnMapCreationCompleted;
        EventManager.OnSimAnimationFinished += OnAnimationFinished;
        EventManager.OnStoppedOnACell += OnStoppedOnACell;
    }

    private void OnDisable()
    {
        EventManager.OnMapCreationCompleted -= OnMapCreationCompleted;
        EventManager.OnSimAnimationFinished -= OnAnimationFinished;
        EventManager.OnStoppedOnACell -= OnStoppedOnACell;
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
    playerAnimator.SetBool(IsWalking, true);

    while (_currentGridIndex != targetIndex)
    {
        Debug.Log("Moving through path...");
        var currentCellTransform = GridManager.Instance.finalPathGameObjects[_currentGridIndex].transform;
        var nextCellTransform = GridManager.Instance
            .finalPathGameObjects[(_currentGridIndex + 1) % GridManager.Instance.finalPathGameObjects.Count]
            .transform;

        // Calculate the target direction for rotation
        Vector3 targetDirection = (nextCellTransform.position - currentCellTransform.position).normalized;

        // Check if the player needs to adjust its turn
        Vector3 currentDirection = player.transform.forward;
        if (Vector3.Angle(currentDirection, targetDirection) > 0.1f)
        {
            yield return StartCoroutine(AdjustPlayerTurns(targetDirection));
        }

        // Move to the next cell
        yield return StartCoroutine(LerpHelper.LerpPosition(playerParent.transform, currentCellTransform.position, nextCellTransform.position, lerpDuration, LerpHelper.GetEasingFunction(EasingFunctionType.Linear)));

        // Increment the current grid index
        _currentGridIndex = (_currentGridIndex + 1) % GridManager.Instance.finalPathGameObjects.Count;
    }

    playerAnimator.SetBool(IsWalking, false);
    var gridObject = GridManager.Instance.finalPathGameObjects[_currentGridIndex].GetComponent<GridObject>();
    var fruitCount = gridObject.fruitCount;
    var tileType = gridObject.tileTypeIndex;
    EventManager.OnStoppedOnACell?.Invoke(fruitCount, tileType, _currentGridIndex);
}

private IEnumerator AdjustPlayerTurns(Vector3 targetDirection)
{
    Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

    while (Quaternion.Angle(player.transform.rotation, targetRotation) > 0.1f)
    {
        player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, Time.deltaTime * slerpDuration);
        yield return null;
    }

    player.transform.rotation = targetRotation;
}




    private void OnMapCreationCompleted()
    {
        player.SetActive(true);
        var startTransform = GridManager.Instance.finalPathGameObjects[0].transform;
         player.transform.position = startTransform.position;
        // var r0 = new Vector3(0, -90, 0);
        // player.transform.rotation = Quaternion.Euler(r0);
    }

    private void OnStoppedOnACell(int fruitCount, int tileType,int currentGridIndex)
    {
        switch (tileType)
        {
            case 1:
                DataManager.Instance.appleCount += fruitCount;
            break;
            case 2:
                DataManager.Instance.strawberryCount += fruitCount;
                break;
            case 3:
                DataManager.Instance.pearCount += fruitCount;
                break;
            case 4:
                var random = Random.Range(0, 3);
                if (random == 0)
                {
                    DataManager.Instance.appleCount -= fruitCount;
                    Debug.Log("Fruit " +  DataManager.Instance.appleCount);
                    
                }else if (random == Random.Range(0, 3))
                {
                    DataManager.Instance.strawberryCount -= fruitCount;
                    Debug.Log("Fruit " +  DataManager.Instance.strawberryCount);
                }
                else
                {
                    DataManager.Instance.pearCount -= fruitCount;
                    Debug.Log("Fruit " +  DataManager.Instance.pearCount);
                }
              
                break;
            
        }
        DataManager.Instance.SaveData();
    }
}
