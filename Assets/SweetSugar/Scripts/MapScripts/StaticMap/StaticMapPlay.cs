using System;
using SweetSugar.Scripts.Core;
using SweetSugar.Scripts.Localization;
using TMPro;
using UnityEngine;

namespace SweetSugar.Scripts.MapScripts
{
    public class StaticMapPlay : MonoBehaviour
    {
        public TextMeshProUGUI text;
        private int level;

        private void OnEnable()
        {
            level = LevelsMap._instance.GetLastestReachedLevel();
            text.text = LocalizationManager.GetText(83, "Level") + " " + level;
        }

        public void PressPlay()
        {
            InitScript.OpenMenuPlay(level);
        }
    }
}