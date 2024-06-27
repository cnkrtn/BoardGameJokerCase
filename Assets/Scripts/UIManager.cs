using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private RectTransform dicePanel;
    [SerializeField] private TextMeshProUGUI[] amountTexts;
    public RectTransform targetPosition;
    public float lerpDurationMoreDicePanel = 1f;
    private bool isGoingDown;
    private Vector3 initialPosition;

    private void OnEnable()
    {
   
        EventManager.OnStoppedOnACell += TileOutcome;
    }

    
    private void OnDisable()
    {
        
        EventManager.OnStoppedOnACell -= TileOutcome;
    }
    
    
    void Start()
    {
        initialPosition = dicePanel.localPosition;
        SetLoadedValues();

    }

    private void SetLoadedValues()
    {
        amountTexts[0].text = DataManager.Instance.appleCount.ToString();
        amountTexts[1].text = DataManager.Instance.strawberryCount.ToString();
        amountTexts[2].text = DataManager.Instance.pearCount.ToString();
    }

    public void MoreDiceButtonPressed()
    {
        if (!isGoingDown)
        {
            Func<float, float> easingFunction = LerpHelper.GetEasingFunction(EasingFunctionType.Linear);
            StartCoroutine(LerpHelper.LerpPosition(dicePanel, initialPosition, targetPosition, lerpDurationMoreDicePanel, easingFunction));
            isGoingDown = !isGoingDown;
        }else if (isGoingDown)
        {
            
            Func<float, float> easingFunction = LerpHelper.GetEasingFunction(EasingFunctionType.Linear);
            StartCoroutine(LerpHelper.LerpPosition(dicePanel, targetPosition.localPosition, initialPosition, lerpDurationMoreDicePanel, easingFunction));
            isGoingDown = !isGoingDown;
        }
       
    }
    private void TileOutcome(int fruitCount, int tileType)
    {
        int previousCount;
        TextMeshProUGUI targetText;
        RectTransform transform;
        switch (tileType)
        {
            case 1:
                previousCount = DataManager.Instance.appleCount;
                targetText = amountTexts[0];
               transform = amountTexts[0].rectTransform;
                break;
            case 2:
                previousCount = DataManager.Instance.strawberryCount;
                targetText = amountTexts[1];
                 transform = amountTexts[1].rectTransform;
                break;
            case 3:
                previousCount = DataManager.Instance.pearCount;
                transform = amountTexts[2].rectTransform;
                targetText = amountTexts[2];
                break;
            default:
                return;
        }

        int newCount = previousCount + fruitCount;
        StartCoroutine(CountUpText(targetText, previousCount, newCount, .5f));
        StartCoroutine(LerpHelper.LerpScaleYoyo(transform,Vector3.one, 1.5f*Vector3.one,1f));
        
    }

    
    private IEnumerator CountUpText(TextMeshProUGUI textComponent, int startValue, int endValue, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            int currentValue = Mathf.RoundToInt(Mathf.Lerp(startValue, endValue, t));
            textComponent.text = currentValue.ToString();
            yield return null;
        }
        textComponent.text = endValue.ToString();
    }

}
