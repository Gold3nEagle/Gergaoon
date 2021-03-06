﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;

[Serializable]
public class SaveData
{
    public bool[] isActive;
    public int[] highScores, stars;
}

public class GameData : MonoBehaviour
{
    public static GameData gameData;
    public SaveData saveData;
    int totalLevels = 170;

    // Start is called before the first frame update
    void Awake()
    {
        if (gameData == null)
        {
            DontDestroyOnLoad(gameObject);
            gameData = this;
        }
        else
        {
            Destroy(gameObject);
        }
        Load();
    }


    public void Save()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/player.geagle", FileMode.Create);
        SaveData data = new SaveData();
        data = saveData;
        formatter.Serialize(file, data);
        file.Close();
        Debug.Log("Progress Saved!");
    }

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/player.geagle"))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/player.geagle", FileMode.Open);
            saveData = formatter.Deserialize(file) as SaveData;
            file.Close();
            Debug.Log("Progress Loaded!");
        }
        else
        {
            saveData = new SaveData();
            saveData.isActive = new bool[totalLevels];
            saveData.stars = new int[totalLevels];
            saveData.highScores = new int[totalLevels];
            saveData.isActive[0] = true;
        }
    }

    public void CheckLevels()
    {
 
            if (saveData.isActive.Length < totalLevels)
            {
                int lastLevel = GetLatestUnlockedLevel();

                saveData.isActive = new bool[totalLevels];
                saveData.stars = new int[totalLevels];
                saveData.highScores = new int[totalLevels];

                for (int i = 0; i <= lastLevel; i++)
                {
                    saveData.isActive[i] = true;
                }

                for (int i = lastLevel; i <= saveData.isActive.Length; i++)
                {
                    saveData.isActive[i] = false;
                }

                Debug.Log("Reconfigured Save File");
                Save();
            } 
    } 

    public int GetLatestUnlockedLevel()
    {
        int lastLevel = 0;
        for (int i = 0; i < saveData.isActive.Length; i++)
        {
            if (saveData.isActive[i] == true)
            {
                lastLevel++;
            }
        }

        return lastLevel;
    }
}
