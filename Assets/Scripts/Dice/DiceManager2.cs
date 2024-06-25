using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceManager2 : MonoBehaviour
{
    public GameObject dicePrefab;
    public AnimationRecorder animRecorder;
    public int generateAmount = 1; // Set to 1 for single dice testing
    public List<Elements> targetedResult;
    public List<DiceData> diceDataList;

    private void Start()
    {
        InitializeTargetedResult();
    }

    private void InitializeTargetedResult()
    {
        if (targetedResult == null)
        {
            targetedResult = new List<Elements>();
        }

        // Ensure targetedResult has the same number of elements as generateAmount
        for (int i = targetedResult.Count; i < generateAmount; i++)
        {
            targetedResult.Add(Elements.Any); // Default to any face if not set
        }
    }

    public void ThrowTheDice()
    {
        InitializeTargetedResult(); // Ensure targetedResult is initialized

        GenerateDice(generateAmount);

        // Generate list of dices, then put it into the simulation
        List<GameObject> diceList = new List<GameObject>();
        for (int i = 0; i < generateAmount; i++)
        {
            diceList.Add(diceDataList[i].diceObject);
        }
        animRecorder.StartSimulation(diceList);

        // Log positions after the simulation to verify face detectors
        for (int i = 0; i < generateAmount; i++)
        {
            int simulationResult = diceDataList[i].diceLogic.FindFaceResult(); // Ensure face result is found
            Debug.Log($"Simulation result for dice {i}: {simulationResult}");
        }

        // Reset to initial state
        animRecorder.ResetToInitialState();

        // Rotate to the desired face
        for (int i = 0; i < generateAmount; i++)
        {
            // Ensure the dice has a valid default face result before rotating
            if (diceDataList[i].diceLogic.defaultFaceResult == -1)
            {
                diceDataList[i].diceLogic.FindFaceResult();
            }
            diceDataList[i].diceLogic.RotateDice((int)targetedResult[i]);
        }

        // Play the recorded animation
        animRecorder.PlayRecording();
    }

    private void GenerateDice(int count)
    {
        Physics.autoSimulation = false; // Temporarily disable physics

        if (count > diceDataList.Count)
        {
            int diceToGenerate = count - diceDataList.Count;
            for (int i = 0; i < diceToGenerate; i++)
            {
                DiceData newDiceData = new DiceData(Instantiate(dicePrefab));
                diceDataList.Add(newDiceData);
            }
        }
        else if (count < diceDataList.Count)
        {
            int diceToDispose = diceDataList.Count - count;
            for (int i = diceDataList.Count - diceToDispose - 1; i < diceDataList.Count; i++)
            {
                diceDataList[i].diceObject.transform.position = Vector3.down * 10000;
            }
        }

        for (int i = 0; i < count; i++)
        {
            InitialState initial = SetInitialState();

            diceDataList[i].diceLogic.ResetDice();
            diceDataList[i].diceUI.Reset();
            diceDataList[i].diceObject.transform.position = initial.position;
            diceDataList[i].diceObject.transform.rotation = Quaternion.identity; // Ensure no initial rotation
            diceDataList[i].rb.useGravity = true;
            diceDataList[i].rb.isKinematic = false;
            diceDataList[i].rb.velocity = initial.force;
            diceDataList[i].rb.angularVelocity = initial.torque; // Use angular velocity instead of AddTorque
            Debug.Log($"Generated dice at position: {initial.position}, with force: {initial.force} and torque: {initial.torque}");
        }

        Physics.autoSimulation = true; // Re-enable physics
    }

    private InitialState SetInitialState()
    {
        float x = transform.position.x + Random.Range(-transform.localScale.x / 2, transform.localScale.x / 2);
        float y = transform.position.y + 2.0f; // Start 2 units above ground to ensure it falls
        float z = transform.position.z + Random.Range(-transform.localScale.z / 2, transform.localScale.z / 2);
        Vector3 position = new Vector3(x, y, z);

        Quaternion rotation = Quaternion.identity; // No initial rotation

        x = Random.Range(0, 25);
        y = Random.Range(0, 25);
        z = Random.Range(0, 25);
        Vector3 force = new Vector3(x, -y, z);

        x = Random.Range(0, 50);
        y = Random.Range(0, 50);
        z = Random.Range(0, 50);
        Vector3 torque = new Vector3(x, y, z);

        return new InitialState(position, rotation, force, torque);
    }

    [System.Serializable]
    public struct DiceData
    {
        public GameObject diceObject;
        public Rigidbody rb;
        public Dice diceLogic;
        public DiceUI diceUI;

        public DiceData(GameObject diceObject)
        {
            this.diceObject = diceObject;
            this.rb = diceObject.GetComponent<Rigidbody>();
            this.diceLogic = diceObject.transform.GetChild(0).GetComponent<Dice>();
            this.diceUI = diceObject.GetComponent<DiceUI>();
            this.rb.maxAngularVelocity = 1000;
        }
    }

    [System.Serializable]
    public struct InitialState
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 force;
        public Vector3 torque;

        public InitialState(Vector3 position, Quaternion rotation, Vector3 force, Vector3 torque)
        {
            this.position = position;
            this.rotation = rotation;
            this.force = force;
            this.torque = torque;
        }
    }
}

public enum Elements
{
    One, Two, Three, Four, Five, Six, Any
}
