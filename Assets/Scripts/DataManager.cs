using System.IO;
using UnityEngine;

// Class to hold your data
[System.Serializable]
public class FruitData
{
    public int strawberryCount;
    public int appleCount;
    public int pearCount;
}

// Singleton class to manage your data
public class DataManager : Singleton<DataManager>
{
    public int strawberryCount, appleCount, pearCount;

    private string dataFilePath;

    private void Awake()
    {
        dataFilePath = Path.Combine(Application.persistentDataPath, "fruitData.json");
        
        LoadData();
    }

    // Method to save data to JSON
    public void SaveData()
    {
        FruitData data = new FruitData
        {
            strawberryCount = this.strawberryCount,
            appleCount = this.appleCount,
            pearCount = this.pearCount
        };

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(dataFilePath, json);
        Debug.Log("Data saved to " + dataFilePath);
    }

    // Method to load data from JSON
    public void LoadData()
    {
        if (File.Exists(dataFilePath))
        {
            string json = File.ReadAllText(dataFilePath);
            FruitData data = JsonUtility.FromJson<FruitData>(json);

            this.strawberryCount = data.strawberryCount;
            this.appleCount = data.appleCount;
            this.pearCount = data.pearCount;
            
            Debug.Log("Data loaded from " + dataFilePath);
        }
        else
        {
            Debug.LogWarning("Save file not found at " + dataFilePath);
            strawberryCount = 0;
            appleCount = 0;
            pearCount = 0;
        }
    }
}