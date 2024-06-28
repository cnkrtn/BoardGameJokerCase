using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public DiceManager diceManager;
    public TMP_Dropdown diceAmountDropdown;
    public List<TMP_InputField> targetResultInputFields;
    [SerializeField] private List<GameObject> dicePanels;
    [SerializeField] private RectTransform foregroundWoodDicePanelParent, backgroundWoodDicePanelParent;

    private void Start()
    {
        
        diceAmountDropdown.onValueChanged.AddListener(OnDiceAmountChanged);
    }

    public void OnDiceAmountChanged(int value)
    {
        int amount = value + 1; // Dropdown index starts from 0
        diceManager.generateAmount = amount;

        // Enable/Disable InputFields based on the amount
        for (int i = 0; i < targetResultInputFields.Count; i++)
        {
            targetResultInputFields[i].gameObject.SetActive(i < amount);
        }

        // Deactivate all panels first
        foreach (var panel in dicePanels)
        {
            panel.SetActive(false);
        }

        // Activate panels up to the selected value
        for (int i = 0; i < amount && i < dicePanels.Count; i++)
        {
            dicePanels[i].SetActive(true);
        }

        // Adjust the size of the targetedResult list
        if (diceManager.targetedResult.Count > amount)
        {
            diceManager.targetedResult.RemoveRange(amount, diceManager.targetedResult.Count - amount);
        }
        else
        {
            for (int i = diceManager.targetedResult.Count; i < amount; i++)
            {
                diceManager.targetedResult.Add(DiceManager.Elements.Any); // Default to any face if not set
            }
        }

        // Generate or remove dice objects based on the new amount without animating them
        diceManager.GenerateDice(amount, false);

        // Force rebuild the layout
        LayoutRebuilder.ForceRebuildLayoutImmediate(backgroundWoodDicePanelParent);
        LayoutRebuilder.ForceRebuildLayoutImmediate(foregroundWoodDicePanelParent);
    }





    public void UpdateTargetedResult(int index)
    {
        string inputText = targetResultInputFields[index].text.Trim();
        DiceManager.Elements newTarget;

        if (int.TryParse(inputText, out int faceValue) && faceValue >= 1 && faceValue <= 6)
        {
            newTarget = (DiceManager.Elements)(faceValue - 1);
        }
        else
        {
            newTarget = DiceManager.Elements.Any; // Default to any face if input is invalid
        }

        // Set the desired face for the selected dice
        if (index < diceManager.targetedResult.Count)
        {
            diceManager.targetedResult[index] = newTarget;
        }
        else
        {
            // Ensure the targetedResult list is correctly sized
            for (int i = diceManager.targetedResult.Count; i <= index; i++)
            {
                diceManager.targetedResult.Add(DiceManager.Elements.Any); // Default to any face if not set
            }
            diceManager.targetedResult[index] = newTarget;
        }
    }
}
