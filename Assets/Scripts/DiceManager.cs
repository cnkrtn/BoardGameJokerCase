using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceManager : MonoBehaviour
{
    [SerializeField]private Rigidbody rb;
    [SerializeField] private GameObject[] faceDetectors;
    private void Start()
    {
        SetInitialState();
    }

    private void Update()
    {
        if (CheckObjectHasStopped() == true)
        {
            var index = FindFaceResult();
        }
    }

    private object FindFaceResult()
    {
        int maxIndex = 0;
        for (int i = 1; i < faceDetectors.Length; i++)
        {
            if (faceDetectors[maxIndex].transform.position.y < faceDetectors[i].transform.position.y)
            {
                maxIndex = i;
            }
        }

        return maxIndex;
    }

    private bool CheckObjectHasStopped()
    {
        return rb.velocity == Vector3.zero && rb.angularVelocity == Vector3.zero;
    }
    private void SetInitialState()
    {
        int x = Random.Range(0, 360);
        int y = Random.Range(0, 360);
        int z = Random.Range(0, 360);
        Quaternion rotation = Quaternion.Euler(x, y, z);

        x = Random.Range(0, 25);
        y = Random.Range(0, 25);
        z = Random.Range(0, 25);
        Vector3 force = new Vector3(-x, -y, z);

        x = Random.Range(0, 50);
        y = Random.Range(0, 50);
        z = Random.Range(0, 50);
        Vector3 torque = new Vector3(x, y, z);

        transform.rotation = rotation;
        rb.velocity = force;

        // By default Max Angular Velocity is capped at 7
        this.rb.maxAngularVelocity = 1000;
        rb.AddTorque(torque, ForceMode.VelocityChange);
    }

}
