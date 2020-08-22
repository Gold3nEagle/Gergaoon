using System.Collections.Generic;
using SweetSugar.Scripts.System;
using UnityEngine;

namespace SweetSugar.Scripts
{
    public static class DebugLogKeeper
    {
        public static List<DebugElement> logListFalling = new List<DebugElement>();
        public static List<DebugElement> logListDestroying = new List<DebugElement>();
        public static List<DebugElement> logListBonus = new List<DebugElement>();
        private static DebugSettings _debugSettings;

        public static void Log(string str, LogType type)
        {
#if UNITY_EDITOR
            // StackFrame stackFrame = new System.Diagnostics.StackTrace(1).GetFrame(1);
            // string fileName = stackFrame.GetFileName();
            // string methodName = stackFrame.GetMethod().ToString();
            // int lineNumber = stackFrame.GetFileLineNumber();

            // UnityEngine.Debug.Log(string.Format("{0}({1}:{2})\n{3}", methodName, Path.GetFileName(fileName), lineNumber, ""));
            string extractStackTrace;
            switch (type)
            {
                case LogType.Falling:
                    extractStackTrace = StackTraceUtility.ExtractStackTrace ();
                    logListFalling.Add(new DebugElement(str,extractStackTrace, Time.time));
                    break;
                case LogType.Destroying:
                    extractStackTrace = StackTraceUtility.ExtractStackTrace ();
                    logListDestroying.Add(new DebugElement(str,extractStackTrace, Time.time));
                    break;
                case LogType.BonusAppearance:
                    extractStackTrace = StackTraceUtility.ExtractStackTrace ();
                    logListBonus.Add(new DebugElement(str,extractStackTrace, Time.time));
                    break;
            }

//        Debug.Log(UnityEngine.StackTraceUtility.ExtractStackTrace ());
            if (_debugSettings.ShowLogImmediately)
                Debug.Log(str);

#endif
        }

        public static string GetLog(string id, LogType type)
        {
            string txt = "";
            List<DebugElement> list = new List<DebugElement>();
            switch (type)
            {
                case LogType.Falling:
                    list = logListFalling;
                    break;
                case LogType.Destroying:
                    list = logListDestroying;
                    break;
                case LogType.BonusAppearance:
                    list = logListBonus;
                    break;
            }
            foreach (var item in list)
            {
                if (item.str.Contains(id))
                {
                    string v = item.str + "\n";
                    Debug.Log(item.time + " " + v);
                    v = item.references + "\n";
                    Debug.Log(v);
                    txt += v;
                }
            }
            return txt;
        }

        public enum LogType
        {
            Falling,
            Destroying,
            BonusAppearance
        }

        public class DebugElement
        {
            public string str;
            public string references;
            public float time;

            public DebugElement(string str, string references, float time)
            {
                this.str = str;
                this.references = references;
                this.time = time;
            }
        }

        public static void Init()
        {
            _debugSettings = Resources.Load("Scriptable/DebugSettings") as DebugSettings;
        }
    }
}
