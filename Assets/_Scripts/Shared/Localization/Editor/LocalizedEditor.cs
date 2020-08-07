using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class LocalizedEditor : EditorWindow
{

    public LocalizationData localizationData;

    Vector2 scrollPos;


    private string gameDataProjectFilePath = "/StreamingAssets/data.json";

    [MenuItem("Window/Localized Text Editor")]
    static void Init()
    {
       // EditorWindow.GetWindow(typeof(LocalizedEditor)).Show();

        LocalizedEditor window = (LocalizedEditor)EditorWindow.GetWindow(typeof(LocalizedEditor));
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical();
        scrollPos = GUILayout.BeginScrollView(scrollPos, false, true, GUILayout.MinWidth(400), GUILayout.MaxWidth(1000), GUILayout.MinHeight(200), GUILayout.MaxHeight(1000), GUILayout.ExpandHeight(true));

        if (localizationData != null)
        {
            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty serializedProperty = serializedObject.FindProperty("localizationData");
            EditorGUILayout.PropertyField(serializedProperty, true);
            serializedObject.ApplyModifiedProperties();

            if(GUILayout.Button("Save data"))
            {
                SaveGameData();
            }
        }

        if(GUILayout.Button("Load data"))
        {
            LoadGameData();
        }

        if(GUILayout.Button("Create data"))
        {
            CreateNewData();
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    private void LoadGameData()
    {
        string filePath = EditorUtility.OpenFilePanel("Select localization data file", Application.streamingAssetsPath, "json");

        if (!string.IsNullOrEmpty(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            localizationData = JsonUtility.FromJson<LocalizationData>(dataAsJson);
        }
    }

    private void SaveGameData()
    {
        string filePath = EditorUtility.SaveFilePanel("Save localization data file", Application.streamingAssetsPath, "", "json");

        if (!string.IsNullOrEmpty(filePath))
        {
            string dataAsJson = JsonUtility.ToJson(localizationData);
            File.WriteAllText(filePath, dataAsJson);

        }
    }

    private void CreateNewData()
    {
        localizationData = new LocalizationData();  
    }

}
