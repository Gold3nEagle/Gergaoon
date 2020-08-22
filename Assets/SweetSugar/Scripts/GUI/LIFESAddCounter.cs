using System;
using SweetSugar.Scripts.Core;
using SweetSugar.Scripts.System;
using TMPro;
using UnityEngine;

namespace SweetSugar.Scripts.GUI
{
    /// <summary>
    /// Life time counter on the map
    /// </summary>
    public class LIFESAddCounter : MonoBehaviour
    {
        TextMeshProUGUI text;
        static float TimeLeft;
        float TotalTimeForRestLife = 15f * 60;  //8 minutes for restore life
        bool startTimer;
        DateTime templateTime;
        // Use this for initialization
        void Start()
        {
            text = GetComponent<TextMeshProUGUI>();
            TotalTimeForRestLife = InitScript.Instance.TotalTimeForRestLifeHours * 60 * 60 + InitScript.Instance.TotalTimeForRestLifeMin * 60 + InitScript.Instance.TotalTimeForRestLifeSec;
        }

        bool CheckPassedTime()
        {
            //print(InitScript.DateOfExit);
            if (InitScript.DateOfExit == "" || InitScript.DateOfExit == default(DateTime).ToString())
                InitScript.DateOfExit = ServerTime.THIS.serverTime.ToString();

            var dateOfExit = DateTime.Parse(InitScript.DateOfExit);
            if (ServerTime.THIS.serverTime.Subtract(dateOfExit).TotalSeconds > TotalTimeForRestLife * (InitScript.Instance.CapOfLife - InitScript.lifes))
            {
                //Debug.Log(dateOfExit + " " + InitScript.today);
                InitScript.Instance.RestoreLifes();
                InitScript.RestLifeTimer = 0;
                return false;    ///we dont need lifes
            }

            TimeCount((float)ServerTime.THIS.serverTime.Subtract(dateOfExit).TotalSeconds);
            //Debug.Log((float)ServerTime.THIS.serverTime.Subtract(dateOfExit).TotalSeconds + " " + dateOfExit + " " + ServerTime.THIS.serverTime);
            return true;     ///we need lifes
        }

        void TimeCount(float tick)
        {
            if (InitScript.RestLifeTimer <= 0)
                ResetTimer();

            InitScript.RestLifeTimer -= tick;
            if (InitScript.RestLifeTimer <= 1 && InitScript.lifes < InitScript.Instance.CapOfLife)
            {
                InitScript.Instance.AddLife(1);
                ResetTimer();
            }
            //		}
        }

        void ResetTimer()
        {
            InitScript.RestLifeTimer = TotalTimeForRestLife;
        }

        // Update is called once per frame
        void Update()
        {
            if (!startTimer && ServerTime.THIS.dateReceived && ServerTime.THIS.serverTime.Subtract(ServerTime.THIS.serverTime).Days == 0)
            {
                if (InitScript.lifes < InitScript.Instance.CapOfLife)
                {
                    if (CheckPassedTime())
                        startTimer = true;
                    //	StartCoroutine(TimeCount());
                }
            }

            if (startTimer)
                TimeCount(Time.deltaTime);

            if (gameObject.activeSelf)
            {
                if (InitScript.lifes < InitScript.Instance.CapOfLife)
                {
                    if (InitScript.Instance.TotalTimeForRestLifeHours > 0)
                    {
                        var hours = Mathf.FloorToInt(InitScript.RestLifeTimer / 3600);
                        var minutes = Mathf.FloorToInt((InitScript.RestLifeTimer - hours * 3600) / 60);
                        var seconds = Mathf.FloorToInt((InitScript.RestLifeTimer - hours * 3600) - minutes * 60);

                        text.enabled = true;
                        text.text = "" + string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
                    }
                    else
                    {
                        var minutes = Mathf.FloorToInt(InitScript.RestLifeTimer / 60F);
                        var seconds = Mathf.FloorToInt(InitScript.RestLifeTimer - minutes * 60);

                        text.enabled = true;
                        text.text = "" + string.Format("{0:00}:{1:00}", minutes, seconds);

                    }

                    //				//	text.text = "+1 in \n " + Mathf.FloorToInt( MainMenu.RestLifeTimer/60f) + ":" + Mathf.RoundToInt( (MainMenu.RestLifeTimer/60f - Mathf.FloorToInt( MainMenu.RestLifeTimer/60f))*60f);
                }
                else
                {
                    text.text = "   Full";
                }
            }
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                //	StopCoroutine("TimeCount");
                InitScript.DateOfExit = ServerTime.THIS.serverTime.ToString();
                //print(InitScript.DateOfExit);

                //			PlayerPrefs.SetString("DateOfExit",ServerTime.THIS.serverTime.ToString());
                //			PlayerPrefs.Save();
            }
            else
            {
                startTimer = false;
                // InitScript.today = ServerTime.THIS.serverTime;
                //		MainMenu.DateOfExit = PlayerPrefs.GetString("DateOfExit");
            }
        }

        void OnEnable()
        {
            startTimer = false;
        }

        //void OnDisable()  //1.4    
        //{
        //    InitScript.DateOfExit = ServerTime.THIS.serverTime.ToString();
        //    //print(InitScript.DateOfExit);

        //}


        void OnApplicationQuit()  //1.4  
        {
            InitScript.DateOfExit = ServerTime.THIS.serverTime.ToString();
            //print(InitScript.DateOfExit);

        }
    }
}
