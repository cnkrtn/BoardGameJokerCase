using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private RectTransform dicePanel;
    public RectTransform targetPosition;
    public float lerpDuration = 1f;
    public EasingFunctionType easingFunctionType;
    private bool isGoingDown;
    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = dicePanel.localPosition;
        
    }
    
    public void MoreDiceButtonPressed()
    {
        if (!isGoingDown)
        {
            Func<float, float> easingFunction = LerpHelper.GetEasingFunction(easingFunctionType);
            StartCoroutine(LerpHelper.LerpPosition(dicePanel, initialPosition, targetPosition, lerpDuration, easingFunction));
            isGoingDown = !isGoingDown;
        }else if (isGoingDown)
        {
            
            Func<float, float> easingFunction = LerpHelper.GetEasingFunction(easingFunctionType);
            StartCoroutine(LerpHelper.LerpPosition(dicePanel, targetPosition.localPosition, initialPosition, lerpDuration, easingFunction));
            isGoingDown = !isGoingDown;
        }
       
    }
}
