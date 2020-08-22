using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SweetSugar.Scripts.System
{
    public class ServerTime : UnityEngine.MonoBehaviour
    {
        public static ServerTime THIS;
        public DateTime serverTime;
        public bool dateReceived;
        public delegate void DateReceived();
        public static event DateReceived OnDateReceived;
        [Header("Test date example: 2019-08-27 09:12:29")]
        public string TestDate; 
        private void Awake()
        {
            THIS = this;
            GetServerTime();
        }

        private void OnEnable()
        {
            GetServerTime();
        }

        void GetServerTime ()
        {
                StartCoroutine(getTime());
        }
  
        IEnumerator getTime()
        {
#if UNITY_WEBGL
            serverTime = DateTime.Now;
            #else
            WWW www = new WWW("https://candy-smith.info/gettime.php");
            yield return www;
                if(www.text != "")
                    serverTime = DateTime.Parse(www.text);
                else
                    serverTime = DateTime.Now;
                if(TestDate!="" && (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.LinuxEditor))
                    serverTime = DateTime.Parse(TestDate);
            #endif
            yield return  null;
            dateReceived = true;
            OnDateReceived?.Invoke();
        }
    }
}