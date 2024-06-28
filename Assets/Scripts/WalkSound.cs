using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkSound : MonoBehaviour
{
    public void WalkSoundSetter()
    {
        AudioManager.Instance.PlaySound("Walk");
    }
}
