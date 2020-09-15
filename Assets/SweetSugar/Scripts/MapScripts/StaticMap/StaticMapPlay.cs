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
        public GameObject tutsGO, jellyTutorial;

        private void OnEnable()
        {
            level = LevelsMap._instance.GetLastestReachedLevel();
            text.text = LocalizationManager.GetText(83, "Level") + " " + level;
            InitScript.OpenMenuPlay(level);

            if(level == 11)
            {
                tutsGO.SetActive(true);
                Transform[] allChildren = tutsGO.GetComponentsInChildren<Transform>();

                allChildren[1].gameObject.SetActive(false);
                jellyTutorial.SetActive(true);

            }

        }

        public void PressPlay()
        {
            InitScript.OpenMenuPlay(level);
        }
    }
}