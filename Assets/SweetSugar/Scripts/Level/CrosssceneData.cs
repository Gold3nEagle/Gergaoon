using UnityEngine;

namespace SweetSugar.Scripts.Level
{
    /// <summary>
    /// Cross scene data transfer
    /// </summary>
    public static class CrosssceneData
    {
        public static bool openNextLevel;
        public static bool win;
        public static int passLevelCounter;
        public static int totalLevels;
        public static SystemLanguage? selectedLanguage;
    }
}