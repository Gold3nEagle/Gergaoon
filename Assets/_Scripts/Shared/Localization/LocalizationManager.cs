using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager instance;

    private Dictionary<string, string> localizedText;
    private bool isReady = false;
    private string missingTextString = "Localized text not found.";

    // Start is called before the first frame update
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        } else if (instance != this)
        {
            Destroy(gameObject);
        }

        if (PlayerPrefs.HasKey("LanguageNum"))
        {
            int language = PlayerPrefs.GetInt("LanguageNum");
            if(language == 1)
            {
                LoadLocalizedText("localizedText_ar.json");
            } else if (language == 2)
            {
                LoadLocalizedText("localizedText_en.json");
            } else
            {
                Debug.Log("Language was Reset");
            }
        }

    }
     
    public void LoadLocalizedText(string fileName)
    {

        localizedText = new Dictionary<string, string>();
        string dataAsJson;

#if UNITY_EDITOR
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

#elif UNITY_IOS
        string filePath = Path.Combine (Application.streamingAssetsPath + "/Raw", fileName);
 
#elif UNITY_ANDROID
        string filePath = Path.Combine(Application.streamingAssetsPath + "/", fileName);
 
#endif



#if UNITY_EDITOR || UNITY_IOS
        dataAsJson = File.ReadAllText(filePath);

#elif UNITY_ANDROID
            WWW reader = new WWW (filePath);
            while (!reader.isDone) {
            }
            dataAsJson = reader.text;
#endif

            LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

            for (int i = 0; i < loadedData.items.Length; i++)
            {
                localizedText.Add(loadedData.items[i].key, loadedData.items[i].value);
            }
            PlayerPrefs.SetString("locale", fileName);
            Debug.Log("Data loaded, dictionary contains: " + localizedText.Count + " entries");


        if(fileName == "localizedText_ar.json")
        {
            PlayerPrefs.SetInt("LanguageNum", 1);
        } else if (fileName == "localizedText_en.json")
        {
            PlayerPrefs.SetInt("LanguageNum", 2);
        }

    
        isReady = true;
    }

    public bool GetIsReady()
    {

        return isReady;
    }

    public string GetLocalizedValue(string key)
    {
        string result = missingTextString;
        if (localizedText.ContainsKey(key))
        {
            result = localizedText[key];
        }

        return result;
    }

}
