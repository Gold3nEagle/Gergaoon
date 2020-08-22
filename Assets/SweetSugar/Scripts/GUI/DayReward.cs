using System;
using SweetSugar.Scripts.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SweetSugar.Scripts.GUI
{
    /// <summary>
    /// Class for any day reward
    /// </summary>
    public class DayReward : MonoBehaviour
    {
        public Image check;
        public Image image;
        public Sprite CurrentDay;
        public Sprite AheadDay;
        public Sprite PassedDay;
        public TextMeshProUGUI count;
        public TextMeshProUGUI textday;
        public Color colorCurrent;
        public Color colorPassed;
        public Color colorAhead;
        public int day;

        private void OnEnable()
        {
            textday.text = day.ToString() + " " + LocalizationManager.GetText(15, "DAY");
        }

        public void SetDayAhead()
        {
            image.sprite = AheadDay;
            count.color = colorAhead;
            check.enabled = false;
        }

        public void SetCurrentDay()
        {
            image.sprite = CurrentDay;
            count.color = colorCurrent;
            check.enabled = false;
        }

        public void SetPassedDay()
        {
            image.sprite = PassedDay;
            count.color = colorPassed;
            check.enabled = true;
        }
    }
}