using System;
using SweetSugar.Scripts.System;
using UnityEngine;

namespace SweetSugar.Scripts.GUI.BonusSpin
{
    /// <summary>
    /// Opens spinning wheel bonus game
    /// </summary>
    public class BonusSpinButton : MonoBehaviour {
        public GameObject spin;

        private void Start()
        {
            if (ServerTime.THIS.dateReceived)
                CheckSpin();
            else ServerTime.OnDateReceived += CheckSpin;
        }
        
        /// <summary>
        /// Check server to show or hide the button
        /// </summary>
        private void CheckSpin()
        {
            string latestSpinDate = PlayerPrefs.GetString("Spin");
            if (latestSpinDate == "" || latestSpinDate == default(DateTime).ToString())
            {
                latestSpinDate = ServerTime.THIS.serverTime.ToString();
                if(this != null)
                    gameObject.SetActive(true);
                return;
            }

            var latestDate = DateTime.Parse(latestSpinDate);
            if (spin != null)
            {
                var spinned = Mathf.Clamp(PlayerPrefs.GetInt("Spinned", 0), 0, spin.GetComponent<BonusSpin>().spinPrice.Length );
                if (ServerTime.THIS.serverTime.Subtract(latestDate).TotalHours < 24 && spinned >= spin.GetComponent<BonusSpin>().spinPrice.Length )
                    gameObject.SetActive(false);
                else if(ServerTime.THIS.serverTime.Subtract(latestDate).TotalHours >= 24)
                {
                    PlayerPrefs.SetInt("Spinned", 0);
                    gameObject.SetActive(true);
                }
                else
                    gameObject.SetActive(true);
            }
        }

        private void OnDisable()
        {
//        ServerTime.OnDateReceived -= CheckSpin;
        }

        public void OnClick()
        {
            spin.SetActive(true);
        }

        public void OnSpin()
        {
            SetDate();
            CheckSpin();
        }

        void SetDate(){
            PlayerPrefs.SetString("Spin",ServerTime.THIS.serverTime.ToString());
        }
    }
}