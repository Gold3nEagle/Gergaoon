using System;
using System.Collections.Generic;
using System.Linq;
using Malee.Editor;
using UnityEditor;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace SweetSugar.Scripts.Localization.Editor
{
    public class LocalizationWindow : EditorWindow
    {
        string myString = "Hello World";
        bool groupEnabled;
        bool myBool = true;
        float myFloat = 1.23f;
        private static List<MyStruct> array;
        private static LocalizationWindow window;
        private Vector2 scrollPos;
        private SystemLanguage lang;
        private Dictionary<int, string> _dic;
        private IOrderedEnumerable<LocalizeText> _findObjectsOfLocalizeText;
        private List<MyStruct> _list;

        // Add menu item named "My Window" to the Window menu
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            window = (LocalizationWindow)GetWindow(typeof(LocalizationWindow));
            window.Show();
        }

        private void OnEnable()
        {
            lang = SystemLanguage.English;
        }

        struct MyStruct
        {
            public GameObject obj;
            public int id;
            public string text;
        }
        
        public void OnFocus()
        {
            _findObjectsOfLocalizeText = Resources.FindObjectsOfTypeAll<LocalizeText>().OrderBy(i=>i.instanceID);
            LocalizationManager.LoadLanguage(lang);
            _dic = LocalizationManager._dic;
        }

        void OnGUI()
        {
            lang = (SystemLanguage) EditorGUILayout.EnumPopup(lang);
            _list = GetList();
            if (GUILayout.Button("Save"))
            {
                
            }

            EditorGUILayout.BeginVertical();
            scrollPos =
                EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(position.width), GUILayout.Height(position.height-100));
            foreach (var langLine in _list)
            {
                GUILayout.BeginHorizontal();
                {
                    
                    
                }
                GUILayout.EndHorizontal();

            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

        }

        List<MyStruct> GetList()
        {
            List<MyStruct> list = new List<MyStruct>();
            foreach (var langLine in _dic)
            {
                var l = _findObjectsOfLocalizeText.Where(i => i.instanceID == langLine.Key);
                list.AddRange(l.Select(localizeText => new MyStruct {obj = localizeText.gameObject, id = localizeText.instanceID, text = langLine.Value}));
            }
            return list;
        }
    }
}

