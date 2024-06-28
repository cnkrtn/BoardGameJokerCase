using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DiceUI : MonoBehaviour
{
    [Header("States")]
    public bool isContactWithFloor;
    public bool isContactWithDice;
    public bool isInSimulation = true;
    public bool isNotMoving = false;
    public bool isTextureLit = false;

    [Header("References")]
    public DiceRotationData diceData;
    public Dice diceLogic;
    public MeshRenderer meshRenderer;
    public AudioSource soundLit;

    [FormerlySerializedAs("soundCollideFloor")]
    public AudioSource soundCollideFloor;

    [FormerlySerializedAs("soundCollideDice")]
    public AudioSource soundCollideDice;

    /// <summary>
    /// For a possible object pooling system,
    /// we could reset the dice back and reuse it again
    /// </summary>
    public void Reset()
    {
        
        isContactWithFloor = false;
        isContactWithDice = false;
        isInSimulation = true;
        isNotMoving = false;
        isTextureLit = false;
    }

//     public void ShowDiceResult()
//     {
//         if (isTextureLit == false)
//         {
// //            soundLit.Play();
//             isTextureLit = true;
//         }
//     }

   

    #region Audio-Related Functions
    //This is to help the Animation Recorder capture the event
    //when the sound should be played
    public void PlaySoundRollLow()
    {
        
             AudioManager.Instance.PlaySound("DiceToFloor");
    }

    public void PlaySoundRollHigh()
    {
      
            AudioManager.Instance.PlaySound("DiceToDice");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Floor"))
        {
            isContactWithFloor = true;
        }

        if (collision.transform.CompareTag("Dice"))
        {
            isContactWithDice = true;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.CompareTag("Floor"))
        {
            isContactWithFloor = false;
        }

        if (collision.transform.CompareTag("Dice"))
        {
            isContactWithDice = false;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.CompareTag("Floor"))
        {
            isContactWithFloor = false;
        }

        if (collision.transform.CompareTag("Dice"))
        {
            isContactWithDice = false;
        }
    }
    #endregion
}
