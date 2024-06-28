using System.IO;
using UnityEngine;

// Class to hold your data
[System.Serializable]
public class PlayerData
{
    public int strawberryCount;
    public int appleCount;
    public int pearCount;
    public int gridPosition;
}

// Singleton class to manage your data
public class DataManager : Singleton<DataManager>
{
    public int strawberryCount, appleCount, pearCount;
    public int gridPosition;
    public bool isNewGame;
    private string _dataFilePath;

    private void Awake()
    {
        
        DontDestroyOnLoad(gameObject);
        // Initialize file path for saving data
        _dataFilePath = Path.Combine(Application.persistentDataPath, "fruitData.json");

        
    }

    // Method to save data to JSON
    public void SaveData()
    {
        PlayerData data = new PlayerData
        {
            strawberryCount = this.strawberryCount,
            appleCount = this.appleCount,
            pearCount = this.pearCount,
            gridPosition = this.gridPosition
        };

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(_dataFilePath, json);
        Debug.Log("Data saved to " + _dataFilePath);
    }

    // Method to load data from JSON
    public void LoadData()
    {
        if (isNewGame)
        {
            strawberryCount = 0;
            appleCount = 0;
            pearCount = 0;
            gridPosition = 0;
        }
        else
        {
            if (File.Exists(_dataFilePath))
            {
                string json = File.ReadAllText(_dataFilePath);
                PlayerData data = JsonUtility.FromJson<PlayerData>(json);

                strawberryCount = data.strawberryCount;
                appleCount = data.appleCount;
                pearCount = data.pearCount;
                gridPosition = data.gridPosition;
                
                Debug.Log("Data loaded from " + _dataFilePath);
            }
            else
            {
                Debug.LogWarning("Save file not found at " + _dataFilePath);
                strawberryCount = 0;
                appleCount = 0;
                pearCount = 0;
                gridPosition = 0;
            }
        }
    }
}
