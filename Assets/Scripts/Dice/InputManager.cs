using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public DiceManager2 diceManager;
    public TMP_InputField diceInputField;
    public Button setFaceButton;

    private void Start()
    {
        setFaceButton.onClick.AddListener(UpdateTargetedResult);
    }

    public void UpdateTargetedResult()
    {
        int faceValue;
        if (int.TryParse(diceInputField.text, out faceValue) && faceValue >= 1 && faceValue <= 6)
        {
            Elements newTarget = (Elements)(faceValue - 1);
            List<Elements> newTargetedResults = new List<Elements>();

            // Set the desired face for each dice (assuming all dice should show the same face)
            for (int i = 0; i < diceManager.generateAmount; i++)
            {
                newTargetedResults.Add(newTarget);
            }

            diceManager.targetedResult = newTargetedResults;
        }
        else
        {
            Debug.LogError("Invalid input: " + diceInputField.text);
        }
    }
}