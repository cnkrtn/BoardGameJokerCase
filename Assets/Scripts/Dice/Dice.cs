using UnityEngine;

public class Dice : MonoBehaviour
{
    [Header("References")]
    public DiceRotationData diceData;
    public GameObject[] faceDetectors;
    public int[] faceValues = { 6, 3, 2, 5, 1, 4 }; // Mapping from indices to face values

    [Header("Debug")]
    public int defaultFaceResult = -1;
    public int alteredFaceResult = -1;

    public void ResetDice()
    {
        defaultFaceResult = -1;
        alteredFaceResult = -1;
    }

    public int FindFaceResult()
    {
        int maxIndex = 0;
        for (int i = 1; i < faceDetectors.Length; i++)
        {
            if (faceDetectors[maxIndex].transform.position.y < faceDetectors[i].transform.position.y)
            {
                maxIndex = i;
            }
        }
        defaultFaceResult = maxIndex;
        //Debug.Log($"The face that is shown: {defaultFaceResult}");
        return maxIndex;
    }

    public void RotateDice(int alteredFaceResult)
    {
        if (alteredFaceResult != 8)
        {
            this.alteredFaceResult = alteredFaceResult;
            Vector3 rotate = diceData.faceRelativeRotation[defaultFaceResult].rotation[alteredFaceResult];
            transform.Rotate(rotate);
        }
        else
        {
            this.alteredFaceResult = defaultFaceResult;
        }
    }
}