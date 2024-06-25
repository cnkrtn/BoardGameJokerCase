using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DiceRotationData", menuName = "ScriptableObjects/DiceRotationData", order = 1)]
public class DiceRotationData : ScriptableObject
{
    public List<FaceRelativeRotation> faceRelativeRotation;
  

    [System.Serializable]
    public struct FaceRelativeRotation
    {
        public string element;
        public List<Vector3> rotation;
    }
}