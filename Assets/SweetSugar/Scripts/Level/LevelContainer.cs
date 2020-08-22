using System;
using System.Collections.Generic;
using System.Linq;
using SweetSugar.Scripts.TargetScripts.TargetSystem;
using UnityEngine;

namespace SweetSugar.Scripts.Level
{
    /// <summary>
    /// Level scriptable file. Resources/Levels/
    /// </summary>
    [CreateAssetMenu(fileName = "LevelContainer", menuName = "LevelContainer", order = 1)]
    public class LevelContainer : ScriptableObject
    {
        public LevelData levelData;

        public void SetData(LevelData levelData)
        {
            this.levelData = levelData;
        }
    }
}