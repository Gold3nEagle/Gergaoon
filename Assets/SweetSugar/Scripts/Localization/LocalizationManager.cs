using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SweetSugar.Scripts.Level;
using SweetSugar.Scripts.System;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace SweetSugar.Scripts.Localization
{
    public class LocalizationManager : UnityEngine.MonoBehaviour
    {
        public static LocalizationManager THIS;
        private static DebugSettings _debugSettings;
        public static Dictionary<int, string> _dic;

        private void Awake()
        {
            if (THIS == null) THIS = this;
            else if (THIS != this) Destroy(gameObject);
            DontDestroyOnLoad(this);
            _debugSettings = Resources.Load("Scriptable/DebugSettings") as DebugSettings;
            LoadLanguage(GetSystemLanguage(_debugSettings));
        }
        
        public static void LoadLanguage(SystemLanguage? language)
        {
            var txt = Resources.Load<TextAsset>("Localization/" + language);
            if (txt == null) txt = Resources.Load<TextAsset>("Localization/" + SystemLanguage.English);
            _dic = new Dictionary<int, string>();
            string[] lines = txt.text.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string inp_ln in lines)
            {
                string[] l = inp_ln.Split(':');
                var n = l[0];
                var text = l[1];
                _dic.Add(int.Parse(n),text.Trim());
            }
        }

        public static SystemLanguage? GetSystemLanguage(DebugSettings _debugSettings)
        {
            if (CrosssceneData.selectedLanguage != null) return CrosssceneData.selectedLanguage;
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.LinuxEditor)
                return _debugSettings.TestLanguage;
            return Application.systemLanguage;
        }

        public static string GetText(int instanceId, string defaultText)
        {
            if (_dic == null || _dic.Count == 0)
            {
//                Debug.LogError(GetSystemLanguage(_debugSettings) + " language file not exist");
                return "";
            }
            return _dic[instanceId] != ""? _dic[instanceId] : defaultText;
        }
    }
}