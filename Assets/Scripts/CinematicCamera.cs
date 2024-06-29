using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CinematicCamera : MonoBehaviour
{
    public CinemachineVirtualCamera cineCamera;
    public void SetStartPriority()
    {
        cineCamera.Priority = 12;
    }
    
    public void SetEndPriority()
    {
        cineCamera.Priority = 5;
    }
}
