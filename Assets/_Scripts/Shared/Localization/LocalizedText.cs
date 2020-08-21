using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using TMPro;

public class LocalizedText : MonoBehaviour
{

    //public TMP_FontAsset[] font;
    //TextMeshProUGUI text;
    Text text;
    string chosenLanguage;
    public string key;

    //private void Awake()
    //{
    //   chosenLanguage = PlayerPrefs.GetString("locale");
    //   text = GetComponent<TextMeshProUGUI>();

    //    if (chosenLanguage == "localizedText_en.json")
    //    {
    //        text.font = font[0];
    //    } else if (chosenLanguage == "localizedText_ar.json")
    //    {
    //        text.font = font[1];
    //    } else
    //    {
    //        text.font = font[0];
    //    }
    //}

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
        text.text = LocaleManager.instance.GetLocalizedValue(key);
    }
     
}
