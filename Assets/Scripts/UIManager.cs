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
    [SerializeField] private RectTransform shopPanel;
    [SerializeField] private List<Sprite> buttonSprites;
    [SerializeField] private List<Button> sellButtons;
    [SerializeField] private List<Button> buyButtons;
    [SerializeField] private List<Button> barterButtons;
    [SerializeField] private Button rollButton;
    [SerializeField] private Slider musicSlider, soundSlider;
    private int _valueToRise=1, _valueToSink=5;
    public RectTransform targetPosition;
    public float lerpDurationMoreDicePanel = 1f;
    private bool isGoingDown;
    private Vector3 initialPosition;

    private void OnEnable()
    {
   
        EventManager.OnStoppedOnACell += OnStoppedOnACell;
    }

    
    private void OnDisable()
    {
        
        EventManager.OnStoppedOnACell -= OnStoppedOnACell;
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
    private void OnStoppedOnACell(int fruitCount, int tileType,int currentGridIndex)
    {
        ActivateDeactivateRollButton();
        if (tileType == 7)
        {
            if (dicePanel.gameObject.activeSelf)
            {
                dicePanel.gameObject.SetActive(false);
            }

            OpenPanel(shopPanel);
        }
        int previousCount;
        int newCount;
        TextMeshProUGUI targetText;
        RectTransform transform;
        switch (tileType)
        {
            case 1:
                previousCount = DataManager.Instance.appleCount;
                targetText = amountTexts[0];
                transform = amountTexts[0].rectTransform;
                newCount = previousCount + fruitCount;
                DataManager.Instance.appleCount = newCount;
                break;
            case 5:
                previousCount = DataManager.Instance.appleCount;
                targetText = amountTexts[0];
                transform = amountTexts[0].rectTransform;
                newCount = previousCount - fruitCount;
                DataManager.Instance.appleCount = newCount;
                break;
            case 2:
                previousCount = DataManager.Instance.strawberryCount;
                targetText = amountTexts[1];
                transform = amountTexts[1].rectTransform;
                newCount = previousCount + fruitCount;
                DataManager.Instance.strawberryCount = newCount;
                break;
            case 6:
                previousCount = DataManager.Instance.strawberryCount;
                targetText = amountTexts[1];
                transform = amountTexts[1].rectTransform;
                newCount = previousCount - fruitCount;
                DataManager.Instance.strawberryCount = newCount;
                break;
            case 3:
                previousCount = DataManager.Instance.pearCount;
                targetText = amountTexts[2];
                transform = amountTexts[2].rectTransform;
                newCount = previousCount + fruitCount;
                DataManager.Instance.pearCount = newCount;
                break;
            case 4:
                previousCount = DataManager.Instance.pearCount;
                targetText = amountTexts[2];
                transform = amountTexts[2].rectTransform;
                newCount = previousCount - fruitCount;
                DataManager.Instance.pearCount = newCount;
                break;
            default:
                return;
        }

        StartCoroutine(CountUpText(targetText, previousCount, newCount, 0.5f));
        StartCoroutine(LerpHelper.LerpScaleYoyo(transform, Vector3.one, 1.5f * Vector3.one, 1f));
        
    }

    public void OpenPanel(RectTransform panel)
    {
        panel.localScale = Vector3.zero;
        panel.gameObject.SetActive(true);
        StartCoroutine(LerpHelper.LerpScale(panel, panel.localScale, Vector3.one * 0.8f, .2f,
            LerpHelper.GetEasingFunction(EasingFunctionType.Linear)));
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

// Shop related methods
    public void FruitsToSellButtons(int buttonIndex)
    {
        Color newColor;
        switch (buttonIndex)
        {
            case 0:
                if (ColorUtility.TryParseHtmlString("#FFE697", out newColor))
                {
                    sellButtons[1].image.color = newColor;
                    sellButtons[2].image.color = newColor;
                    sellButtons[0].image.color = Color.white;
                    sellButtons[0].transform.GetChild(1).gameObject.SetActive(true);
                    sellButtons[1].transform.GetChild(1).gameObject.SetActive(false);
                    sellButtons[2].transform.GetChild(1).gameObject.SetActive(false);
                    _valueToRise = 1;
                    _valueToSink = 5;
                }
               
                break;
            case 1 :  
                if (ColorUtility.TryParseHtmlString("#FFE697", out newColor))
                {
                    sellButtons[0].image.color = newColor;
                    sellButtons[2].image.color = newColor;
                    sellButtons[1].image.color = Color.white;
                    sellButtons[0].transform.GetChild(1).gameObject.SetActive(false);
                    sellButtons[1].transform.GetChild(1).gameObject.SetActive(true);
                    sellButtons[2].transform.GetChild(1).gameObject.SetActive(false);
                    _valueToRise = 5;
                    _valueToSink = 1;
                }
                break;
            case 2 :  
                if (ColorUtility.TryParseHtmlString("#FFE697", out newColor))
                {
                    sellButtons[1].image.color = newColor;
                    sellButtons[0].image.color = newColor;
                    sellButtons[2].image.color = Color.white;
                    sellButtons[0].transform.GetChild(1).gameObject.SetActive(false);
                    sellButtons[1].transform.GetChild(1).gameObject.SetActive(false);
                    sellButtons[2].transform.GetChild(1).gameObject.SetActive(true);
                    _valueToRise = 1;
                    _valueToSink = 3;
                }
                break;
            
        }
        
      
    }

    public void ChangeFruitToBuy(int index)
    {
        switch (index)
        {
            case 0:
                if (buyButtons[0].image.sprite == buttonSprites[1]) return;
                SetButtonSprite(0, 1, 3);
                SetButtonSprite(1, 0, 2);
                _valueToRise = 1;
                break;

            case 1:

                if (buyButtons[1].image.sprite == buttonSprites[1]) return;
                SetButtonSprite(1, 1, 3);
                SetButtonSprite(0, 0, 2);
                _valueToRise = 3;
                break;
            case 2:
                if (buyButtons[2].image.sprite == buttonSprites[1]) return;
                SetButtonSprite(2, 1, 3);
                SetButtonSprite(3, 0, 2);
                break;
                _valueToRise = 5;

            case 3:

                if (buyButtons[3].image.sprite == buttonSprites[1]) return;
                SetButtonSprite(3, 1, 3);
                SetButtonSprite(2, 0, 2);
                _valueToRise = 4;
                break;
            case 4:
                if (buyButtons[4].image.sprite == buttonSprites[1]) return;
                SetButtonSprite(4, 1, 3);
                SetButtonSprite(5, 0, 2);
                _valueToRise = 1;
                break;

            case 5:
                _valueToRise = 5;
                if (buyButtons[5].image.sprite == buttonSprites[1]) return;
                SetButtonSprite(5, 1, 3);
                SetButtonSprite(4, 0, 2);
                break;
        }
        
    }
    private void SetButtonSprite(int buttonIndex, int defaultSpriteIndex, int pressedSpriteIndex)
    {
        buyButtons[buttonIndex].image.sprite = buttonSprites[defaultSpriteIndex];
    
        // Create a new SpriteState and set its properties
        var spriteState = new SpriteState
        {
            pressedSprite = buttonSprites[pressedSpriteIndex]
        };

        // Assign the new SpriteState back to the button
        buyButtons[buttonIndex].spriteState = spriteState;
    }

    public void BarterButtons(int index)
    {
        int previousCountR = 0, previousCountS = 0;
        int newCountR = 0, newCountS = 0;
        TextMeshProUGUI targetTextR = null, targetTextS = null;
        RectTransform transformR = null, transformS = null;
        
        switch (index)
        {
            case 0:
                previousCountS = DataManager.Instance.appleCount;
                targetTextS = amountTexts[0];
                transformS = amountTexts[0].rectTransform;
                newCountS = DataManager.Instance.appleCount -= _valueToSink;
                DataManager.Instance.appleCount = newCountS;

                if (_valueToRise == 1)
                {
                    previousCountR = DataManager.Instance.strawberryCount;
                    targetTextR = amountTexts[1];
                    transformR = amountTexts[1].rectTransform;
                    newCountR = DataManager.Instance.strawberryCount += _valueToRise;
                    DataManager.Instance.strawberryCount = newCountR;
                }
                else
                {
                    previousCountR = DataManager.Instance.pearCount;
                    targetTextR = amountTexts[2];
                    transformR = amountTexts[2].rectTransform;
                    newCountR = DataManager.Instance.pearCount += _valueToRise;
                    DataManager.Instance.pearCount = newCountR;
                }
                
                break;
            case 1:
                previousCountS = DataManager.Instance.strawberryCount;
                targetTextS = amountTexts[1];
                transformS = amountTexts[1].rectTransform;
                newCountS = DataManager.Instance.strawberryCount -= _valueToSink;
                DataManager.Instance.strawberryCount = newCountS;

                if (_valueToRise == 5)
                {
                    previousCountR = DataManager.Instance.appleCount;
                    targetTextR = amountTexts[0];
                    transformR = amountTexts[0].rectTransform;
                    newCountR = DataManager.Instance.appleCount += _valueToRise;
                    DataManager.Instance.appleCount = newCountR;
                }
                else
                {
                    previousCountR = DataManager.Instance.pearCount;
                    targetTextR = amountTexts[2];
                    transformR = amountTexts[2].rectTransform;
                    newCountR = DataManager.Instance.pearCount += _valueToRise;
                    DataManager.Instance.pearCount = newCountR;
                }
                
                break;
            case 2:
                previousCountS = DataManager.Instance.pearCount;
                targetTextS = amountTexts[2];
                transformS = amountTexts[2].rectTransform;
                newCountS = DataManager.Instance.pearCount -= _valueToSink;
                DataManager.Instance.pearCount = newCountS;

                if (_valueToRise == 1)
                {
                    previousCountR = DataManager.Instance.strawberryCount;
                    targetTextR = amountTexts[1];
                    transformR = amountTexts[1].rectTransform;
                    newCountR = DataManager.Instance.strawberryCount += _valueToRise;
                    DataManager.Instance.strawberryCount = newCountR;
                }
                else
                {
                    previousCountR = DataManager.Instance.appleCount;
                    targetTextR = amountTexts[0];
                    transformR = amountTexts[0].rectTransform;
                    newCountR = DataManager.Instance.appleCount += _valueToRise;
                    DataManager.Instance.appleCount = newCountR;
                }
                
                break;
          
          
            
        } 
        StartCoroutine(CountUpText(targetTextR, previousCountR, newCountR, 0.5f));
        StartCoroutine(CountUpText(targetTextS, previousCountS, newCountS, 0.5f));
        StartCoroutine(LerpHelper.LerpScaleYoyo(transformR, Vector3.one, 1.5f * Vector3.one, 1f));
        StartCoroutine(LerpHelper.LerpScaleYoyo(transformS, Vector3.one, 1.5f * Vector3.one, 1f));
    }

    public void CloseDown(RectTransform panel)
    {
        panel.localScale = Vector3.one * .8f;
        StartCoroutine(LerpHelper.LerpScale(panel, panel.localScale, Vector3.zero, .2f,
            LerpHelper.GetEasingFunction(EasingFunctionType.Linear)));
    }

    public void ActivateDeactivateRollButton()
    {
        if (rollButton.interactable != !rollButton.interactable)
        {
            rollButton.interactable = !rollButton.interactable;
        }
    }
    
    public void ToggleMusic(){AudioManager.Instance.ToggleMusic();}
    public void ToggleSound(){AudioManager.Instance.ToggleSfx();}
    public void MusicVolume(){AudioManager.Instance.MusicVolume(musicSlider.value);}
    public void SfxVolume(){AudioManager.Instance.SfxVolume(soundSlider.value);}

}
