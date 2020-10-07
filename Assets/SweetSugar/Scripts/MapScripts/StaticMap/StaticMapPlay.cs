using System.Collections;
using System.Collections.Generic;
using SweetSugar.Scripts.Core;
using SweetSugar.Scripts.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;

namespace SweetSugar.Scripts.MapScripts
{
    public class StaticMapPlay : MonoBehaviour
    {
        public TextMeshProUGUI text;
        private int level;
        public GameObject tutsGO, jellyTutorial, powerUpPanel;
         

        private void OnEnable()
        {
            level = LevelsMap._instance.GetLastestReachedLevel();
            text.text = LocalizationManager.GetText(83, "Level") + " " + level;
            InitScript.OpenMenuPlay(level);

            if(level == 11)
            {
                StartCoroutine(ShowPowerUpPanel(level));
            }

            if(level == 5)
            { 
                StartCoroutine(ShowPowerUpPanel(level));
            }


            //Log Starting Level
           AnalyticsResult analyticsResult = AnalyticsEvent.LevelStart(level);
            Debug.Log("Analytics Result: " + analyticsResult);

        }

        public void PressPlay()
        {
            InitScript.OpenMenuPlay(level);
        }

        IEnumerator ShowPowerUpPanel(int level)
        { 
            if(level == 11)
            {
                yield return new WaitForSeconds(6f);
                tutsGO.SetActive(true);
                Transform[] allChildren = tutsGO.GetComponentsInChildren<Transform>(); 
                allChildren[1].gameObject.SetActive(false);
                jellyTutorial.SetActive(true);
            }

            if(level == 5)
            {
                yield return new WaitForSeconds(6f);
                powerUpPanel.SetActive(true);
            }  
            yield return null;
        }

    }
       
}