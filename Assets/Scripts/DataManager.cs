using System;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

// Class to hold your data
[System.Serializable]
public class PlayerData
{
    public int strawberryCount;
    public int appleCount;
    public int pearCount;
    public int gridPosition;
    public float musicVolume;
    public float soundVolume;
    public bool musicToggle;
    public bool soundToggle;
}

// Singleton class to manage your data
public class DataManager : Singleton<DataManager>
{
    public int strawberryCount, appleCount, pearCount;
    public float musicVolume;
    public float soundVolume;
    public int gridPosition;
    public bool isNewGame;
    public bool musicToggle, soundToggle;
    private string _dataFilePath;

    

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject); // Ensure DataManager persists across scenes

        // Initialize file path for saving data
        _dataFilePath = Path.Combine(Application.persistentDataPath, "gameData.json");
        LoadData();
    }
    // Method to save data to JSON
    public void SaveData()
    {
        PlayerData data = new PlayerData
        {
            strawberryCount = this.strawberryCount,
            appleCount = this.appleCount,
            pearCount = this.pearCount,
            gridPosition = this.gridPosition,
            musicVolume = musicVolume,
            soundVolume = soundVolume,
            musicToggle = musicToggle,
            soundToggle = soundToggle
        };

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(_dataFilePath, json);
        Debug.Log("Data saved to " + _dataFilePath);
    }


    public void LoadData()
    {
        if (isNewGame)
        {
            InitializeNewGameData();
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
                musicVolume = data.musicVolume;
                soundVolume = data.soundVolume;
                musicToggle = data.musicToggle;
                soundToggle = data.soundToggle;

                Debug.Log("Data loaded from " + _dataFilePath);
            }
            else
            {
                Debug.LogWarning("Save file not found at " + _dataFilePath);
                InitializeNewGameData();
            }
        }
    }

    private void InitializeNewGameData()
    {
        strawberryCount = 0;
        appleCount = 0;
        pearCount = 0;
        gridPosition = 0;
        soundVolume = 1;
        musicVolume = .7f;
        musicToggle = true;
        soundToggle = true;
        SaveData();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveData();
        }
    }

    protected override void OnApplicationQuit()
    {
        SaveData();
        base.OnApplicationQuit();
    }
}
