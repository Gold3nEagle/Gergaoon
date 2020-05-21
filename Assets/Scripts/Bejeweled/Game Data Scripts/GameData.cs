using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

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

    // Start is called before the first frame update
    void Awake()
    {
        if(gameData == null)
        {
            DontDestroyOnLoad(gameObject);
            gameData = this;
        } else
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
        if(File.Exists(Application.persistentDataPath + "/player.geagle"))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/player.geagle", FileMode.Open);
            saveData = formatter.Deserialize(file) as SaveData;
            file.Close();
            Debug.Log("Progress Loaded!");
        } else
        {
            saveData = new SaveData();
            saveData.isActive = new bool[20];
            saveData.stars = new int[20];
            saveData.highScores = new int[20];
            saveData.isActive[0] = true;
        }
    }

    private void OnApplicationQuit()
    {
        Save();
    }

    private void OnDisable()
    {
        Save();
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
